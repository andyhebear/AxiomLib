#region MIT License
/*
-----------------------------------------------------------------------------
This source file is part of the Jitter Sample
Jitter Physics Engine Copyright (c) 2010 Thorben Linneweber (admin@jitter-physics.com)

This a port for Axiom of samples using Jitter Physics Engine,
developed by Thorben Linneweber and ported by Francesco Guastella (aka romeoxbm).
 
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
-----------------------------------------------------------------------------
*/
#endregion

#region Using Statements

using System;
using System.Collections.Generic;
using System.Reflection;
using Axiom.Core;
using Axiom.Math;
using Axiom.Samples;
using SharpGorilla;

#endregion
using System;
using System.Collections.Generic;
using SharpGorilla;
using Axiom.Core;
using System.Runtime.InteropServices;

namespace SharpGorilla.Samples
{
    public class AxiomConsole
    {
        const int CONSOLE_FONT_INDEX = 9;
        const int CONSOLE_LINE_LENGTH = 85;
        const int CONSOLE_LINE_COUNT = 15;
        //const char[] legalchars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890+!\"'#%&/()=?[]\\*-_.:,; ".ToCharArray();
        char[] legalchars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890+!\"'#%&/()=?[]\\*-_.:,; ".ToCharArray();
        public delegate void CommandDelegate(List<string> list);
        private Screen _screen;
        private Layer _layer;
        private Caption _promptText;
        private MarkupText _consoleText;
        private SharpGorilla.Rectangle _decoration;
        private GlyphData _glyphData;
        private bool _updateConsole;
        private bool _updatePrompt;
        private int _startLine;
        private string _prompt = string.Empty;
        private List<string> _lines = new List<string>();
        private Dictionary<string, CommandDelegate> _commands = new Dictionary<string, CommandDelegate>();
        //Input input;
        protected float _cursorBlink = .5f;
        /// <summary>
        /// 
        /// </summary>
        public bool IsVisible
        {
            get;
            set;
        }
        /// <summary>
        /// 
        /// </summary>
        public bool IsInitialized
        {
            get;
            protected set;
        }
        /// <summary>
        /// 
        /// </summary>
        public AxiomConsole()
        {
            IsVisible = true;
        }

        void FrameEnded(object sender, FrameEventArgs e)
        {

        }
        bool _promptVisible = true;
        void FrameStarted(object sender, FrameEventArgs e)
        {
            _cursorBlink -= e.TimeSinceLastFrame;
            if (_cursorBlink <= 0)
            {
                if (_promptVisible)
                {
                    _prompt = _prompt.Replace("_", "");
                    _promptVisible = false;

                }
                else
                {
                    _promptVisible = true;
                }
                _cursorBlink = .5f;
                _updatePrompt = true;
            }

            if (_updateConsole)
                UpdateConsole();
            if (_updatePrompt)
                UpdatePrompt();
            
        }

