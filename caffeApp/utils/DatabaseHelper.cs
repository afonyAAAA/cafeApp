using caffeApp.Desktop;
using caffeApp.Sources;
using DynamicData.Binding;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace caffeApp.utils
{
    public static class DatabaseHelper
    {
        public static ObservableCollection<T> refreshEntity<T>() where T : class
        {
            ObservableCollection<T> freshData = new ObservableCollection<T>(DbContextProvider.GetContext().Set<T>().IgnoreAutoIncludes<T>());
            return freshData;
        }
    }
}
