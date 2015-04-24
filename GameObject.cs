using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Expand
{
    public class GameObject
    {
        protected bool alive = true;
        public int[] pos = { 0, 0 };
        public GameObject()
        {
            Program.game.object_handler.addObject(this);
        }

        public virtual bool preUpdate()
        {
            update();
            return this.alive;
        }

        public virtual void update()
        {

        }

        public virtual void draw()
        {

        }

        public virtual void setDead()
        {
            this.alive = false;
        }
    }

    public abstract class SpaceObject: GameObject
    {
        // Anything that should be saved in a sector file is a space object
        bool space_object = true;
        protected Object collide_object;

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

        public void removeFromSector()
        {
            getContainingSector().space_objects.Remove(this);
        }

        public Sector getContainingSector()
        {
            int[] sector_coords = Space.getSector(pos[0], pos[1]);
            return Program.game.space.findSector(sector_coords[0], sector_coords[1]);
        }

        public abstract Object getCollideShape();
    }
}