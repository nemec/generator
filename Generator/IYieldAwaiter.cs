using System;

namespace GeneratorAsync
{
    public interface IYieldAwaiter : IDisposable
    {
        bool MoveNext();

        bool Send<TIn>(TIn obj);

        object ObjOut { get; }
    }

    public interface IYieldAwaiter<TIn, TOut> : IDisposable
    {
        bool MoveNext();

        bool Send(TIn obj);

        TOut ObjOut { get; }
    }
}
