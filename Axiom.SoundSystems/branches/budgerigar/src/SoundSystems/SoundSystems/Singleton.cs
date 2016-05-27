#region MIT License
/*
The MIT License

Copyright (c) 2010 Axiom Contrib Developers

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

#region Namespace Declarations

using System;
using System.Globalization;
using System.Reflection;

#endregion Namespace Declarations

namespace Axiom.SoundSystems
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ISingleton<T> where T : class
    {
        bool Initialize(params object[] args);
    }

    /// <summary>
    /// 
    /// </summary>
    public class SingletonException : Exception
    {
        public SingletonException(string message)
            : base(message)
        {
        }

        public SingletonException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }

    /// <summary>
    /// A generic singleton
    /// </summary>
    /// <remarks>
    /// Although this class will allow it, don't try to do this: Singleton&lt; interface &gt;
    /// </remarks>
    /// <typeparam name="T">a class</typeparam>
    public abstract class Singleton<T> : ISingleton<T>, IDisposable where T : class
    {

        #region Nested

        class SingletonFactory
        {
            static SingletonFactory() { }

            internal static T instance = 
                (T)typeof(T).InvokeMember(typeof(T).FullName, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.NonPublic, null, null, null);
        }

        #endregion

        /// <summary>
        /// Initialize the singleton instance
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public virtual bool Initialize(params object[] args)
        {
            return true;
        }

        protected static T _instance;
        /// <summary>
        /// Instance of the singleton.
        /// </summary>
        /// <remarks>
        /// It is possible to have singleton classes that further derive from the type specified by the 'T' generic type parameter.
        /// Those must be instantiated using the 'new' keyword and initialize the <see cref="_instance"/> field in their constructor.
        /// </remarks>
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    try
                    {
                        _instance = SingletonFactory.instance;
                    }
                    catch (TypeInitializationException ex)
                    {
                        throw new SingletonException(String.Format(CultureInfo.InvariantCulture, "Type {0} must be instantiable and implement a non-public parameterless constructor.", typeof(T)), ex);
                    }
                }

                return _instance;
            }
        }

        private static void Destroy()
        {
            SingletonFactory.instance = null;
            _instance = null;
        }

        #region IDisposable

        ~Singleton()
        {
            dispose(false);
        }

        private bool _disposed = false;
        /// <summary>
        /// Determines if this instance has been disposed of already.
        /// </summary>
        protected bool isDisposed
        {
            get
            {
                return _disposed;
            }
            set
            {
                _disposed = value;
            }
        }

        /// <summary>
        /// Class level dispose method
        /// </summary>
        /// <remarks>
        /// When implementing this method in an inherited class the following template should be used;
        /// protected override void dispose( bool disposeManagedResources )
        /// {
        /// 	if ( !isDisposed )
        /// 	{
        /// 		if ( disposeManagedResources )
        /// 		{
        /// 			// Dispose managed resources.
        /// 		}
        /// 
        /// 		// There are no unmanaged resources to release, but
        /// 		// if we add them, they need to be released here.
        /// 	}
        ///
        /// 	// If it is available, make the call to the
        /// 	// base class's dispose(bool) method
        /// 	base.dispose( disposeManagedResources );
        /// }
        /// </remarks>
        /// <param name="disposeManagedResources">True if Unmanaged resources should be released.</param>
        protected virtual void dispose(bool disposeManagedResources)
        {
            if (!isDisposed)
            {
                if (disposeManagedResources)
                {
                    Singleton<T>.Destroy();
                }

                // There are no unmanaged resources to release, but
                // if we add them, they need to be released here.
            }
            isDisposed = true;
        }

        public void Dispose()
        {
            dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable
    }
}
