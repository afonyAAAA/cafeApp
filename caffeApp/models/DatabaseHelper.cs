using caffeApp.Desktop;
using caffeApp.Sources;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace caffeApp.models
{
    public static class DatabaseHelper
    {
        public static long GetIdentityCounterValue(string sequenceName)
        {
            var sql = $"SELECT last_value FROM {sequenceName}";

            using (var command = DbContextProvider.GetContext().Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = sql;
                DbContextProvider.GetContext().Database.OpenConnection();

                using (var result = command.ExecuteReader())
                {
                    if (result.Read())
                    {
                        return Convert.ToInt64(result[0]);
                    }
                }
            }

            throw new Exception("Не удалось получить следующее значение для последовательности.");
        }
    }
}
