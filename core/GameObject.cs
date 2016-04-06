using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Expand.core
{
    /// <summary>
    /// Base class for each object in the game. Provides dummy update and draw methods, along with basic data of every GameObject.
    /// </summary>
    public class GameObject
    {
        protected bool alive = true;
        public int[] pos = { 0, 0 };
        public GameObject()
        {
            Program.game.object_handler.addObject(this);
        }

        /// <summary>
        /// Called instead of update, makes sure to return alive status so you don't have to.
        /// </summary>
        /// <returns>Boolean whether or not the GameObject is still alive.</returns>
        public virtual bool preUpdate()
        {
            update();
            return this.alive;
        }

        /// <summary>
        /// Dummy update method.
        /// </summary>
        public virtual void update()
        {

        }

        /// <summary>
        /// Dummy draw method.
        /// </summary>
        public virtual void draw()
        {

        }

        /// <summary>
        /// Sets a GameObject as dead. It will be removed in the next update cycle.
        /// </summary>
        public virtual void setDead()
        {
            this.alive = false;
        }
    }

    /// <summary>
    /// Any GameObject that is saved in a sector should be a SpaceObject.
    /// </summary>
    public abstract class SpaceObject: GameObject
    {
        // Anything that should be saved in a sector file is a space object
        bool space_object = true;
        protected Object collide_object;
        private Texture2D texture;
        private Color[] texture_bytes;

        public SpaceObject(Texture2D texture)
        {
            texture_bytes = new Color[texture.Width * texture.Height];
            texture.GetData(texture_bytes);
        }

        public SpaceObject()
        {

        }

        /// <summary>
        /// Adds self to the sector that created it upon initialization.
        /// </summary>
        /// <param name="sector_inside">The sector that this object is contained in.</param>
        public void addToSector(Sector sector_inside)
        {
            if (sector_inside != null)
            {
                sector_inside.space_objects.Add(this);
            }
        }

        public override void setDead()
        {
            this.alive = false;
        }

        /// <summary>
        /// Deletes SpaceObject from sector.
        /// </summary>
        public void removeFromSector()
        {
            getContainingSector().space_objects.Remove(this);
        }

        /// <summary>
        /// Returns the sector that the SpaceObject is contained in.
        /// </summary>
        /// <returns></returns>
        public Sector getContainingSector()
        {
            int[] sector_coords = Space.getSector(pos[0], pos[1]);
            return Program.game.space.findLoadedSector(sector_coords[0], sector_coords[1]);
        }

        /// <summary>
        /// Must be implemented to be able to collide with other SpaceObjects.
        /// </summary>
        /// <returns>Any collision object that has fully implemented collision methods.</returns>
        public bool collidesWith(SpaceObject obj)
        {
            // Calculate the intersecting rectangle
            int x1 = Math.Max(this.pos[0], obj.pos[0]);
            int x2 = Math.Min(this.pos[0] + this.texture.Bounds.Width, obj.pos[0] + obj.texture.Bounds.Width);

            int y1 = Math.Max(this.pos[0], obj.pos[1]);
            int y2 = Math.Min(this.pos[1] + this.texture.Bounds.Height, obj.pos[1] + obj.texture.Bounds.Height);

            for (int y = y1; y < y2; ++y)
            {
                for (int x = x1; x < x2; ++x)
                {
                    // Get the color from each texture
                    Color a = this.texture_bytes[(x - this.pos[0]) + (y - this.pos[1]) * this.texture.Width];
                    Color b = obj.texture_bytes[(x - obj.pos[0]) + (y - obj.pos[1]) * obj.texture.Width];

                    if (a.A != 0 && b.A != 0) // If both colors are not transparent (the alpha channel is not 0), then there is a collision
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public abstract Object getCollideShape();
    }
}