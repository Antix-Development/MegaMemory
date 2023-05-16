
// CheckBox 1.0, by Cliff Earl, Antix Development, April 2019

using System;

namespace MegaMemory
{
    class CheckBox : Sprite
    {
        private bool Enabled;

        private TextureRegion TrueImage;
        private TextureRegion FalseImage;
        private bool ImageState;

        public event EventHandler OnClicked = delegate { };

        public CheckBox(TextureRegion trueImage, TextureRegion falseImage, bool checkState)
        {
            Enabled = true;

            TrueImage = trueImage;
            FalseImage = falseImage;
            ImageState = checkState;
            SetImageState(checkState);
        }

        /// <summary>
        /// Call custom code when widget changes state
        /// </summary>
        public void CheckboxClicked()
        {
            if (OnClicked != null)
            {
                OnClicked(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Set CheckBox state
        /// </summary>
        /// <param name="state"></param>
        public void Enable(bool state)
        {
            Enabled = state;
        }

        /// <summary>
        /// Get CheckBox state
        /// </summary>
        /// <returns></returns>
        public bool IsChecked()
        {
            return ImageState;
        }

        /// <summary>
        /// Toggle CheckBox checked state
        /// </summary>
        /// <param name="state"></param>
        public void ToggleCheckState()
        {
            ImageState = !ImageState;

            SetImageState(ImageState);
        }

        /// <summary>
        /// Set CheckBox image
        /// </summary>
        /// <param name="state"></param>
        public void SetImageState(bool state)
        {
            if (state)
            {
                SetTextureRegion(TrueImage);
            }
            else
            {
                SetTextureRegion(FalseImage);
            }
        }
    }
}
