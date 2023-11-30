using caffeApp.Desktop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace caffeApp.Sources
{
    public class DbContextProvider
    {
        private static CafeContext _context;

        public static CafeContext GetContext() {
            return _context = new CafeContext();
        }
    }
}
