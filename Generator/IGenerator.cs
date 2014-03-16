using System.Collections;

namespace Generator
{
    public interface IGenerator : IEnumerator
    {
        /// <summary>
        /// Move to and return the next value in the sequence.
        /// </summary>
        /// <typeparam name="TOut"></typeparam>
        /// <returns></returns>
        TOut Next<TOut>();

        /// <summary>
        /// Send a value into the generator and ignore the output.
        /// </summary>
        /// <typeparam name="TIn"></typeparam>
        /// <param name="obj"></param>
        void Send<TIn>(TIn obj);

        /// <summary>
        /// Send a value into the generator and return the output.
        /// </summary>
        /// <typeparam name="TIn"></typeparam>
        /// <typeparam name="TOut"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        TOut Send<TIn, TOut>(TIn obj);

        /// <summary>
        /// Attempt to send a value into the generator. Returns false
        /// if the end of the generator is reached.
        /// </summary>
        /// <typeparam name="TIn"></typeparam>
        /// <typeparam name="TOut"></typeparam>
        /// <param name="obj"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        bool TrySend<TIn, TOut>(TIn obj, out TOut result);
    }

    public interface IGenerator<in TIn, TOut> : IEnumerator
    {
        TOut Next();

        TOut Send(TIn obj);

        bool TrySend(TIn obj, out TOut result);
    }
}
