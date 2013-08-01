using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;


namespace QuizEngine
{
    public static class ThreadSafeRandom
    {
        [DllImport("Kernel32.dll")]
        public static extern uint GetCurrentThreadId();

        [ThreadStatic]
        private static Random _local;

        public static Random ThisThreadsRandom
        {
            get { return _local ?? (_local = new Random(unchecked(Environment.TickCount * 31 + (int)GetCurrentThreadId()))); }
        }
    }

    static class ExtensionMethods
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
    }
}
