
// SoundBank 1.0, by Cliff Earl, Antix Development, April 2019

using System;
using System.Collections.Generic;
using System.IO;
using System.Media;
using System.Windows.Forms;

namespace MegaMemory
{
    /// <summary>
    /// A collection of SoundPlayers that can be played and stopped by name
    /// </summary>
    class SoundBank
    {
        private Dictionary<string, SoundPlayer> Sounds; // we use a dictionary so that we can play sounds by name

        private bool SoundEnabled;
        private string Path;

        /// <summary>
        /// Create a SoundBank
        /// </summary>
        public SoundBank()
        {
            SoundEnabled = true;
            Sounds = new Dictionary<string, SoundPlayer>(); // initialize the sounds table
            Path = Directory.GetCurrentDirectory() + @"\assets\sounds\"; // the directory where wav files are stored
        }

        /// <summary>
        /// Add a new wav sound effect to the soundbank
        /// </summary>
        /// <param name="name"></param>
        public void loadSound(string name)
        {
            try
            {
                Sounds.Add(name, new SoundPlayer(Path + name + ".wav")); // create a new SoundPlayer and add it to the SoundBank
            }
            catch (Exception)
            {
                MessageBox.Show("Unable to load sound file!");
                throw;
            }
        }

        /// <summary>
        /// Play named sound
        /// </summary>
        /// <param name="soundName"></param>
        public void playSound(string soundName)
        {
            if (Sounds.ContainsKey(soundName) && SoundEnabled)
            {
                SoundPlayer sound = (SoundPlayer)Sounds[soundName];
                sound.Play();
            }
        }

        /// <summary>
        /// Play repeating named sound
        /// </summary>
        /// <param name="soundName"></param>
        public void playSoundLooped(string soundName)
        {
            if (Sounds.ContainsKey(soundName) && SoundEnabled)
            {
                SoundPlayer sound = (SoundPlayer)Sounds[soundName];
                sound.PlayLooping();
            }
        }

        /// <summary>
        /// Stop named sound
        /// </summary>
        /// <param name="soundName"></param>
        public void stopSound(string soundName)
        {
            if (Sounds.ContainsKey(soundName))
            {
                SoundPlayer sound = (SoundPlayer)Sounds[soundName]; // retrieve the SoundPlayer and stop it
                sound.Stop();
            }
        }

        /// <summary>
        /// Stop all sound
        /// </summary>
        public void stopAll()
        {
            foreach (KeyValuePair<string, SoundPlayer> item in Sounds)
            {
                item.Value.Stop();
            }
        }

        /// <summary>
        /// Toggle sound effect output
        /// </summary>
        /// <param name="state"></param>
        public void enable(bool state)
        {
            SoundEnabled = state;
            if (!state)
            {
                stopAll();
            }
        }
    }
}
