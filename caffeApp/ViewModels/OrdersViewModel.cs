using caffeApp.Desktop;
using caffeApp.models;
using caffeApp.Sources;
using caffeApp.utils;
using caffeApp.ViewModels.Waiter;
using MsBox.Avalonia.Enums;
using MsBox.Avalonia;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace caffeApp.ViewModels
{
    public class OrdersViewModel : ViewModelBase
    {
        public override string? UrlPathSegment { get; set; }
        public override IScreen HostScreen { get; set; }
        public override ViewModelActivator Activator { get; set; }

        private ObservableCollection<Ordersview> _orders;
        private readonly User authorizedUser = UserHelper.getAuthorizedUserInfo();
        private ObservableCollection<Workshift> _workshifts;
        private ObservableCollection<Workshiftview> _workShiftsUser;
        private ObservableCollection<Foodorder> _foodorders;
        private ObservableCollection<Statusorder> _statusesOrder;
        private Ordersview _selectedOrderView;
        private Workshift _selectedWorkShift;
        private Statusorder _selectedStatusOrder;
        private bool _isOrderPayed;
        private bool _isPaymentAction;
        public bool _isAdmin;
        private bool _isWaiter;
        private bool _isSheff;
        private bool _isCreditCard;
        private bool _isCash;
        private ObservableCollection<SplitFood> _splitFoods;
        private bool _isSelectedOrder = false;

        public ReactiveCommand<Unit, Unit> OpenAddOrderView { get; }
        public ReactiveCommand<Unit, Unit> OpenAddOrderForUpdateView { get; }
        public ReactiveCommand<Unit, Unit> PaymentCommand { get; }
        public ReactiveCommand<Unit, Unit> SubmitPayment { get; }

        public ObservableCollection<Ordersview> Orders {
            get
            {
                var orders = _orders.ToList();

                if (IsWaiter)
                {
                    orders = orders.Where(x => x.UserId == authorizedUser.UserId).ToList();
                    return new ObservableCollection<Ordersview>(orders);
                }else if (IsSheff)
                {
                    orders = orders.Where(x => x.Status == "Принят" || x.Status == "Готовится").ToList();
                    return new ObservableCollection<Ordersview>(orders);
                }
               
                return new ObservableCollection<Ordersview>(orders);
         
            }
            set => this.RaiseAndSetIfChanged(ref _orders, value);
        }
        public ObservableCollection<Workshiftview> WorkShiftsUser { 
            get => _workShiftsUser;
            set => this.RaiseAndSetIfChanged(ref _workShiftsUser, value);
        }
        public Ordersview SelectedOrderView { 
            get => _selectedOrderView; 
            set => this.RaiseAndSetIfChanged(ref _selectedOrderView, value); 
        }
        public ObservableCollection<Workshift> Workshifts { 
            get => _workshifts;
            set => this.RaiseAndSetIfChanged(ref _workshifts, value);
        }
        public Workshift SelectedWorkShift { 
            get => _selectedWorkShift;
            set => this.RaiseAndSetIfChanged(ref _selectedWorkShift, value);
        }
        public ObservableCollection<Foodorder> Foodorders {
            get => _foodorders; 
            set => this.RaiseAndSetIfChanged(ref _foodorders, value);
        }
        public ObservableCollection<SplitFood> SplitFoods { 
            get => _splitFoods;
            set => this.RaiseAndSetIfChanged(ref _splitFoods, value);
        }
        public bool IsSelectedOrder {
            get => _isSelectedOrder;
            set => this.RaiseAndSetIfChanged(ref _isSelectedOrder, value);
        }
        public bool IsAdmin {
            get => _isAdmin;
            set => this.RaiseAndSetIfChanged(ref _isAdmin, value);
        }
        public bool IsWaiter { 
            get => _isWaiter;
            set => this.RaiseAndSetIfChanged(ref _isWaiter, value);
        }
        public bool IsSheff { 
            get => _isSheff; 
            set => this.RaiseAndSetIfChanged(ref _isSheff, value);
        }
        public ObservableCollection<Statusorder> StatusesOrder {
            get{
                var statuses = _statusesOrder.ToList();

                if (IsSheff)
                {
                    statuses = statuses.Where(x => x.Name != "Принят").ToList();
                }
              
                return new(statuses);
            }
            set => this.RaiseAndSetIfChanged(ref _statusesOrder, value);
        }
        public bool IsPaymentAction { 
            get => _isPaymentAction; 
            set => this.RaiseAndSetIfChanged(ref _isPaymentAction, value);
        }

        public Statusorder SelectedStatusOrder { 
            get => _selectedStatusOrder;
            set => this.RaiseAndSetIfChanged(ref _selectedStatusOrder, value);
        }
        public bool IsCreditCard {
            get => _isCreditCard;
            set => this.RaiseAndSetIfChanged(ref _isCreditCard, value);
        }
        public bool IsCash { 
            get => _isCash;
            set => this.RaiseAndSetIfChanged(ref _isCash, value);
        }
        public bool IsOrderPayed { 
            get => _isOrderPayed; 
            set => this.RaiseAndSetIfChanged(ref _isOrderPayed, value);
        }

        public OrdersViewModel(IScreen screen)
        {
            Activator = new();

            HostScreen = screen;

            IsWaiter = authorizedUser.RoleId == 1;

            IsSheff = authorizedUser.RoleId == 2;

            IsAdmin = authorizedUser.RoleId == 3;

            StatusesOrder = DatabaseHelper.refreshEntity<Statusorder>();

            Orders = DatabaseHelper.refreshEntity<Ordersview>();

            Workshifts = DatabaseHelper.refreshEntity<Workshift>();

            this.WhenAnyValue(x => x.SelectedWorkShift).WhereNotNull().Subscribe(x =>
            {
                IsSelectedOrder = false;

                Orders = DatabaseHelper.refreshEntity<Ordersview>();
                    
                var newListOrders = Orders.Where(x => x.WorkshiftId == SelectedWorkShift.WorkshiftId).ToList();

                Orders = new ObservableCollection<Ordersview>(newListOrders);
                
            });

            this.WhenAnyValue(x => x.SelectedOrderView).WhereNotNull().Subscribe(selectedOrder =>
            {
                var foodsOfSelectedOrder = DatabaseHelper.refreshEntity<Foodorder>(x => x.Food)
                .Where(x => x.OrderId == selectedOrder.OrderId).ToList();

                Foodorders = new(foodsOfSelectedOrder);

                SplitFoods = new();

                var counter = 0;

                foreach (var food in Foodorders)
                {
                    var sameFood = SplitFoods.ToList().FindIndex(x => x.Name == food.Food.Name);

                    if(sameFood == -1)
                    {
                        SplitFoods.Add(new SplitFood(food.Food.Name, 1));
                    }
                    else
                    {
                        var currentFood = SplitFoods[sameFood];
                        SplitFoods[sameFood] = new(currentFood.Name, ++currentFood.Count);
                    }
                    ++counter;
                }
                
                if(selectedOrder.Isnoncash != null)
                {
                    IsOrderPayed = true;
                }
                else
                {
                    IsOrderPayed = false;
                }

                IsSelectedOrder = true;
            });

            this.WhenAnyValue(x => x.SelectedStatusOrder).WhereNotNull().Subscribe(x => {
                UpdateStatusOrder();
            });

            //Userworkshift = DatabaseHelper.refreshEntity<>

            PaymentCommand = ReactiveCommand.Create(() =>
            {
                IsPaymentAction = true;
                IsSelectedOrder = false;
            });

            SubmitPayment = ReactiveCommand.Create(() => {
                UpdatePayment();
            });

            OpenAddOrderView = ReactiveCommand.Create(() => {
                HostScreen.Router.Navigate.Execute(new AddOrderViewModel(HostScreen));
            });

            OpenAddOrderForUpdateView = ReactiveCommand.Create(() => {
                HostScreen.Router.Navigate.Execute(new AddOrderViewModel(HostScreen, SelectedOrderView));
            });

        }

        private void UpdateStatusOrder()
        {
            var context = DbContextProvider.GetContext();

            var order = DatabaseHelper.refreshEntity<Order>()
                .ToList().First(x => x.OrderId == SelectedOrderView.OrderId);

            order.StatusorderId = SelectedStatusOrder.StatusorderId;

            context.Orders.Update(order);

            context.SaveChanges();

            Orders = DatabaseHelper.refreshEntity<Ordersview>();

            IsSelectedOrder = false;
        }

        private async Task UpdatePayment()
        {
            try
            {
                if (IsCreditCard == false && IsCash == false)
                {

                    var box = MessageBoxManager
                .GetMessageBoxStandard("Оплата",
                        "Выберите вид оплаты",
                    ButtonEnum.Ok);

                    box.ShowAsync();
                }
                else
                {
                    var context = DbContextProvider.GetContext();

                    var paymentId = DatabaseHelper.refreshEntity<Order>()
                        .ToList().First(x => x.OrderId == SelectedOrderView.OrderId).PaymentId;

                    var payment = DatabaseHelper.refreshEntity<Payment>().First(x => x.PaymentId == paymentId);

                    payment.Isnoncash = IsCreditCard;

                    payment.Sum = (decimal)SelectedOrderView.Sum;

                    payment.StatuspaymentId = 2;

                    payment.Datepayment = DateTime.UtcNow;

                    context.Payments.Update(payment);

                    context.SaveChanges();

                    IsPaymentAction = false;

                    var box = MessageBoxManager
                 .GetMessageBoxStandard("Оплата",
                    "Оплата прошла успешно",
                    ButtonEnum.Ok);

                    box.ShowAsync();

                    Orders = DatabaseHelper.refreshEntity<Ordersview>();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
