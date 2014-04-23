using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace GeneratorAsync
{
    public abstract class YieldAwaiterBase<TIn, TOut> : INotifyCompletion, IDisposable
    {
        Action _continuation;
        readonly CancellationTokenSource _cts = new CancellationTokenSource();

        protected TIn _objIn;


        // resume after await, called upon external event
        public bool MoveNext()
        {
            _objIn = default(TIn);
            return MoveNextPrv();
        }

        public bool MoveNextPrv()
        {
            if (_continuation == null)
            {
                return false;
            }

            var continuation = _continuation;
            _continuation = null;
            continuation();
            return _continuation != null;
        }

        public bool IsCompleted
        {
            get { return false; }
        }

        // IDispose
        public void Dispose()
        {
            if (_continuation != null)
            {
                Cancel();
                MoveNext();
            }
        }

        // INotifyCompletion
        public void OnCompleted(Action continuation)
        {
            _continuation = continuation;
        }

        public void Cancel()
        {
            _cts.Cancel();
        }

        // let the client observe cancellation
        public CancellationToken Token { get { return _cts.Token; } }
    }
}
