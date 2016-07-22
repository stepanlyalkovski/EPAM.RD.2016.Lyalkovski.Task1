using System;
using System.Linq;

namespace immutableValType
{
    public interface IChangeable
    {
        void Change(int x, int y);
    }

    // Point  размерный тип.
    internal struct Point : IChangeable
    {
        private int _x, _y;
        public Point(int x, int y)
        {
            _x = x;
            _y = y;
        }
        public void Change(int x, int y)
        {
            _x = x; _y = y;
        }
        public override string ToString()
        {
            return string.Format("({0}, {1})", _x, _y);
        }
    }
    public sealed class Program
    {
        public static void Main()
        {
           var list1 = Enumerable.T
        }
    }
}
