#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
using System.Diagnostics;
using System.Threading;
using System.IO;
#endregion

namespace Expand
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Expand : Game
    {
        public Random rand_gen = new Random();
        public ObjectHandler object_handler;
        public Space space;
        public Ship ship;
        public GUI gui;
        public int[] screen_size = { 800, 600 };
        public Texture2D line_texture;
        public Stopwatch game_time = new Stopwatch();
        public GraphicsDeviceManager graphics;
        public Dictionary<String, Texture2D> textures;
        public SpriteBatch spriteBatch;
        public SpriteFont default_font;
        public SpriteFont default_font16;
        public MouseState mouse = Mouse.GetState();
        public Effect test_shader;

        public Expand()
            : base()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.IsFullScreen = true;
            graphics.PreferredBackBufferHeight = screen_size[1];
            graphics.PreferredBackBufferWidth = screen_size[0];
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            this.textures = loadAllContent();
            this.IsMouseVisible = true;
            game_time.Start();
            object_handler = new ObjectHandler();
            ship = Ship.load();
            space = new Space();
            gui = new GUI();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            line_texture = new Texture2D(GraphicsDevice, 1, 1);
            line_texture.SetData<Color>(new Color[] { Color.White });
            default_font = Content.Load<SpriteFont>("font//space_font12");
            default_font16 = Content.Load<SpriteFont>("font//space_font16");
        }

        public Dictionary<String, Texture2D> loadAllContent()
        {
            Dictionary<String, Texture2D> texture_dict = new Dictionary<String, Texture2D>();
            IEnumerable<String> files = Directory.EnumerateFiles("Content", "*.*", SearchOption.AllDirectories);
            foreach(String file in files)
            {
                if(file.EndsWith(".png"))
                {
                    texture_dict.Add(file.Substring(8), this.loadTexture(file.Substring(8)));
                }
            }
            return texture_dict;
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            this.mouse = Mouse.GetState();
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            object_handler.updateObjects();
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(space.space_color);
            object_handler.drawObjects();

            base.Draw(gameTime);
        }

        public Texture2D loadTexture(String file_path)
        {
            return this.Content.Load<Texture2D>(file_path);
        }

        public void drawLine(int x1, int y1, int x2, int y2, int thickness = 1)
        {
            Vector2 start = new Vector2(x1, y1);
            Vector2 end = new Vector2(x2, y2);
            Vector2 along_line = end - start;
            float line_angle = (float) Math.Atan2(along_line.Y, along_line.X);
            Rectangle line_rect = new Rectangle((int)start.X, (int)start.Y, (int)along_line.Length(), thickness);
            Vector2 origin = new Vector2(0, 0);
            this.spriteBatch.Draw(this.line_texture, line_rect, null, Color.Blue, line_angle, origin, SpriteEffects.None, 0);
        }

        public int[] drawOffset(int x, int y)
        {
            // Offsets a set of coordinates as they would be if they were drawn
            int new_x = x - Program.game.ship.pos[0] + Program.game.ship.draw_location[0];
            int new_y = y - Program.game.ship.pos[1] + Program.game.ship.draw_location[1];
            return new int[] {new_x, new_y};
        }

        public Vector2 drawOffset(Vector2 initial_draw)
        {
            return new Vector2(initial_draw.X - Program.game.ship.pos[0] + Program.game.ship.draw_location[0], initial_draw.Y - Program.game.ship.pos[1] + Program.game.ship.draw_location[1]);
        }

        public bool inView(int x, int y)
        {
            // Check if sprite is in player view
            return (x - Program.game.ship.pos[0]) * (x - Program.game.ship.pos[0]) + (y - Program.game.ship.pos[1])*(y - Program.game.ship.pos[1]) < 250000;
        }

        protected override void OnExiting(Object sender, EventArgs args)
        {
            base.OnExiting(sender, args);
            this.ship.save();
            int[] sector_location = this.space.getPlayerSector();
            this.space.findSector(sector_location[0], sector_location[1]).saveAsync();
        }

        public void drawSprite(Texture2D texture, int x, int y, float scale = 1, float layer = 1, Color? color = null)
        {
            if (this.inView(x, y))
            {
                Color draw_color = color ?? Color.White;
                int[] draw_pos = this.drawOffset(x, y);
                Vector2 pos_vector = new Vector2(draw_pos[0], draw_pos[1]);
                Vector2 origin = new Vector2(0, 0);
                this.spriteBatch.Draw(texture, pos_vector, null, draw_color, 0F, origin, scale, SpriteEffects.None, layer);
            }
        }

        public void drawText(String text, int[] pos, Color text_color, float layer = 1F, float scale = 1F)
        {
            Vector2 pos_vector = new Vector2(pos[0], pos[1]);
            Vector2 origin = new Vector2(0, 0);
            this.spriteBatch.DrawString(this.default_font, text, pos_vector, text_color, 0F, origin, scale, SpriteEffects.None, layer);
        }
    }
}
