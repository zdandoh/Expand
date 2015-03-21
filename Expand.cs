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
        public Stopwatch game_time = new Stopwatch();
        public GraphicsDeviceManager graphics;
        public SpriteBatch spriteBatch;

        public Expand()
            : base()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            this.IsMouseVisible = true;
            game_time.Start();
            object_handler = new ObjectHandler();
            space = new Space();
            ship = new Ship();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
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

        public void drawSprite(Texture2D texture, int x, int y)
        {
            // Check if sprite is in player view
            if (Math.Pow((x - Program.game.ship.pos[0] - Program.game.ship.draw_location[0]), 2) + Math.Pow((y - Program.game.ship.pos[1] - Program.game.ship.draw_location[1]), 2) < 250000)
            {
                Vector2 pos_vector = new Vector2(x - Program.game.ship.pos[0], y - Program.game.ship.pos[1]);
                this.spriteBatch.Draw(texture, pos_vector);
            }
        }
    }
}
