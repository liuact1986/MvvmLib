using System;

namespace MvvmLib.Mvvm
{
    /// <summary>
    /// Factory for Actions and Funcs
    /// </summary>
    public class DefaultDelegateFactory
    {
        /// <summary>
        /// Creates an empty <see cref="Action"/>.
        /// </summary>
        /// <returns>The action</returns>
        public Action GetAction0()
        {
            return new Action(() => { });
        }

        /// <summary>
        /// Creates an action with 1 parameter.
        /// </summary>
        /// <typeparam name="T1">Type of parameter 1</typeparam>
        /// <returns>The action</returns>
        public Action<T1> GetAction1<T1>()
        {
            return new Action<T1>((v1) => { });
        }

        /// <summary>
        /// Creates an action with 2 parameters.
        /// </summary>
        /// <typeparam name="T1">Type of parameter 1</typeparam>
        /// <typeparam name="T2">Type of parameter 2</typeparam>
        /// <returns>The action</returns>
        public Action<T1, T2> GetAction2<T1, T2>()
        {
            return new Action<T1, T2>((v1, v2) => { });
        }

        /// <summary>
        /// Creates an action with 3 parameters.
        /// </summary>
        /// <typeparam name="T1">Type of parameter 1</typeparam>
        /// <typeparam name="T2">Type of parameter 2</typeparam>
        /// <typeparam name="T3">Type of parameter 3</typeparam>
        /// <returns>The action</returns>
        public Action<T1, T2, T3> GetAction3<T1, T2, T3>()
        {
            return new Action<T1, T2, T3>((v1, v2, v3) => { });
        }

        /// <summary>
        /// Creates an action with 4 parameters.
        /// </summary>
        /// <typeparam name="T1">Type of parameter 1</typeparam>
        /// <typeparam name="T2">Type of parameter 2</typeparam>
        /// <typeparam name="T3">Type of parameter 3</typeparam>
        /// <typeparam name="T4">Type of parameter 4</typeparam>
        /// <returns>The action</returns>
        public Action<T1, T2, T3, T4> GetAction4<T1, T2, T3, T4>()
        {
            return new Action<T1, T2, T3, T4>((v1, v2, v3, v4) => { });
        }

        /// <summary>
        /// Creates an action with 5 parameters.
        /// </summary>
        /// <typeparam name="T1">Type of parameter 1</typeparam>
        /// <typeparam name="T2">Type of parameter 2</typeparam>
        /// <typeparam name="T3">Type of parameter 3</typeparam>
        /// <typeparam name="T4">Type of parameter 4</typeparam>
        /// <typeparam name="T5">Type of parameter 5</typeparam>
        /// <returns>The action</returns>
        public Action<T1, T2, T3, T4, T5> GetAction5<T1, T2, T3, T4, T5>()
        {
            return new Action<T1, T2, T3, T4, T5>((v1, v2, v3, v4, v5) => { });
        }

        /// <summary>
        /// Creates an action with 6 parameters.
        /// </summary>
        /// <typeparam name="T1">Type of parameter 1</typeparam>
        /// <typeparam name="T2">Type of parameter 2</typeparam>
        /// <typeparam name="T3">Type of parameter 3</typeparam>
        /// <typeparam name="T4">Type of parameter 4</typeparam>
        /// <typeparam name="T5">Type of parameter 5</typeparam>
        /// <typeparam name="T6">Type of parameter 6</typeparam>
        /// <returns>The action</returns>
        public Action<T1, T2, T3, T4, T5, T6> GetAction6<T1, T2, T3, T4, T5, T6>()
        {
            return new Action<T1, T2, T3, T4, T5, T6>((v1, v2, v3, v4, v5, v6) => { });
        }


