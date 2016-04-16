using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Expand.core.space
{
    /// <summary>
    /// A "star" SpaceObject. The most common, fills most of space.
    /// </summary>
    public class Star : SpaceObject
    {
        public int texture_number;
        public static int MAX_SIZE = 20;
        public static int PER_SECTOR = 5000;

        public Star(Sector sector_inside, int x, int y) : base()
        {
            this.pos[0] = x;
            this.pos[1] = y;
            this.addToSector(sector_inside);
        }

        /// <summary>
        /// Returns star collideshape, which is always false.
        /// </summary>
        /// <returns>false</returns>
        public override bool collidesWith(SpaceObject obj)
        {
            return false;
        }

        /// <summary>
        /// Draws a star texture in space.
        /// </summary>
        public override void draw()
        {
            Program.game.drawSprite(this.sprite.frame, pos[0], pos[1], layer: 0f);
        }

        public override void setSprite()
        {
            this.texture_number = Program.game.rand_gen.Next(1, 3);
            this.sprite = getRandomSprite(texture_number);
        }

        /// <summary>
        /// Returns one of the star textures. Based on the value that is saved with the star.
        /// </summary>
        /// <param name="number">Texture number of star.</param>
        /// <returns>A Texture2D of a star.</returns>
        public Sprite getRandomSprite(int number)
        {
            if (number == 1)
            {
                return Program.game.sprites["space\\star1.png"];
            }
            else if (number == 2)
            {
                return Program.game.sprites["space\\star2.png"];
            }
            else
            {
                throw new NotImplementedException(String.Format("No star type {0}", number));
            }
        }
    }
}
