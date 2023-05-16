
// Button 1.0, by Cliff Earl, Antix Development, April 2019

using System;

namespace MegaMemory
{
    class Button : Sprite
    {
        public bool Enabled; //{ get; set; }

        private TextureRegion UpImage;
        private TextureRegion DownImage;
        private bool ImageState;

        public event EventHandler OnButtonUp = delegate { };
        public event EventHandler OnButtonDown = delegate { };

        public Button(TextureRegion upImage, TextureRegion downImage)
        {
            Enabled = true;

            UpImage = upImage;
            DownImage = downImage;
            ImageState = false;

            SetTextureRegion(upImage);
        }

        /// <summary>
        /// Call custom code when widget was clicked
        /// </summary>
        public void ButtonUp()
        {
            if (OnButtonUp != null)
                OnButtonUp(this, EventArgs.Empty);
        }

        /// <summary>
        /// Call custom code when widget was pressed
        /// </summary>
        public void ButtonDown()
        {
            if (OnButtonDown != null)
                OnButtonDown(this, EventArgs.Empty);
        }

        /// <summary>
        /// Set Button state
        /// </summary>
        /// <param name="state"></param>
        public void Enable(bool state)
        {
            Enabled = state;
        }

        /// <summary>
        /// Get Button state
        /// </summary>
        /// <returns></returns>
        public bool IsEnabled()
        {
            return Enabled;
        }

        /// <summary>
        /// Set image state (up image or down image)
        /// </summary>
        /// <param name="state"></param>
        public void SetImageState(bool state)
        {
            if (state)
            {
                SetTextureRegion(DownImage);
            }
            else
            {
                SetTextureRegion(UpImage);
            }
            ImageState = !ImageState;
        }
    }
}
