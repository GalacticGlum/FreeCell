/*
 * Author: Shon Verch
 * File Name: TextButton.cs
 * Project Name: FreeCell
 * Creation Date: 05/19/19
 * Modified Date: 05/19/19
 * Description: A basic text-based button UI element.
 */

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameUtilities;

namespace FreeCell
{
    /// <summary>
    /// A basic text-based button UI element.
    /// </summary>
    public class TextButton
    {
        /// <summary>
        /// The text of this <see cref="TextButton"/>.
        /// </summary>
        public string Text { get; }

        /// <summary>
        /// The <see cref="SpriteFont"/> to render this <see cref="TextButton"/>.
        /// </summary>
        public SpriteFont Font { get; }

        /// <summary>
        /// The colour of the text when the mouse button is not hovering over this <see cref="TextButton"/>.
        /// </summary>
        public Color RegularTextColour { get; set; } = Color.White;

        /// <summary>
        /// The colour of the text when the mouse button is hovering over this <see cref="TextButton"/>.
        /// </summary>
        public Color HoverTextColour { get; set; } = Color.White;

        /// <summary>
        /// The colour of the background.
        /// </summary>
        public Color RegularBackgroundColour { get; set; } = Color.Transparent;

        /// <summary>
        /// The colour of the background.
        /// </summary>
        public Color HoverBackgroundColour { get; set; } = Color.Transparent;

        /// <summary>
        /// The inner padding between the text and background borders.
        /// </summary>
        public Vector2 InnerPadding
        {
            get => innerPadding;
            set
            {
                if (innerPadding == value) return;

                innerPadding = value;
                UpdateRectangle();
            }
        }

        /// <summary>
        /// The bounding rectangle of this <see cref="TextButton"/>.
        /// </summary>
        public RectangleF Rectangle;

        /// <summary>
        /// The <see cref="TextButton"/> clicked callback.
        /// Invoked when the mouse button is over this <see cref="TextButton"/> and the left mouse button is pressed.
        /// </summary>
        public event Action Clicked;

        private Color currentTextColour;
        private Color currentBackgroundColour;
        private Vector2 innerPadding;

        /// <summary>
        /// Initializes a new <see cref="TextButton"/>.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="font">The <see cref="SpriteFont"/>.</param>
        /// <param name="text">The text.</param>
        /// <param name="innerPadding">The inner padding from the text to the button border.</param>
        public TextButton(Vector2 position, SpriteFont font, string text, Vector2? innerPadding = null)
        {
            Text = text;
            Font = font;

            Rectangle.Position = position;
            InnerPadding = innerPadding.GetValueOrDefault(Vector2.Zero);

            UpdateRectangle();

            currentTextColour = RegularTextColour;
            currentBackgroundColour = RegularBackgroundColour;
        }

        /// <summary>
        /// Updates the bounding rectangle for this<see cref="TextButton"/>.
        /// </summary>
        private void UpdateRectangle()
        {
            Vector2 size = new Vector2(Font.MeasureString(Text).X + InnerPadding.X * 2, Font.LineSpacing + InnerPadding.Y * 2);
            Rectangle = new RectangleF(Rectangle.Position, size);
        }

        /// <summary>
        /// Update this <see cref="TextButton"/>.
        /// </summary>
        public void Update()
        {
            if (Rectangle.Contains(Input.MousePosition))
            {
                currentTextColour = HoverTextColour;
                currentBackgroundColour = HoverBackgroundColour;

                if (Input.GetMouseButtonDown(MouseButton.Left))
                {
                    Clicked?.Invoke();
                }
            }
            else
            {
                currentTextColour = RegularTextColour;
                currentBackgroundColour = RegularBackgroundColour;
            }
        }

        /// <summary>
        /// Render this <see cref="TextButton"/>.
        /// </summary>-
        /// <param name="spriteBatch"></param>
        /// <param name="layerDepth"></param>
        public void Draw(SpriteBatch spriteBatch, float layerDepth = 0)
        {
            Vector2 offset = new Vector2(0, Rectangle.Height * 0.5f);
            spriteBatch.DrawLine(Rectangle.Position + offset, Rectangle.TopRight + offset, currentBackgroundColour, Rectangle.Height);

            float textX = Rectangle.X + (Rectangle.Width - Font.MeasureString(Text).X) * 0.5f;
            float textY = Rectangle.Y + (Rectangle.Height - Font.LineSpacing) * 0.5f;
            Vector2 textPosition = new Vector2(textX, textY);

            spriteBatch.DrawString(Font, Text, textPosition, currentTextColour, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, layerDepth);
        }
    }
}