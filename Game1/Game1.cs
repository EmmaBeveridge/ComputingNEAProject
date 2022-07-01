using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;
using Game1.Rooms;
using System.Threading;

namespace Game1


{

    public class Game1 : Game
    {
        GraphicsDeviceManager graphicsManager;
        
        List<Avatar> avatars = new List<Avatar>();
        List<People> people = new List<People>();
        List<House> houses = new List<House>();
        List<Texture2D> loadingScreens = new List<Texture2D>();

        SpriteBatch spriteBatch;




        Camera camera;

        Player player;



        GameStates.States gameState = GameStates.States.Loading;
        bool isLoading=false;
        

        public Game1()
        {
            graphicsManager = new GraphicsDeviceManager(this);
            
            Content.RootDirectory = "Content";
           
        }


       

        protected  override void Initialize()
        {
            
             
            
            base.Initialize();
            //graphicsManager.PreferredBackBufferWidth = GraphicsDevice.DisplayMode.Width;
            //graphicsManager.PreferredBackBufferHeight = GraphicsDevice.DisplayMode.Height;
            //graphicsManager.IsFullScreen = true;
            graphicsManager.ApplyChanges();
            camera = new Camera(GraphicsDevice, this.Window, new Vector3(0,60,-200), Vector3.Zero);
            this.IsMouseVisible = true;


        }


        public  async Task BuildHouseAsync()
        {

            

            List<Room> rooms = await GetRooms();
            
            

            using (var cloudDBHandler = new CloudDBHandler())
            {
                foreach (Room room in rooms)
                {
                    room.items = await cloudDBHandler.GetItemsInRoom(room);
                }

            }
            
            House house = new House(rooms);
            
            houses.Add(house);
            

        }









        public static async Task<List<Room>> GetRooms()
        {

            using (var cloudDBHandler = new CloudDBHandler())
            {
                List<Room> rooms = await cloudDBHandler.GetRoomsInHouse();


                return rooms;

            }

            

        }

        public static async Task CreateTownDB()
        {            
            using (var cloudDBHandler = new CloudDBHandler())
            {
                await cloudDBHandler.CreateTownAsync();
               
            }
        }




        protected  override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            loadingScreens.Add(Content.Load<Texture2D>("LoadingScreen-1"));
            loadingScreens.Add(Content.Load<Texture2D>("LoadingScreen0"));
            loadingScreens.Add(Content.Load<Texture2D>("LoadingScreen1"));
            loadingScreens.Add(Content.Load<Texture2D>("LoadingScreen2"));
            loadingScreens.Add(Content.Load<Texture2D>("LoadingScreen3"));
            loadingScreens.Add(Content.Load<Texture2D>("LoadingScreen4"));
            loadingScreens.Add(Content.Load<Texture2D>("LoadingScreen5"));
            loadingScreens.Add(Content.Load<Texture2D>("LoadingScreen6"));
            loadingScreens.Add(Content.Load<Texture2D>("LoadingScreen7"));
            loadingScreens.Add(Content.Load<Texture2D>("LoadingScreen8"));
           

        }

        protected override void UnloadContent()
        {
        }



