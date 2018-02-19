using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Threading;

namespace Twitch_Reigns
{

    public class Game1 : Game
    {
        

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        IrcClient irc;
        GameHandler gameHandler;

        SpriteFont font;
        int monitorWidth;
        int monitorHeight;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            monitorWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            monitorHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            graphics.PreferredBackBufferWidth = monitorWidth;
            graphics.PreferredBackBufferHeight = monitorHeight;
            Window.Position = new Microsoft.Xna.Framework.Point(0, 0);
            Window.IsBorderless = true;
            IsMouseVisible = true;
        }
        protected override void Initialize()
        {
            //"oauth:r7kgm860nltwq1djqcuf9cxjdh0hy9"
            //"oauth:vu3d7hk8wao4ojlnkwlmqrsv2hlfxr"
            
            // TODO: Add your initialization logic here
            irc = new IrcClient("52.34.236.73", 6667, "ThotRobot", "oauth:chxqjcpd321nbqkb3q5wpcmttc4i2o");
            font = Content.Load<SpriteFont>("Font");

            gameHandler = new GameHandler();
      
         

            GameHandler.Bot = irc;
            GameHandler.Font = font;
            

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            GameHandler.SpriteBatch = spriteBatch;
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
            // TODO: Add your update logic here

            gameHandler.Update();
           /* Thread thread = new Thread(gameHandler.Update);
            thread.Start();*/
            base.Update(gameTime);
        }
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin();
            gameHandler.Draw();

            /*if (!IsActive)
            {
                spriteBatch.DrawString(font, "OUT", new Vector2(100, 100), Microsoft.Xna.Framework.Color.White);
            }*/
            
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
