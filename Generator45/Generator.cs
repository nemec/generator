using System;
using System.Threading.Tasks;

namespace GeneratorAsync
{
    public class Generator<TState> : Generator
    {
        public Generator(Func<IYield, TState, Task> meth, TState state)
            : base(y => meth(y, state))
        {
        }
    }

    public class Generator : IDisposable
    {
        internal IYieldAwaiter Awaiter { get; set; }

        private readonly IYield _yielder;

        public Generator(Func<IYield, Task> meth)
        {
            if (meth == null)
            {
                throw new ArgumentNullException("meth");
            }
            _yielder = new Yielder(this);
            var task = meth(_yielder);
        }

        public TOut Next<TOut>()
        {
            var objOut = Awaiter.ObjOut;
            Awaiter.MoveNext();

            var fromType = objOut.GetType();
            var toType = typeof(TOut);
            if (!toType.IsAssignableFrom(fromType))
            {
                throw new ArgumentException(String.Format(
                    "Object '{0}' of type {1} cannot be assigned to {2}",
                    objOut, fromType, toType));
            }

            return (TOut)objOut;
        }

        public bool Send<T>(T obj)
        {
            var ret = Awaiter.Send(obj);
            return ret;
        }

        public TOut Send<TIn, TOut>(TIn obj)
        {
            var objOut = Awaiter.ObjOut;
            var ret = Awaiter.Send(obj);

            var fromType = objOut.GetType();
            var toType = typeof(TOut);
            if (!toType.IsAssignableFrom(fromType))
            {
                throw new ArgumentException(String.Format(
                    "Object '{0}' of type {1} cannot be assigned to {2}",
                    objOut, fromType, toType));
            }

            return (TOut)objOut;
        }

        public void Dispose()
        {
            if (Awaiter != null)
            {
                Awaiter.MoveNext();
                Awaiter.Dispose();
            }
        }
    }

    public class Generator<TIn, TOut> : IDisposable
    {
        internal IYieldAwaiter<TIn, TOut> Awaiter { get; set; }

        private readonly IYield<TIn, TOut> _yielder;

        public Generator(Func<IYield<TIn, TOut>, Task> meth)
        {
            if (meth == null)
            {
                throw new ArgumentNullException("meth");
            }
            _yielder = new Yielder<TIn, TOut>(this);
            meth(_yielder);
        }

        public TOut Next()
        {
            var objOut = Awaiter.ObjOut;
            Awaiter.MoveNext();
            return objOut;
        }

        public TOut Send(TIn obj)
        {
            var objOut = Awaiter.ObjOut;
            var ret = Awaiter.Send(obj);
            return objOut;
        }

        public void Dispose()
        {
            if (Awaiter != null)
            {
                Awaiter.MoveNext();
                Awaiter.Dispose();
            }
        }
    }
}
