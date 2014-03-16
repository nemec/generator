using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Generator
{
    public static class Gen
    {
        /// <summary>
        /// Create a new generator from an existing IEnumerable.
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        public static IGenerator Create(IEnumerable<IYield> func)
        {
            return new Generator(func.GetEnumerator());
        }

        /// <summary>
        /// Create a new generator from a function that takes no parameters.
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        public static IGenerator Create(Func<IEnumerable<IYield>> func)
        {
            return new Generator(func().GetEnumerator());
        }
        /// <summary>
        /// Create a new generator from an existing IEnumerable.
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        public static IGenerator<TIn, TOut> Create<TIn, TOut>(IEnumerable<IYield<TIn, TOut>> func)
        {
            return new Generator<TIn, TOut>(func.GetEnumerator());
        }

        /// <summary>
        /// Create a new generator from a function that takes no parameters.
        /// The 
        /// </summary>
        /// <typeparam name="TIn"></typeparam>
        /// <typeparam name="TOut"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        public static IGenerator<TIn, TOut> Create<TIn, TOut>(Func<IEnumerable<IYield<TIn, TOut>>> func)
        {
            return new Generator<TIn, TOut>(func().GetEnumerator());
        }

        public static Yield<TIn, TOut> Yield<TIn, TOut>(TOut ret, Expression<Func<TIn>> cont)
        {
            return new Yield<TIn, TOut>(ret, cont);
        }

        public static Yield<TIn, object> Yield<TIn>(Expression<Func<TIn>> cont)
        {
            return new Yield<TIn, object>(null, cont);
        }

        public static Yield<object, TOut> Yield<TOut>(TOut ret)
        {
            return new Yield<object, TOut>(ret, null);
        }
    }

    public static class Gen<TIn>
    {
        public static Yield<TIn, TOut> Yield<TOut>(TOut ret)
        {
            return new Yield<TIn, TOut>(ret, null);
        }
    }
}
