using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Axiom.Core;
using Axiom.Graphics;
using Axiom.Utilities;

namespace Hydrax.Demo
{
    public class ApplicationWindowEventListener : IWindowEventListener
    {
        private RenderWindow _mw;
        public ApplicationWindowEventListener( RenderWindow mainWindow )
        {
            Contract.RequiresNotNull( mainWindow, "mainWindow" );

            _mw = mainWindow;
        }

        /// <summary>
        /// Window has moved position
        /// </summary>
        /// <param name="rw">The RenderWindow which created this event</param>
        public void WindowMoved( RenderWindow rw )
        {
        }

        /// <summary>
        /// Window has resized
        /// </summary>
        /// <param name="rw">The RenderWindow which created this event</param>
        public void WindowResized( RenderWindow rw )
        {
        }

        /// <summary>
        /// Window has closed
        /// </summary>
        /// <param name="rw">The RenderWindow which created this event</param>
        public void WindowClosed( RenderWindow rw )
        {
            Contract.RequiresNotNull( rw, "RenderWindow" );

            // Only do this for the Main Window
            if ( rw == _mw )
            {
                Root.Instance.QueueEndRendering();
            }
        }

        /// <summary>
        /// Window lost/regained the focus
        /// </summary>
        /// <param name="rw">The RenderWindow which created this event</param>
        public void WindowFocusChange( RenderWindow rw )
        {
            Contract.RequiresNotNull( rw, "RenderWindow" );

            if ( !rw.IsActive )
                rw.IsActive = true;
        }

    }
}