        /// <summary>
        /// Creates an action with 7 parameters.
        /// </summary>
        /// <typeparam name="T1">Type of parameter 1</typeparam>
        /// <typeparam name="T2">Type of parameter 2</typeparam>
        /// <typeparam name="T3">Type of parameter 3</typeparam>
        /// <typeparam name="T4">Type of parameter 4</typeparam>
        /// <typeparam name="T5">Type of parameter 5</typeparam>
        /// <typeparam name="T6">Type of parameter 6</typeparam>
        /// <typeparam name="T7">Type of parameter 7</typeparam>
        /// <returns>The action</returns>
        public Action<T1, T2, T3, T4, T5, T6, T7> GetAction7<T1, T2, T3, T4, T5, T6, T7>()
        {
            return new Action<T1, T2, T3, T4, T5, T6, T7>((v1, v2, v3, v4, v5, v6, v7) => { });
        }

        /// <summary>
        /// Creates an action with 8 parameters.
        /// </summary>
        /// <typeparam name="T1">Type of parameter 1</typeparam>
        /// <typeparam name="T2">Type of parameter 2</typeparam>
        /// <typeparam name="T3">Type of parameter 3</typeparam>
        /// <typeparam name="T4">Type of parameter 4</typeparam>
        /// <typeparam name="T5">Type of parameter 5</typeparam>
        /// <typeparam name="T6">Type of parameter 6</typeparam>
        /// <typeparam name="T7">Type of parameter 7</typeparam>
        /// <typeparam name="T8">Type of parameter 8</typeparam>
        /// <returns>The action</returns>
        public Action<T1, T2, T3, T4, T5, T6, T7, T8> GetAction8<T1, T2, T3, T4, T5, T6, T7, T8>()
        {
            return new Action<T1, T2, T3, T4, T5, T6, T7, T8>((v1, v2, v3, v4, v5, v6, v7, v8) => { });
        }

        /// <summary>
        /// Creates a func.
        /// </summary>
        /// <typeparam name="TResult">Type of the result</typeparam>
        /// <returns>The func</returns>
        public Func<TResult> GetFunc1<TResult>()
        {
            return new Func<TResult>(() => default(TResult));
        }

        /// <summary>
        /// Creates a func with 1 parameter.
        /// </summary>
        /// <typeparam name="T1">Type of parameter 1</typeparam>
        /// <typeparam name="TResult">Type of the result</typeparam>
        /// <returns>The func</returns>
        public Func<T1, TResult> GetFunc2<T1, TResult>()
        {
            return new Func<T1, TResult>((v1) => default(TResult));
        }

        /// <summary>
        /// Creates a func with 2 parameters.
        /// </summary>
        /// <typeparam name="T1">Type of parameter 1</typeparam>
        /// <typeparam name="T2">Type of parameter 2</typeparam>
        /// <typeparam name="TResult">Type of the result</typeparam>
        /// <returns>The func</returns>
        public Func<T1, T2, TResult> GetFunc3<T1, T2, TResult>()
        {
            return new Func<T1, T2, TResult>((v1, v2) => default(TResult));
        }

        /// <summary>
        /// Creates a func with 3 parameters.
        /// </summary>
        /// <typeparam name="T1">Type of parameter 1</typeparam>
        /// <typeparam name="T2">Type of parameter 2</typeparam>
        /// <typeparam name="T3">Type of parameter 3</typeparam>
        /// <typeparam name="TResult">Type of the result</typeparam>
        /// <returns>The func</returns>
        public Func<T1, T2, T3, TResult> GetFunc4<T1, T2, T3, TResult>()
        {
            return new Func<T1, T2, T3, TResult>((v1, v2, v3) => default(TResult));
        }

        /// <summary>
        /// Creates a func with 4 parameters.
        /// </summary>
        /// <typeparam name="T1">Type of parameter 1</typeparam>
        /// <typeparam name="T2">Type of parameter 2</typeparam>
        /// <typeparam name="T3">Type of parameter 3</typeparam>
        /// <typeparam name="T4">Type of parameter 4</typeparam>
        /// <typeparam name="TResult">Type of the result</typeparam>
        /// <returns>The func</returns>
        public Func<T1, T2, T3, T4, TResult> GetFunc5<T1, T2, T3, T4, TResult>()
        {
            return new Func<T1, T2, T3, T4, TResult>((v1, v2, v3, v4) => default(TResult));
        }

