using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace AdvancedConsoleViewNS
{
    /// <summary>
    /// An advanced console view.
    /// </summary>
    /// <author>Hielke Hielkema</author>
    [System.ComponentModel.DesignerCategory("")]
    public partial class AdvancedConsoleView : UserControl
    {
        // Properties
        public string PromptInfo { get; set; }
        public bool IsPrompting {  get { return _prompting; }  set { _prompting = value; } }
        public VScrollBar ConsoleStrollBar { get; set; }
        public Color PromptInfoColor 
        {
            get
            {
                return new Pen(_promptInfoColor).Color;
            }
            set
            {
                _promptInfoColor = new SolidBrush(value);
            }
        }
        public Color PromptColor 
        {
            get
            {
                return new Pen(_promptColor).Color;
            }
            set
            {
                _promptColor = new SolidBrush(value);
            }
        }
        public Color TextColor 
        {
            get
            {
                return new Pen(_textColor).Color;
            }
            set
            {
                _textColor = new SolidBrush(value);
            }
        }
        public Color PromptCursorColor 
        {
            get
            {
                return new Pen(_promptCursorColor).Color;
            }
            set
            {
                _promptCursorColor = new SolidBrush(value);
            }
        }
        public Color PromptSelectCursorColor 
        {
            get
            {
                return _promptSelectCursorColor.Color;
            }
            set
            {
                _promptSelectCursorColor = new Pen(value);
            }
        }

        public List<string> Suggestions
        {
            get
            {
                return _suggestionManager.Suggestions;
            }
        }

        // Private fields
        private List<ConsoleLine> _consoleLines = new List<ConsoleLine>();
        private Font _stringFont = new Font("Courier New", 12F);
        private SuggestionManager _suggestionManager;
        private IAdvancedConsoleOwner _owner;
        private bool _prompting = false;
        private bool _fillPromptIndicator = false;
        private string _readBuffer = "";
        private bool _useOpenPromptIndicator = false;
        private int _selectedCharFromEnd = 0;
        private string _readBufferBack = "";
        private int _fromTopHeightShift = 0;

        // Style Variables
        private Brush _promptInfoColor = Brushes.Gray;
        private Brush _promptColor = Brushes.Magenta;
        private Brush _textColor = Brushes.Lime;
        private Brush _promptCursorColor = Brushes.Lime;
        private Pen _promptSelectCursorColor = Pens.Lime;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="owner">the owner of the console view</param>
        public AdvancedConsoleView(IAdvancedConsoleOwner owner)
        {
            // Connect the owner
            if (owner != null)
                _owner = owner;
            else
                throw new ArgumentNullException("owner");
            
            // Create suggestion manager
            _suggestionManager = new SuggestionManager();

            // Initialize the component
            InitializeComponent();
        }

        /// <summary>
        /// Load event
        /// </summary>
        private void AdvancedConsoleView_Load(object sender, EventArgs e)
        {
            // Init the scrolling
            InitScrolling();

        }

        #region Drawing

        private void AdvancedConsoleView_Paint(object sender, PaintEventArgs e)
        {
            // here we set gfx as e.Graphics
            // This is because gfx is shorter and much more easy to use.
            Graphics gfx = e.Graphics;

            // This will draw the text, suggestions, cursor.
            DrawConsoleText(gfx);
        }

        /// <summary>
        /// Draw the console text
        /// </summary>
        /// <param name="gfx">graphics to draw on</param>
        public void DrawConsoleText(Graphics gfx)
        {
            // The height of the text in pix.
            // This will be increased in the draw process.
            // "fromTopHeightShift" is for scrolling.
            int drawHeight = _fromTopHeightShift;

            // The horizontal position of the text in chars(1 char is 10 pix).
            // This will be increased in the draw process.
            int horizontalDrawPosition = 0;

            // Calculate how many chars we put on one line.
            int charsOnLine = this.Width / 10;

            // Loop through all the lines
            foreach (ConsoleLine item in _consoleLines)
            {
                // Reset the x of the drawing position.
                horizontalDrawPosition = 0;

                // Loop through all the parts of the line.
                foreach (ConsoleLinePart part in item.parts)
                    DrawPart(gfx, part, charsOnLine, ref horizontalDrawPosition, ref drawHeight);

                // Increase the vertical draw height.
                // This is because we go to a new line.
                drawHeight += 12;
            }

            // Check if we're prompting
            if (_prompting)
            {
                // Reset the x of the drawing position.
                horizontalDrawPosition = 0;

                if (!string.IsNullOrEmpty(PromptInfo))
                {
                    // Create a part containing the prompt info.
                    ConsoleLinePart promprInfoPart = new ConsoleLinePart(PromptInfo, _promptInfoColor);

                    // Draw the part
                    DrawPart(gfx, promprInfoPart, charsOnLine, ref horizontalDrawPosition, ref drawHeight);
                }

                if (_suggestionManager != null)
                    _suggestionManager.DrawSuggestions(gfx, drawHeight, _readBufferBack, horizontalDrawPosition, this.Height);

                // Create a part containing the readbuffer.
                ConsoleLinePart readBufferPart = new ConsoleLinePart(_readBuffer, _promptColor);

                // Draw the part
                DrawPart(gfx, readBufferPart, charsOnLine, ref horizontalDrawPosition, ref drawHeight);

                // Draw the cursor
                DrawPromptIndicator(gfx, new Point(horizontalDrawPosition * 10 + 3, drawHeight + 3));
            }
        }

        /// <summary>
        /// Draw a part of a line to the console
        /// </summary>
        /// <param name="gfx">graphics to draw on</param>
        /// <param name="part">the part to draw</param>
        /// <param name="charsOnLine">the amount of chars that can be place on the line</param>
        /// <param name="horizontalDrawPosition">the horizontal draw position</param>
        /// <param name="verticalDrawPosition">the vertical draw position</param>
        private void DrawPart(Graphics gfx, ConsoleLinePart part, int charsOnLine, ref int horizontalDrawPosition, ref int verticalDrawPosition)
        {
            // This variable holdes the text that is going to be drawed.
            // If a part is drawed on the screen, it will be removed from the string.
            // The string will be drawed directly if it fits on one line.
            string stringToDrawLeft = part.Text;

            // Loop until the string to draw is empty.
            while (true)
            {
                // Check if the string fully fits on the line.
                if (stringToDrawLeft.Length <= charsOnLine - horizontalDrawPosition)
                {
                    // It Fits

                    // Draw the string.
                    gfx.DrawString(stringToDrawLeft,                                        // The text of the string.
                                   _stringFont,                                             // The font to use.
                                   part.Color,                                              // The color to use.
                                   new PointF(horizontalDrawPosition * 10, verticalDrawPosition));    // The draw position.

                    // Draw the style
                    StyleDrawer.DrawStyle(gfx, part.Style, part.Color2, new Point(horizontalDrawPosition * 10, verticalDrawPosition), stringToDrawLeft.Length * 10);
                    
                    // Increase the horizontal position to draw by the lengt of the drawed string.
                    horizontalDrawPosition += stringToDrawLeft.Length;

                    // we're done and break :)
                    break;
                }
                else
                {
                    // The string doesn't fit

                    // Draw a part of the string
                    gfx.DrawString(stringToDrawLeft.Substring(0, charsOnLine - horizontalDrawPosition), // The part of the text to draw.
                                   _stringFont,                                                         // The font to use.
                                   part.Color,                                                          // The color to use.
                                   new PointF(horizontalDrawPosition * 10, verticalDrawPosition));                // The draw position.

                    // This removes the drawed part from the string.
                    stringToDrawLeft = stringToDrawLeft.Substring(charsOnLine - horizontalDrawPosition,                             // The lenght of the part we used.
                                                                  stringToDrawLeft.Length - charsOnLine + horizontalDrawPosition);  // The lenght left.

                    // Draw the style
                    StyleDrawer.DrawStyle(gfx, part.Style, part.Color2, new Point(horizontalDrawPosition * 10, verticalDrawPosition), (charsOnLine - horizontalDrawPosition) * 10);

                    // Reset the horizontal draw position.
                    // This is because the line is full.
                    horizontalDrawPosition = 0;

                    // Increase the vertical draw height.
                    // This is because we go to a new line.
                    verticalDrawPosition += 12;

                }
            }
        }

        /// <summary>
        /// Draw the prompt indicator
        /// </summary>
        /// <param name="gfx">graphics to draw on</param>
        /// <param name="position">position to draw the prompt indicator</param>
        private void DrawPromptIndicator(Graphics gfx, Point position)
        {
            // The indicator will go on and off.
            // If it's off we don't draw it.
            // "fillPromptIndicator" indicates if we fill it or not.
            // It will be toggled at the end of this method
            if (_fillPromptIndicator)
            {
                // We draw a other indicator if the cursor position is between the text.
                // The cursor is at the end if "selectedCharFromEnd" is zero.
                if (_selectedCharFromEnd != 0)
                {
                    // The cursor is between the text.
                    // We use an open indicator
                    _useOpenPromptIndicator = true;

                    // This changes the position to draw to a position between the text.
                    position = new Point((int)(position.X - ((_selectedCharFromEnd % 66) * 10)) - 2, position.Y);
                }
                else
                    // The cursor is at the end of the line.
                    // We use a closed indicator
                    _useOpenPromptIndicator = false;

                // Draw an indicator.
                if (_useOpenPromptIndicator)
                    // Draw a open indicator.
                    gfx.DrawLine(_promptSelectCursorColor, new Point(position.X, position.Y + 12), new Point(position.X + 10, position.Y + 12));
                else
                    // Draw a closed indicator.
                    gfx.FillRectangle(_promptCursorColor, new Rectangle(position, new Size(10, 12)));
            }

            // Toggle fillPromptIndicator.
            _fillPromptIndicator = !_fillPromptIndicator;
        }

        #endregion

        #region Key handling

        private void AdvancedConsoleView_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!_prompting) { return; }

            if (e.KeyChar == (char)Keys.Return) // Enter key
            {
                if (!string.IsNullOrEmpty(_readBuffer))
                {
                    if (_suggestionManager != null)
                    {
                        if (_suggestionManager.SelectedSuggestion == -1)
                        {
                            if (!string.IsNullOrEmpty(PromptInfo))
                            {
                                this.WriteLine(PromptInfo, _promptInfoColor);
                                this.Write(_readBuffer, _promptColor);
                            }
                            else
                                this.WriteLine(_readBuffer, _promptColor);
                            _readBufferBack = _readBuffer;
                            _readBuffer = "";
                            if (_owner != null)
                                _owner.ProcessInput(this, _readBufferBack);
                            _readBufferBack = "";
                            _selectedCharFromEnd = 0;
                            _useOpenPromptIndicator = false;
                        }
                        else
                        {
                            _readBufferBack = _readBuffer;
                            _suggestionManager.SelectedSuggestion = -1;
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(PromptInfo))
                        {
                            this.WriteLine(PromptInfo, _promptInfoColor);
                            this.Write(_readBuffer, _promptColor);
                        }
                        else
                            this.WriteLine(_readBuffer, _promptColor);
                        if (_owner != null)
                            _owner.ProcessInput(this, _readBuffer);
                    }
                    

                }
            }
            else if (e.KeyChar == (char)Keys.Back) // Back-space key
            {
                if (_readBuffer.Length > 0)
                {
                    if (_selectedCharFromEnd == 0)
                        _readBuffer = _readBuffer.Substring(0, _readBuffer.Length - 1);
                    else
                    {
                        if (_readBuffer.Length > _selectedCharFromEnd)
                        {
                            _readBuffer = _readBuffer.Remove(_readBuffer.Length - _selectedCharFromEnd - 1, 1);
                            if (_readBuffer.Length < _selectedCharFromEnd)
                                _selectedCharFromEnd--;
                        }
                    }
                    _readBufferBack = _readBuffer;
                }

                if (_suggestionManager != null)
                    _suggestionManager.SelectedSuggestion = -1;
            }
            else
            {
                if (_suggestionManager != null && _readBufferBack != _readBuffer)
                    _suggestionManager.SelectedSuggestion = -1;

                if (_selectedCharFromEnd == 0)
                    _readBuffer += e.KeyChar.ToString();
                else
                    _readBuffer = _readBuffer.Insert(_readBuffer.Length - _selectedCharFromEnd, e.KeyChar.ToString());

                _readBufferBack = _readBuffer;
            }

            this.Refresh();
        }

        protected override bool IsInputKey(Keys keyData)
        {
            
            if (keyData == Keys.Down)
            {
                if (_suggestionManager != null)
                {
                    _readBufferBack = _readBufferBack == "" ? _readBuffer : _readBufferBack;
                    _suggestionManager.SelectedSuggestion++;
                    if (_suggestionManager.SelectedSuggestion > _suggestionManager.GetSuggestionCount(_readBufferBack) - 1)
                    {
                        _suggestionManager.SelectedSuggestion = -1;
                        _readBuffer = _readBufferBack;
                    }
                    else
                        _readBuffer = _suggestionManager.GetSelectedSuggestion(_readBufferBack);
                    _selectedCharFromEnd = 0;
                    _useOpenPromptIndicator = false;
                    this.Refresh();
                }
                return true;
            }
            else if (keyData == Keys.Up)
            {
                if (_suggestionManager != null)
                {
                    if (_suggestionManager.SelectedSuggestion > -1)
                        _suggestionManager.SelectedSuggestion--;

                    if (_suggestionManager.SelectedSuggestion == -1)
                        _readBuffer = _readBufferBack;
                    else
                        _readBuffer = _suggestionManager.GetSelectedSuggestion(_readBufferBack);

                    _selectedCharFromEnd = 0;
                    _useOpenPromptIndicator = false;

                    this.Refresh();
                }
                return true;
            }
            else if (keyData == Keys.Left)
            {
                if (_selectedCharFromEnd < _readBuffer.Length)
                    _selectedCharFromEnd++;
                this.Refresh();
                return true;
            }
            else if (keyData == Keys.Right)
            {
                if (_selectedCharFromEnd > 0)
                    _selectedCharFromEnd--;
                this.Refresh();
                return true;
            }
            else if (keyData == Keys.End)
            {
                _selectedCharFromEnd = 0;
                _useOpenPromptIndicator = false;
                this.Refresh();
                return true;
            }
            else if (keyData == Keys.Home)
            {
                _selectedCharFromEnd = _readBuffer.Length;
                this.Refresh();
                return true;
            }
            else if (keyData == Keys.PageUp)
               // scroll
                return true;
            else if (keyData == Keys.PageDown)
                // scroll
                return true;
            else
                return base.IsInputKey(keyData);
        }

        #endregion

        #region External Input

        /// <summary>
        /// Add an empty line.
        /// </summary>
        public void WriteLine()
        {
            // Call the overload
            WriteLine("", _textColor);
        }

        /// <summary>
        /// Write a line of text to the console.
        /// </summary>
        /// <param name="s">the text to write</param>
        public void WriteLine(string s)
        {
            // Call the overload
            WriteLine(s, _textColor);
        }

        /// <summary>
        /// Write a line of text to the console.
        /// </summary>
        /// <param name="s">the text to write</param>
        /// <param name="color">the color of the text</param>
        public void WriteLine(string s, Brush color)
        {
            // Create a line object.
            ConsoleLine line = new ConsoleLine();

            // Add the text to the line.
            line.AddPart(s, color);

            // Add the line to the line collection.
            _consoleLines.Add(line);

            // Recalculate the scrollbar states
            CalculateNewScrollbarStates(true);

            // Refresh the view.
            this.Refresh();
        }

        /// <summary>
        /// Load a ConsoleLinePart with a string, color, style and a color for the style
        /// </summary>
        public void WriteLine(string s, Brush color, Styles style, Pen color2)
        {
            ConsoleLinePart part = new ConsoleLinePart(s, color);
            part.SetStyle(style, color2);

            // Create a line object.
            ConsoleLine line = new ConsoleLine();

            // Add the text to the line.
            line.AddPart(part);

            // Add the line to the line collection.
            _consoleLines.Add(line);

            // Recalculate the scrollbar states
            CalculateNewScrollbarStates(true);

            // Refresh the view.
            this.Refresh();
        }

        /// <summary>
        /// Write text to the console.
        /// </summary>
        /// <param name="s">The text</param>
        public void Write(string s)
        {
            // Call the overload.
            Write(s, _textColor);
        }

        /// <summary>
        /// Write text to the console
        /// </summary>
        /// <param name="s">text</param>
        /// <param name="color">Color of the text</param>
        public void Write(string s, Brush color)
        {
            // Check if the list contains items
            if (_consoleLines.Count == 0)
                // the list is empty, Use writeline to write text.
                WriteLine(s);
            else
            {
                // Get the last index of the consolelines collection.
                int lastIndex = _consoleLines.Count - 1;

                // Add a part to the line.
                _consoleLines[lastIndex].AddPart(s, color);
            }

            // Recalculate the scrollbar states
            CalculateNewScrollbarStates(true);

            // Refresh the view.
            this.Refresh();
        }

        /// <summary>
        /// Clears all lines of the console.
        /// </summary>
        public void Clear()
        {
            // Clear the collection.
            _consoleLines.Clear();

            // Recalculate the scrollbar states
            CalculateNewScrollbarStates(true);

            // Refresh the view.
            this.Refresh();
        }

        #endregion

        #region UserControl events And Timers

        /// <summary>
        /// This will be called when size of the control changes
        /// </summary>
        private void AdvancedConsoleView_SizeChanged(object sender, EventArgs e)
        {
            CalculateNewScrollbarStates(false);

            // This will refresh the control when the form is resized.
            this.Refresh();
        }

        /// <summary>
        /// The event is called by the refresh timer.
        /// </summary>
        private void RefreshTimer_Tick(object sender, EventArgs e)
        {
            // This refreshes the control
            this.Refresh();
        }

        #endregion

        #region Scroll

        /// <summary>
        /// Initialize the scrollbar
        /// </summary>
        private void InitScrolling()
        {
            if (ConsoleStrollBar != null)
            {
                ConsoleStrollBar.Scroll += new ScrollEventHandler(ConsoleStrollBar_Scroll);

                ConsoleStrollBar.Minimum = 0;

                CalculateNewScrollbarStates(true);
            }
        }

        /// <summary>
        /// Calculate the new states for the scrollbar
        /// </summary>
        /// <param name="moveToEnd"></param>
        private void CalculateNewScrollbarStates(bool moveToEnd)
        {
            if (ConsoleStrollBar != null)
            {
                int displayHeight = Height;
                int contentHeight = CalculateContentHeight();

                if (displayHeight > contentHeight)
                {
                    ConsoleStrollBar.Enabled = false;
                }
                else
                {
                    ConsoleStrollBar.Enabled = true;
                    ConsoleStrollBar.Maximum = contentHeight;
                    ConsoleStrollBar.LargeChange = displayHeight;
                    ConsoleStrollBar.Value = ConsoleStrollBar.Maximum - ConsoleStrollBar.LargeChange;
                    _fromTopHeightShift = -ConsoleStrollBar.Value;
                    this.Refresh();
                }
            }
            else if (moveToEnd)
            {
                int displayHeight = Height;
                int contentHeight = CalculateContentHeight();
                if (displayHeight < contentHeight)
                    _fromTopHeightShift = -contentHeight + displayHeight;
            }
        }

        private int CalculateContentHeight()
        {
            // Calculate how many chars we put on one line.
            int charsOnLine = this.Width / 10;

            int lineCount = 0;

            for (int i = 0; i < _consoleLines.Count; i++)
            {
                int len = _consoleLines[i].getLength();
                int lines = ((len-1) / charsOnLine) + 1;
                lineCount += lines;
            }

            return (int)((float)lineCount * 12.32) + 20;
        }

        void ConsoleStrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            _fromTopHeightShift = -ConsoleStrollBar.Value;
            this.Refresh();
        }

        #endregion

    }
}
