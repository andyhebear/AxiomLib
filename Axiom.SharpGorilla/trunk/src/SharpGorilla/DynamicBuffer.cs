#region License
/*
    Gorilla
    -------
    
    Copyright (c) 2010 Robin Southern
 
    This is a c# (Axiom) port of Gorrilla, developed by Robin Southern, ported by me (bostich)

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:
                                                                                  
    The above copyright notice and this permission notice shall be included in
    all copies or substantial portions of the Software.
                                                                                  
    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
    THE SOFTWARE. 
 */
#endregion

#region SVN Version Information
// <file>
//     <id value="$Id: 2118 2010-09-26 23:56:56Z bostich $"/>
// </file>
#endregion SVN Version Information

#region Namespace Declarations
using System;

#endregion
namespace SharpGorilla
{
    //public class DynamicBuffer
    //{
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    protected dynamic _buffer;
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    protected int _used;
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    protected int _capacity;
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    public int Size
    //    {
    //        get
    //        {
    //            return _used;
    //        }
    //    }
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    public int Capacity
    //    {
    //        get
    //        {
    //            return _capacity;
    //        }
    //    }
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    public dynamic First
    //    {
    //        get
    //        {
    //            return _buffer == null ? null :_buffer[0];
    //        }
    //    }
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    public dynamic Last
    //    {
    //        get
    //        {
    //            return _buffer == null ? null : _buffer[_used - 1];
    //        }
    //    }
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="index"></param>
    //    /// <returns></returns>
    //    public dynamic this[int index]
    //    {
    //        get
    //        {
    //            return _buffer[index];
    //        }
    //    }
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="index"></param>
    //    /// <returns></returns>
    //    public dynamic At(int index)
    //    {
    //        return _buffer[index];
    //    }
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    public void Clear()
    //    {
    //        _used = 0;
    //    }
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="capacity"></param>
    //    public void Resize(int capacity)
    //    {
    //        dynamic newBuffer = new dynamic[capacity];

    //        if (_used != 0)
    //        {
    //            if (_used < capacity)// copy all
    //            {
    //                for (int i = 0; i < _used; i++)
    //                {
    //                    newBuffer[i] = _buffer[i];
    //                }
    //            }
    //            else if (_used >= capacity) // copy some
    //            {
    //                for (int i = 0; i < _capacity;i++ )
    //                {
    //                    newBuffer[i] = _buffer[i];
    //                }
    //            }
    //        }

    //        _capacity = capacity;
    //        _buffer = newBuffer;
    //    }
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="value"></param>
    //    public void Add(dynamic value)
    //    {
    //        if (_used == _capacity)
    //        {
    //            Resize(_used == 0 ? 1 : _used * 2);
    //        }

    //        _buffer[_used] = value;
    //        _used++;
    //    }
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    public void PopBack()
    //    {
    //        if (_used != 0)
    //            _used--;
    //    }
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="index"></param>
    //    public void Remove(int index)
    //    {
    //        _buffer[index] = _buffer[_used - 1];
    //        _used--;
    //    }
    //}

    public class DynamicBuffer<T>
    {
        /// <summary>
        /// 
        /// </summary>
        protected T[] _buffer;
        /// <summary>
        /// 
        /// </summary>
        protected int _used;
        /// <summary>
        /// 
        /// </summary>
        protected int _capacity;
        /// <summary>
        /// 
        /// </summary>
        public int Size
        {
            get
            {
                return _used;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public int Capacity
        {
            get
            {
                return _capacity;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public T First
        {
            get
            {
                return _buffer == null ? default(T) : _buffer[0];
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public T Last
        {
            get
            {
                return _buffer == null ? default(T) : _buffer[_used - 1];
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public T this[int index]
        {
            get
            {
                return _buffer[index];
            }
            set
            {
                _buffer[index] = value;
            }
        }
		public T[] ToArray()
		{
			return _buffer;
		}
		public void SetData( T[] data )
		{
			_buffer = data;
		}
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public T At(int index)
        {
            return _buffer[index];
        }
        /// <summary>
        /// 
        /// </summary>
        public void Clear()
        {
            _used = 0;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="capacity"></param>
        public void Resize(int capacity)
        {
            T[] newBuffer = new T[capacity];

            if (_used != 0)
            {
                if (_used < capacity)// copy all
                {
                    for (int i = 0; i < _used; i++)
                    {
                        newBuffer[i] = _buffer[i];
                    }
                }
                else if (_used >= capacity) // copy some
                {
                    for (int i = 0; i < _capacity; i++)
                    {
                        newBuffer[i] = _buffer[i];
                    }
                }
            }

            _capacity = capacity;
            _buffer = newBuffer;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public void Add(T value)
        {
            if (_used == _capacity)
            {
                Resize(_used == 0 ? 1 : _used * 2);
            }

            _buffer[_used] = value;
            _used++;
        }
        /// <summary>
        /// 
        /// </summary>
        public void PopBack()
        {
            if (_used != 0)
                _used--;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        public void Remove(int index)
        {
            _buffer[index] = _buffer[_used - 1];
            _used--;
        }
    }
}
