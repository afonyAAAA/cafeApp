using caffeApp.Desktop;
using caffeApp.Sources;
using caffeApp.utils;
using MsBox.Avalonia.Enums;
using MsBox.Avalonia;
using NodaTime;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Controls;
using DynamicData.Aggregation;
using Npgsql.Replication;
using System.Reactive.Linq;
using System.Threading;
using System.Diagnostics;
using caffeApp.models;

namespace caffeApp.ViewModels.Waiter
{
    public class AddOrderViewModel : ViewModelBase
    {
        public override string? UrlPathSegment { get; set; }

        public override IScreen HostScreen { get; set; }

        public override ViewModelActivator Activator { get; set; }

        private ObservableCollection<Food> _selectedFoods;

        private ObservableCollection<SplitFood> _splitFoods;

        private ObservableCollection<Place> _places;

        private ObservableCollection<Food> _foods;

        private Food _selectedFood;

        private SplitFood _selectedFoodForDelete;

        private Place _selectedPlace;

        private int _selectedNumberGuests;

        private int _orderId;

        private bool _isEditOrder;

        private decimal _sumOrder;

        private List<int> numberGuests = Enumerable.Range(1, 100).ToList();

        public ObservableCollection<Food> SelectedFoods { 
            get => _selectedFoods; 
            set => this.RaiseAndSetIfChanged(ref _selectedFoods, value); 
        }
        public ObservableCollection<Place> Places { 
            get => _places; 
            set => this.RaiseAndSetIfChanged(ref _places, value); 
        }
        public ObservableCollection<Food> Foods { 
            get => _foods; 
            set => this.RaiseAndSetIfChanged(ref _foods, value);
        }
        public Food SelectedFood {
            get => _selectedFood;
            set {
                this.RaiseAndSetIfChanged(ref _selectedFood, value);
            }
        }
        public Place SelectedPlace {
            get => _selectedPlace; 
            set => this.RaiseAndSetIfChanged(ref _selectedPlace, value);
        }
        public int SelectedNumberGuests { 
            get => _selectedNumberGuests; 
            set => this.RaiseAndSetIfChanged(ref _selectedNumberGuests, value);
        }
        public List<int> NumberGuests { get => numberGuests; }

        public decimal SumOrder { 
            get => _sumOrder;
            set => this.RaiseAndSetIfChanged(ref _sumOrder, value);
        }
        public int OrderId {
            get => _orderId;
            set => this.RaiseAndSetIfChanged(ref _orderId, value);
        }
        public bool IsEditOrder { 
            get => _isEditOrder;
            set => this.RaiseAndSetIfChanged(ref _isEditOrder, value);
        }

        public ReactiveCommand<Unit, Unit> Submit { get; }

        public ReactiveCommand<Unit, Unit> SubmitUpdate { get; }

        public ReactiveCommand<Unit, Unit> DeleteFood { get; }

        public ReactiveCommand<Unit, Unit> AddFood { get; }

        public SplitFood SelectedFoodForDelete {
            get => _selectedFoodForDelete; 
            set => this.RaiseAndSetIfChanged(ref _selectedFoodForDelete, value);
        }

        public ObservableCollection<SplitFood> SplitFoods { 
            get => _splitFoods;
            set => this.RaiseAndSetIfChanged(ref _splitFoods, value);
        }

