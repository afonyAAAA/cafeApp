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

namespace caffeApp.ViewModels
{
    public class ReportViewModel : ViewModelBase
    {
        public override string? UrlPathSegment { get; set; }

        public override IScreen HostScreen { get; set; }

        public override ViewModelActivator Activator { get; set; }

        private ObservableCollection<Ordersview> _orders;

        private ObservableCollection<Workshift> _workshifts;

        private Workshift _selectedWorkShift;

        private bool _isReportOfActiveWorkShift;

        private bool _isReportOfAllOrders;

        private bool _isAdmin;

        private decimal _sumPay;

        private int _countOrders;

        private int _countIsCash;

        private int _countIsCreditCard;

        private int _countIsNotPayedOrders;

        private bool _isWarning;

        private bool _activeWorkShiftNull;

        private string _warningMessage;

        private bool _reportIsReady;

        private int _indexWorkShift;

        public ObservableCollection<Ordersview> Orders
        {
            get => _orders;
            set => this.RaiseAndSetIfChanged(ref _orders, value);
        }

        public Workshift SelectedWorkShift
        {
            get => _selectedWorkShift;
            set => this.RaiseAndSetIfChanged(ref _selectedWorkShift, value);
        }

        public bool IsReportOfActiveWorkShift
        {
            get => _isReportOfActiveWorkShift;
            set => this.RaiseAndSetIfChanged(ref _isReportOfActiveWorkShift, value);
        }

        public bool IsReportOfAllOrders
        {
            get => _isReportOfAllOrders;
            set => this.RaiseAndSetIfChanged(ref _isReportOfAllOrders, value);
        }

        public bool IsAdmin
        {
            get => _isAdmin;
            set => this.RaiseAndSetIfChanged(ref _isAdmin, value);
        }

        public decimal SumPay
        {
            get => _sumPay;
            set => this.RaiseAndSetIfChanged(ref _sumPay, value);
        }

        public int CountOrders
        {
            get => _countOrders;
            set => this.RaiseAndSetIfChanged(ref _countOrders, value);
        }

        public int CountIsCash
        {
            get => _countIsCash;
            set => this.RaiseAndSetIfChanged(ref _countIsCash, value);
        }

        public int CountIsCreditCard
        {
            get => _countIsCreditCard;
            set => this.RaiseAndSetIfChanged(ref _countIsCreditCard, value);
        }

        public int CountIsNotPayedOrders
        {
            get => _countIsNotPayedOrders;
            set => this.RaiseAndSetIfChanged(ref _countIsNotPayedOrders, value);
        }

        public string WarningMessage
        {
            get => _warningMessage;
            set => this.RaiseAndSetIfChanged(ref _warningMessage, value);
        }

        public bool IsWarning
        {
            get => _isWarning;
            set => this.RaiseAndSetIfChanged(ref _isWarning, value);
        }

        public bool ReportIsReady { 
            get => _reportIsReady;
            set => this.RaiseAndSetIfChanged(ref _reportIsReady, value);
        }

        public ObservableCollection<Workshift> Workshifts { 
            get => _workshifts; 
            set => this.RaiseAndSetIfChanged(ref _workshifts, value);
        }

        public bool ActiveWorkShiftNull {
            get => _activeWorkShiftNull; 
            set => this.RaiseAndSetIfChanged(ref _activeWorkShiftNull, value);
        }

        public int IndexWorkShift { 
            get => _indexWorkShift; 
            set => this.RaiseAndSetIfChanged(ref _indexWorkShift, value);
        }

        public ReactiveCommand<Unit, Unit> CreateReportWaiter { get; }
        public ReactiveCommand<Unit, Unit> CreateReportAdmin { get; }
        

        public ReportViewModel(IScreen screen)
        {
            Activator = new();

            HostScreen = screen;

            IsAdmin = UserHelper.getAuthorizedUserInfo().RoleId == 3;

            Workshifts = DatabaseHelper.refreshEntity<Workshift>();

            SelectedWorkShift = WorkShiftHelper.getActiveWorkshift();

            if(SelectedWorkShift == null)
            {
                ActiveWorkShiftNull = true;
                IndexWorkShift = -1;
            }
            else
            {
                IndexWorkShift = Workshifts.IndexOf(SelectedWorkShift);
            }

            this.WhenAnyValue(x => x.SelectedWorkShift).WhereNotNull().Subscribe(x => { 
                if(x != WorkShiftHelper.getActiveWorkshift())
                {
                    ActiveWorkShiftNull = true;
                }
                else
                {
                    ActiveWorkShiftNull = false;
                }
            });

            CreateReportWaiter = ReactiveCommand.Create(() =>
            {

                var warningMessage = CreateReportOfWaiter();

                if (warningMessage != null)
                {
                    WarningMessage = warningMessage;
                    IsWarning = true;
                }
                else
                {
                    IsWarning = false;
                }

                ReportIsReady = true;
            });

            CreateReportAdmin = ReactiveCommand.Create(() =>
            {
                string? warningMessage = null;

                if(SelectedWorkShift == null)
                {
                    warningMessage = CreateReportActiveWorkShift();
                }
                else
                {
                    warningMessage = CreataReportOfSelectedWorkShift();
                }
                
                if (warningMessage != null)
                {
                    WarningMessage = warningMessage;
                    IsWarning = true;
                }
                else
                {
                    IsWarning = false;
                }

                ReportIsReady = true;
            });
        }


