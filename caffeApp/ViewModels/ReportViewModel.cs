using caffeApp.Desktop;
using caffeApp.utils;
using OfficeOpenXml;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia.Controls;
using System.Reactive.Linq;
using System.Collections.Generic;
using System.Diagnostics;

namespace caffeApp.ViewModels
{
    public class ReportViewModel : ViewModelBase
    {
        public override string? UrlPathSegment { get; set; }

        public override IScreen HostScreen { get; set; }

        public override ViewModelActivator Activator { get; set; }

        private ObservableCollection<Order> _orders;

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

        public ObservableCollection<Order> Orders
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

        public ReactiveCommand<Unit, Unit> SaveReport { get; }
        

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

            SaveReport = ReactiveCommand.CreateFromTask(async () => {
                var package = await ExportReportInExcel();
                await SaveFileAsAsync(package);
            });
        }


        private string? CreataReportOfSelectedWorkShift()
        {
            Orders = DatabaseHelper.refreshEntity<Order>(
                x => x.Payment, 
                x => x.Place, 
                x => x.Workshift, 
                x => x.User,
                x => x.Statusorder
            );

            if (Orders.Count == 0)
            {
                return "В настоящее время ещё нет заказов";
            }

            var listOrdersOfSelectedWorkShift = DatabaseHelper.refreshEntity<Order>()
                .Where(x => x.WorkshiftId == SelectedWorkShift.WorkshiftId);

            var countOrders = listOrdersOfSelectedWorkShift.Count();

            if (listOrdersOfSelectedWorkShift.Count() == 0)
            {
                return "В эту смену не было заказов";
            }

            SumPay = listOrdersOfSelectedWorkShift.Sum(x => x.Payment.Sum);
            CountOrders = countOrders;
            CountIsCash = listOrdersOfSelectedWorkShift.Where(x => (bool)!x.Payment.Isnoncash).Count();
            CountIsNotPayedOrders = listOrdersOfSelectedWorkShift.Where(x => x.Payment.Isnoncash == null).Count();
            CountIsCreditCard = listOrdersOfSelectedWorkShift.Where(x => (bool)x.Payment.Isnoncash).Count();
            Orders = new(listOrdersOfSelectedWorkShift);


            return null;
        }

