
// Sprite 1.0, by Cliff Earl, Antix Development, April 2019

using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Numerics;

namespace MegaMemory
{
    class Sprite
    {
        public List<Sprite> Children { get; private set; }// Sprites that are attached to this Sprite

        private Sprite Parent; // Sprite that this Sprite is attached to

        public bool HasImage; // if true then this Sprite will be drawn to the display

        private TextureRegion _TextureRegion; // TextureRegion containing information required to draw this Sprite (Sprites current graphic)

        private Vector2 Position; // position

        private Vector2 AnchorPosition; // origin of this Sprite for positioning and rotating

        private Vector2 Scale; // scaling for this Sprite

        private int Width; // dimensions of this Sprite
        private int Height;

        private float Alpha; // opacity!!

        private bool Visible; // if true then this Sprite (and it's children) will be drawn to the display

        /// <summary>
        /// Create Sprite
        /// </summary>
        public Sprite()
        {
            Position = new Vector2(0, 0); // all Sprites start here

            Scale = new Vector2(1, 1); // at full size

            AnchorPosition = new Vector2(0, 0); // origin at top left

            Width = 0; // dimensions in pixels
            Height = 0;

            Alpha = 1f; // no opacity
            Visible = true; // visible

            HasImage = false;

            Parent = null; // no parent
            Children = new List<Sprite>(); // no children
        }

        /// <summary>
        /// Set Sprites TextureRegion
        /// </summary>
        /// <param name="textureRegion"></param>
        public void SetTextureRegion(TextureRegion textureRegion)
        {
            _TextureRegion = textureRegion;

            Width = textureRegion.Region.Width; // save these for other calculations
            Height = textureRegion.Region.Height;

            HasImage = true;
        }
        /// <summary>
        /// Get Sprite TextureRegion
        /// </summary>
        /// <param name="textureRegion"></param>
        /// <returns></returns>
        public TextureRegion GetTextureRegion()
        {
            return _TextureRegion;
        }
        
        /// <summary>
                 /// Get Sprite width
                 /// </summary>
                 /// <returns></returns>
        public int GetWidth()
        {
            return Width;
        }

        /// <summary>
        /// Get Sprite height
        /// </summary>
        /// <returns></returns>
        public int GetHeight()
        {
            return Height;
        }

        /// <summary>
        /// Detach Sprite from its parent
        /// </summary>
        public void RemoveFromParent()
        {
            if (Parent != null)
            {
                Parent.Children.Remove(this); // detach from parent
            }
        }

        /// <summary>
        /// Get Sprites parent
        /// </summary>
        /// <returns></returns>
        public Sprite GetParent()
        {
            return Parent;
        }

        /// <summary>
        /// Add a child to Sprite
        /// </summary>
        /// <param name="child"></param>
        public void AddChild(Sprite child)
        {
            child.RemoveFromParent(); // detach from parent
            Children.Add(child); // add as child
            child.Parent = this; // set parent
        }

        /// <summary>
        /// Add child Sprite to this Sprite at specified index
        /// </summary>
        /// <param name="child"></param>
        /// <param name="index"></param>
        public void AddChildAt(Sprite child, int index)
        {
            child.RemoveFromParent(); // detach from parent
            if (index > Children.Count)
            {
                AddChild(child); // add to tail because index exceeds number of children
            }
            else
            {
                Children.Insert(index, child); // add at specified position
                child.Parent = this;
            }
        }

        /// <summary>
        /// Remove child Sprite
        /// </summary>
        /// <param name="index"></param>
        public void RemoveChild(Sprite child)
        {
            if (Children.Contains(child)) // only remove if child is a child of this Sprite
            {
                Children.Remove(child);
            }
        }

        /// <summary>
        /// Remove child Sprite at specified index
        /// </summary>
        /// <param name="index"></param>
        public void RemoveChildAt(int index)
        {
            if (index > Children.Count)
            {
                Children.RemoveAt(Children.Count - 1); // remove last child because index exceeds number of children
            }
            else
            {
                Children.RemoveAt(index); // remove from specified index
            }
        }

