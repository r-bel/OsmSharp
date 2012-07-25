﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tools.Core.Collections
{
    /// <summary>
    /// Class implementing a thread-safe limited stack.
    /// 
    /// When the limit is reached the oldest element will be removed.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class LimitedStack<T>
    {
        /// <summary>
        /// Containts the list of elements in this queue.
        /// </summary>
        private List<T> _elements;

        /// <summary>
        /// The limit to the size of this queue.
        /// </summary>
        private int _limit;

        /// <summary>
        /// Creates a new limited stack with a limit of 10.
        /// </summary>
        public LimitedStack()
        {
            _limit = 10;
            _elements = new List<T>();
        }

        /// <summary>
        /// Creates a new limited stack with a limit of 10.
        /// </summary>
        public LimitedStack(IEnumerable<T> collection)
        {
            _limit = 10;
            _elements = new List<T>(collection);
            if (_elements.Count > _limit)
            {
                _elements.RemoveRange(0, _elements.Count - _limit);
            }
        }

        /// <summary>
        /// Creates a new limited stack with a limit of 10.
        /// </summary>
        public LimitedStack(int capacity)
        {
            _limit = 10;
            // no use initializing greater than limit.
            _elements = new List<T>(capacity>_limit?_limit:capacity);
        }

        /// <summary>
        /// Returns the number of elements in this stack.
        /// </summary>
        public int Count 
        {
            get
            {
                lock (_elements)
                {
                    return _elements.Count;
                }
            }
        }

        /// <summary>
        /// Clears the elements from this stack.
        /// </summary>
        public void Clear()
        {
            lock (_elements)
            {
                _elements.Clear();
            }
        }

        /// <summary>
        /// Returns true if this stack contains the item.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(T item)
        {
            lock (_elements)
            {
                return _elements.Contains(item);
            }
        }

        /// <summary>
        /// Pops the top element from the stack.
        /// </summary>
        /// <returns></returns>
        public T Pop()
        {
            lock (_elements)
            {
                T element = _elements[_elements.Count - 1];
                _elements.RemoveAt(_elements.Count - 1);
                return element;
            }
        }

        /// <summary>
        /// Pushes an item on the stack.
        /// </summary>
        /// <param name="item"></param>
        public void Push(T item)
        {
            lock (_elements)
            {
                if (_elements.Count == _limit)
                {
                    // remove the last fist element to enter the stack
                    _elements.RemoveAt(0);
                }
                _elements.Add(item);
            }
        }

        /// <summary>
        /// Moves an item to the top of the stack if it already exists.
        /// </summary>
        /// <param name="item"></param>
        public void PushToTop(T item)
        {
            lock (_elements)
            {
                if(_elements.Contains(item))
                { // remove the item.
                    _elements.Remove(item);
                }
                // pushes the item to the top of the stack.
                this.Push(item);                
            }
        }

        /// <summary>
        /// The maximum amount of elements in this queue.
        /// </summary>
        public int Limit 
        {
            get
            {
                return _limit;
            }
            set
            {
                _limit = value;
                if (_elements.Count > _limit)
                {
                    _elements.RemoveRange(0, _elements.Count - _limit);
                }
            }
        }

        /// <summary>
        /// Returns the top element without popping it.
        /// </summary>
        /// <returns></returns>
        public T Peek()
        {
            lock (_elements)
            {
                return _elements[_elements.Count - 1];
            }
        }
    }
}
