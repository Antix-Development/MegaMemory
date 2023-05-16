
// TextField 1.0, by Cliff Earl, Antix Development, April 2019

using System;
using System.Drawing;

namespace MegaMemory
{
    class TextField : Sprite
    {
        public String Text; // text displayed by TextField
        public Font Font; // typeface
        public SolidBrush _SolidBrush; // color
        public StringAlignment Alignment; // alignment

        /// <summary>
        /// Constructor without Color
        /// </summary>
        /// <param name="font"></param>
        /// <param name="text"></param>
        public TextField(Font font, string text)
        {
            Init(font, text);
            _SolidBrush = new SolidBrush(Color.FromArgb(255, 255, 255, 255)); // default color is white
        }

        /// <summary>
        /// Constructor with Color
        /// </summary>
        /// <param name="font"></param>
        /// <param name="text"></param>
        /// <param name="brush"></param>
        public TextField(Font font, string text, Color color)
        {
            Init(font, text);
            _SolidBrush = new SolidBrush(color);
        }

        /// <summary>
        /// Common constructor code
        /// </summary>
        /// <param name="font"></param>
        /// <param name="text"></param>
        private void Init(Font font, string text)
        {
            Font = font;
            Alignment = StringAlignment.Near; // default alignment
            SetText(text);
            HasImage = true; // set this in inherited class so the text actually gets drawn :)
        }

        /// <summary>
        /// Set TextField text alignment
        /// </summary>
        /// <param name="alignment"></param>
        public void SetAlignment(StringAlignment alignment)
        {
            Alignment = alignment;
        }

        /// <summary>
        /// Set TextField text
        /// </summary>
        /// <param name="text"></param>
        public void SetText(string text)
        {
            Text = text;
        }

        /// <summary>
        /// Set TextField color
        /// </summary>
        /// <param name="color"></param>
        public void SetTextColor(Color color)
        {
            _SolidBrush.Color = color;
        }
    }
}
