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

using Game1.ML;
using Game1.UI;
using System.Data;
using Game1.DataClasses;
using Game1.ID3;

namespace Game1


{

    public class Game1 : Game
    {
        public GraphicsDeviceManager graphicsManager;
        
        List<Avatar> avatars = new List<Avatar>();
       
        public UIHandler UIHandler = new UIHandler();

        public List<People> people = new List<People>();
        
        public List<Town.Town> towns = new List<Town.Town>();
        
        List<Texture2D> loadingScreens = new List<Texture2D>();

        Texture2D mainMenuScreen;




        public Dictionary<string, Model> modelDictionary = new Dictionary<string, Model>();
        public Dictionary<string, Texture2D> iconDictionary = new Dictionary<string, Texture2D>();
        


        SpriteBatch spriteBatch;
        SpriteFont spriteFont;
        


        Camera camera;

        public Player player;



        public GameStates.States gameState = GameStates.States.MainMenu;
        bool isLoading=false;




        Texture2D tempTexture;

       

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



        public static async Task CreateTownDB()
        {            
            using (var cloudDBHandler = new CloudDBHandler())
            {
                await cloudDBHandler.CreateTownInDBAsync();
               
            }
        }



        /// <summary>
        /// Loads basic textures and those needed for main menu and loading screen
        /// </summary>
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


            mainMenuScreen = Content.Load<Texture2D>("MainMenu");

            Texture2D exitMainTexture = Content.Load<Texture2D>("ExitMainButtonSized");
            Texture2D newGameTexture = Content.Load<Texture2D>("NewGameButtonSized");
            Texture2D resumeGameTexture = Content.Load<Texture2D>("ResumeGameButtonSized");




            ExitButton.Exit = Exit;

            UIHandler.BuildMainMenuButton(graphicsManager, exitMainTexture, resumeGameTexture, newGameTexture);



            Button.defaultTexture = Content.Load<Texture2D>("BlueButton");
            spriteFont = Content.Load<SpriteFont>("buttonFont");

            Textbox.defaultTextboxTexture = Content.Load<Texture2D>("Textbox");
            Textbox.defaultCursorTexture = Content.Load<Texture2D>("Cursor");
            Textbox.defaultFont = spriteFont;


            

            NeedsButton.defaultTexture = Content.Load<Texture2D>("NeedsButton4");

           
            RelationshipsButton.defaultTexture = Content.Load<Texture2D>("RelationshipButton2");
            
            SkillsButton.defaultTexture = Content.Load<Texture2D>("SkillsButton2");
           
            CareerButton.defaultTexture = Content.Load<Texture2D>("CareerButton2");


            ExitButton.defaultTexture = Content.Load<Texture2D>("ExitButton");

            NeedBar.GenerateTexture(GraphicsDevice);
            ToolbarPanel.GenerateTexture(GraphicsDevice);
            RelationshipBar.GenerateTexture(GraphicsDevice);
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

                    district.BuildStreets();


                   




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
                                room.SetLocation();
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




        /// <summary>
        /// Loads data for game. Loaded when loading screen displayed
        /// </summary>
        /// <returns></returns>
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



            




            MLMain.BuildML();


            People.BuildDecisionTree();



            //NEED TO RENAME MODELS AND ICONS!!!!!!
            modelDictionary.Add("WomanPurple", Content.Load<Model>("WomanPurpleModel")); 
            iconDictionary.Add("WomanPurple", Content.Load<Texture2D>("WomanPurpleIcon"));
            modelDictionary.Add("WomanYellow", Content.Load<Model>("WomanYellowModel"));
            iconDictionary.Add("WomanYellow", Content.Load<Texture2D>("WomanYellowIcon"));

            //Model man2 = Content.Load<Model>("man4");
            //Model woman5 = Content.Load<Model>("woman5");
            //Texture2D womanPurple = Content.Load<Texture2D>("WomanPurple");
            //Texture2D womanYellow = Content.Load<Texture2D>("WomanYellow");



