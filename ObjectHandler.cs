using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace Expand
{
    public class ObjectHandler
    {
        List<GameObject> game_objects = new List<GameObject>();
        List<GameObject> middle_list = new List<GameObject>();
        List<Texture2D> texture_queue = new List<Texture2D>();
        public bool middle_list_locked = false;

        public void addObject(GameObject new_object)
        {
            middle_list.Add(new_object);
        }

        public void updateObjects()
        {
            // Move from middle list to real list
            if (!middle_list_locked)
            {
                for(int middle_counter = 0; middle_counter < middle_list.Count(); middle_counter++)
                {
                    game_objects.Add(middle_list[middle_counter]);
                    middle_list[middle_counter] = null;
                }
                middle_list.RemoveAll(game_obj => game_obj == null);
            }

            for(int obj_counter = 0; obj_counter < game_objects.Count(); obj_counter++)
            {
                bool still_alive = game_objects[obj_counter].preUpdate();
                if (!still_alive)
                {
                    game_objects[obj_counter] = null;
                }
            }
            game_objects.RemoveAll(game_obj => game_obj == null);
        }

        public void drawObjects()
        {
            int count = 0;
            Program.game.spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);
            foreach (GameObject obj in game_objects)
            {
                obj.draw();
                count++;
            }
            Console.WriteLine(count);
            Program.game.spriteBatch.End();
        }
    }
}
