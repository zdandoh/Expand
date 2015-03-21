using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Expand
{
    public class ObjectHandler
    {
        List<GameObject> game_objects = new List<GameObject>();
        List<Texture2D> texture_queue = new List<Texture2D>();


        public void addObject(GameObject new_object)
        {
            game_objects.Add(new_object);
        }

        public void updateObjects()
        {
            for(int obj_counter = 0; obj_counter < game_objects.Count(); obj_counter++)
            {
                bool still_alive = game_objects[obj_counter].preUpdate();
                if (!still_alive)
                {
                    game_objects.RemoveAt(obj_counter);
                }
            }
        }

        public void drawObjects()
        {
            Program.game.spriteBatch.Begin();
            foreach (GameObject obj in game_objects)
            {
                obj.draw();
            }
            Program.game.spriteBatch.End();
        }
    }
}
