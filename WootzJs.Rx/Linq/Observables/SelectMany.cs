﻿#region License

//-----------------------------------------------------------------------
// <copyright>
// The MIT License (MIT)
// 
// Copyright (c) 2014 Kirk S Woll
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the "Software"), to deal in
// the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do so,
// subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
// FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
// IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
// CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// </copyright>
//-----------------------------------------------------------------------

#endregion

using System.Collections.Generic;
using System.Reactive.Disposables;

namespace System.Reactive.Linq.Observables
{
    class SelectMany<TSource, TCollection, TResult> : Producer<TResult>
    {
        private readonly IObservable<TSource> _source;
        private readonly Func<TSource, IObservable<TCollection>> _collectionSelector;
        private readonly Func<TSource, IEnumerable<TCollection>> _collectionSelectorE;
        private readonly Func<TSource, TCollection, TResult> _resultSelector;

        public SelectMany(IObservable<TSource> source, Func<TSource, IObservable<TCollection>> collectionSelector, Func<TSource, TCollection, TResult> resultSelector)
        {
            _source = source;
            _collectionSelector = collectionSelector;
            _resultSelector = resultSelector;
        }

        public SelectMany(IObservable<TSource> source, Func<TSource, IEnumerable<TCollection>> collectionSelector, Func<TSource, TCollection, TResult> resultSelector)
        {
            _source = source;
            _collectionSelectorE = collectionSelector;
            _resultSelector = resultSelector;
        }

        protected override IDisposable Run(IObserver<TResult> observer, IDisposable cancel, Action<IDisposable> setSink)
        {
            if (_collectionSelector != null)
            {
                var sink = new _(this, observer, cancel);
                setSink(sink);
                return sink.Run();
            }
            else
            {
                var sink = new ε(this, observer, cancel);
                setSink(sink);
                return _source.SubscribeSafe(sink);
            }
        }

        class _ : Sink<TResult>, IObserver<TSource>
        {
            private readonly SelectMany<TSource, TCollection, TResult> _parent;

            public _(SelectMany<TSource, TCollection, TResult> parent, IObserver<TResult> observer, IDisposable cancel)
                : base(observer, cancel)
            {
                _parent = parent;
            }

            private bool _isStopped;
            private CompositeDisposable _group;
            private SingleAssignmentDisposable _sourceSubscription;

            public IDisposable Run()
            {
                _isStopped = false;
                _group = new CompositeDisposable();

                _sourceSubscription = new SingleAssignmentDisposable();
                _group.Add(_sourceSubscription);
                _sourceSubscription.Disposable = _parent._source.SubscribeSafe(this);

                return _group;
            }

            public void OnNext(TSource value)
            {
                IObservable<TCollection> collection;

                try
                {
                    collection = _parent._collectionSelector(value);
                }
                catch (Exception ex)
                {
                    _observer.OnError(ex);
                    base.Dispose();
                    return;
                }

                var innerSubscription = new SingleAssignmentDisposable();
                _group.Add(innerSubscription);
                innerSubscription.Disposable = collection.SubscribeSafe(new ι(this, value, innerSubscription));
            }

            public void OnError(Exception error)
            {
                _observer.OnError(error);
                base.Dispose();
            }

            public void OnCompleted()
            {
                _isStopped = true;
                if (_group.Count == 1)
                {
                    //
                    // Notice there can be a race between OnCompleted of the source and any
                    // of the inner sequences, where both see _group.Count == 1, and one is
                    // waiting for the lock. There won't be a double OnCompleted observation
                    // though, because the call to Dispose silences the observer by swapping
                    // in a NopObserver<T>.
                    //
                    _observer.OnCompleted();
                    base.Dispose();
                }
                else
                {
                    _sourceSubscription.Dispose();
                }
            }

            class ι : IObserver<TCollection>
            {
                private readonly _ _parent;
                private readonly TSource _value;
                private readonly IDisposable _self;

                public ι(_ parent, TSource value, IDisposable self)
                {
                    _parent = parent;
                    _value = value;
                    _self = self;
                }

                public void OnNext(TCollection value)
                {
                    TResult res;

                    try
                    {
                        res = _parent._parent._resultSelector(_value, value);
                    }
                    catch (Exception ex)
                    {
                        _parent._observer.OnError(ex);
                        _parent.Dispose();
                        return;
                    }

                    _parent._observer.OnNext(res);
                }

                public void OnError(Exception error)
                {
                    _parent._observer.OnError(error);
                    _parent.Dispose();
                }

