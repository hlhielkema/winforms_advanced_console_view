using System.Collections.Generic;
using System.Drawing;

namespace AdvancedConsoleViewNS
{
    internal class ConsoleLine
    {
        /// <summary>
        /// Collection of the string parts.
        /// </summary>
        public List<ConsoleLinePart> parts;

        /// <summary>
        /// Constructor
        /// </summary>
        public ConsoleLine()
        {
            parts = new List<ConsoleLinePart>();
        }

        /// <summary>
        /// Add text to the line.
        /// </summary>
        /// <param name="text">text to add</param>
        /// <param name="color">the color of the text</param>
        public void AddPart(string text, Brush color)
        {
            // Create the part object.
            ConsoleLinePart part = new ConsoleLinePart(text, color);

            // Add the object to the collection.
            parts.Add(part);
        }

        /// <summary>
        /// Add a part to the line.
        /// </summary>
        /// <param name="part">the part to add.</param>
        public void AddPart(ConsoleLinePart part)
        {
            // Add the part object to the collection.
            parts.Add(part);
        }

        /// <summary>
        /// Get the amount of chars on this line
        /// </summary>
        /// <returns>amount of chars</returns>
        public int getLength()
        {
            // Create a value to store the length.
            int len = 0;

            // Loop all trough all the parts
            foreach (ConsoleLinePart part in parts)
                // Add the lenght of the part to the total length
                len += part.Text.Length;

            // Return the value
            return len;
        }

        /// <summary>
        /// Get the full line as a string
        /// </summary>
        /// <returns>the full line as a string</returns>
        public string getFullString()
        {
            // Create a value to store the text.
            string text = "";

            // Loop all trough all the parts
            foreach (ConsoleLinePart part in parts)
                // Add the text of the part to the total text
                text += part.Text;

            // Return the value
            return text;
        }
    }
}
