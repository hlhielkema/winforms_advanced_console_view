using System.Drawing;

namespace AdvancedConsoleViewNS
{
    internal static class StyleDrawer
    {
        /// <summary>
        /// This method draws the style
        /// </summary>
        /// <param name="gfx">The graphics to draw on</param>
        /// <param name="style">The style to use</param>
        /// <param name="color">Color to use</param>
        /// <param name="position">Draw Position of the text</param>
        /// <param name="width">Width of the text</param>
        internal static void DrawStyle(Graphics gfx, Styles style, Pen color, Point position, int width)
        {
            switch (style)
            {
                case Styles.StrikeOut:
                    {
                        // Draw a Strike-out
                        Point initPoint = new Point(position.X + 3, position.Y + 8);
                        gfx.DrawLine(color, initPoint, new Point(initPoint.X + width - 6, initPoint.Y));
                        break;
                    }
                case Styles.Underlined:
                    {
                        // Draw a line under the text
                        Point initPoint = new Point(position.X + 3, position.Y + 16);
                        gfx.DrawLine(color, initPoint, new Point(initPoint.X + width - 6, initPoint.Y));
                        break;
                    }
                case Styles.Circled:
                    {
                        // Draw a circle arount the text
                        Point initPoint = new Point(position.X + 3, position.Y + 3);
                        gfx.DrawEllipse(color, initPoint.X, initPoint.Y, width, 12);
                        break;
                    }
                case Styles.Cross:
                    {
                        // Draw a cross through the text
                        Point initPoint = new Point(position.X + 3, position.Y + 3);

                        gfx.DrawLine(color, initPoint,
                                            new Point(initPoint.X + width, initPoint.Y + 12));

                        gfx.DrawLine(color, new Point(initPoint.X, initPoint.Y + 12),
                                            new Point(initPoint.X + width, initPoint.Y));
                        break;
                    }

                default: break;
            }
        }
    }
}