                public void OnCompleted()
                {
                    _parent._group.Remove(_self);
                    if (_parent._isStopped && _parent._group.Count == 1)
                    {
                        //
                        // Notice there can be a race between OnCompleted of the source and any
                        // of the inner sequences, where both see _group.Count == 1, and one is
                        // waiting for the lock. There won't be a double OnCompleted observation
                        // though, because the call to Dispose silences the observer by swapping
                        // in a NopObserver<T>.
                        //
                        _parent._observer.OnCompleted();
                        _parent.Dispose();
                    }
                }
            }
        }

        class ε : Sink<TResult>, IObserver<TSource>
        {
            private readonly SelectMany<TSource, TCollection, TResult> _parent;

            public ε(SelectMany<TSource, TCollection, TResult> parent, IObserver<TResult> observer, IDisposable cancel)
                : base(observer, cancel)
            {
                _parent = parent;
            }

            public void OnNext(TSource value)
            {
                IEnumerable<TCollection> xs;
                try
                {
                    xs = _parent._collectionSelectorE(value);
                }
                catch (Exception exception)
                {
                    _observer.OnError(exception);
                    base.Dispose();
                    return;
                }

                IEnumerator<TCollection> e;
                try
                {
                    e = xs.GetEnumerator();
                }
                catch (Exception exception)
                {
                    _observer.OnError(exception);
                    base.Dispose();
                    return;
                }

                try
                {
                    var hasNext = true;
                    while (hasNext)
                    {
                        var current = default(TResult);

                        try
                        {
                            hasNext = e.MoveNext();
                            if (hasNext)
                                current = _parent._resultSelector(value, e.Current);
                        }
                        catch (Exception exception)
                        {
                            _observer.OnError(exception);
                            base.Dispose();
                            return;
                        }

                        if (hasNext)
                            _observer.OnNext(current);
                    }
                }
                finally
                {
                    e.Dispose();
                }
            }

            public void OnError(Exception error)
            {
                _observer.OnError(error);
                base.Dispose();
            }

            public void OnCompleted()
            {
                _observer.OnCompleted();
                base.Dispose();
            }
        }
    }

    class SelectMany<TSource, TResult> : Producer<TResult>
    {
        private readonly IObservable<TSource> _source;
        private readonly Func<TSource, IObservable<TResult>> _selector;
        private readonly Func<Exception, IObservable<TResult>> _selectorOnError;
        private readonly Func<IObservable<TResult>> _selectorOnCompleted;
        private readonly Func<TSource, IEnumerable<TResult>> _selectorE;

        public SelectMany(IObservable<TSource> source, Func<TSource, IObservable<TResult>> selector)
        {
            _source = source;
            _selector = selector;
        }

        public SelectMany(IObservable<TSource> source, Func<TSource, IObservable<TResult>> selector, Func<Exception, IObservable<TResult>> selectorOnError, Func<IObservable<TResult>> selectorOnCompleted)
        {
            _source = source;
            _selector = selector;
            _selectorOnError = selectorOnError;
            _selectorOnCompleted = selectorOnCompleted;
        }

        public SelectMany(IObservable<TSource> source, Func<TSource, IEnumerable<TResult>> selector)
        {
            _source = source;
            _selectorE = selector;
        }

        protected override IDisposable Run(IObserver<TResult> observer, IDisposable cancel, Action<IDisposable> setSink)
        {
            if (_selector != null)
            {
                var sink = new _(this, observer, cancel);
                setSink(sink);
                return sink.Run();
            }
            else
            {
                var sink = new ε(this, observer, cancel);
                setSink(sink);
                return _source.SubscribeSafe(sink);
            }
        }

        class _ : Sink<TResult>, IObserver<TSource>
        {
            private readonly SelectMany<TSource, TResult> _parent;

            public _(SelectMany<TSource, TResult> parent, IObserver<TResult> observer, IDisposable cancel)
                : base(observer, cancel)
            {
                _parent = parent;
            }

            private bool _isStopped;
            private CompositeDisposable _group;
            private SingleAssignmentDisposable _sourceSubscription;

            public IDisposable Run()
            {
                _isStopped = false;
                _group = new CompositeDisposable();

                _sourceSubscription = new SingleAssignmentDisposable();
                _group.Add(_sourceSubscription);
                _sourceSubscription.Disposable = _parent._source.SubscribeSafe(this);

                return _group;
            }

