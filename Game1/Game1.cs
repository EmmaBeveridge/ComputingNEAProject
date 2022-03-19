using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;
namespace Game1


{

    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
    
        List<Models> models = new List<Models>();

        Camera camera;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
           
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            base.Initialize();
            this.IsMouseVisible = true;

            graphics.PreferredBackBufferWidth = GraphicsDevice.DisplayMode.Width;
            graphics.PreferredBackBufferHeight = GraphicsDevice.DisplayMode.Height;
            //graphics.IsFullScreen = true;
            graphics.ApplyChanges();

            camera = new Camera(GraphicsDevice, this.Window, new Vector3(0,60,-200), Vector3.Right);
            

           

        }

        protected override void LoadContent()
        {
            models.Add(new Models( Content.Load<Model>("woman2"), new Vector3 (0,10,0)));
            models.Add(new Models( Content.Load<Model>("house"), new Vector3 (0,0,0)));
           

        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {

            
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();

            }

            Quaternion rotationQuaternion;
            camera.world.Decompose(out _, out rotationQuaternion, out _);


            camera.Update(gameTime);


            
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {

            GraphicsDevice.Clear(Color.SkyBlue);
            
            foreach (Models model in models)
            {
                model.Draw(camera.view, camera.projection);  
            }
           
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            


            base.Draw(gameTime);
        }
    }
}