using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;
using Game1.Rooms;
using System.Threading;
using Game1.NavMesh;
using Game1.Town;
using Game1.Town.Districts;
using Game1.Characters;
 

namespace Game1


{

    public class Game1 : Game
    {
        GraphicsDeviceManager graphicsManager;
        
        List<Avatar> avatars = new List<Avatar>();
        public List<Button> buttons = new List<Button>();
        List<ToolbarButton> toolbarButtons = new List<ToolbarButton>();

        List<People> people = new List<People>();
        
        List<Town.Town> towns = new List<Town.Town>();
        
        List<Texture2D> loadingScreens = new List<Texture2D>();

        


        SpriteBatch spriteBatch;
        SpriteFont spriteFont;
        


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


        
        








        //public static async Task<List<Room>> GetRooms()
        //{

        //    using (var cloudDBHandler = new CloudDBHandler())
        //    {
        //        List<Room> rooms = await cloudDBHandler.GetRoomsInHouse();


        //        return rooms;

        //    }

            

        //}

        public static async Task CreateTownDB()
        {            
            using (var cloudDBHandler = new CloudDBHandler())
            {
                await cloudDBHandler.CreateTownInDBAsync();
               
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

            Button.defaultTexture = Content.Load<Texture2D>("BlueButton");
            spriteFont = Content.Load<SpriteFont>("buttonFont");


            Vector2 toolbarButtonTextureDim = new Vector2(50);

            Texture2D needsButtonTexture = Content.Load<Texture2D>("NeedsButton4");
            toolbarButtons.Add(new ToolbarButton(null, new Vector2((graphicsManager.GraphicsDevice.Viewport.Width - 4*toolbarButtonTextureDim.X),(graphicsManager.GraphicsDevice.Viewport.Height - toolbarButtonTextureDim.Y)), needsButtonTexture));
            
            
            Texture2D relationshipButtonTexture = Content.Load<Texture2D>("RelationshipButton2");
            toolbarButtons.Add(new ToolbarButton(null, new Vector2((graphicsManager.GraphicsDevice.Viewport.Width - 3 *toolbarButtonTextureDim.X), (graphicsManager.GraphicsDevice.Viewport.Height - toolbarButtonTextureDim.Y)), relationshipButtonTexture));
            
            Texture2D skillsButtonTexture = Content.Load<Texture2D>("SkillsButton2");
            toolbarButtons.Add(new ToolbarButton(null, new Vector2((graphicsManager.GraphicsDevice.Viewport.Width - 2 * toolbarButtonTextureDim.X), (graphicsManager.GraphicsDevice.Viewport.Height - toolbarButtonTextureDim.Y)), skillsButtonTexture));
            
            Texture2D careerButtonTexture = Content.Load<Texture2D>("CareerButton2");
            toolbarButtons.Add(new ToolbarButton(null, new Vector2((graphicsManager.GraphicsDevice.Viewport.Width - toolbarButtonTextureDim.X), (graphicsManager.GraphicsDevice.Viewport.Height - toolbarButtonTextureDim.Y)), careerButtonTexture));



        }

        protected override void UnloadContent()
        {
        }

        

        public async Task BuildTownAsync()
        {
            CloudDBHandler cloudDBHandler = new CloudDBHandler();


            towns = await cloudDBHandler.GetTownsInDB();

            foreach (Town.Town town in towns)
            {
                town.districts = await cloudDBHandler.GetDistrictsInTown(town);
                foreach (District district in town.districts)
                {

                    district.mapModel = Content.Load<Model>(district.mapName);
                    district.GenerateAvatar();

                    avatars.Add(district.avatar);

                    district.streets = await cloudDBHandler.GetStreetsInDistrict(district);
                    district.streets = await cloudDBHandler.SetStreetPointers(district.streets);
                    


                    foreach (Street street in district.streets)
                    {
                        street.houses = await cloudDBHandler.GetHousesOnStreet(street);
                        street.buildings = await cloudDBHandler.GetBuildingsOnStreet(street);

                        foreach (House house in street.houses)
                        {
                            house.setWallsFromColourScheme();
                            house.exteriorWallsModel = Content.Load<Model>(house.exteriorWallsName);
                            house.interiorWallsModel = Content.Load<Model>(house.interiorWallsName);
                            house.roofModel = Content.Load<Model>(house.roofName);
                            house.GenerateAvatar();
                           
                            avatars.AddRange(house.wallAvatars);
                            House.houses.Add(house);
                            house.rooms = await cloudDBHandler.GetRoomsInHouse(house);
                            town.houses.Add(house);

                            foreach (Room room in house.rooms)
                            {
                                room.house = house;
                                room.items = await cloudDBHandler.GetItemsInRoom(room);

                                foreach (Item item in room.items)
                                {
                                    item.rotation = house.rotation;
                                    item.room = room;
                                    item.model = Content.Load<Model>(item.modelName);
                                    item.GenerateAvatar();
                                    avatars.Add(item.avatar);
                                    item.BuildButtons(graphicsManager);

                                    
                                    room.GOAPActions.AddRange(item.GOAPActions);



                                }

                                house.GOAPActions.AddRange(room.GOAPActions);


                            }

                            town.GOAPActions.AddRange(house.GOAPActions);

                            

                        }
                        
                        



                    }


                }


                

            }
            
            
            House.navMesh = new Mesh(house: true);
            Town.Town.navMesh = new Mesh(town: true);
            

        }



        //public static async Task<List<District>> GetTowns(CloudDBHandler cloudDBHandler)
        //{

        //    List<Town.Town> towns = await cloudDBHandler.GetTownsInDB

        //    using (var cloudDBHandler = new CloudDBHandler())
        //    {
        //        List<District> districts = await cloudDBHandler.GetDistrictsInTown();


        //        return districts;

        //    }



        //}


        //public static async Task<List<District>> GetDistricts()
        //{

        //    using (var cloudDBHandler = new CloudDBHandler())
        //    {
        //        List<District> districts = await cloudDBHandler.GetDistrictsInTown();


        //        return districts;

        //    }



        //}

        public void RemoveAvatar(Avatar avatar)
        {
            int index = avatars.FindIndex(a => a.id == avatar.id);
            if (index != -1)
            {
                avatars.RemoveAt(index);
            }

        }

        public void AddAvatar(Avatar avatar)
        {
            avatars.Add(avatar);
        }


        public void BuildActions()
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





            //await BuildHouseAsync().ConfigureAwait(false);

            await BuildTownAsync().ConfigureAwait(false);







            Model woman2 = Content.Load<Model>("woman4");
            Model man2 = Content.Load<Model>("man4");
            Model woman5 = Content.Load<Model>("woman5");
            player = new Player(woman2, new Vector3(10, 0, 0), Town.Town.navMesh, towns[0], this);
            people.Add(player);
            //people.Add(new People(man2, new Vector3(10, 5, 10), navMesh));
            //people.Add(new People(woman5, new Vector3(-20, 5, -20), navMesh));


            avatars.AddRange(people.Select(people => people.avatar));


            //avatars.Add(new Avatar(Content.Load<Model>("houseBake6"), new Vector3(0, 41, 0)));
            
            
            avatars.Add(new Avatar(Content.Load<Model>("Grass"), new Vector3(0, -12, 0))); // was at y=-12
            avatars.Add(new Avatar(Content.Load<Model>("HouseInteriorWalls"), new Vector3(0, -5, 0))); //was at y=-2
            avatars.Add(new Avatar(Content.Load<Model>("HouseExteriorWalls"), new Vector3(0, -5, 0))); //was at y=-2
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
          





            //foreach (House house in houses)
            //{
            //    foreach (Room room in house.rooms)
            //    {
            //        foreach (Item item in room.items)
            //        {
            //            avatars.Add(new Avatar(Content.Load<Model>(item.modelName), new Vector3(float.Parse(item.locationX), -2, float.Parse(item.locationZ))));
            //        }
            //    }
            //}


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


            //buttons.AddRange(toolbarButtons);
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

                spriteBatch.Begin();

                foreach (Button button in buttons)
                {

                    spriteBatch.Draw(button.buttonTexture, button.position); 

                    if (button.buttonLabel != null)
                    {
                        spriteBatch.DrawString(spriteFont, button.buttonLabel, new Vector2( button.position.X + button.buttonTexture.Width/4, button.position.Y + button.buttonTexture.Height/20), Color.Black);

                    }
                    
                }

                foreach (ToolbarButton button in toolbarButtons)
                {
                    spriteBatch.Draw(button.buttonTexture, button.position);

                }

                spriteBatch.End();
                
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