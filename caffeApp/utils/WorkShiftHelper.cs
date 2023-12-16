using caffeApp.Desktop;
using MsBox.Avalonia.Enums;
using MsBox.Avalonia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;

namespace caffeApp.utils
{
    public static class WorkShiftHelper
    {
        public static Workshift? getActiveWorkshift()
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

                return null;
            }

            return workshift;
        }

        public static bool isActiveWorkShiftUser(int userId)
        {
            DateTime currentDateTime = DateTime.Now;
            TimeOnly currentTime = TimeOnly.FromDateTime(currentDateTime);
            DateOnly currentDateToDateOnly = DateOnly.FromDateTime(currentDateTime);

            var workshift = getActiveWorkshift();

            var userInWorkShift = DatabaseHelper.refreshEntity<Workshiftview>().FirstOrDefault(x =>
                x.WorkshiftId == workshift.WorkshiftId
                && x.UserId == userId
            );

            return userInWorkShift != null;
        }
    }
}
