﻿// This file is part of Emzi0767.Common project
//
// Copyright 2019 Emzi0767
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//   http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System.Collections.Generic;

namespace System.Threading.Tasks
{
    /// <summary>
    /// Implementation of asynchronous event. The handlers of such events are executed asynchronously, but sequentially.
    /// </summary>
    /// <typeparam name="TSender">Type of the object that dispatches this event.</typeparam>
    /// <typeparam name="TArgs">Type of event argument object passed to this event's handlers.</typeparam>
    public sealed class AsyncEvent<TSender, TArgs> where TArgs : AsyncEventArgs
    {
        /// <summary>
        /// Gets the name of this event.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the maximum alloted execution time for all handlers. Any event which causes the handler to time out will raise a non-fatal <see cref="AsyncEventTimeoutException{TSender, TArgs}"/>.
        /// </summary>
        public TimeSpan MaximumExecutionTime { get; }

        private readonly object _lock;
        private readonly List<AsyncEventHandler<TSender, TArgs>> _handlers;
        private readonly AsyncEventExceptionHandler<TSender, TArgs> _exceptionHandler;

        /// <summary>
        /// Creates a new asynchronous event with specified name and exception handler.
        /// </summary>
        /// <param name="name">Name of this event.</param>
        /// <param name="maxExecutionTime">Maximum handler execution time. A value of <see cref="TimeSpan.Zero"/> means infinite.</param>
        /// <param name="exceptionHandler">Delegate which handles exceptions caused by this event.</param>
        public AsyncEvent(string name, TimeSpan maxExecutionTime, AsyncEventExceptionHandler<TSender, TArgs> exceptionHandler)
        {
            this._lock = new object();
            this._handlers = new List<AsyncEventHandler<TSender, TArgs>>();
            this._exceptionHandler = exceptionHandler;

            this.Name = name;
            this.MaximumExecutionTime = maxExecutionTime;
        }

        /// <summary>
        /// Registers a new handler for this event.
        /// </summary>
        /// <param name="handler">Handler to register for this event.</param>
        public void Register(AsyncEventHandler<TSender, TArgs> handler)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            lock (this._lock)
                this._handlers.Add(handler);
        }

        /// <summary>
        /// Unregisters an existing handler from this event.
        /// </summary>
        /// <param name="handler">Handler to unregister from the event.</param>
        public void Unregister(AsyncEventHandler<TSender, TArgs> handler)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            lock (this._lock)
                this._handlers.Remove(handler);
        }

        /// <summary>
        /// <para>Raises this event by invoking all of its registered handlers, in order of registration.</para>
        /// <para>All exceptions throw during invokation will be handled by the event's registered exception handler.</para>
        /// </summary>
        /// <param name="sender">Object which raised this event.</param>
        /// <param name="e">Arguments for this event.</param>
        /// <returns></returns>
        public async Task InvokeAsync(TSender sender, TArgs e)
        {
            AsyncEventHandler<TSender, TArgs>[] handlers;
            lock (this._lock)
                handlers = this._handlers.ToArray();

            if (handlers.Length == 0)
                return;

            // If we have a timeout configured, start the timeout task
            var timeout = this.MaximumExecutionTime > TimeSpan.Zero ? Task.Delay(this.MaximumExecutionTime) : null;
            for (var i = 0; i < handlers.Length; i++)
            {
                var handler = handlers[i];
                try
                {
                    // Start the handler execution
                    var handlerTask = handler(sender, e);
                    if (timeout != null)
                    {
                        // If timeout is configured, wait for any task to finish
                        // If the timeout task finishes first, the handler is causing a timeout

                        var result = await Task.WhenAny(handlerTask, timeout).ConfigureAwait(false);
                        if (result == timeout)
                        {
                            // Notify about the timeout and complete execution
                            timeout = null;
                            this.HandleException(new AsyncEventTimeoutException<TSender, TArgs>(this, handler), handler, sender);
                            await handlerTask.ConfigureAwait(false);
                        }
                    }
                    else
                    {
                        // No timeout is configured, or timeout already expired, proceed as usual

                        await handlerTask.ConfigureAwait(false);
                    }

                    if (e.Handled)
                        break;
                }
                catch (Exception ex)
                {
                    e.Handled = false;
                    this.HandleException(ex, handler, sender);
                }
            }
        }

        private void HandleException(Exception ex, AsyncEventHandler<TSender, TArgs> handler, TSender sender)
            => this._exceptionHandler(this, ex, handler, sender);
    }
}
