using Expand.core.space;
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
}