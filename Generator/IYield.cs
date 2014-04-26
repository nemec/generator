
using System;

namespace GeneratorAsync
{
    /// <summary>
    /// Allows a 
    /// </summary>
    public interface IYield
    {
        YieldAwaiter<T> Yield<T>(T obj);

        YieldAwaiter<object> Yield();
    }

    public interface IYield<TIn, TOut>
    {
        YieldAwaiterWellDefined<TIn, TOut> Yield(TOut obj);

        YieldAwaiterWellDefined<TIn, TOut> Yield();
    }
}
