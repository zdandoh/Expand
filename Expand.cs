﻿#region Using Statements
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
        public Stopwatch game_time = new Stopwatch();
        public GraphicsDeviceManager graphics;
        public SpriteBatch spriteBatch;

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
            this.IsMouseVisible = true;
            game_time.Start();
            object_handler = new ObjectHandler();
            ship = new Ship();
            space = new Space();
            gui = new GUI();
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
            if ((x - Program.game.ship.pos[0]) * (x - Program.game.ship.pos[0]) + (y - Program.game.ship.pos[1])*(y - Program.game.ship.pos[1]) < 250000)
            {
                Vector2 pos_vector = new Vector2(x - Program.game.ship.pos[0] + Program.game.ship.draw_location[0], y - Program.game.ship.pos[1] + Program.game.ship.draw_location[1]);
                Vector2 origin = new Vector2(0, 0);
                this.spriteBatch.Draw(texture, pos_vector, null, Color.White, 0F, origin, 1, SpriteEffects.None, 0);
            }
        }
    }
}
