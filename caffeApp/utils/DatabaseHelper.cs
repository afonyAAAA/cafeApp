using caffeApp.Desktop;
using caffeApp.Sources;
using DynamicData.Binding;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace caffeApp.utils
{
    public static class DatabaseHelper
    {
        public static ObservableCollection<T> refreshEntity<T>(params Expression<Func<T, object>>[] includes) where T : class
        {
            IQueryable<T> query = DbContextProvider.GetContext().Set<T>();

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            ObservableCollection<T> freshData = new ObservableCollection<T>(query);
            return freshData;
        }
    }
}
