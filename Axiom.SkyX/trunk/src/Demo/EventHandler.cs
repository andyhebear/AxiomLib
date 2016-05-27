using System;
using System.Collections.Generic;
using System.Text;

using SharpInputSystem;

namespace Demo.SkyX
{
    public class EventHandler : IKeyboardListener, IMouseListener, IJoystickListener
    {
        private bool appRunning = true;

        public bool AppRunning
        {
            get
            {
                return appRunning;
            }
            set
            {
                appRunning = value;
            }
        }

        #region IKeyboardListener Members

        public bool KeyPressed( KeyEventArgs e )
        {
            // TODO: Whatever you want to do on a KeyPress
            return true;
        }

        public bool KeyReleased( KeyEventArgs e )
        {
            // TODO: Whatever you want to do on a KeyRelease
            return true;
        }

        #endregion

        #region IMouseListener Members

        public bool MouseMoved( MouseEventArgs arg )
        {
            // TODO: Whatever you want to do on a MouseMove
            return true;
        }

        public bool MousePressed( MouseEventArgs arg, MouseButtonID id )
        {
            // TODO: Whatever you want to do on a MousePress
            return true;
        }

        public bool MouseReleased( MouseEventArgs arg, MouseButtonID id )
        {
            // TODO: Whatever you want to do on a MouseRelease
            return true;
        }

        #endregion

        #region IJoystickListener Members

        public bool ButtonPressed( JoystickEventArgs arg, int button )
        {
            // TODO: Whatever you want to do on a JoyPress
            return true;
        }

        public bool ButtonReleased( JoystickEventArgs arg, int button )
        {
            // TODO: Whatever you want to do on a JoyRelease
            return true;
        }

        public bool AxisMoved( JoystickEventArgs arg, int axis )
        {
            // TODO: Whatever you want to do on a JoyAxisMoved
            return true;
        }

        public bool SliderMoved( JoystickEventArgs arg, int slider )
        {
            // TODO: Whatever you want to do on a SliderMove
            return true;
        }

        public bool PovMoved( JoystickEventArgs arg, int pov )
        {
            // TODO: Whatever you want to do on a PovMove
            return true;
        }

        #endregion IJoystickListener Members
    }
}
