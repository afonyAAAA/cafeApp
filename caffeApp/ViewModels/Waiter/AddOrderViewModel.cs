using caffeApp.Desktop;
using caffeApp.utils;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;

namespace caffeApp.ViewModels.Waiter
{
    public class AddOrderViewModel : ViewModelBase
    {
        public override string? UrlPathSegment { get; set; }

        public override IScreen HostScreen { get; set; }

        public override ViewModelActivator Activator { get; set; }

        private List<Food> _selectedFoods;

        private ObservableCollection<Place> _places;

        private ObservableCollection<Food> _foods;

        private Food _selectedFood;

        private Place _selectedPlace;

        private int _selectedNumberGuests;

        private decimal _sumOrder;

        private List<int> numberGuests = Enumerable.Range(0, 100).ToList();

        public List<Food> SelectedFoods { 
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
            set => this.RaiseAndSetIfChanged(ref _selectedFood, value);
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

        public ReactiveCommand<Unit, Unit> Submit { get; }
        public decimal SumOrder { 
            get => _sumOrder;
            set => this.RaiseAndSetIfChanged(ref _sumOrder, value);
        }

        public AddOrderViewModel(IScreen screen) {

            HostScreen = screen;

            Activator = new();

            Places = DatabaseHelper.refreshEntity<Place>();

            Foods = DatabaseHelper.refreshEntity<Food>();

            SelectedFood = new();

            SelectedFoods = new();


            this.WhenAnyValue(x => x.SelectedFood).Subscribe(SelectedFoods.Add);

            this.WhenAnyValue(x => x.SelectedFoods).Subscribe(x => {

                decimal sum = 0;

                foreach (var food in x)
                {
                    sum += food.Price;
                }

                SumOrder = sum;
            });

            Submit = ReactiveCommand.Create(() => { 
                   
            });
        }

    }
}
