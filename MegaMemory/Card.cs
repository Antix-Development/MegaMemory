
// Card 1.0, by Cliff Earl, Antix Development, April 2019

using System.Numerics;

namespace MegaMemory
{
    class Card : Sprite
    {
        public int FaceValue;

        private TextureRegion BackImage;
        private TextureRegion FaceImage;

        public bool Enabled; // card can be clicked or not

        public bool FaceUp; // card face up or face down?

        public Vector2 GridPosition;

        /// <summary>
        /// Create a Card
        /// </summary>
        /// <param name="faceValue"></param>
        /// <param name="backImage"></param>
        /// <param name="faceImage"></param>
        public Card(int faceValue, TextureRegion faceImage, TextureRegion backImage)
        {
            FaceValue = faceValue;

            FaceImage = faceImage;
            BackImage = backImage;

            SetFaceUp(false);
            SetAnchorPoint(0.5f, 0.5f);

            GridPosition = new Vector2(0, 0);
        }

        /// <summary>
        /// Enable or disable Card
        /// </summary>
        /// <param name="state"></param>
        public void Enable(bool state)
        {
            Enabled = state;
        }

        /// <summary>
        /// Set Card face up or face down
        /// </summary>
        /// <param name="state"></param>
        public void SetFaceUp(bool state)
        {
            if (state)
            {
                SetTextureRegion(FaceImage);
            }
            else
            {
                SetTextureRegion(BackImage);
            }
            FaceUp = state;
        }

    }
}
