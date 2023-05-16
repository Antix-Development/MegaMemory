
// Image 1.0, by Cliff Earl, Antix Development, April 2019

namespace MegaMemory
{
    class Image : Sprite
    {
        private TextureRegion _Image;

        /// <summary>
        /// Create new Image
        /// </summary>
        /// <param name="image"></param>
        public Image(TextureRegion image)
        {
            _Image = image;
            SetTextureRegion(image);
        }
    }
}