        async Task LoadGame()
        {


            //Thread createTownThread = new Thread(CreateTownDB);
            //createTownThread.Start();

            //Thread createHouseThread = new Thread(BuildHouseAsync);
            //createHouseThread.Start();
            //Thread.Sleep(5000);


            //await Task.Run(() => CreateTownDB());
            //await Task.Run(() => BuildHouseAsync());


            await CreateTownDB().ConfigureAwait(false);
            await BuildHouseAsync().ConfigureAwait(false);

            

            Model woman2 = Content.Load<Model>("woman4");
            Model man2 = Content.Load<Model>("man4");
            Model woman5 = Content.Load<Model>("woman5");
            player = new Player(woman2, new Vector3(0,5,0));
            people.Add(player);
            people.Add(new People(man2, new Vector3(10, 5, 10)));
            people.Add(new People(woman5, new Vector3(-20, 5, -20)));


            avatars.AddRange(people.Select(people => people.avatar));


            //avatars.Add(new Avatar(Content.Load<Model>("houseBake6"), new Vector3(0, 41, 0)));
            
            
            avatars.Add(new Avatar(Content.Load<Model>("grass"), new Vector3(0, -12, 0))); // was at y=-12
            avatars.Add(new Avatar(Content.Load<Model>("HouseInteriorWalls"), new Vector3(0, -2, 0)));
            avatars.Add(new Avatar(Content.Load<Model>("HouseExteriorWalls"), new Vector3(0, -2, 0)));
            //avatars.Add(new Avatar(Content.Load<Model>("Countertop"), new Vector3(60, 0, 100)));
            //avatars.Add(new Avatar(Content.Load<Model>("CountertopSink"), new Vector3(105, 0, 100)));
            //avatars.Add(new Avatar(Content.Load<Model>("Shower"), new Vector3(330, 0, -105)));
            //avatars.Add(new Avatar(Content.Load<Model>("Toilet"), new Vector3(285, 0, -55)));
            //avatars.Add(new Avatar(Content.Load<Model>("Bookcase"), new Vector3(70, 0, 5)));
            //avatars.Add(new Avatar(Content.Load<Model>("Fridge"), new Vector3(155, 0, 100)));
            //avatars.Add(new Avatar(Content.Load<Model>("Sink"), new Vector3(230, 0, -115)));
            //avatars.Add(new Avatar(Content.Load<Model>("Dresser"), new Vector3(215, 0, 70)));
            //avatars.Add(new Avatar(Content.Load<Model>("Chair"), new Vector3(45, 0, 55)));
            //avatars.Add(new Avatar(Content.Load<Model>("Chair"), new Vector3(85, 0, 55)));
            //avatars.Add(new Avatar(Content.Load<Model>("Table"), new Vector3(50, 0, 55)));
            //avatars.Add(new Avatar(Content.Load<Model>("EndTable"), new Vector3(330, 0, 40)));
            //avatars.Add(new Avatar(Content.Load<Model>("Bin"), new Vector3(180, 0, 75)));
            //avatars.Add(new Avatar(Content.Load<Model>("Sofa"), new Vector3(90, 0, -105)));
            //avatars.Add(new Avatar(Content.Load<Model>("Oven"), new Vector3(25, 0, 100)));
            //avatars.Add(new Avatar(Content.Load<Model>("TV"), new Vector3(105, 0, -50)));
          





            foreach (House house in houses)
            {
                foreach (Room room in house.rooms)
                {
                    foreach (Item item in room.items)
                    {
                        avatars.Add(new Avatar(Content.Load<Model>(item.modelName), new Vector3(float.Parse(item.locationX), -2, float.Parse(item.locationZ))));
                    }
                }
            }


            gameState = GameStates.States.Playing;
            

            //avatars.Add(new Avatar( Content.Load<Model>("Oven"), new Vector3 (204,-5,13)));
            //avatars.Add(new Avatar( Content.Load<Model>("Fridge"), new Vector3 (204,-5, -184)));
            //avatars.Add(new Avatar( Content.Load<Model>("Sofa"), new Vector3 (-11,-5,-88)));
            //avatars.Add(new Avatar( Content.Load<Model>("Bed"), new Vector3 (187,-5,-347)));
            //avatars.Add(new Avatar( Content.Load<Model>("toiletWhite"), new Vector3 (39,-5, -303)));
            //avatars.Add(new Avatar( Content.Load<Model>("TV"), new Vector3 (79,-5, -88)));
            ////avatars.Add(new Avatar( Content.Load<Model>("shower2"), new Vector3 (70,0,50)));




        }



        protected override async void Update(GameTime gameTime)
        {

            
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();

            }

            
            if (gameState==GameStates.States.Loading && !isLoading)
            {
                //backgroundThread = new Thread(LoadGame);
                isLoading = true;
                //backgroundThread.Start();

                await Task.Run(async () => { await LoadGame(); } );

                //await LoadGame();


            }


            else if (gameState==GameStates.States.Playing)
            {
                foreach (People person in people)
                {
                    person.Update(gameTime, GraphicsDevice, camera.projection, camera.view, this);
                }

            }

            if (gameState == GameStates.States.Playing && isLoading)
            {

                
                isLoading = false;
            }


            
            camera.Update(gameTime);

            
            
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {

           
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;


            if (gameState== GameStates.States.Playing)
            {
                GraphicsDevice.Clear(Color.SkyBlue);
                foreach (Avatar avatar in avatars)
                {
                    avatar.Draw(camera.view, camera.projection);  
                }

            }
           
           
           else if (gameState == GameStates.States.Loading)
           {
                
                spriteBatch.Begin();
                spriteBatch.Draw(loadingScreens[0], new Rectangle(0, 0, graphicsManager.PreferredBackBufferWidth, graphicsManager.PreferredBackBufferHeight), Color.White);
                loadingScreens.Add(loadingScreens[0]);
                loadingScreens.RemoveAt(0);
                spriteBatch.End();
                Thread.Sleep(65);


           }

            


            base.Draw(gameTime);
        }
    }
}