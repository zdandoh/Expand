#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using System.IO;
using Expand.core.gui;
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
        public Toolbar toolbar;
        public TechTree tech_tree;
        public int[] screen_size = { 800, 600 };
        public Texture2D line_texture;
        public Stopwatch game_time = new Stopwatch();
        public GraphicsDeviceManager graphics;
        public Dictionary<String, Texture2D> textures;
        public SpriteBatch spriteBatch;
        public SpriteFont default_font;
        public SpriteFont default_font16;
        public MouseState mouse = Mouse.GetState();
        public Vector2 space_mouse = new Vector2(0, 0);
        public Effect test_shader;
        public FPSCounter fps_counter;

        public Expand()
            : base()
        {
            base.IsFixedTimeStep = false;
            graphics = new GraphicsDeviceManager(this);
            // graphics.IsFullScreen = true;
            graphics.PreferredBackBufferHeight = screen_size[1];
            graphics.PreferredBackBufferWidth = screen_size[0];

            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Initializes base game classes.
        /// </summary>
        protected override void Initialize()
        {
            this.textures = loadAllContent();
            this.IsMouseVisible = true;
            game_time.Start();
            object_handler = new ObjectHandler();
            ship = Ship.load();
            space = new Space();
            tech_tree = new TechTree();
            toolbar = new Toolbar();
            fps_counter = new FPSCounter();
            base.Initialize();
        }

        /// <summary>
        /// Loads fonts and some textures. Initializes graphics device.
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            line_texture = new Texture2D(GraphicsDevice, 1, 1);
            line_texture.SetData<Color>(new Color[] { Color.White });
            default_font = Content.Load<SpriteFont>("font//space_font12");
            default_font16 = Content.Load<SpriteFont>("font//space_font16");
        }

        /// <summary>
        /// Loads all game textures into a Dictionary with keys of path.
        /// </summary>
        /// <returns>Dictionary that contains all Texture2D's mapped to keys of their file paths.</returns>
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

        /// <summary>
        /// Unused unloading function.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Main game tick. Called 60 times per second.
        /// </summary>
        /// <param name="gameTime">Time passed since last update.</param>
        protected override void Update(GameTime gameTime)
        {
            this.mouse = Mouse.GetState();
            int[] mouse_pos = Util.screenPosToSpacePos(mouse.X, mouse.Y);
            this.space_mouse = new Vector2(mouse_pos[0], mouse_pos[1]);
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            object_handler.updateObjects();
            base.Update(gameTime);
        }

        /// <summary>
        /// Main game draw function. Called 60 times per second.
        /// </summary>
        /// <param name="gameTime"></param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(space.space_color);
            object_handler.drawObjects();

            base.Draw(gameTime);
        }

        /// <summary>
        /// Loads a Texture2D from file.
        /// </summary>
        /// <param name="file_path">Path of texture file.</param>
        /// <returns>Texture2D at file path.</returns>
        public Texture2D loadTexture(String file_path)
        {
            return this.Content.Load<Texture2D>(file_path);
        }

        /// <summary>
        /// Mostly broken drawline function. Doesn't actually obey arguments. Only used for drawing mining laser.
        /// </summary>
        /// <param name="x1">Line start X</param>
        /// <param name="y1">Line start Y</param>
        /// <param name="x2">Line end X</param>
        /// <param name="y2">Line end Y</param>
        /// <param name="thickness">Tickness of line in pixels.</param>
        /// <param name="color">Color of line.</param>
        public void drawLine(int x1, int y1, int x2, int y2, int thickness = 1, Color? color = null)
        {
            Color draw_color = color ?? Color.White;
            Vector2 start = new Vector2(x1, y1);
            Vector2 end = new Vector2(x2, y2);
            Vector2 along_line = end - start;
            float line_angle = (float) Math.Atan2(along_line.Y, along_line.X);
            Rectangle line_rect = new Rectangle((int)start.X, (int)start.Y, (int)along_line.Length(), thickness);
            Vector2 origin = new Vector2(0, 0);
            this.spriteBatch.Draw(this.line_texture, line_rect, null, draw_color, line_angle, origin, SpriteEffects.None, 0);
        }

        /// <summary>
        /// Takes space coordinates and converts them to coordinates that can be drawn on screen.
        /// </summary>
        /// <param name="x">Space coordinate X.</param>
        /// <param name="y">Space coordinate Y.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Takes screen coordinates and converts them to space coordinates.
        /// </summary>
        /// <param name="x">Screen coordinate X.</param>
        /// <param name="y">Screen coordinate Y.</param>
        /// <returns>Vector2D of space coordinates.</returns>
        public Vector2 spacePos(int x, int y)
        {
            return new Vector2(x + Program.game.ship.pos[0] - Program.game.ship.draw_location[0], y + Program.game.ship.pos[1] - Program.game.ship.draw_location[1]);
        }

        /// <summary>
        /// Draws a small red square at space coordinates x, y.
        /// </summary>
        /// <param name="x">Space coordinate X.</param>
        /// <param name="y">Space coordinate Y.</param>
        public void drawDebugSquare(int x, int y)
        {
            Program.game.drawSprite(textures["gui\\icon\\debug_square.png"], x, y, layer: 0.99F);
        }

        /// <summary>
        /// Checks if space coordinates are in the view of the player.
        /// </summary>
        /// <param name="x">Space coordinate X.</param>
        /// <param name="y">Space coordinate Y.</param>
        /// <returns>Boolean whether coordinates are in view.</returns>
        public bool inView(int x, int y)
        {
            // Check if sprite is in player view
            return (x - Program.game.ship.pos[0]) * (x - Program.game.ship.pos[0]) + (y - Program.game.ship.pos[1])*(y - Program.game.ship.pos[1]) < 2500000;
        }

        /// <summary>
        /// Performs cleanup actions upon game exit. Saves current Sector and ship data.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected override void OnExiting(Object sender, EventArgs args)
        {
            base.OnExiting(sender, args);
            this.ship.save();
            int[] sector_location = this.space.getPlayerSector();
            this.space.findSector(sector_location[0], sector_location[1]).saveAsync();
        }

        /// <summary>
        /// Draws a sprite, offset to screen coordinates.
        /// </summary>
        /// <param name="texture">Texture2D to be drawn.</param>
        /// <param name="x">Space coordinate X.</param>
        /// <param name="y">Space coordinate Y.</param>
        /// <param name="scale">Scale of sprite to be drawn. Default is 1x.</param>
        /// <param name="layer">Depth layer of sprite to be drawn. From 0-1. Default is 1.</param>
        /// <param name="color">Color modifier of Texture2D.</param>
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

        /// <summary>
        /// Draws supplied text to position.
        /// </summary>
        /// <param name="text">String text to draw.</param>
        /// <param name="pos">Position of top left corner of text.</param>
        /// <param name="text_color">Color of text. Default is white.</param>
        /// <param name="layer">Depth layer of text. Default is 1.</param>
        /// <param name="scale">Scale level of text. Default is 1x.</param>
        public void drawText(String text, int[] pos, Color text_color, float layer = 1F, float scale = 1F)
        {
            Vector2 pos_vector = new Vector2(pos[0], pos[1]);
            Vector2 origin = new Vector2(0, 0);
            this.spriteBatch.DrawString(this.default_font, text, pos_vector, text_color, 0F, origin, scale, SpriteEffects.None, layer);
        }
    }
}
