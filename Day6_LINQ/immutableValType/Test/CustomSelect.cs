using System;
using System.Collections.Generic;

namespace Test
{
    public static class CustomSelect
    {
        public static IEnumerable<TOut> MyCustomSelect<TIn,TOut>(this IEnumerable<TIn> collection, 
                        Func<TIn, TOut> predicate)
        {
            foreach (var element in collection)
            {
                yield return predicate(element);
            }
        }
    }
}