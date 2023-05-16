
// ToggleButton 1.0, by Cliff Earl, Antix Development, April 2019

using System;
using System.Collections.Generic;

namespace MegaMemory
{
    class ToggleButton : Sprite
    {
        private bool Enabled;

        private TextureRegion TrueImage;
        private TextureRegion FalseImage;
        private bool ImageState;

        public event EventHandler OnToggled = delegate { };

        private List<ToggleButton> ToggleGroup; // group this ToggleButton belongs to

        public ToggleButton(TextureRegion trueImage, TextureRegion falseImage, List<ToggleButton> group, bool toggleState)
        {
            Enabled = true;

            TrueImage = trueImage;
            FalseImage = falseImage;
            ImageState = toggleState;
            SetImageState(toggleState);

            ToggleGroup = group; // save and add to group
            group.Add(this);
        }

        /// <summary>
        /// Call custom code when widget changes state
        /// </summary>
        public void TogglebuttonToggled()
        {
            if (OnToggled != null)
            {
                OnToggled(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Set ToggleButton state
        /// </summary>
        /// <param name="state"></param>
        public void Enable(bool state)
        {
            Enabled = state;
        }

        /// <summary>
        /// Get ToggleButton state
        /// </summary>
        /// <returns></returns>
        public bool IsChecked()
        {
            return ImageState;
        }

        /// <summary>
        /// Toggle ToggleButton checked state (mutually exclude others in group)
        /// </summary>
        /// <param name="state"></param>
        public void ToggleCheckState()
        {
            foreach (ToggleButton togglebutton in ToggleGroup)
            {
                togglebutton.SetTogglestate(false);
            }

            ImageState = !ImageState;
            SetImageState(ImageState);
        }

        public void SetTogglestate(bool state)
        {
            ImageState = state;
            SetImageState(state);
        }

        /// <summary>
        /// Set ToggleButton image
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
