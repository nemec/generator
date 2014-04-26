
namespace GeneratorAsync
{
    public class YieldAwaiterWellDefined<TIn, TOut> : YieldAwaiterBase<TIn, TOut>, IYieldAwaiter<TIn, TOut>
    {
        public TOut ObjOut { get; private set; }

        internal YieldAwaiterWellDefined(TOut objOut)
        {
            ObjOut = objOut;
        }

        public bool Send(TIn obj)
        {
            _objIn = obj;
            return MoveNextPrv();
        }

        public YieldAwaiterWellDefined<TIn, TOut> GetAwaiter()
        {
            return this;
        }

        public TIn GetResult()
        {
            return _objIn;
        }
    }
}