            ResumeGameButton.ResumeGame(this);

            
            //player = new Player(woman2, new Vector3(10, 0, 0), Town.Town.navMesh, towns[0], this, womanPurple);


            //people.Add(player);
            ////people.Add(new People(man2, new Vector3(10, 5, 10), navMesh));
            //people.Add(new People(woman5, new Vector3(60, 0, 40), Town.Town.navMesh, towns[0], this, womanYellow));


            avatars.AddRange(people.Select(people => people.avatar));

            People.people.AddRange(people);


            foreach (People person in people)
            {
                towns[0].GOAPActions.Add(person.DefineActions()); //only one town but would need to change if added more

            }


            foreach (People person in people)
            {
                person.BuildAI();
            }









            //avatars.Add(new Avatar(Content.Load<Model>("houseBake6"), new Vector3(0, 41, 0)));
            










            
            avatars.Add(new Avatar(Content.Load<Model>("Grass"), new Vector3(0, -12, 0))); // was at y=-12
            avatars.Add(new Avatar(Content.Load<Model>("HouseInteriorWalls"), new Vector3(0, -5, 0))); //was at y=-2
            avatars.Add(new Avatar(Content.Load<Model>("HouseExteriorWalls"), new Vector3(0, -5, 0))); //was at y=-2
                //avatars.Add(new Avatar(Content.Load<Model>("Countertop"), new Vector3(60, 0, 100)));
                //avatars.Add(new Avatar(Content.Load<Model>("CountertopSink"), new Vector3(105, 0, 100)));
                //avatars.Add(new Avatar(Content.Load<Model>("Shower"), new Vector3(330, 0, -105)));
                //avatars.Add(new Avatar(Content.Load<Model>("Toilet"), new Vector3(285, 0, -55)));
                //avatars.Add(new Avatar(Content.Load<Model>("Bookcase"), new Vector3(70, 0, 5)));
                avatars.Add(new Avatar(Content.Load<Model>("Fridge"), new Vector3(155, 0, 100)));
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



            UIHandler.BuildToolbarButtons(graphicsManager, player);
            UIHandler.BuildExitButton(graphicsManager);

            
            

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

            MouseInput.SetPreviousState();



            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();

            }



            if (gameState == GameStates.States.MainMenu)
            {

                bool transitionToLoading = UIHandler.HandleMainMenuInput();
                if (transitionToLoading) { gameState = GameStates.States.Loading; }


            }

            
            else if (gameState==GameStates.States.Loading && !isLoading)
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

                UIHandler.Update(gameTime, graphicsManager, player);

            }

            if (gameState == GameStates.States.Playing && isLoading)
            {

                
                isLoading = false;
            }


            if (!UIHandler.displayTextbox)
            {
                camera.Update(gameTime);

            }
            

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

                UIHandler.Draw(spriteBatch, spriteFont);



                spriteBatch.End();
                
            }
           
           
           else if (gameState == GameStates.States.Loading)
           {
                
                spriteBatch.Begin();
                spriteBatch.Draw(loadingScreens[0], new Rectangle(0, 0, graphicsManager.PreferredBackBufferWidth, graphicsManager.PreferredBackBufferHeight), Color.White);
                loadingScreens.Add(loadingScreens[0]);
                loadingScreens.RemoveAt(0);

                //spriteBatch.Draw(tempTexture, position:Vector2.Zero, scale: new Vector2(0.7f, 0.8f));

                spriteBatch.End();
                Thread.Sleep(65);
                

           }



            else if (gameState == GameStates.States.MainMenu)
            {
                spriteBatch.Begin();
                
                spriteBatch.Draw(mainMenuScreen, position: Vector2.Zero, scale: new Vector2 (0.7f, 0.8f));

                UIHandler.DrawMainMenuButtons(spriteBatch);

                spriteBatch.End();
            }

            


            base.Draw(gameTime);
        }
    }
}