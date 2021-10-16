using System;
using System.Collections.Generic;

static class SuffleCollection
{
    private static Random s_rng = null;

    public static void Shuffle<T>(this IList<T> array, Random customRng = null)
    {
        customRng = s_rng = customRng ?? (s_rng = s_rng ?? new Random());
        int n = array.Count;
        while (n > 1)
        {
            int k = customRng.Next(n--);
            T temp = array[n];
            array[n] = array[k];
            array[k] = temp;
        }
    }
}