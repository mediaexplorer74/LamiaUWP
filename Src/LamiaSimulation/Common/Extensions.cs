using System.Collections.Generic;

namespace LamiaSimulation;

static class Extensions
{
    /*
     * Stole this nice bit of a code from a SO post
     * https://stackoverflow.com/questions/7389047/is-there-an-equivalent-to-pythons-enumerate-for-net-ienumerable
     *
     * Usage:
     *   foreach(var(i, obj) in enumerableObject.Enumerate())
     *       Console.WriteLine($"{i}: {obj}");
     */
    public static IEnumerable<(int, T)> Enumerate<T>(this IEnumerable<T> input, int start = 0)
    {
        int i = start;
        foreach (var t in input)
            yield return (i++, t);
    }
}