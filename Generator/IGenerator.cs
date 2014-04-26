using System.Collections;

namespace GeneratorAsync
{
    public interface IGenerator
    {
        /// <summary>
        /// Move to and return the next value in the sequence.
        /// </summary>
        /// <typeparam name="TOut"></typeparam>
        /// <returns></returns>
        TOut Next<TOut>();

        /// <summary>
        /// Attempt to retrieve a value from the generator. Returns false
        /// if the end of the generator is reached or the result could
        /// not be converted to TOut.
        /// </summary>
        /// <typeparam name="TOut"></typeparam>
        /// <param name="result"></param>
        /// <returns></returns>
        bool TryNext<TOut>(out TOut result);

        /// <summary>
        /// Send a value into the generator and ignore the output.
        /// </summary>
        /// <typeparam name="TIn"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
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
        /// Send a value into the generator and ignore the output.
        /// </summary>
        /// <typeparam name="TIn"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        bool TrySend<TIn>(TIn obj);

        /// <summary>
        /// Attempt to send a value into the generator. Returns false
        /// if the end of the generator is reached or the result could
        /// not be converted to TOut.
        /// </summary>
        /// <typeparam name="TIn"></typeparam>
        /// <typeparam name="TOut"></typeparam>
        /// <param name="obj"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        bool TrySend<TIn, TOut>(TIn obj, out TOut result);
    }

    public interface IGenerator<in TIn, TOut>
    {
        /// <summary>
        /// Move to and return the next value in the sequence.
        /// </summary>
        /// <typeparam name="TOut"></typeparam>
        /// <returns></returns>
        TOut Next();

        /// <summary>
        /// Attempt to retrieve a value from the generator. Returns false
        /// if the end of the generator is reached or the result could
        /// not be converted to TOut.
        /// </summary>
        /// <typeparam name="TOut"></typeparam>
        /// <param name="result"></param>
        /// <returns></returns>
        bool TryNext(out TOut result);

        /// <summary>
        /// Send a value into the generator and return the output.
        /// </summary>
        /// <typeparam name="TIn"></typeparam>
        /// <typeparam name="TOut"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        TOut Send(TIn obj);

        /// <summary>
        /// Attempt to send a value into the generator. Returns false
        /// if the end of the generator is reached or the result could
        /// not be converted to TOut.
        /// </summary>
        /// <typeparam name="TIn"></typeparam>
        /// <typeparam name="TOut"></typeparam>
        /// <param name="obj"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        bool TrySend(TIn obj, out TOut result);
    }
}
