using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace caffeApp.models
{
    public class SplitFood
    {

        public string Name { get; set; }

        public int Count { get; set; }

        public SplitFood(string name, int count)
        {
            Name = name;
            Count = count;
        } 
    }
}
