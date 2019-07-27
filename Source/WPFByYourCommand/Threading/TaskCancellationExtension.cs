using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace WPFByYourCommand.Threading
{
    [SuppressMessage("Qualité du code", "IDE0067:Supprimer les objets avant la mise hors de portée")]
    [SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope")]
    [SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task", Justification = "<En attente>")]
    public static class TaskCancellationExtension
    {
        /// <summary>
        /// add cancellation functionality to Task T 
        /// </summary>
        /// <param name="task"></param>
        /// <param name="cancellationToken"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="OperationCanceledException"></exception>
        public static async Task<T> CancelAfter<T>(
            this Task<T> task, CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<bool>();
            using (cancellationToken.Register(
                s => ((TaskCompletionSource<bool>)s).TrySetResult(true), tcs))
                if (task != await Task.WhenAny(task, tcs.Task))
                    throw new OperationCanceledException(cancellationToken);
            return await task;
        }



        /// <summary>
        /// add cancellation functionality to Task T with exception message 
        /// </summary>
        /// <param name="task"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="message"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="OperationCanceledException"></exception>
        public static async Task<T> CancelAfter<T>(
            this Task<T> task, string message, CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<bool>();
            using (cancellationToken.Register(
                s => ((TaskCompletionSource<bool>)s).TrySetResult(true), tcs))
                if (task != await Task.WhenAny(task, tcs.Task))
                    throw new OperationCanceledException(message, cancellationToken);
            return await task;
        }


        /// <summary>
        /// add cancellation functionality to Tasks 
        /// </summary>
        /// <param name="task"></param>
        /// <param name="cancellationToken"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="OperationCanceledException"></exception>
        public static async Task CancelAfter(
            this Task task, CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<bool>();
            using (cancellationToken.Register(
                s => ((TaskCompletionSource<bool>)s).TrySetResult(true), tcs))
                if (task != await Task.WhenAny(task, tcs.Task))
                    throw new OperationCanceledException(cancellationToken);
            await task;
        }


        /// <summary>
        /// add cancellation functionality to Tasks with exception message 
        /// </summary>
        /// <param name="task"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="message"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="OperationCanceledException"></exception>
        public static async Task CancelAfter(
            this Task task, string message, CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<bool>();
            using (cancellationToken.Register(
                s => ((TaskCompletionSource<bool>)s).TrySetResult(true), tcs))
                if (task != await Task.WhenAny(task, tcs.Task))
                    throw new OperationCanceledException(message, cancellationToken);
            await task;
        }


        /// <summary>
        /// add cancellation functionality to Task T
        /// </summary>
        /// <param name="task"></param>
        /// <param name="milliseconds"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="OperationCanceledException"></exception>
        public static async Task<T> CancelAfter<T>(
            this Task<T> task, int milliseconds)
        {
            var cts = new CancellationTokenSource();
            cts.CancelAfter(milliseconds);
            var tcs = new TaskCompletionSource<bool>();
            using (cts.Token.Register(
                s => ((TaskCompletionSource<bool>)s).TrySetResult(true), tcs))
                if (task != await Task.WhenAny(task, tcs.Task))
                    throw new OperationCanceledException(cts.Token);
            return await task;
        }


        /// <summary>
        /// add cancellation functionality to Task T with exception message
        /// </summary>
        /// <param name="task"></param>
        /// <param name="milliseconds"></param>
        /// <param name="message"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="OperationCanceledException"></exception>
        public static async Task<T> CancelAfter<T>(
            this Task<T> task, int milliseconds, string message)
        {
            var cts = new CancellationTokenSource();
            cts.CancelAfter(milliseconds);
            var tcs = new TaskCompletionSource<bool>();
            using (cts.Token.Register(
                s => ((TaskCompletionSource<bool>)s).TrySetResult(true), tcs))
                if (task != await Task.WhenAny(task, tcs.Task))
                    throw new OperationCanceledException(message, cts.Token);
            return await task;
        }

        /// <summary>
        /// add cancellation functionality to Task
        /// </summary>
        /// <param name="task"></param>
        /// <param name="milliseconds"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="OperationCanceledException"></exception>
        public static async Task CancelAfter(
            this Task task, int milliseconds)
        {
            var cts = new CancellationTokenSource();
            cts.CancelAfter(milliseconds);
            var tcs = new TaskCompletionSource<bool>();
            using (cts.Token.Register(
                s => ((TaskCompletionSource<bool>)s).TrySetResult(true), tcs))
                if (task != await Task.WhenAny(task, tcs.Task))
                    throw new OperationCanceledException(cts.Token);
            await task;
        }


        /// <summary>
        /// add cancellation functionality to Task with exception message
        /// </summary>
        /// <param name="task"></param>
        /// <param name="milliseconds"></param>
        /// <param name="message"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="OperationCanceledException"></exception>
        public static async Task CancelAfter(
            this Task task, int milliseconds, string message)
        {
            var cts = new CancellationTokenSource();
            cts.CancelAfter(milliseconds);
            var tcs = new TaskCompletionSource<bool>();
            using (cts.Token.Register(
                s => ((TaskCompletionSource<bool>)s).TrySetResult(true), tcs))
                if (task != await Task.WhenAny(task, tcs.Task))
                    throw new OperationCanceledException(message, cts.Token);
            await task;
        }
    }
}