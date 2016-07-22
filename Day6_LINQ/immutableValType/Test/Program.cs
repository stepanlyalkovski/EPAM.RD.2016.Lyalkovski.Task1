using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    class Program
    {
        public delegate void MyDelegate(int x);

        public delegate void Mydelegate2(string str);
        static void Main(string[] args)
        {
            int[] nums = {1, 2, 3, 4, 5};
            var strings = nums.MyCustomSelect(n => "number " + n.ToString());
            foreach (var s in strings)
            {
                Console.WriteLine(s);
            }
        }


    }
}
