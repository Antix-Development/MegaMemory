
// TextureRegion 1.0, by Cliff Earl, Antix Development, April 2019

using System.Drawing;

namespace MegaMemory
{
    /// <summary>
    /// Bitmap & Rectangle container
    /// </summary>
    class TextureRegion
    {
        public Bitmap Texture; // texture atlas
        public Rectangle Region; // TextureRegions position and dimensions within atlas

        /// <summary>
        /// Create a new TextureRegion
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="w"></param>
        /// <param name="h"></param>
        public TextureRegion(Bitmap texture, int x, int y, int w, int h)
        {
            Texture = texture;
            Region = new Rectangle(x, y, w, h); // Rectangle containing textures coordinates inside a texture atlas
        }
    }
}
