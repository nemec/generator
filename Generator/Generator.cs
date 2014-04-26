using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GeneratorAsync
{
    /// <summary>
    /// A generator that takes a single state argument in the method
    /// "constructor".
    /// </summary>
    /// <typeparam name="TState"></typeparam>
    public class Generator<TState> : Generator
    {
        /// <summary>
        /// A generator that takes a single state argument in the method
        /// "constructor".
        /// </summary>
        /// <param name="meth"></param>
        /// <param name="state"></param>
        public Generator(Func<IYield, TState, Task> meth, TState state)
            : base(y => meth(y, state))
        {
        }
    }

    public class Generator : IGenerator, IDisposable
    {
        internal IYieldAwaiter Awaiter { get; set; }

        private readonly IYield _yielder;

        private bool _generatorExhausted;

        public Generator(Func<IYield, Task> meth)
        {
            if (meth == null)
            {
                throw new ArgumentNullException("meth");
            }
            _yielder = new Yielder(this);
            _generatorExhausted = false;
            var task = meth(_yielder);
        }

        public TOut Next<TOut>()
        {
            var objOut = Awaiter.ObjOut;
            if(!Awaiter.MoveNext())
            {
                if (_generatorExhausted)
                {
                    //throw new GeneratorExhaustedException();
                }
                _generatorExhausted = true;
            }

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

        public bool TryNext<TOut>(out TOut result)
        {
            var objOut = Awaiter.ObjOut;
            var fromType = objOut.GetType();
            var toType = typeof(TOut);

            if(Awaiter.MoveNext() && toType.IsAssignableFrom(fromType))
            {
                result = (TOut)objOut;
                return true;
            }
            result = default(TOut);
            return false;
        }

        public TOut Send<TIn, TOut>(TIn obj)
        {
            var objOut = Awaiter.ObjOut;
            if (!Awaiter.Send(obj))
            {
                if (_generatorExhausted)
                {
                    //throw new GeneratorExhaustedException();
                }
                _generatorExhausted = true;
            }

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

        public void Send<TIn>(TIn obj)
        {
            if (!Awaiter.Send(obj))
            {
                if (_generatorExhausted)
                {
                    throw new GeneratorExhaustedException();
                }
                _generatorExhausted = true;
            }
        }

        public bool TrySend<T>(T obj)
        {
            var ret = Awaiter.Send(obj);
            return ret;
        }

        public bool TrySend<TIn, TOut>(TIn obj, out TOut result)
        {
            var objOut = Awaiter.ObjOut;

            var fromType = objOut.GetType();
            var toType = typeof(TOut);

            if (Awaiter.Send(obj) && toType.IsAssignableFrom(fromType))
            {
                result = (TOut)objOut;
                return true;
            }
            result = default(TOut);
            return false;
        }

        /// <summary>
        /// Convert the generator into an Enumerable of a single type.
        /// You are unable to send values into the generator when it's
        /// an Enumerable, but you gain the ability to use LINQ.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IEnumerable<T> AsEnumerable<T>()
        {
            T result;
            while (TryNext(out result))
            {
                yield return result;
            }
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

    public class Generator<TIn, TOut> : IGenerator<TIn, TOut>, IDisposable
    {
        internal IYieldAwaiter<TIn, TOut> Awaiter { get; set; }

        private readonly IYield<TIn, TOut> _yielder;

        private bool _generatorExhausted;

        public Generator(Func<IYield<TIn, TOut>, Task> meth)
        {
            if (meth == null)
            {
                throw new ArgumentNullException("meth");
            }
            _yielder = new Yielder<TIn, TOut>(this);
            _generatorExhausted = false;
            meth(_yielder);
        }

        public TOut Next()
        {
            var objOut = Awaiter.ObjOut;
            if(!Awaiter.MoveNext())
            {
                if (_generatorExhausted)
                {
                    //throw new GeneratorExhaustedException();
                }
                _generatorExhausted = true;
            }
            return objOut;
        }

        public bool TryNext(out TOut result)
        {
            var objOut = Awaiter.ObjOut;
            if(Awaiter.MoveNext())
            {
                result = objOut;
                return true;
            }
            result = default(TOut);
            return false;
        }

        public TOut Send(TIn obj)
        {
            var objOut = Awaiter.ObjOut;
            if(!Awaiter.Send(obj))
            {
                if (_generatorExhausted)
                {
                    throw new GeneratorExhaustedException();
                }
                _generatorExhausted = true;
            }
            return objOut;
        }

        public bool TrySend(TIn obj, out TOut result)
        {
            var objOut = Awaiter.ObjOut;
            if (Awaiter.Send(obj))
            {
                result = objOut;
                return true;
            }
            result = default(TOut);
            return false;
        }

        public IEnumerable<TOut> AsEnumerable()
        {
            TOut value;
            while (TryNext(out value))
            {
                yield return value;
            }
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
