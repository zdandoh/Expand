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

        public void setDead()
        {
            this.alive = false;
        }
    }

    public class SpaceObject: GameObject
    {
        // Anything that should be saved in a sector file is a space object
        bool space_object = true;

        public void addToSector(Sector sector_inside)
        {
            if (sector_inside != null)
            {
                sector_inside.space_objects.Add(this);
            }
        }

        public virtual void onCollide(GameObject collider)
        {

        }
    }
}