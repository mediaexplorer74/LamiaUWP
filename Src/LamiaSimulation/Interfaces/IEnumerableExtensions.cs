using System;
using System.Collections.Generic;
using System.Linq;

namespace LamiaSimulation
{
    // Stolen from cscore
    // https://github.com/cs-util-com/cscore/
    public static class IEnumerableExtensions
    {
        public static IEnumerable<R> Map<T, R>(this IEnumerable<T> self, Func<T, R> selector)
        {
            return self.Select(selector);
        }

        public static T Reduce<T>(this IEnumerable<T> self, Func<T, T, T> func)
        {
            return self.Aggregate(func);
        }

        public static R Reduce<T, R>(this IEnumerable<T> self, R seed, Func<R, T, R> func)
        {
            return self.Aggregate<T, R>(seed, func);
        }

        public static IEnumerable<T> Filter<T>(this IEnumerable<T> self, Func<T, bool> predicate)
        {
            return self.Where(predicate);
        }

        public static void Apply<T>(this IEnumerable<T> self, Action<T> action)
        {
            foreach(T element in self)
                action(element);
        }
    }
}