using System;

namespace GeneratorAsync
{
    public class Yielder : IYield
    {
        private readonly Generator _gen;

        public Yielder(Generator gen)
        {
            if (gen == null)
            {
                throw new ArgumentNullException("gen");
            }
            _gen = gen;
        }

        public YieldAwaiter<T> Yield<T>(T obj)
        {
            var awaiter = new YieldAwaiter<T>(obj);
            _gen.Awaiter = awaiter;
            return awaiter;
        }

        public YieldAwaiter<object> Yield()
        {
            var awaiter = new YieldAwaiter<object>(null);
            _gen.Awaiter = awaiter;
            return awaiter;
        }
    }

    public class Yielder<TIn, TOut> : IYield<TIn, TOut>
    {
        private readonly Generator<TIn, TOut> _gen;

        public Yielder(Generator<TIn, TOut> gen)
        {
            if (gen == null)
            {
                throw new ArgumentNullException("gen");
            }
            _gen = gen;
        }

        public YieldAwaiterWellDefined<TIn, TOut> Yield(TOut obj)
        {
            var awaiter = new YieldAwaiterWellDefined<TIn, TOut>(obj);
            _gen.Awaiter = awaiter;
            return awaiter;
        }

        public YieldAwaiterWellDefined<TIn, TOut> Yield()
        {
            var awaiter = new YieldAwaiterWellDefined<TIn, TOut>(default(TOut));
            _gen.Awaiter = awaiter;
            return awaiter;
        }
    }
}
