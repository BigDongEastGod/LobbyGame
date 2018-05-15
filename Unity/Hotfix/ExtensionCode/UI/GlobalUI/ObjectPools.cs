using System;
using System.Collections.Generic;

namespace ETHotfix
{
    public class ObjectPools<T> where T : class, new()
    {
        private Stack<T> _objectStack;

        private Action<T> _resetAction = null;

        private Func<T> _oneTimeAction = null;

        public ObjectPools(Action<T> reset = null, Func<T> oneTime = null)
        {
            _objectStack = new Stack<T>();
            _resetAction = reset;
            _oneTimeAction = oneTime;
        }

        public T Pull()
        {
            T t;
            if (_objectStack.Count <= 0)
            {
                t = _oneTimeAction();
                return t;
            }

            t = _objectStack.Pop();
            _resetAction?.Invoke(t);
            return t;
        }

        public void Push(T t)
        {
            _objectStack.Push(t);
        }
    }
}