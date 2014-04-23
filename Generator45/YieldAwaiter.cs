
namespace GeneratorAsync
{
    public class YieldAwaiter<TOut> : YieldAwaiterBase<object, TOut>, IYieldAwaiter
    {
        public object ObjOut { get; private set; }

        internal YieldAwaiter(TOut objOut)
        {
            ObjOut = objOut;
        }

        public bool Send<TIn>(TIn obj)
        {
            _objIn = obj;
            return MoveNextPrv();
        }

        // custom Awaiter methods
        public YieldAwaiter<TOut> GetAwaiter()
        {
            return this;
        }

        public object GetResult()
        {
            return _objIn;
        }
    }
}
