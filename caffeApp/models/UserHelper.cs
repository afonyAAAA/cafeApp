using caffeApp.Desktop;
using caffeApp.Sources;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace caffeApp.models
{
    public static class UserHelper
    {
        public static User? getAuthorizedUserInfo()
        {
            User user = new User();

            try
            {
                string pathToFile = Path.Combine(Environment.CurrentDirectory, "UserData.json");
                string json = File.ReadAllText(pathToFile);
                user = JsonConvert.DeserializeObject<User>(json);

                if(user == null)
                {
                    return null;
                }
                else if (DbContextProvider.GetContext().Users.ToList().Find(x => x.UserId == user.UserId) == null)
                {
                    return null;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return user;
        }
    }
}