        public AddOrderViewModel(IScreen screen, Ordersview selectedOrderView = null) {

            HostScreen = screen;

            SplitFoods = new();

            Activator = new();

            Places = DatabaseHelper.refreshEntity<Place>();

            Foods = DatabaseHelper.refreshEntity<Food>();

            if (selectedOrderView != null)
            {
                IsEditOrder = true;
                OrderId = (int)selectedOrderView.OrderId;
                SelectedPlace = Places.First(x => x.Number == selectedOrderView.Numberplace);
                SelectedNumberGuests = (int)selectedOrderView.Quantityclients;
                SumOrder = (decimal)selectedOrderView.Sum;

                var selectedFoods = DatabaseHelper
                    .refreshEntity<Foodorder>(x => x.Food)
                    .Where(x => x.OrderId == selectedOrderView.OrderId)
                    .Select(x => new Food { FoodId = x.FoodId, Name = x.Food.Name, Price = x.Food.Price })
                    .ToList();

                SelectedFoods = new(selectedFoods);
            }
            else
            {
                IsEditOrder = false;
                SelectedFoods = new();
            }

            this.WhenAnyValue(x => x.SelectedFoods).Subscribe(x => {

                var counter = 0;

                SplitFoods = new();

                foreach (var food in SelectedFoods)
                {
                    var sameFood = SplitFoods.ToList().FindIndex(x => x.Name == food.Name);

                    if (sameFood == -1)
                    {
                        SplitFoods.Add(new SplitFood(food.Name, 1, food.Price));
                    }
                    else
                    {
                        var currentFood = SplitFoods[sameFood];
                        SplitFoods[sameFood] = new(currentFood.Name, ++currentFood.Count, currentFood.Sum + food.Price);
                    }
                    ++counter;
                }

                decimal sum = 0;

                foreach (var food in x)
                {
                    sum += food.Price;
                }

                SumOrder = sum;
            });

            var isCanDeleteFood = this.WhenAnyValue(x => x.SelectedFoodForDelete).Select(x => x != null);

            var isCanAddFood = this.WhenAnyValue(x => x.SelectedFood).WhereNotNull().Select(x => x.FoodId != 0);

            DeleteFood = ReactiveCommand.Create(() =>
            {
                var deletedFood = SelectedFoods.FirstOrDefault(x => x.Name == SelectedFoodForDelete.Name);
                var newListSelectedFoods = SelectedFoods.ToList();
                newListSelectedFoods.Remove(deletedFood);
                SelectedFoods = new(newListSelectedFoods);
            }, isCanDeleteFood);

            AddFood = ReactiveCommand.Create(() => {
                var newListFoods = SelectedFoods.ToList();
                newListFoods.Add(SelectedFood);
                SelectedFoods = new(newListFoods.Where(x => x.FoodId != 0));
            }, isCanAddFood);

            Submit = ReactiveCommand.CreateFromTask(async () => {
                if (CheckValidOrder().Result)
                {
                    var createOrderTask = await CreateOrder();

                    if (createOrderTask)
                    {
                        var box = MessageBoxManager
                        .GetMessageBoxStandard("Добавление заказа",
                        "Заказ создан",
                        ButtonEnum.Ok);

                        box.ShowAsync();

                        HostScreen.Router.Navigate.Execute(new OrdersViewModel(HostScreen));
                    } 
                }
            });

            SubmitUpdate = ReactiveCommand.Create(() => {
                if (CheckValidOrder().Result)
                {
                    UpdateOrder();

                    var box = MessageBoxManager
                .GetMessageBoxStandard("Редактирование заказа",
                       "Заказ отредактирован",
                   ButtonEnum.Ok);

                    box.ShowAsync();

                    HostScreen.Router.Navigate.Execute(new OrdersViewModel(HostScreen));
                }
            });
        }

