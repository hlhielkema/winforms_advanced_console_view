using System.Drawing;

namespace AdvancedConsoleViewNS
{
    internal class ConsoleLinePart
    {
        /// <summary>
        /// The text of the part.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// The color of the text.
        /// </summary>
        public Brush Color { get; set; }

        /// <summary>
        /// The style of the text.
        /// Default is normal.
        /// </summary>
        public Styles Style { get; set; }

        /// <summary>
        /// The color for style components.
        /// </summary>
        public Pen Color2 { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="text">Text value of the part</param>
        /// <param name="color">Color of the part</param>
        public ConsoleLinePart(string text, Brush color)
        {
            Text = text;
            Color = color;
        }

        /// <summary>
        /// Get the amount of chars in this part.
        /// </summary>
        /// <returns>Lenght of text</returns>
        public int getLength()
        {
            // Return the length
            return Text.Length;
        }

        /// <summary>
        /// Set the style of the text.
        /// </summary>
        /// <param name="style">The style</param>
        /// <param name="color">Color of the style</param>
        public void SetStyle(Styles style, Pen color)
        {
            Style = style;
            Color2 = color;
        }
    }
}
