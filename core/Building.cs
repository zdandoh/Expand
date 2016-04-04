using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Expand.core
{
    public abstract class Building: SpaceObject
    {
        public abstract override Object getCollideShape();

        public static int BUILD_COST;

        public virtual void create()
        {

        }

        public virtual void onClick()
        {

        }

        public virtual void onContact(GameObject obj)
        {
            
        }

        public override void update()
        {

        }

        public override void draw()
        {
            base.draw();
        }
    }
}
