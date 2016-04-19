using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Expand.core.space
{
    /// <summary>
    /// Mineral giving SpaceObject.
    /// </summary>
    public class Asteroid : SpaceObject
    {
        public int diameter;
        public float minerals;
        public static int MAX_SIZE = 100;
        public static int MIN_SIZE = 25;
        public static int PER_SECTOR = 15;
        private static int PADDING_DISTANCE = 15;

        public Asteroid(Sector sector_inside, int x, int y)
        {
            this.solid = true;
            this.pos[0] = x;
            this.pos[1] = y;
            this.diameter = Program.game.rand_gen.Next(MIN_SIZE, MAX_SIZE);
            this.minerals = (float)(Math.Pow((diameter / 8 / 2), 3) * Math.PI * 4 / 3);
            this.scale = (float)diameter / sprite.frame.Width;
            this.addToSector(sector_inside);
        }

        public override void setSprite()
        {
            this.sprite = Program.game.sprites["space\\asteroid.png"];
        }
        
        /// <summary>
        /// Scales and draws asteroid based on diameter and level of unmined minerals.
        /// </summary>
        public override void draw()
        {
            Vector2 pos_vector = new Vector2(pos[0], pos[1]);
            int green_level = (int)((float)this.minerals / (float)(this.diameter * 5) * 80);
            Color asteroid_color = new Color(0, green_level + 47, 14);
            Vector2 origin = sprite.getOrigin();
            Program.game.drawSprite(this.sprite.frame, pos[0], pos[1], scale: scale, origin: origin, color: asteroid_color, layer: 0.1f);
        }

        /// <summary>
        /// Attempts to remove minerals from asteroid.
        /// </summary>
        /// <param name="count">Number of minerals to remove.</param>
        /// <returns>Boolean whether or not the minerals could be removed.</returns>
        public int harvestMinerals(int count = 1)
        {
            if (this.minerals > 0)
            {
                this.minerals -= count;
                Program.game.ship.minerals += count;
            }
            else
            {
                return 0;
            }
            return count;
        }

        /// <summary>
        /// Increases the mineral count by a small amount.
        /// </summary>
        public void replenishMinerals()
        {
            if (this.minerals < this.diameter * 5)
            {
                this.minerals += 0.02F;
            }
        }

        /// <summary>
        /// Handles collision detection, mineral regeneration, and mining.
        /// </summary>
        public override void update()
        {
            Vector2 origin = sprite.getOrigin();
            int[] draw_offset = Program.game.drawOffset(pos[0], pos[1]);
            if (Program.game.ship.tool.getTool() == 1 && Program.game.inView(pos[0], pos[1]) && Program.game.ship.collideLaser(draw_offset[0], draw_offset[1], (int)(scale * sprite.frame.Width / 2)))
            {
                this.harvestMinerals();
            }
            this.replenishMinerals();
        }
    }
}