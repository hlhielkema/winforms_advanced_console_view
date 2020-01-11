using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace AdvancedConsoleViewNS
{
    /// <summary>
    /// An Internal class to manage the suggestions and draw them.
    /// </summary>
    /// <author>Hielke Hielkema</author>
    internal class SuggestionManager
    {
        // Public:
        public Brush SuggestionsMenuColor { get; set; } = Brushes.Green;
        public Brush SuggestionsTextColor { get; set; } = Brushes.Gray;
        public Pen SuggestionsCursorColor { get; set; } = Pens.Magenta;        
        public Font StringFont { get; set; }
        public List<string> Suggestions { get; set; }

        // Internal:
        internal int SelectedSuggestion = -1;

        /// <summary>
        /// Constructor
        /// </summary>
        internal SuggestionManager()
        {
            StringFont = new Font("Courier New", 12F);
            Suggestions = new List<string>();
        }

        #region Rendering

        /// <summary>
        /// This draws the suggestions on the screen.
        /// The suggestions depend on the searchWord.
        /// This "searchWord" is the input from the user.
        /// </summary>
        /// <param name="gfx">the graphics</param>
        /// <param name="drawHeight">y</param>
        /// <param name="searchWord">word to search for</param>
        /// <param name="horizontalDrawPosition">x</param>
        /// <param name="windowHeight">the height of the control. (this.height)</param>
        public void DrawSuggestions(Graphics gfx, int drawHeight, string searchWord, int horizontalDrawPosition, int windowHeight)
        {
            // Get the amount of suggestions
            int suggestionCount = GetSuggestionCount(searchWord);

            // Check if suggestions are found and Enabled
            if (suggestionCount > 0)
            {
                // Check if we need to draw it under or above the prompt line.
                if (drawHeight + 19 + ((suggestionCount > 6) ? 90 : suggestionCount * 16) <= windowHeight)
                {
                    // Under

                    // Draw the suggestion list.
                    DrawSuggestion(gfx,                                                          // The graphics
                                   new Point(horizontalDrawPosition * 10 - 6, drawHeight + 19),  // Draw Position
                                   searchWord);                                          // Selected suggestion
                }
                else
                {
                    // Above

                    // Create a background for the list.
                    gfx.FillRectangle(Brushes.Black, new Rectangle(new Point(
                                                                            horizontalDrawPosition * 10 - 5,                                       // Vertical position
                                                                            drawHeight - ((suggestionCount > 6) ? 6 : suggestionCount) * 16),      // Horizontal position
                                                                   new Size(
                                                                            getLongestSuggestion(GetSuggestions(searchWord),searchWord).Length * 12,                                    // Width
                                                                            ((suggestionCount > 6) ? 6 : suggestionCount) * 16)));                 // Height

                    // Draw the suggestion list.
                    DrawSuggestion(gfx,                                                                                                          // The graphics
                                   new Point(horizontalDrawPosition * 10 - 5, drawHeight - ((suggestionCount > 6) ? 6 : suggestionCount) * 16),  // Draw position
                                   searchWord);                                                                                          // Selected suggestion

                }
            }
        }

        /// <summary>
        /// Draws the suggestions.
        /// The position will not be calculated in here.
        /// </summary>
        /// <param name="gfx">the graphics</param>
        /// <param name="locaction">draw locations</param>
        /// <param name="searchWord">the word to search for</param>
        private void DrawSuggestion(Graphics gfx, Point locaction, string searchWord)
        {
            // Get the suggestions count
            int suggestionCount = GetSuggestionCount(searchWord);
            List<string> suggestions = GetSuggestions(searchWord);
            
            try
            {
                // If the there are more then 5 suggestions, scroll gets turned on.
                if (suggestionCount <= 5)
                {
                    // No stroll

                    // This int stores the amount of chars we move to the right.
                    // This is because there is an auto hide function build-in.
                    int lenToRight = 0;

                    // Loop through all the suggestions
                    for (int i = 0; i < suggestionCount; i++)
                    {
                        // Auto hides the first part of the string
                        string stringToDraw = ShortSuggestion(suggestions[i], ref lenToRight, searchWord);

                        // Draw the string
                        gfx.DrawString(stringToDraw,                                                              // String
                                       StringFont,                                                                // Font
                                       SuggestionsTextColor,                                                      // Color
                                       new PointF(locaction.X + 6 + (lenToRight * 10), locaction.Y + (i * 16)));  // Position
                    }

                    // Draw the menu bar at the left size.
                    gfx.FillRectangle(SuggestionsMenuColor, new Rectangle(new Point(locaction.X + (lenToRight * 10), locaction.Y), new Size(4, suggestions.Count * 16)));

                    // Draw a cursor for the selected item.
                    if (SelectedSuggestion != -1)
                        gfx.DrawLine(SuggestionsCursorColor,
                                    new Point(locaction.X + 8 + (lenToRight * 10), locaction.Y + (SelectedSuggestion * 16) + 16),
                                    new Point(locaction.X + (suggestions[SelectedSuggestion].Length * 12) + 2, locaction.Y + (SelectedSuggestion * 16) + 16));
                }
                else
                {
                    // Scroll

                    int scrollLevel = (SelectedSuggestion > 2) ? SelectedSuggestion - 2 : 0;

                    // The number of items we need to load
                    int amoutToLoad = (suggestions.Count - scrollLevel > 5) ? 5 : (suggestions.Count - scrollLevel);

                    // The index of the first item to load
                    int startLoadIndex = scrollLevel;


                    if (amoutToLoad != 5)
                    {
                        startLoadIndex -= 5 - amoutToLoad;
                        amoutToLoad = 5;
                    }

                    // A collection of all the items to view
                    List<string> itemsToView = GetPartOfList(suggestions, startLoadIndex, amoutToLoad);

                    // This int stores the amount of chars we move to the right.
                    // This is because there is an auto hide function build-in.
                    int lenToRight = 0;

                    // Loop through all the suggestions to view
                    for (int i = 0; i < itemsToView.Count; i++)
                    {
                        // Auto hides the first part of the string
                        string stringToDraw = ShortSuggestion(itemsToView[i], ref lenToRight, searchWord);

                        // Draw the string
                        gfx.DrawString(stringToDraw,                                                              // String
                                       StringFont,                                                                // Font
                                       SuggestionsTextColor,                                                      // Color
                                       new PointF(locaction.X + 6 + (lenToRight * 10), locaction.Y + (i * 16)));  // Position
                    }

                    // Correct the cursor position depending on the selected item.
                    int cursorPosition = (SelectedSuggestion > 2) ? 2 : SelectedSuggestion;

                    // Correct the cursor position by this.
                    int fromTopDiff = 5 - ((suggestions.Count - scrollLevel > 5) ? 5 : (suggestions.Count - scrollLevel));   

                    // Draw a cursor for the selected item.
                    if (SelectedSuggestion != -1)
                        gfx.DrawLine(SuggestionsCursorColor,
                                     new Point(locaction.X + 8 + (lenToRight * 10),                                 // X1
                                               locaction.Y + ((cursorPosition + fromTopDiff) * 16) + 16),           // Y1
                                     new Point(locaction.X + (suggestions[SelectedSuggestion].Length * 10) + 2,     // X2
                                               locaction.Y + ((cursorPosition + fromTopDiff) * 16) + 16));          // Y2

                    // Draw the menu bar at the left size.
                    gfx.FillRectangle(SuggestionsMenuColor,
                                     new Rectangle(new Point(locaction.X + (lenToRight * 10), // X
                                                             locaction.Y),                    // Y
                                                   new Size(4,                                // Width
                                                            itemsToView.Count * 16)));        // Height

                    // Check if there are items hidden
                    if (itemsToView.Count == 5 && ((suggestions.Count - scrollLevel) - itemsToView.Count) > 0)
                    {
                        // Indicate how many items are hidden(up in list).
                        gfx.DrawString(string.Format("... +{0}", (suggestions.Count - scrollLevel) - itemsToView.Count), StringFont, SuggestionsMenuColor, new PointF(locaction.X + 6 + (lenToRight * 10), locaction.Y + 80));
                    }

                    // Check if there are items hidden
                    if (scrollLevel > 0)
                    {
                        // Indicate how many items are hidden(down in list).
                        gfx.DrawString(string.Format("↑{0}", scrollLevel - (5 - amoutToLoad)), StringFont, SuggestionsMenuColor, new Point(locaction.X - 25 + (lenToRight * 10), locaction.Y));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error is suggestions system: " + ex.Message);
            }
        }

        #endregion

        #region Internal Working.

        /// <summary>
        /// Gives all the matching suggestions
        /// </summary>
        /// <param name="searchWord">the word to search for</param>
        /// <returns>a list of suggestions</returns>
        internal List<string> GetSuggestions(string searchWord)
        {
            // Create a list to temporary store the founded items
            List<string> returnSuggestionList = new List<string>();

            // Check if the searchWord contains something
            if (searchWord != "")
            {
                // Loop through the items in items in the suggestions list.
                for (int i = 0; i < Suggestions.Count; i++)
                {
                    // Check if the suggestion contains the word to search for.
                    if (Suggestions[i].Contains(searchWord) && Suggestions[i] != searchWord)
                    {
                        // if yes, But the suggestion in the matches list
                        returnSuggestionList.Add(Suggestions[i]);
                    }
                }
            }

            // return the founded suggestions.
            return returnSuggestionList;
        }

        /// <summary>
        /// Gets the value of the selected word
        /// </summary>
        /// <param name="searchWord">the word to search for</param>
        /// <returns>the selected word</returns>
        internal string GetSelectedSuggestion(string searchWord)
        {
            // Gets the suggestion currently selected.
            return GetSuggestions(searchWord)[SelectedSuggestion];
        }

        /// <summary>
        /// Gives the amount of suggestions
        /// </summary>
        /// <param name="searchWord">the word to search</param>
        /// <returns>suggestion count</returns>
        internal int GetSuggestionCount(string searchWord)
        {
            // Gets the amount of suggestions
            return GetSuggestions(searchWord).Count;
        }

        /// <summary>
        /// This gets the longest suggestion in the list.
        /// </summary>
        /// <returns>the longest suggestion</returns>
        private string getLongestSuggestion(List<string> suggestions, string searchWord)
        {
            int i = 0; // To use as a ref.

            // String to store the currently longest string.
            string longest = "";

            // Loop through all the suggestions
            foreach (string s in suggestions)
            {
                // If the suggestion to check is longer then the current longest suggestion,
                // Set is as the longest.
                if (s.Length > longest.Length)
                {
                    // Set it as the longest
                    longest = s;
                }
            }
            return ShortSuggestion(longest, ref i, searchWord); ;
        }

        /// <summary>
        /// This method gives the first x elements of the list in a list.
        /// </summary>
        /// <param name="list">the list</param>
        /// <param name="startIndex">where to start in the list</param>
        /// <param name="len">how many items (x)</param>
        /// <returns></returns>
        private List<string> GetPartOfList(List<string> list, int startIndex, int len)
        {
            // The new list to temporary store the items
            List<string> newList = new List<string>();
            
            // Loop until we get enough items in the list.
            for (int i = startIndex; newList.Count != len; i++)
            {
                newList.Add(list[i]);
            }
            
            // Return the new list.
            return newList;
        }

        /// <summary>
        /// This method makes the suggestion shorter
        /// </summary>
        /// <param name="s">Input string</param>
        /// <param name="numOfCharsRemoved">How many chars are moved</param>
        /// <returns>The new string</returns>
        private string ShortSuggestion(string s, ref int numOfCharsRemoved, string searchWord)
        {
            // Check if the suggestion can be shorten
            if (s.Contains("."))
            {
                // Declare "index" and set it at zero.
                int index = 0;

                // Loop throuh the chars of "searchWord"
                // Here we search for the last '.' in "searchWord"
                for (int i = searchWord.Length - 1; i >= 0; i--)
                {
                    // Check if it's a '.'
                    if (searchWord[i] == '.')
                    {
                        // Set index to the index of the last found.
                        index = i;
                        break;
                    }
                }

                // Return the amount of chars removed.
                numOfCharsRemoved = index;
                
                // Return the shorten suggestion
                return s.Substring(index, s.Length - index);
            }
            else
                return s;
        }

        #endregion

        #region External access

        /// <summary>
        /// Clear the list of suggestions
        /// </summary>
        internal void ClearList()
        {
            Suggestions.Clear();
        }

        /// <summary>
        /// Add a item to the suggestions list.
        /// </summary>
        /// <param name="item">item to add</param>
        internal void AddItem(string item)
        {
            Suggestions.Add(item);
        }

        /// <summary>
        /// Add multiple items to the suggestions list.
        /// </summary>
        /// <param name="items">a list of the items to add.</param>
        internal void AddRange(List<string> items)
        {
            Suggestions.AddRange(items);
        }

        #endregion

    }
}