        private string? CreataReportOfSelectedWorkShift()
        {
            Orders = DatabaseHelper.refreshEntity<Ordersview>();

            if (Orders.Count == 0)
            {
                return "В настоящее время ещё нет заказов";
            }

            var listOrdersOfSelectedWorkShift = DatabaseHelper.refreshEntity<Ordersview>()
                .Where(x => x.WorkshiftId == SelectedWorkShift.WorkshiftId);

            var countOrders = listOrdersOfSelectedWorkShift.Count();

            if (listOrdersOfSelectedWorkShift.Count() == 0)
            {
                return "В эту смену не было заказов";
            }

            SumPay = (decimal)listOrdersOfSelectedWorkShift.Sum(x => x.Sum);
            CountOrders = countOrders;
            CountIsCash = listOrdersOfSelectedWorkShift.Where(x => (bool)!x.Isnoncash).Count();
            CountIsNotPayedOrders = listOrdersOfSelectedWorkShift.Where(x => x.Isnoncash == null).Count();
            CountIsCreditCard = listOrdersOfSelectedWorkShift.Where(x => (bool)x.Isnoncash).Count();
            Orders = new(listOrdersOfSelectedWorkShift);

            return null;
        }

        private string? CreateReportActiveWorkShift()
        {

            Orders = DatabaseHelper.refreshEntity<Ordersview>();

            if (Orders.Count == 0)
            {
                return "В настоящее время ещё нет заказов";
            }

            var activeWorkShiftid = WorkShiftHelper.getActiveWorkshift();

            if (activeWorkShiftid == null)
            {
                return "В настоящее время нет активной смены";
            }

            var listOrdersOfActiveWorkShift = DatabaseHelper.refreshEntity<Ordersview>()
                .Where(x => x.WorkshiftId == activeWorkShiftid.WorkshiftId);

            var countOrders = listOrdersOfActiveWorkShift.Count();

            if (countOrders == 0)
            {
                return "В настоящее время ещё нет за активную смену";
            }

            SumPay = (decimal)listOrdersOfActiveWorkShift.Sum(x => x.Sum);
            CountOrders = countOrders;
            CountIsCash = listOrdersOfActiveWorkShift.Where(x => (bool)!x.Isnoncash).Count();
            CountIsNotPayedOrders = listOrdersOfActiveWorkShift.Where(x => x.Isnoncash == null).Count();
            CountIsCreditCard = listOrdersOfActiveWorkShift.Where(x => (bool)x.Isnoncash).Count();
            Orders = new(listOrdersOfActiveWorkShift);

            return null;
        }

        private string? CreateReportOfWaiter()
        {
            Orders = DatabaseHelper.refreshEntity<Ordersview>();

            if (Orders.Count == 0)
            {
                return "В настоящее время ещё нет заказов";
            }

            var activeWorkShiftid = WorkShiftHelper.getActiveWorkshift();

            var userId = UserHelper.getAuthorizedUserInfo().UserId;

            var isActiveUserInWorkShift = WorkShiftHelper.
                isActiveWorkShiftUser(userId);

            if (activeWorkShiftid == null)
            {
                return "В настоящее время нет активной смены";
            }

            if (!isActiveUserInWorkShift)
            {
                return "В настоящее время вы не состоите в активной смене";
            }

            var listOrdersOfActiveWorkShift = DatabaseHelper.refreshEntity<Ordersview>()
                .Where(x => x.WorkshiftId == activeWorkShiftid.WorkshiftId && x.UserId == userId);

            var countOrders = listOrdersOfActiveWorkShift.Count();

            if (countOrders == 0)
            {
                return "В настоящее время ещё нет за активную смену";
            }

            SumPay = (decimal)listOrdersOfActiveWorkShift.Sum(x => x.Sum);
            CountOrders = countOrders;
            CountIsCash = listOrdersOfActiveWorkShift.Where(x => (bool)!x.Isnoncash).Count();
            CountIsCreditCard = listOrdersOfActiveWorkShift.Where(x => (bool)x.Isnoncash).Count();
            CountIsNotPayedOrders = listOrdersOfActiveWorkShift.Where(x => x.Isnoncash == null).Count();
            Orders = new(listOrdersOfActiveWorkShift);

            return null;
        }


    }
}