        /// <summary>
        /// Get child at specified position, or null if it has no children
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Sprite GetChildAt(int index)
        {
            int count = Children.Count;

            if (count > 0)
            {
                if (index > count)
                {
                    return Children.ElementAt(count); // return the last child because index exceeds number of children
                }
                else
                {
                    return Children.ElementAt(index); // return Sprite at position
                }
            }
            else
            {
                return null; // there are no children
            }
        }

        /// <summary>
        /// Set Sprite origin
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void SetAnchorPoint(float x, float y)
        {
            AnchorPosition.X = Width * x;
            AnchorPosition.Y = Height * y;
        }

        /// <summary>
        /// Get Sprite anchor X position
        /// </summary>
        /// <returns></returns>
        public float GetAnchorPositionX()
        {
            return AnchorPosition.X;
        }

        /// <summary>
        /// Get Sprite anchor Y position
        /// </summary>
        /// <returns></returns>
        public float GetAnchorPositionY()
        {
            return AnchorPosition.Y;
        }

        /// <summary>
        /// Determine Sprite visibility
        /// </summary>
        /// <returns></returns>
        public bool IsVisible()
        {
            return Visible;
        }

        /// <summary>
        /// Set Sprite visibility
        /// </summary>
        /// <param name="state"></param>
        public void SetVisible(bool state)
        {
            Visible = state;
        }

         //
        // POSITIONING METHODS
        //

        /// <summary>
        /// Get Sprite X position
        /// </summary>
        /// <returns></returns>
        public float GetX()
        {
            return Position.X;
        }

        /// <summary>
        /// Set Sprite X position
        /// </summary>
        /// <param name="x"></param>
        public void SetX(float x)
        {
            Position.X = x;
        }

        /// <summary>
        /// Get Sprite X position
        /// </summary>
        /// <returns></returns>
        public float GetY()
        {
            return Position.Y;
        }

        /// <summary>
        /// Set Sprite Y position
        /// </summary>
        /// <param name="y"></param>
        public void SetY(float y)
        {
            Position.Y = y;
        }

        /// <summary>
        /// Set Sprite X and Y positions
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void SetPosition(float x, float y)
        {
            Position.X = x;
            Position.Y = y;
        }

        //
        // SCALING METHODS
        //

        /// <summary>
        /// Get Sprite X scale
        /// </summary>
        /// <returns></returns>
        public float GtScaleX()
        {
            return Scale.X;
        }

        /// <summary>
        /// Set Sprite X scale
        /// </summary>
        /// <param name="x"></param>
        public void SetScaleX(float x)
        {
            Scale.X = x;
        }

        /// <summary>
        /// Get Sprite X scale
        /// </summary>
        /// <returns></returns>
        public float GetScaleY()
        {
            return Scale.Y;
        }

        /// <summary>
        /// Set Sprite Y scale
        /// </summary>
        /// <param name="y"></param>
        public void SetScaleY(float y)
        {
            Scale.Y = y;
        }

        /// <summary>
        /// Set Sprite X and Y scale
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void SetScale(float scale)
        {
            Scale.X = scale;
            Scale.Y = scale;
        }

        //
        // OPACITY METHODS
        //

        /// <summary>
        /// Set Sprite opacity
        /// </summary>
        /// <param name="a"></param>
        public void SetAlpha(float a)
        {
            Alpha = a;
        }

        /// <summary>
        /// Get Sprite opacity
        /// </summary>
        /// <returns></returns>
        public float GetAlpha()
        {
            return Alpha;
        }

        //
        // HIT TEST METHODS
        //

        /// <summary>
        /// Check if Point is inside Sprite
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool ContainsPoint(Point point)
        {
            Rectangle rect = new Rectangle((int)(Position.X - AnchorPosition.X), (int)(Position.Y - AnchorPosition.Y), Width, Height);
            return rect.Contains(point);
        }
    }
}
