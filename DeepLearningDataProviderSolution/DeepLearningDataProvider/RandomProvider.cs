using System;
using System.Threading;

namespace DeepLearningDataProvider
{
    /// <summary>
    /// https://stackoverflow.com/a/7251724
    /// </summary>
    public static class RandomProvider
    {
        static int seed = Environment.TickCount;

        static ThreadLocal<Random> randomWrapper = new ThreadLocal<Random>
            (() => new Random(Interlocked.Increment(ref seed)));

        public static Random GetThreadRandom()
        {
            return randomWrapper.Value;
        }
    }
}
