
// TexturePack 1.0, by Cliff Earl, Antix Development, April 2019

using System;
using System.IO;
using System.Drawing;
using System.Collections;
using System.Windows.Forms;

namespace MegaMemory
{
    /// <summary>
    /// A pack of many images (combined into a single large image) that can be fetched by name for drawing purposes
    /// </summary>
    class TexturePack
    {
        private Bitmap Texture; // texture atlas
        private Hashtable TextureRegions; // we use a hash table so that we can request texture regions for images using their actual names, ie; "gocart.png", etc, etc

        /// <summary>
        /// Create a new TexturePack
        /// </summary>
        /// <param name="packName"></param>
        public TexturePack(string packName)
        {
            try
            {
                string path = Directory.GetCurrentDirectory() + @"\assets\texturepacks\"; // the directory where TexturePacks are stored

                Texture = new Bitmap(path + packName + ".png"); // load the atlas image

                TextureRegions = new Hashtable(); // initialize the regions table

                foreach (string line in File.ReadAllLines(path + packName + ".txt")) // read the regions file and process each line (1 region per line)
                {
                    string[] parts = line.Split(','); // split string into an array of strings using , as the delimiter

                    // the array of strings now contains the follolwing data...
                    // parts[0] is the name of the sub image inside the atlas
                    // parts[1] is the x position of the image in the atlas
                    // parts[2] is the y position of the image in the atlas
                    // parts[3] is the width of the image
                    // parts[4] is the height of the image
                    // parts[...] are other values written by Gideros TexturePacker and not used in this application

                    TextureRegions.Add(parts[0], new TextureRegion(Texture, int.Parse(parts[1]), int.Parse(parts[2]), int.Parse(parts[3]), int.Parse(parts[4]))); // add a new region to the regions table
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Unable to create TexturePack!"); // one or more files failed to load
                throw;
            }
        }

        /// <summary>
        /// Returns the region associated with the named image
        /// </summary>
        /// <param name="textureName"></param>
        /// <returns></returns>
        public TextureRegion GetTextureRegion(string textureName)
        {
            if (TextureRegions.Contains(textureName)) // does the image exist?
            {
                return (TextureRegion)TextureRegions[textureName]; // return the region for the image
            }
            else
            {
                return new TextureRegion(Texture, 0, 0, 2, 2); // return a default 2x2 pixel region if name was incorrect
            }
        }
    }
}
