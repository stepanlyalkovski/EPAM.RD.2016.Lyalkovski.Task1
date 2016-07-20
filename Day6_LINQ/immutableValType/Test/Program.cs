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
            MyDelegate del = x => { Console.WriteLine("First"); };
            MyDelegate del2 = x => { Console.WriteLine("Second"); };
            del2 += x => { Console.WriteLine("Another"); };
            Console.WriteLine(del.GetHashCode() == del2.GetHashCode());

            Mydelegate2 delegate2 = x => { Console.WriteLine(x); };
            Console.WriteLine(delegate2.GetHashCode() == del.GetHashCode());
        }


    }
}