        private async Task<bool> CreateOrder()
        {
            try
            {
                DateTime currentDateTime = DateTime.Now;
                TimeOnly currentTime = TimeOnly.FromDateTime(currentDateTime);
                DateOnly currentDateToDateOnly = DateOnly.FromDateTime(currentDateTime);


                var workshift = DatabaseHelper
                    .refreshEntity<Workshift>()
                    .FirstOrDefault(x =>
                    x.Date == currentDateToDateOnly
                    && x.Timestart <= currentTime
                    && x.Timeend > currentTime
                    );

                if (workshift == null)
                {
                    var box = MessageBoxManager
                    .GetMessageBoxStandard("Создание заказа",
                       "В настоящее время нет рабочей смены",
                    ButtonEnum.Ok);

                    box.ShowAsync();

                    return await Task.FromResult(false);
                }

                var userId = UserHelper.getAuthorizedUserInfo().UserId;

                var userInWorkShift = DatabaseHelper.refreshEntity<Workshiftview>().FirstOrDefault(x =>
                    x.WorkshiftId == workshift.WorkshiftId
                    && x.UserId == userId
                );

                if (userInWorkShift == null)
                {
                    var box = MessageBoxManager
                    .GetMessageBoxStandard("Создание заказа",
                       "В настоящее время, вы не работаете в активной смене",
                    ButtonEnum.Ok);

                    box.ShowAsync();

                    return await Task.FromResult(false);
                }

                var paymentId = CreatePayment();

                Order order = new();
                order.Quantityclients = SelectedNumberGuests;
                order.Dateorder = currentDateToDateOnly;
                order.PlaceId = SelectedPlace.PlaceId;
                order.StatusorderId = 1;
                order.WorkshiftId = workshift.WorkshiftId;
                order.PaymentId = paymentId;
                order.UserId = UserHelper.getAuthorizedUserInfo().UserId;

                var context = DbContextProvider.GetContext();

                var createdOrder = context.Orders.Add(order);

                var foodOrders = new List<Foodorder>();

                context.SaveChanges();

                foreach (var food in SelectedFoods)
                {
                    Foodorder fd = new();
                    fd.FoodId = food.FoodId;
                    fd.OrderId = createdOrder.Entity.OrderId;
                    foodOrders.Add(fd);
                }

                context.Foodorders.AddRange(foodOrders);

                context.SaveChanges();

                return await Task.FromResult(true);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return await Task.FromResult(false);
            }
        }

        private int CreatePayment()
        {
            Payment payment = new();
            payment.Datepayment = DateTime.UtcNow;
            payment.Sum = SumOrder;
            payment.StatuspaymentId = 1;

            var context = DbContextProvider.GetContext();
            var createdPayment = context.Payments.Add(payment);
            context.SaveChanges();
            return createdPayment.Entity.PaymentId;
        }
            
        private void UpdateOrder()
        {
            var context = DbContextProvider.GetContext();
            Order order = DatabaseHelper.refreshEntity<Order>().First(x => x.OrderId == OrderId);
            order.Quantityclients = SelectedNumberGuests;
            order.PlaceId = SelectedPlace.PlaceId;
            
            context.Orders.Update(order);
            context.SaveChanges();

            var foodOrders = new List<Foodorder>();

            foreach (var food in SelectedFoods)
            {
                Foodorder fd = new();
                fd.FoodId = food.FoodId;
                fd.OrderId = order.OrderId;
                foodOrders.Add(fd);
            }

            var foodOrdersForRemove = DatabaseHelper.refreshEntity<Foodorder>().Where(x => x.OrderId == order.OrderId);
            context.Foodorders.RemoveRange(foodOrdersForRemove);
            context.SaveChanges();

            context.Foodorders.AddRange(foodOrders);
            context.SaveChanges();

            UpdatePayment((int)order.PaymentId, SumOrder);
       
        }

        private void UpdatePayment(int paymentId, decimal sumOrder)
        {
            var context = DbContextProvider.GetContext();
            var payment = DatabaseHelper.refreshEntity<Payment>().First(x => x.PaymentId == paymentId);
            payment.Sum = sumOrder;
            context.Update(payment);
            context.SaveChanges();
        }

        private async Task<bool> CheckValidOrder()
        {
            if(SelectedPlace == null || SelectedNumberGuests == null)
            {
                var box = MessageBoxManager
                  .GetMessageBoxStandard("Заказ",
                  "Все поля должны быть заполнены",
                ButtonEnum.Ok);

                box.ShowAsync();

                return false;
            }else if(SelectedFoods.Count == 0)
            {
                var box = MessageBoxManager
             .GetMessageBoxStandard("Заказ",
                    "Список выбранных блюд не должен быть пустым",
                ButtonEnum.Ok);

                box.ShowAsync();

                return false;
            }

            return true;
        }

    }
}