        public void OnKeyPressed( SharpInputSystem.KeyEventArgs args )
        {
			switch ( args.Key )
			{
				case SharpInputSystem.KeyCode.Key_F1:
					IsVisible = !IsVisible;
					_layer.IsVisible = IsVisible;
					break;

				case SharpInputSystem.KeyCode.Key_RETURN:
					if ( !IsVisible && IsInitialized )
						return;

					Print("%3> " + _prompt + "%R");

					//split the parameter list
					string[] split = _prompt.Split(' ');

					if (split.Length > 0 && !(split[0] == ""))
					{
						foreach (KeyValuePair<string, CommandDelegate> del in _commands)
						{
							if (split[0].ToLower() == del.Key)
								if (del.Value != null)
								{
									List<string> pass = new List<string>();
									pass.AddRange(split);
									del.Value(pass);
									break;
								}
						}

						_prompt = string.Empty;
						_updateConsole = true;
						_updatePrompt = true;
					}
					break;

				case SharpInputSystem.KeyCode.Key_BACK:
					if ( !IsVisible && IsInitialized )
						return;
					if (_prompt.Length > 0)
					{
						_prompt = _prompt.Remove(_prompt.Length - 1);
						_updatePrompt = true;
					}
					break;

				case SharpInputSystem.KeyCode.Key_PGUP:
					if ( !IsVisible && IsInitialized )
						return;
					if (_startLine > 0)
						_startLine--;
					_updateConsole = true;
					break;

				case SharpInputSystem.KeyCode.Key_PGDOWN:
					if ( !IsVisible && IsInitialized )
						return;
					if (_startLine < _lines.Count)
						_startLine++;
					_updateConsole = true;
					break;
				
				case SharpInputSystem.KeyCode.Key_SPACE:
					_prompt += " ";
					_updatePrompt = true;
					break;
				default:
					if ( !IsVisible && IsInitialized )
						return;
					for ( int c = 0; c < legalchars.Length; c++ )
						if ( args.Key.ToString().Split( new char[] { '_'} )[1] == legalchars[ c ].ToString()  ) 
						{
							_prompt += legalchars[ c ].ToString().ToLower();
							_updatePrompt = true;
							break;
						}
					break;
			}

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="screenToUse"></param>
        public void Init(Screen screenToUse)
        {
            if (IsInitialized)
                Shutdown();

            Root.Instance.FrameStarted += new EventHandler<FrameEventArgs>(FrameStarted);
            Root.Instance.FrameEnded += new EventHandler<FrameEventArgs>(FrameEnded);
            LogManager.Instance.DefaultLog.MessageLogged += new EventHandler<LogListenerEventArgs>(MessageLogged);
            _screen = screenToUse;
            _layer = _screen.CreateLayer(15);
            _glyphData = _layer.GetGlyphData(CONSOLE_FONT_INDEX);
            _consoleText = _layer.CreateMarkupText(CONSOLE_FONT_INDEX, 10, 10, string.Empty);
            _consoleText.Width = _screen.Width - 10;
            _promptText = _layer.CreateCaption(CONSOLE_FONT_INDEX, 10, 10, "> _");
            _decoration = _layer.CreateRectangle(8, 8, _screen.Width - 16, _glyphData.LineHeight);
            _decoration.SetBackgroundGradient(Gradient.NorthSouth, Converter.ToRGB(128, 128, 128, 128), Converter.ToRGB(64, 64, 64, 128));
            _decoration.SetBorder(2, Converter.ToRGB(128, 128, 128, 128));
            //input = new Input();
            IsInitialized = true;
            Print("%5Axiom%R%6Console%0 Activated. Press F1 to show/hide.%R");
        }

        void MessageLogged(object sender, LogListenerEventArgs e)
        {
            Print("Log: " + e.Message);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Shutdown()
        {
            if (!IsInitialized)
                return;

            IsInitialized = false;
            Root.Instance.FrameEnded -= FrameEnded;
            Root.Instance.FrameStarted -= FrameStarted;
            LogManager.Instance.DefaultLog.MessageLogged -= MessageLogged;

            _screen.Destroy(ref _layer);
        }
        /// <summary>
        /// 
        /// </summary>
        public void Print(string text)
        {
            //subdivide it into lines
            string str = text;
            int start = 0, count = 0;
            int len = text.Length;
            String line = string.Empty;
            for (int c = 0; c < len; c++)
            {
                if (str[c] == '\n' || line.Length >= CONSOLE_LINE_LENGTH)
                {
                    _lines.Add(line);
                    line = "";
                }
                if (str[c] != '\n')
                    line += str[c];
            }
            if (line.Length != 0)
                _lines.Add(line);
            if (_lines.Count > CONSOLE_LINE_COUNT)
                _startLine = _lines.Count - CONSOLE_LINE_COUNT;
            else
                _startLine = 0;
            _updateConsole = true;
        }
        /// <summary>
        /// 
        /// </summary>
        private void UpdateConsole()
        {
            _updateConsole = false;

            string text = string.Empty;
            int i = 0;
            int start = 0;
            int end = _lines.Count;
            //make sure is in range
            if (_startLine > _lines.Count)
                _startLine = _lines.Count;

            int lcount = 0;

            for (int c = 0; c < _startLine; c++)
                start++;
            end = start;
            for (int c = 0; c < CONSOLE_LINE_COUNT; c++)
            {
                if (end == _lines.Count)
                    break;
                end++;
            }
            for (i = start; i != end; i++)
            {
                lcount++;
                text += _lines[i] + "\n";
            }
            _consoleText.Text = text;
            // Move prompt downwards.
            _promptText.Top = 10 + (lcount * _glyphData.LineHeight);

            // Change background height so it covers the text and prompt
            _decoration.Height = ((lcount + 1) * _glyphData.LineHeight) + 4;

            _consoleText.Width = _screen.Width - 20;
            _decoration.Width = _screen.Width - 16;
            _promptText.Width = _screen.Width - 20;
        }
        /// <summary>
        /// 
        /// </summary>
        private void UpdatePrompt()
        {
            _updatePrompt = false;
            string text = string.Empty;
            text = "> " + _prompt + (_promptVisible ? "_" : "");
            _promptText.Text = text;
        }

        public void AddCommand(string command, CommandDelegate target)
        {
            _commands.Add(command, target);
        }
        public void RemoveCommand(string command)
        {
        }

        public void MessageLogged(string message, LogMessageLevel lml, bool maskDebug, string logName)
        {
        }
    }
}
