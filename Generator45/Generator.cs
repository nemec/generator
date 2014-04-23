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

    public class Generator : IYield, IDisposable
    {
        private readonly Task _task;

        private IYieldAwaiter _awaiter;

        public Generator(Func<IYield, Task> meth)
        {
            if (meth != null)
            {
                _task = meth(this);
            }
        }

        public TOut Next<TOut>()
        {
            var objOut = _awaiter.ObjOut;
            _awaiter.MoveNext();

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
            var ret = _awaiter.Send(obj);
            return ret;
        }

        public TOut Send<TIn, TOut>(TIn obj)
        {
            var objOut = _awaiter.ObjOut;
            var ret = _awaiter.Send(obj);

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

        YieldAwaiter<T> IYield.Yield<T>(T obj)
        {
            var awaiter = new YieldAwaiter<T>(obj);
            _awaiter = awaiter;
            return awaiter;
        }

        YieldAwaiter<object> IYield.Yield()
        {
            var awaiter = new YieldAwaiter<object>(null);
            _awaiter = awaiter;
            return awaiter;
        }

        public void Dispose()
        {
            if (_awaiter != null)
            {
                _awaiter.MoveNext();
                _awaiter.Dispose();
            }
        }
    }

    public class Generator<TIn, TOut> : IYield<TIn, TOut>, IDisposable
    {
        private readonly Task _task;

        private IYieldAwaiter<TIn, TOut> _awaiter;

        public Generator(Func<IYield<TIn, TOut>, Task> meth)
        {
            if (meth != null)
            {
                _task = meth(this);
            }
        }

        public TOut Next()
        {
            var objOut = _awaiter.ObjOut;
            _awaiter.MoveNext();
            return objOut;
        }

        public TOut Send(TIn obj)
        {
            var objOut = _awaiter.ObjOut;
            var ret = _awaiter.Send(obj);
            return objOut;
        }

        YieldAwaiterWellDefined<TIn, TOut> IYield<TIn, TOut>.Yield(TOut obj)
        {
            var awaiter = new YieldAwaiterWellDefined<TIn, TOut>(obj);
            _awaiter = awaiter;
            return awaiter;
        }

        YieldAwaiterWellDefined<TIn, TOut> IYield<TIn, TOut>.Yield()
        {
            var awaiter = new YieldAwaiterWellDefined<TIn, TOut>(default(TOut));
            _awaiter = awaiter;
            return awaiter;
        }

        public void Dispose()
        {
            if (_awaiter != null)
            {
                _awaiter.MoveNext();
                _awaiter.Dispose();
            }
        }
    }
}
