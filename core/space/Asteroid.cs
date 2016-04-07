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
        public int[] center_point = { 0, 0 };

        public Asteroid(Sector sector_inside, int x, int y)
        {
            this.pos[0] = x;
            this.pos[1] = y;
            this.diameter = Program.game.rand_gen.Next(MIN_SIZE, MAX_SIZE);
            this.minerals = this.diameter * 5;
            this.center_point[0] = this.pos[0] + this.diameter / 2;
            this.center_point[1] = this.pos[1] + this.diameter / 2;
            this.addToSector(sector_inside);
        }

        public override void setSprite()
        {
            this.sprite = Program.game.sprites["space\\asteroid.png"];
        }

        /// <summary>
        /// Returns circular collideshape of asteroid
        /// </summary>
        /// <returns>Circle collideShape</returns>
        public override Object getCollideShape()
        {
            return new Circle(pos[0], pos[1], this.diameter / 2);
        }

        /// <summary>
        /// Scales and draws asteroid based on diameter and level of unmined minerals.
        /// </summary>
        public override void draw()
        {
            float scale = diameter / 50f;
            Vector2 pos_vector = new Vector2(pos[0], pos[1]);
            int green_level = (int)((float)this.minerals / (float)(this.diameter * 5) * 80);
            Color asteroid_color = new Color(0, green_level + 47, 14);
            Vector2 origin = new Vector2(0, 0);
            Program.game.drawSprite(this.sprite.getFrame(), pos[0], pos[1], scale: scale, color: asteroid_color, layer: 0.1f);
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
            if (Util.distance(Program.game.ship.pos[0], Program.game.ship.pos[1], this.center_point[0], this.center_point[1]) <= this.diameter / 2 + Asteroid.PADDING_DISTANCE)
            {
                // Stop the player from moving closer to asteroid
                Program.game.ship.reverse();
            }
            int[] asteroid_draw_pos = Program.game.drawOffset(this.center_point[0], this.center_point[1]);
            if (Program.game.ship.tool.getTool() == 1 && Program.game.inView(this.center_point[0], this.center_point[1]) && Program.game.ship.collideLaser(asteroid_draw_pos[0], asteroid_draw_pos[1], this.diameter / 2))
            {
                this.harvestMinerals();
            }
            this.replenishMinerals();
        }
    }
}