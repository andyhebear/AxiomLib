#region MIT/X11 License
// This file is part of the Axiom.SkyX project
//
// Copyright (c) 2009 Michael Cummings <cummings.michael@gmail.com>
// Copyright (c) 2009 Bostich <bostich83@googlemail.com>

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//
// Axiom.SkyX is a reimplementation of the SkyX project for .Net/Mono
// SkyX is Copyright (C) 2009 Xavier Verguín González <xavierverguin@hotmail.com> <xavyiy@gmail.com>
#endregion MIT/X11 License
using System;
using System.Collections.Generic;
using Axiom.Input;
using Axiom.Math;
using Axiom.Core;
using Axiom.Graphics;
namespace Axiom.SkyX
{
    /// <summary>
    /// Handles basic input action
    /// </summary>
    public class InputManager
    {
        #region - ButtonState -
        /// <summary>
        /// Indicates if a mouse button is pressed or not!
        /// </summary>
        public enum ButtonState
        {
            Pressed,
            Released,
        }
        #endregion

        #region - MouseState -
        /// <summary>
        /// 
        /// </summary>
        public struct MouseState
        {
            float mX;
            float mY;
            float mAbsX;
            float mAbsY;
            float mAbsZ;
            ButtonState mLeftButton;
            ButtonState mRightButton;
            ButtonState mMiddleButton;
            /// <summary>
            /// 
            /// </summary>
            public ButtonState LeftButton
            {
                get { return mLeftButton; }
            }
            /// <summary>
            /// 
            /// </summary>
            public ButtonState RightButton
            {
                get { return mRightButton; }
            }
            /// <summary>
            /// 
            /// </summary>
            public ButtonState MiddleButton
            {
                get { return mMiddleButton; }
            }
            /// <summary>
            /// 
            /// </summary>
            public float X
            {
                get { return mX; }
                internal set { mX = value; }
            }
            /// <summary>
            /// 
            /// </summary>
            public float Y
            {
                get { return mY; }
                internal set { mY = value; }
            }
            /// <summary>
            /// 
            /// </summary>
            public float AbsoluteX
            {
                get { return mAbsX; }
                internal set
                {
                    mAbsX = value;
                }
            }
            /// <summary>
            /// 
            /// </summary>
            public float AbsoluteY
            {
                get { return mAbsY; }
                internal set
                {
                    mAbsY = value;
                }
            }
            /// <summary>
            /// 
            /// </summary>
            public float AbsoluteZ
            {
                get { return mAbsZ; }
                internal set
                {
                    mAbsZ = value;
                }
            }
            /// <summary>
            /// 
            /// </summary>
            public Vector2 Position
            {
                get { return new Vector2(mX, mY); }
            }
            internal List<MouseButtons> mPressedButtons;
            internal void AddPressedMouseButton(MouseButtons button)
            {
                switch (button)
                {
                    case MouseButtons.Left:
                        mLeftButton = ButtonState.Pressed;
                        break;
                    case MouseButtons.Middle:
                        mMiddleButton = ButtonState.Pressed;
                        break;
                    case MouseButtons.Right:
                        mRightButton = ButtonState.Pressed;
                        break;
                }
                mPressedButtons.Add(button);
            }
            internal void RemovePressedMouseButton(MouseButtons button)
            {
                switch (button)
                {
                    case MouseButtons.Left:
                        mLeftButton = ButtonState.Released;
                        break;
                    case MouseButtons.Middle:
                        mMiddleButton = ButtonState.Released;
                        break;
                    case MouseButtons.Right:
                        mRightButton = ButtonState.Released;
                        break;
                }
                if (mPressedButtons.Contains(button))
                    mPressedButtons.Remove(button);
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="button"></param>
            /// <returns></returns>
            public bool IsMouseUp(MouseButtons button)
            {
                return !mPressedButtons.Contains(button);
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="button"></param>
            /// <returns></returns>
            public bool IsMouseDown(MouseButtons button)
            {
                return mPressedButtons.Contains(button);
            }
        }
        #endregion

        #region - KeyboardState -
        /// <summary>
        /// 
        /// </summary>
        public struct KeyboardState
        {
            internal List<KeyCodes> mPressedKeyCode;

            internal void AddPressedKey(KeyCodes key)
            {
                mPressedKeyCode.Add(key);
            }
            internal void RemovePressedKey(KeyCodes key)
            {
                mPressedKeyCode.Remove(key);
            }
            public KeyCodes[] GetPressedKeyCode()
            {
                return mPressedKeyCode.ToArray();
            }
            public bool IsKeyUp(KeyCodes key)
            {
                return !mPressedKeyCode.Contains(key);
            }
            public bool IsKeyDown(KeyCodes key)
            {
                return mPressedKeyCode.Contains(key);
            }

        }
        #endregion

        #region - fields -
        protected static InputManager mSingleTon;
        /// <summary>
        /// 
        /// </summary>
        protected KeyboardState mCurrentKeyCodeState;
        /// <summary>
        /// 
        /// </summary>
        protected KeyboardState mPreviousKeyCodeState;
        /// <summary>
        /// 
        /// </summary>
        protected MouseState mCurrentMouseState;
        /// <summary>
        /// 
        /// </summary>
        protected MouseState mPreviousMouseState;
        /// <summary>
        /// 
        /// </summary>
        protected InputReader mInputReader;
        /// <summary>
        /// 
        /// </summary>
        protected bool mIsKeyPressed;
        /// <summary>
        /// 
        /// </summary>
        protected bool mIsSpecialKeyPressed;
        /// <summary>
        /// 
        /// </summary>
        protected Dictionary<KeyCodes, float> mKeyCache;
        /// <summary>
        /// 
        /// </summary>
        protected Dictionary<MouseButtons, float> mMouseCache;
        /// <summary>
        /// 
        /// </summary>
        protected InputAction mKeyBinds = new InputAction();
        /// <summary>
        /// Handles input given by the user.
        /// </summary>
        protected InputManager mInputManager;
        /// <summary>
        /// Handles keyboard input, given by the user.
        /// </summary>
        protected KeyboardEventHandler mKeyboard;
        /// <summary>
        /// Handles mouse input, given by the user.
        /// </summary>
        protected MouseEventHandler mMouse;
        protected Viewport mViewport;
        #endregion

        #region - Properties -
        public static InputManager SingleTon
        {
            get { return mSingleTon; }
            internal set { mSingleTon = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public Vector2 MousePosition
        {
            get { return mCurrentMouseState.Position; }
        }
        public Vector3 AbsoluteMousePosition
        {
            get
            {
                return new Vector3(
                    mCurrentMouseState.AbsoluteX,
                    mCurrentMouseState.AbsoluteY,
                    mCurrentMouseState.AbsoluteZ);
            }
        }
        public KeyboardEventHandler Keyboard
        {
            get { return mKeyboard; }
        }
        public MouseEventHandler Mouse
        {
            get { return mMouse; }
        }
        #endregion

        #region - Constructor's -
        /// <summary>
        /// 
        /// </summary>
        public InputManager(RenderWindow window, Viewport viewport)
            : this(window, viewport, true, true, true)
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ownMouse"></param>
        public InputManager(RenderWindow window, Viewport viewport, bool ownMouse)
            : this(window, viewport, true, true, ownMouse)
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="keyBoard"></param>
        /// <param name="mouse"></param>
        /// <param name="ownMouse"></param>
        public InputManager(RenderWindow window, Viewport viewport, bool keyBoard, bool mouse, bool ownMouse)
        {
            if (window == null)
                throw new Exception("Renderwindow can't be null!");
            //create only ONE instance.
            if (SingleTon == null)
                SingleTon = this;
            mViewport = viewport;
            IntPtr wndHandle;
            wndHandle = (IntPtr)window["WINDOW"];
            if (wndHandle == IntPtr.Zero)
                throw new Exception("Error while trying to get the GameWindow Handle");
            mInputReader = PlatformManager.Instance.CreateInputReader();
            mInputReader.Initialize(window, true, true, false, false);
            mKeyCache = new Dictionary<KeyCodes, float>();
            mInputReader.Capture();
            mPreviousKeyCodeState = mCurrentKeyCodeState = GetKeyboardState();
            mMouseCache = new Dictionary<MouseButtons, float>();
            mPreviousMouseState = mCurrentMouseState = GetMouseState();
            foreach (KeyCodes key in Enum.GetValues(typeof(KeyCodes)))
                mKeyCache.Add(key, 0f);

            foreach (MouseButtons mb in Enum.GetValues(typeof(MouseButtons)))
                mMouseCache.Add(mb, 0.0f);
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <param name="key"></param>
        public void AddKeyBind(InputAction.KeyAction action, KeyCodes key)
        {
            mKeyBinds.AddBind(key, action);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public KeyCodes? GetKeyBind(InputAction.KeyAction action)
        {
            foreach (KeyValuePair<KeyCodes, InputAction.KeyAction> kvp in mKeyBinds.KeyBinds)
            {
                if (kvp.Value == action)
                    return kvp.Key;
            }
            return null;
        }
        /// <summary>
        /// 
        /// </summary>
        public virtual void Update(FrameEventArgs e)
        {

            float elapsedTime = e.TimeSinceLastFrame;
            mInputReader.Capture();
            mPreviousKeyCodeState = mCurrentKeyCodeState;
            mCurrentKeyCodeState = GetKeyboardState();

            mPreviousMouseState = mCurrentMouseState;
            mCurrentMouseState = GetMouseState();

            foreach (KeyCodes key in Enum.GetValues(typeof(KeyCodes)))
            {
                if (IsKeyDown(KeyCodeToKeyCode(key)))
                {
                    mKeyCache[key] += elapsedTime;
                }
                else
                    mKeyCache[key] = 0;
            }

            foreach (MouseButtons mb in Enum.GetValues(typeof(MouseButtons)))
            {
                if (IsMouseButtonDown(mb))
                    mMouseCache[mb] += elapsedTime;
                else
                    mMouseCache[mb] = 0;
            }


        }

        private bool IsMouseButtonState(MouseButtons mb, ButtonState state,
            ref MouseState mouseState)
        {
            switch (mb)
            {
                case MouseButtons.Left:
                    return (mouseState.LeftButton == state);
                case MouseButtons.Right:
                    return (mouseState.RightButton == state);
                case MouseButtons.Middle:
                    return (mouseState.MiddleButton == state);
            }

            return false;
        }
        public KeyCodes[] GetPressedKeyCode()
        {
            KeyCodes[] mKeyCode = new KeyCodes[mCurrentKeyCodeState.mPressedKeyCode.Count];
            int i = 0;
            foreach (KeyCodes keyToAdd in mCurrentKeyCodeState.mPressedKeyCode)
            {
                mKeyCode[i++] = KeyCodeToKeyCode(keyToAdd);
            }
            return mKeyCode;
        }
        // Newly Pressed Button
        public bool IsMouseButtonPress(MouseButtons mb)
        {
            return (IsMouseButtonDown(mb) &&
                IsMouseButtonState(mb, ButtonState.Released,
                    ref mPreviousMouseState));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mb"></param>
        /// <returns></returns>
        public bool IsMouseButtonRelease(MouseButtons mb)
        {
            return (IsMouseButtonUp(mb) &&
                IsMouseButtonState(mb, ButtonState.Pressed,
                ref mPreviousMouseState));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mb"></param>
        /// <returns></returns>
        public bool IsMouseButtonUp(MouseButtons mb)
        {
            return IsMouseButtonState(mb, ButtonState.Released,
                ref mCurrentMouseState);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mb"></param>
        /// <returns></returns>
        public bool IsMouseButtonDown(MouseButtons mb)
        {
            return IsMouseButtonState(mb, ButtonState.Pressed,
                ref mCurrentMouseState);

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool IsKeyPress(KeyCodes key)
        {

            return (mCurrentKeyCodeState.IsKeyDown(KeyCodeToKey(key)) &&
                mPreviousKeyCodeState.IsKeyUp(KeyCodeToKey(key)));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool IsKeyRelease(KeyCodes key)
        {
            KeyCodes kKeyCode = KeyCodeToKey(key);
            return (mCurrentKeyCodeState.IsKeyUp(kKeyCode) &&
                mPreviousKeyCodeState.IsKeyDown(kKeyCode));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool IsKeyUp(KeyCodes key)
        {
            KeyCodes kKeyCode = KeyCodeToKey(key);
            return mCurrentKeyCodeState.IsKeyUp(kKeyCode);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool IsKeyDown(KeyCodes key)
        {
            KeyCodes kKeyCode = KeyCodeToKey(key);
            return mCurrentKeyCodeState.IsKeyDown(kKeyCode);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public MouseState GetMouseState()
        {
            MouseState state = new MouseState();
            state.mPressedButtons = new List<MouseButtons>();

            foreach (MouseButtons mb in Enum.GetValues(typeof(MouseButtons)))
            {
                if (mInputReader.IsMousePressed(mb))
                {
                    state.AddPressedMouseButton(mb);
                }
                else
                {
                    state.RemovePressedMouseButton(mb);
                }
            }
            state.X = (float)mInputReader.RelativeMouseX;
            state.Y = (float)mInputReader.RelativeMouseY;
            state.AbsoluteX = (float)mInputReader.AbsoluteMouseX;
            state.AbsoluteY = (float)mInputReader.AbsoluteMouseY;
            state.AbsoluteZ = (float)mInputReader.AbsoluteMouseZ;
            return state;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public float TimePressed(KeyCodes key)
        {
            KeyCodes kkey = KeyCodeToKey(key);
            return mKeyCache[kkey];
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mb"></param>
        /// <returns></returns>
        public float TimePressed(MouseButtons btn)
        {
            return mMouseCache[ToMouseButtons(btn)];
        }
        private MouseButtons ToMouseButtons(MouseButtons mb)
        {
            string enumName = Enum.GetName(typeof(MouseButtons), mb);
            MouseButtons btn = (MouseButtons)Enum.Parse(typeof(MouseButtons), enumName, true);
            return btn;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private KeyCodes KeyCodeToKey(KeyCodes key)
        {
            string enumName = Enum.GetName(typeof(KeyCodes), key);
            KeyCodes mKey = (KeyCodes)Enum.Parse(typeof(KeyCodes), enumName, true);
            return mKey;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private KeyCodes KeyCodeToKeyCode(KeyCodes key)
        {
            string enumName = Enum.GetName(typeof(KeyCodes), key);
            KeyCodes mKey = (KeyCodes)Enum.Parse(typeof(KeyCodes), enumName, true);
            return mKey;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public KeyboardState GetKeyboardState()
        {
            KeyboardState state = new KeyboardState();
            state.mPressedKeyCode = new List<KeyCodes>();

            foreach (KeyCodes key in Enum.GetValues(typeof(KeyCodes)))
            {
                if (mInputReader.IsKeyPressed(key))
                {
                    state.AddPressedKey(key);
                }
                else
                {
                    state.RemovePressedKey(key);
                }
            }

            return state;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public class InputAction
    {
        protected Dictionary<KeyCodes, KeyAction> mKeyBindings = new Dictionary<KeyCodes, KeyAction>();

        internal Dictionary<KeyCodes, KeyAction> KeyBinds
        {
            get { return mKeyBindings; }
        }
        /// <summary>
        /// Binds a key to an action
        /// </summary>
        public enum KeyAction
        {
            CamForward,
            CamBackward,
            CamRotateLeft,
            CamRotateRight,
            CamRotateUp,
            CamRotateDown,
            CamStrafeLeft,
            CamStrafeRight
        }

        /// <summary>
        /// 
        /// </summary>
        public InputAction()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="action"></param>
        public void AddBind(KeyCodes key, KeyAction action)
        {

            if (!mKeyBindings.ContainsKey(key))
            {
                mKeyBindings.Add(key, action);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        public void RemoveBind(KeyCodes key)
        {
            if (mKeyBindings.ContainsKey(key))
            {
                mKeyBindings.Remove(key);
            }
        }
    }
}