        private async Task<ExcelPackage?> ExportReportInExcel()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            try
            {
                var package = new ExcelPackage();
                
                if (Orders != null && Orders.Any())
                {
                    var worksheet = package.Workbook.Worksheets.Add("Отчёт");

                    // Добавление содержимого в документ
                    // Заголовки столбцов
                    worksheet.Cells["A1"].Value = "Идентификатор заказа";
                    worksheet.Cells["B1"].Value = "Количество клиентов";
                    worksheet.Cells["C1"].Value = "Дата заказа";
                    worksheet.Cells["D1"].Value = "Статус заказа";
                    worksheet.Cells["E1"].Value = "Статус оплаты";
                    worksheet.Cells["F1"].Value = "Cумма заказа";
                    worksheet.Cells["G1"].Value = "Метод оплаты";
                    worksheet.Cells["H1"].Value = "Идентификатор официанта";
                    worksheet.Cells["I1"].Value = "ФИО официанта";


                    // Заполнение данными
                    int row = 2;
                    foreach (var order in Orders)
                    {
                        var typeMoney = "";

                        if(order.Payment.Isnoncash == null)
                        {
                            typeMoney = "-";
                        }
                        else if ((bool)order.Payment.Isnoncash)
                        {
                            typeMoney = "Карта";
                        }
                        else
                        {
                            typeMoney = "Наличные";
                        }

                        var statusPayment = DatabaseHelper.refreshEntity<Statuspayment>().First(x => x.StatuspaymentId == order.Payment.StatuspaymentId);

                        var waiter = DatabaseHelper.refreshEntity<User>().First(x => x.UserId == order.UserId);

                        worksheet.Cells[row, 1].Value = order.OrderId;
                        worksheet.Cells[row, 2].Value = order.Quantityclients;
                        worksheet.Cells[row, 3].Value = order.Dateorder.ToString("dd-MM-yyyy");
                        worksheet.Cells[row, 4].Value = order.Statusorder.Name;
                        worksheet.Cells[row, 5].Value = statusPayment.Name;
                        worksheet.Cells[row, 6].Value = order.Payment.Sum;
                        worksheet.Cells[row, 7].Value = typeMoney;
                        worksheet.Cells[row, 8].Value = order.UserId;
                        worksheet.Cells[row, 8].Value = waiter.UserId;
                        worksheet.Cells[row, 9].Value = waiter.getFullName();

                        row++;
                    }

                    int lastUsedRow = worksheet.Dimension?.End.Row + 1 ?? 1;

                    worksheet.Cells[lastUsedRow++, 1].Value = "Количество заказов";
                    worksheet.Cells[lastUsedRow++, 1].Value = "Количество заказов оплаченных наличными";
                    worksheet.Cells[lastUsedRow++, 1].Value = "Количество заказов оплаченных картой";
                    worksheet.Cells[lastUsedRow++, 1].Value = "Количество не оплаченных заказов";
                    worksheet.Cells[lastUsedRow++, 1].Value = "Общая сумма средств за заказы (выручка)";

                    worksheet.Cells[lastUsedRow++, 2].Value = CountOrders;
                    worksheet.Cells[lastUsedRow++, 2].Value = CountIsCash;
                    worksheet.Cells[lastUsedRow++, 2].Value = CountIsCreditCard;
                    worksheet.Cells[lastUsedRow++, 2].Value = CountIsNotPayedOrders;
                    worksheet.Cells[lastUsedRow++, 2].Value = SumPay.ToString() + " руб.";

                    package.Save();

                    return package;
                }
                else
                {
                    Debug.WriteLine("Orders collection is empty.");
                    return null;
                }
                
            }
            catch (Exception e)
            {
                Debug.WriteLine($"An error occurred during Excel export: {e}");
                return null;
            }
        }

        [Obsolete]
        private async Task SaveFileAsAsync(ExcelPackage package)
        {
            try
            {
                var saveFileDialog = new SaveFileDialog
                {
                    Title = "Save Excel File",
                    DefaultExtension = "xlsx",
                    Filters = new List<FileDialogFilter>
                {
                    new FileDialogFilter { Name = "Excel Files", Extensions = new List<string> { "xlsx" } }
                }
                };

                var result = await saveFileDialog.ShowAsync(new Window());

                if (result != null)
                {
                    await package.SaveAsAsync(result);
                }
                else
                {

                }
            }
            catch(Exception ex) 
            {
                Debug.WriteLine(ex);
            }
        }

        private string? CreateReportActiveWorkShift()
        {

            Orders = DatabaseHelper.refreshEntity<Order>(
                 x => x.Payment,
                 x => x.Place,
                 x => x.Workshift,
                 x => x.User,
                 x => x.Statusorder
            );

            if (Orders.Count == 0)
            {
                return "В настоящее время ещё нет заказов";
            }

            var activeWorkShiftid = WorkShiftHelper.getActiveWorkshift();

            if (activeWorkShiftid == null)
            {
                return "В настоящее время нет активной смены";
            }

            var listOrdersOfActiveWorkShift = DatabaseHelper.refreshEntity<Order>()
                .Where(x => x.WorkshiftId == activeWorkShiftid.WorkshiftId);

            var countOrders = listOrdersOfActiveWorkShift.Count();

            if (countOrders == 0)
            {
                return "В настоящее время ещё нет за активную смену";
            }

            SumPay = listOrdersOfActiveWorkShift.Sum(x => x.Payment.Sum);
            CountOrders = countOrders;
            CountIsCash = listOrdersOfActiveWorkShift.Where(x => !x.Payment.Isnoncash ?? false).Count();
            CountIsNotPayedOrders = listOrdersOfActiveWorkShift.Where(x => x.Payment.Isnoncash == null).Count();
            CountIsCreditCard = listOrdersOfActiveWorkShift.Where(x => x.Payment.Isnoncash ?? false).Count();
            Orders = new(listOrdersOfActiveWorkShift);

            return null;
        }

        private string? CreateReportOfWaiter()
        {
            Orders = DatabaseHelper.refreshEntity<Order>(
                x => x.Payment,
                x => x.Place,
                x => x.Workshift,
                x => x.User,
                x => x.Statusorder
            );

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

            var listOrdersOfActiveWorkShift = DatabaseHelper.refreshEntity<Order>()
                .Where(x => x.WorkshiftId == activeWorkShiftid.WorkshiftId && x.UserId == userId);

            var countOrders = listOrdersOfActiveWorkShift.Count();

            if (countOrders == 0)
            {
                return "В настоящее время ещё нет за активную смену";
            }

            SumPay = listOrdersOfActiveWorkShift.Sum(x => x.Payment.Sum);
            CountOrders = countOrders;
            
            CountIsCash = listOrdersOfActiveWorkShift.Where(x => !x.Payment.Isnoncash ?? false).Count();
            CountIsNotPayedOrders = listOrdersOfActiveWorkShift.Where(x => x.Payment.Isnoncash == null).Count();
            CountIsCreditCard = listOrdersOfActiveWorkShift.Where(x => x.Payment.Isnoncash ?? false).Count();
            Orders = new(listOrdersOfActiveWorkShift);

            return null;
        }


    }
}