            public void OnNext(TSource value)
            {
                IObservable<TResult> inner;

                try
                {
                    inner = _parent._selector(value);
                }
                catch (Exception ex)
                {
                    _observer.OnError(ex);
                    base.Dispose();
                    return;
                }

                SubscribeInner(inner);
            }

            public void OnError(Exception error)
            {
                if (_parent._selectorOnError != null)
                {
                    IObservable<TResult> inner;

                    try
                    {
                        inner = _parent._selectorOnError(error);
                    }
                    catch (Exception ex)
                    {
                        _observer.OnError(ex);
                        base.Dispose();
                        return;
                    }

                    SubscribeInner(inner);

                    Final();
                }
                else
                {
                    _observer.OnError(error);

                    base.Dispose();
                }
            }

            public void OnCompleted()
            {
                if (_parent._selectorOnCompleted != null)
                {
                    IObservable<TResult> inner;

                    try
                    {
                        inner = _parent._selectorOnCompleted();
                    }
                    catch (Exception ex)
                    {
                        _observer.OnError(ex);
                        base.Dispose();
                        return;
                    }

                    SubscribeInner(inner);
                }

                Final();
            }

            private void Final()
            {
                _isStopped = true;
                if (_group.Count == 1)
                {
                    //
                    // Notice there can be a race between OnCompleted of the source and any
                    // of the inner sequences, where both see _group.Count == 1, and one is
                    // waiting for the lock. There won't be a double OnCompleted observation
                    // though, because the call to Dispose silences the observer by swapping
                    // in a NopObserver<T>.
                    //
                    _observer.OnCompleted();
                    base.Dispose();
                }
                else
                {
                    _sourceSubscription.Dispose();
                }
            }

            private void SubscribeInner(IObservable<TResult> inner)
            {
                var innerSubscription = new SingleAssignmentDisposable();
                _group.Add(innerSubscription);
                innerSubscription.Disposable = inner.SubscribeSafe(new ι(this, innerSubscription));
            }

            class ι : IObserver<TResult>
            {
                private readonly _ _parent;
                private readonly IDisposable _self;

                public ι(_ parent, IDisposable self)
                {
                    _parent = parent;
                    _self = self;
                }

                public void OnNext(TResult value)
                {
                    _parent._observer.OnNext(value);
                }

                public void OnError(Exception error)
                {
                    _parent._observer.OnError(error);
                    _parent.Dispose();
                }

                public void OnCompleted()
                {
                    _parent._group.Remove(_self);
                    if (_parent._isStopped && _parent._group.Count == 1)
                    {
                        //
                        // Notice there can be a race between OnCompleted of the source and any
                        // of the inner sequences, where both see _group.Count == 1, and one is
                        // waiting for the lock. There won't be a double OnCompleted observation
                        // though, because the call to Dispose silences the observer by swapping
                        // in a NopObserver<T>.
                        //
                        _parent._observer.OnCompleted();
                        _parent.Dispose();
                    }
                }
            }
        }

        class ε : Sink<TResult>, IObserver<TSource>
        {
            private readonly SelectMany<TSource, TResult> _parent;

            public ε(SelectMany<TSource, TResult> parent, IObserver<TResult> observer, IDisposable cancel)
                : base(observer, cancel)
            {
                _parent = parent;
            }

            public void OnNext(TSource value)
            {
                IEnumerable<TResult> xs;
                try
                {
                    xs = _parent._selectorE(value);
                }
                catch (Exception exception)
                {
                    _observer.OnError(exception);
                    base.Dispose();
                    return;
                }

                IEnumerator<TResult> e;
                try
                {
                    e = xs.GetEnumerator();
                }
                catch (Exception exception)
                {
                    _observer.OnError(exception);
                    base.Dispose();
                    return;
                }

                try
                {
                    var hasNext = true;
                    while (hasNext)
                    {
                        var current = default(TResult);

                        try
                        {
                            hasNext = e.MoveNext();
                            if (hasNext)
                                current = e.Current;
                        }
                        catch (Exception exception)
                        {
                            _observer.OnError(exception);
                            base.Dispose();
                            return;
                        }

                        if (hasNext)
                            _observer.OnNext(current);
                    }
                }
                finally
                {
                    e.Dispose();
                }
            }

            public void OnError(Exception error)
            {
                _observer.OnError(error);
                base.Dispose();
            }

            public void OnCompleted()
            {
                _observer.OnCompleted();
                base.Dispose();
            }
        }
    }
}