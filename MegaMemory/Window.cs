
// Window 1.0, by Cliff Earl, Antix Development, April 2019

namespace MegaMemory
{
    class Window : Sprite
    {
        private bool Enabled;

        /// <summary>
        /// Create Window
        /// </summary>
        public Window() => Enabled = true;

        /// <summary>
        /// Set Window state
        /// </summary>
        /// <param name="state"></param>
        public void Enable(bool state) => Enabled = state;

        /// <summary>
        /// Get Window state
        /// </summary>
        /// <returns></returns>
        public bool IsEnabled() => (Enabled);
    }
}
