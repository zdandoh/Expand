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

        public virtual String saveString()
        {
            // Returns a string of all the data that will be saved in the sector file
            return "";
        }
    }
}
