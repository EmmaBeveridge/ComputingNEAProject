using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Game1


{

    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
    
        List<Avatar> avatars = new List<Avatar>();
        List<People> people = new List<People>();
        


        Camera camera;

        Player player;


       

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

            camera = new Camera(GraphicsDevice, this.Window, new Vector3(0,60,-200), Vector3.Zero);

           


        }

        protected override void LoadContent()
        {

            
            Model woman2= Content.Load<Model>("woman4");
            Model man2 = Content.Load<Model>("man4");
            Model woman5 = Content.Load<Model>("woman5");
            player = new Player(woman2, Vector3.Zero);
            people.Add(player);
            people.Add(new People(man2, new Vector3(10, 0, 10)));
            people.Add(new People(woman5, new Vector3(-20, 0, -20)));
           

            avatars.AddRange(people.Select(people => people.avatar));

            avatars.Add(new Avatar( Content.Load<Model>("houseBake6"), new Vector3 (0,41,0)));
            avatars.Add(new Avatar( Content.Load<Model>("Oven"), new Vector3 (80,-5,-40)));
            avatars.Add(new Avatar( Content.Load<Model>("Fridge"), new Vector3 (80,-5,-70)));
            avatars.Add(new Avatar( Content.Load<Model>("Sofa"), new Vector3 (-75,-5,-50)));
            avatars.Add(new Avatar( Content.Load<Model>("Bed"), new Vector3 (-60,-5,50)));
            avatars.Add(new Avatar( Content.Load<Model>("toiletWhite"), new Vector3 (87,-5,10)));
            avatars.Add(new Avatar( Content.Load<Model>("shower3"), new Vector3 (70,0,50)));
            
            avatars.Add( new Avatar( Content.Load<Model>("grass"), new Vector3 (0,-12,0)));
           

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


            foreach (People person in people)
            {
                person.Update(gameTime, GraphicsDevice, camera.projection, camera.view);
            }
            


            camera.Update(gameTime);

            
            
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {

            GraphicsDevice.Clear(Color.SkyBlue);
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;


            foreach (Avatar avatar in avatars)
            {
                avatar.Draw(camera.view, camera.projection);  
            }
           
           

            


            base.Draw(gameTime);
        }
    }
}