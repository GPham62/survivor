using System;
using System.Collections.Generic;
using System.Threading;

namespace WingsMob.Survival.Utils
{
    public static class ListUtils
    {
        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = ThreadSafeRandom.ThisThreadsRandom.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static void ShuffleWithRange<T>(this IList<T> list, Random rng, int first, int count)
        {
            for (int n = count; n > 1;)
            {
                int k = rng.Next(n);
                --n;
                T temp = list[n + first];
                list[n + first] = list[k + first];
                list[k + first] = temp;
            }
        }

        public static class ThreadSafeRandom
        {
            [ThreadStatic] private static Random Local;

            public static Random ThisThreadsRandom
            {
                get { return Local ?? (Local = new Random(unchecked(System.Environment.TickCount * 31 + Thread.CurrentThread.ManagedThreadId))); }
            }
        }
    }
}
