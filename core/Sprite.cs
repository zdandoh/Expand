using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Expand.core
{
    public class Sprite: GameObject
    {
        private List<Texture2D> textures;
        private List<Color[]> meshes;
        private int texture_index = 0;

        public Sprite(Texture2D first_texture)
        {
            textures = new List<Texture2D>();
            meshes = new List<Color[]>();
            this.addTexture(first_texture);
        }

        public void addTexture(Texture2D new_texture)
        {
            Color[] new_mesh = new Color[new_texture.Width * new_texture.Height];
            new_texture.GetData(new_mesh);

            textures.Add(new_texture);
            meshes.Add(new_mesh);
        }

        public Texture2D getFrame()
        {
            return textures[texture_index];
        }

        public virtual void animate()
        {

        }

        public override void update()
        {
            this.animate();
        }
    }
}