        /// <summary>
        /// Creates a func with 5 parameters.
        /// </summary>
        /// <typeparam name="T1">Type of parameter 1</typeparam>
        /// <typeparam name="T2">Type of parameter 2</typeparam>
        /// <typeparam name="T3">Type of parameter 3</typeparam>
        /// <typeparam name="T4">Type of parameter 4</typeparam>
        /// <typeparam name="T5">Type of parameter 5</typeparam>
        /// <typeparam name="TResult">Type of the result</typeparam>
        /// <returns>The func</returns>
        public Func<T1, T2, T3, T4, T5, TResult> GetFunc6<T1, T2, T3, T4, T5, TResult>()
        {
            return new Func<T1, T2, T3, T4, T5, TResult>((v1, v2, v3, v4, v5) => default(TResult));
        }

        /// <summary>
        /// Creates a func with 6 parameters.
        /// </summary>
        /// <typeparam name="T1">Type of parameter 1</typeparam>
        /// <typeparam name="T2">Type of parameter 2</typeparam>
        /// <typeparam name="T3">Type of parameter 3</typeparam>
        /// <typeparam name="T4">Type of parameter 4</typeparam>
        /// <typeparam name="T5">Type of parameter 5</typeparam>
        /// <typeparam name="T6">Type of parameter 6</typeparam>
        /// <typeparam name="TResult">Type of the result</typeparam>
        /// <returns>The func</returns>
        public Func<T1, T2, T3, T4, T5, T6, TResult> GetFunc7<T1, T2, T3, T4, T5, T6, TResult>()
        {
            return new Func<T1, T2, T3, T4, T5, T6, TResult>((v1, v2, v3, v4, v5, v6) => default(TResult));
        }

        /// <summary>
        /// Creates a func with 7 parameters.
        /// </summary>
        /// <typeparam name="T1">Type of parameter 1</typeparam>
        /// <typeparam name="T2">Type of parameter 2</typeparam>
        /// <typeparam name="T3">Type of parameter 3</typeparam>
        /// <typeparam name="T4">Type of parameter 4</typeparam>
        /// <typeparam name="T5">Type of parameter 5</typeparam>
        /// <typeparam name="T6">Type of parameter 6</typeparam>
        /// <typeparam name="T7">Type of parameter 7</typeparam>
        /// <typeparam name="TResult">Type of the result</typeparam>
        /// <returns>The func</returns>
        public Func<T1, T2, T3, T4, T5, T6, T7, TResult> GetFunc8<T1, T2, T3, T4, T5, T6, T7, TResult>()
        {
            return new Func<T1, T2, T3, T4, T5, T6, T7, TResult>((v1, v2, v3, v4, v5, v6, v7) => default(TResult));
        }

        /// <summary>
        /// Creates a func with 8 parameters.
        /// </summary>
        /// <typeparam name="T1">Type of parameter 1</typeparam>
        /// <typeparam name="T2">Type of parameter 2</typeparam>
        /// <typeparam name="T3">Type of parameter 3</typeparam>
        /// <typeparam name="T4">Type of parameter 4</typeparam>
        /// <typeparam name="T5">Type of parameter 5</typeparam>
        /// <typeparam name="T6">Type of parameter 6</typeparam>
        /// <typeparam name="T7">Type of parameter 7</typeparam>
        /// <typeparam name="T8">Type of parameter 8</typeparam>
        /// <typeparam name="TResult">Type of the result</typeparam>
        /// <returns>The func</returns>
        public Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> GetFunc9<T1, T2, T3, T4, T5, T6, T7, T8, TResult>()
        {
            return new Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult>((v1, v2, v3, v4, v5, v6, v7, v8) => default(TResult));
        }
    }
}
