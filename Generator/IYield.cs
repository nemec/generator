using System;
using System.Linq.Expressions;

namespace Generator
{
    public interface IYield
    {
        object Value { get; }
    }

    public interface IYield<in TIn, in TOut> : IYield
    {
    }

    public class Yield<TIn, TOut> : IYield<TIn, TOut>
    {
        public Yield(TOut value, Expression<Func<TIn>> setter)
        {
            Value = value;
            Setter = setter;
        }

        public object Value { get; private set; }

        public Expression<Func<TIn>> Setter { get; private set; } 
    }
}
