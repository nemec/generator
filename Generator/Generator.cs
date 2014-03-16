using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Reflection;

namespace Generator
{
    public class Generator : IGenerator
    {
        private readonly IEnumerator<IYield> _enum;

        private IYield _lastValue;

        internal Generator(IEnumerator<IYield> enm)
        {
            _enum = enm;
            _lastValue = null;
        }

        public TOut Next<TOut>()
        {
            //MoveNext();
            if (!MoveNext())
            {
                throw new Exception();
            }
            return (TOut) _enum.Current.Value;
        }

        public bool MoveNext()
        {
            return SendNext((Object)null);
        }

        public TOut Send<TIn, TOut>(TIn obj)
        {
            SendNext(obj);
            return (TOut)_lastValue.Value;
        }

        public void Send<TIn>(TIn obj)
        {
            SendNext(obj);
        }

        public bool TrySend<TIn, TOut>(TIn obj, out TOut response)
        {
            if (SendNext(obj))
            {
                response = (TOut) _lastValue.Value;
                return true;
            }
            response = default(TOut);
            return false;
        }

        private bool SendNext<T>(T obj)
        {
            if (_lastValue == null && !ReferenceEquals(obj, null))
            {
                /*throw new InvalidOperationException(
                    "Cannot send value to uninitialized generator. " +
                    "Call MoveNext instead to advance to the first breakpoint.");*/
                MoveNext();
            }

            _lastValue = _enum.Current;

            // Do reflection magic to set value
            var continuation = _lastValue as Yield<T, object>;
            if (continuation != null && continuation.Setter != null)
            {
                GetterToSetterExpressionVisitor.VisitAndSet(obj, continuation.Setter);
            }

            return _enum.MoveNext();
        }

        public object Current
        {
            get { return _enum.Current; }
        }

        public void Reset()
        {
            throw new InvalidOperationException();
        }

        private class GetterToSetterExpressionVisitor : ExpressionVisitor
        {
            private GetterToSetterExpressionVisitor(object newValue)
            {
                _newValue = newValue;
            }

            private readonly object _newValue;

            private object _setterInstance;

            private int _memberAccessDepth;

            public static void VisitAndSet(object newValue, Expression setter)
            {
                var visitor = new GetterToSetterExpressionVisitor(newValue);
                visitor.Visit(setter);
            }

            protected override Expression VisitMemberAccess(MemberExpression m)
            {
                _memberAccessDepth++;
                var result = base.VisitMemberAccess(m);
                _memberAccessDepth--;

                if (_memberAccessDepth == 0)  // Set the new value
                {
                    var propInfo = m.Member as PropertyInfo;
                    if (propInfo != null)
                    {
                        propInfo.SetValue(_setterInstance, _newValue, null);
                        return result;
                    }
                    var fieldInfo = m.Member as FieldInfo;
                    if (fieldInfo != null)
                    {
                        fieldInfo.SetValue(_setterInstance, _newValue);
                        return result;
                    }
                    throw new NotImplementedException();
                }
                else  // Traverse nested property/field access
                {
                    var propInfo = m.Member as PropertyInfo;
                    if (propInfo != null)
                    {
                        _setterInstance = propInfo.GetValue(
                            _setterInstance, BindingFlags.Default, 
                            null, null, null);
                        return result;
                    }
                    var fieldInfo = m.Member as FieldInfo;
                    if (fieldInfo != null)
                    {
                        _setterInstance = fieldInfo.GetValue(_setterInstance);
                        return result;
                    }
                    throw new NotImplementedException();
                }
            }

            protected override Expression VisitConstant(ConstantExpression c)
            {
                var result = base.VisitConstant(c);
                _setterInstance = c.Value;
                return result;
            }

            protected override Expression VisitMethodCall(MethodCallExpression m)
            {
                _memberAccessDepth++;
                var result = base.VisitMethodCall(m);
                _memberAccessDepth--;

                if (_memberAccessDepth == 0 && m.NodeType == ExpressionType.Call)
                {
                    var getter = m.Method;

                    // Special case for Indexers. Method must be a "special name"
                    // and begin with "get_".
                    if (!getter.Name.StartsWith("get_") && getter.IsSpecialName)
                    {
                        throw new NotImplementedException(
                            "The only method call expressions allowed are indexer-gets.");
                    }

                    var setMethodName = "set_" + getter.Name.Substring(4);

                    var declaringType = getter.DeclaringType;
                    if (declaringType == null)
                    {
                        throw new InvalidOperationException(String.Format(
                            "Cannot find declaring type for getter {0}.",
                            getter.Name));
                    }

                    var setter = declaringType.GetMethod(setMethodName);
                    if (setter == null || !setter.IsSpecialName)
                    {
                        throw new InvalidOperationException(String.Format(
                            "Cannot find setter for getter {0}.",
                            getter.Name));
                    }
                    var indexArgExpr = Expression.Lambda(m.Arguments[0]);
                    var indexArg = ((Func<object>)indexArgExpr.Compile())();

                    

                    setter.Invoke(_setterInstance, new[] {indexArg, _newValue});
                    return result;
                }
                throw new NotImplementedException();
            }

            // Called by method calls with arguments. We don't really care to visit those.
            protected override ReadOnlyCollection<Expression> VisitExpressionList(ReadOnlyCollection<Expression> original)
            {
                return original;
            }
        }
    }

    /// <summary>
    /// A generator with explicitly defined input and output types.
    /// </summary>
    /// <typeparam name="TIn"></typeparam>
    /// <typeparam name="TOut"></typeparam>
    public class Generator<TIn, TOut> : Generator, IGenerator<TIn, TOut>
    {
        public Generator(IEnumerator<IYield> enm)
            : base(enm)
        {
        }

        public TOut Send(TIn obj)
        {
            return Send<TIn, TOut>(obj);
        }

        public TOut Next()
        {
            return Next<TOut>();
        }

        public bool TrySend(TIn obj, out TOut result)
        {
            return TrySend<TIn, TOut>(obj, out result);
        }
    }
}
