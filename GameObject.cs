using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Expand
{
    public class GameObject
    {
        protected bool alive = true;
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
    }
}
