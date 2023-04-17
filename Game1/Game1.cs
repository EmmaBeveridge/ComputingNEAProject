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
using Game1.Traits;
using Game1.Characters;

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

        Texture2D characterSelectionBackground;

        Texture2D traitSelectionBackground;



        public Dictionary<string, Model> modelDictionary = new Dictionary<string, Model>();
        public Dictionary<string, Texture2D> iconDictionary = new Dictionary<string, Texture2D>();
        public Dictionary<Texture2D, string> characterNameDictionary = new Dictionary<Texture2D, string>();

        private Texture2D selectedCharacterTexture = null;


        SpriteBatch spriteBatch;
        SpriteFont spriteFont;
        


        Camera camera;

        public Player player;



        public GameStates.States gameState = GameStates.States.MainMenu;
        bool isLoading=false;




        

        public Game1()
        {
            graphicsManager = new GraphicsDeviceManager(this);
            
            Content.RootDirectory = "Content";
           
        }


       

        protected  override void Initialize()
        {

            
           
           
            base.Initialize();
            graphicsManager.ApplyChanges();
            camera = new Camera(GraphicsDevice, this.Window, new Vector3(0,60,-200), Vector3.Zero);
            this.IsMouseVisible = true;
            
            

        }


        /// <summary>
        /// Asynchronous method that calls CreateTownInDBAsync() on cloudDBHandler object to execute Cypher query to create the database (or merge changes if database already exists)
        /// </summary>
        /// <returns></returns>
        public static async Task CreateTownDB()
        {            
            using (var cloudDBHandler = new CloudDBHandler())
            {
                await cloudDBHandler.CreateTownInDBAsync();
               
            }
        }



        /// <summary>
        /// Loads all 2D textures from Monogame pipeline e.g. loading screen and menu backgrounds, button textures etc. And adds to relevant lists to be drawn in Draw(). Calls methods in UIHandler class to build buttons. Calls GenerateTexture methods for NeedBar, ToolbarPanel, and RelationshipBar classes. 
        /// </summary>
        protected override void LoadContent()
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
            characterSelectionBackground = Content.Load<Texture2D>("SelectCharacterScreen");
            traitSelectionBackground = Content.Load<Texture2D>("SelectTraitsScreen");


            Texture2D exitMainTexture = Content.Load<Texture2D>("ExitMainButtonSized");
            Texture2D newGameTexture = Content.Load<Texture2D>("NewGameButtonSized");
            Texture2D resumeGameTexture = Content.Load<Texture2D>("ResumeGameButtonSized");

            characterNameDictionary.Add(Content.Load<Texture2D>("WomanPurpleFullSized"), "WomanPurple");
            characterNameDictionary.Add(Content.Load<Texture2D>("WomanYellowFullSized"), "WomanYellow");
            characterNameDictionary.Add(Content.Load<Texture2D>("ManRedFullSized"), "ManRed");
            characterNameDictionary.Add(Content.Load<Texture2D>("ManGreenFullSized"), "ManGreen");


            Trait.ButtonToString.Add(Content.Load<Texture2D>("Lazy"), TraitLazy.TraitString);
            Trait.ButtonToString.Add(Content.Load<Texture2D>("Gourmand"), TraitGourmand.TraitString);
            Trait.ButtonToString.Add(Content.Load<Texture2D>("Sociable"), TraitSociable.TraitString);
            Trait.ButtonToString.Add(Content.Load<Texture2D>("Clean"), TraitClean.TraitString);
            Trait.ButtonToString.Add(Content.Load<Texture2D>("FunLoving"), TraitFunLoving.TraitString);
            Trait.ButtonToString.Add(Content.Load<Texture2D>("Loner"), TraitLoner.TraitString);

            EmotionButton.NegativeEmotionTexture = Content.Load<Texture2D>("Frown");
            EmotionButton.PositiveEmotionTexture = Content.Load<Texture2D>("Smile");

            

            UIHandler.BuildMainMenuButtons(graphicsManager, exitMainTexture, resumeGameTexture, newGameTexture);
            UIHandler.BuildCharacterSelectionButtons(graphicsManager, characterNameDictionary.Keys.ToList());
            

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




            #region Draw Path Points
            PathPoint.SetPathPointModel(Content.Load<Model>("PathPointModel"));
            #endregion




        }

        protected override void UnloadContent()
        {
        }


        /// <summary>
        ///Asynchronous method using a cloudDBHandler object to execute Cypher queries to return town objects. Iterates through town hierarchy (I.e. each district in town, each street in each district, each house on each street etc.) in order to set object properties. Note: objects e.g. house and town are not instantiated in this method but many of their properties and initialisation methods e.g. GenerateAvatar() are set and called by this method. List of GOAPActions is also created in this method as well as generating the static house and town navmeshes.
        /// </summary>
        /// <returns></returns>
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

                        foreach (Building building in street.buildings)
                        {
                            building.model = Content.Load<Model>(building.modelName);
                            building.GenerateAvatar();
                            avatars.Add(building.avatar);
                            building.BuildButtons(graphicsManager);

                            Building.buildings.Add(building);

                            town.buildings.Add(building);

                            town.GOAPActions.AddRange(building.GOAPActions);
                        }


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



        /// <summary>
        /// Removes avatar from list of avatars to be drawn.
        /// </summary>
        /// <param name="avatar"></param>
        public void RemoveAvatar(Avatar avatar)
        {
            int index = avatars.FindIndex(a => a.id == avatar.id);
            if (index != -1)
            {
                avatars.RemoveAt(index);
            }

        }

        /// <summary>
        /// Adds the avatar to list of avatars to be drawn
        /// </summary>
        /// <param name="avatar"></param>
        public void AddAvatar(Avatar avatar)
        {
            avatars.Add(avatar);
        }





        /// <summary>
        /// Calls methods relating to querying and initialising Neo4j town database. Loads 3D models of characters and creates People and Player objects. Builds dictionaries to convert character model strings stored in database to Model objects and corresponding 2D texture icons. Calls BuildML method on MLMain class to build sentiment analysis model. Calls BuildDecisionTree method on People class to build C4.5 decision tree. Calls ResumeGameMethod on ResumeGameButtonClass to resume game loaded from SQLite database. Adds actions allowing characters to talk to each other to list of available town actions. Calls BuildAI method for each People object. Uses UIHandler to build toolbar, exit, and emotion button objects.
        /// </summary>
        /// <returns></returns>
        async Task LoadGame()
        {


            await CreateTownDB().ConfigureAwait(false); // Create town database
            await BuildTownAsync().ConfigureAwait(false); //Build town objects

            MLMain.BuildML(); //Build sentiment analysis model


            People.BuildDecisionTree(); //Build need selection decision tree


            //Load models and icons and add to dictionaries
            
            modelDictionary.Add("WomanPurple", Content.Load<Model>("WomanPurpleModel")); 
            iconDictionary.Add("WomanPurple", Content.Load<Texture2D>("WomanPurpleIcon"));
            modelDictionary.Add("WomanYellow", Content.Load<Model>("WomanYellowModel"));
            iconDictionary.Add("WomanYellow", Content.Load<Texture2D>("WomanYellowIcon"));
            modelDictionary.Add("ManGreen", Content.Load<Model>("ManGreenModel"));
            iconDictionary.Add("ManGreen", Content.Load<Texture2D>("ManGreenIcon"));
            modelDictionary.Add("ManRed", Content.Load<Model>("ManRedModel"));
            iconDictionary.Add("ManRed", Content.Load<Texture2D>("ManRedIcon"));

            

            ResumeGameButton.ResumeGame(this); //resume the game

          

            avatars.AddRange(people.Select(people => people.avatar)); //add people avatars to avatar list so can be drawn

            People.people.AddRange(people); //add people to list of people


            foreach (People person in people)
            {
                towns[0].GOAPActions.Add(person.DefineActions()); //define GOAP actions for each person

            }


            foreach (People person in people)
            {
                person.BuildAI(); //Create GOAP finite state machine
            }



            #region Logging Need decay rates

            foreach (People person in people)
            {
                
                Console.WriteLine($"\n\nNeed depletion rates for {person.Name}\nTraits: {person.Traits[0]} and {person.Traits[1]}"); 

                foreach (KeyValuePair<NeedNames, Need> need in person.Needs)
                {
                    Console.WriteLine($"\tNeed: {need.Key} Decay Rate:{need.Value.DepletionRate}");
                }



            }


            #endregion


            avatars.Add(new Avatar(Content.Load<Model>("Grass"), new Vector3(0, -12, 0))); 

            //Build UI buttons
            UIHandler.BuildToolbarButtons(graphicsManager, player);
            UIHandler.BuildExitButton(graphicsManager);
            UIHandler.BuildEmotionButton(graphicsManager, player);
            
            
            //Set game state to playing as finished loading
            gameState = GameStates.States.Playing;


        }


        /// <summary>
        /// Resets previous mouse state. Using GameStates, the function will call:
        /// 1.	If in MainMenu state: uses UIHandler to handle main menu input.If Exit button selected, game will exit; if new game button selected, will transition GameState to Character Selection state; if resume game button selected, will transition GameState to Loading state.
        /// 2.	If in CharacterSelection state: Uses UIHandler to handle character selection input.If a character has been selected, uses lookup dictionary to find selected character name to store in database.Uses UIHandler to build trait selection buttons.Transitions GameState to Trait Selection state.
        /// 3.	If in TraitSelection state: uses UIHandler to handle trait selection. As users can select 2 traits for their character: if trait button pressed, switches selected state to allow users to select and unselect trait buttons. When 2 traits have been selected, uses NewGameButton class to create new game.Transitions GameState to Loading.
        /// 4.	 If Loading state: LoadGame() method (not automatically called by Monogame) is called
        /// 5.	If Playing state: Update() function on each person, the people being the only fully dynamic objects in the game and therefore the only objects which must be updated on each Update Draw iteration.Calls Update method on UIHandler to update UI display e.g.change buttons being drawn etc.
        /// The Camera Update method is called in this method regardless of game state, except when a textbox is being displayed to prevent user keyboard input from being interpreted as camera movement.
        /// </summary>
        /// <param name="gameTime"></param>
        protected override async void Update(GameTime gameTime)
        {

            MouseInput.SetPreviousState();



            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();

            }



            if (gameState == GameStates.States.MainMenu)
            {

                Button menuButtonPressed = UIHandler.HandleMainMenuInput();

                if (menuButtonPressed == null)
                {

                }
                
                else if (menuButtonPressed.GetType() == typeof(ExitButton))
                {
                    ExitButton.Exit(this);
                }
                else if (menuButtonPressed.GetType() == typeof(NewGameButton))
                {
                    gameState = GameStates.States.CharacterSelection;
                }
                else if (menuButtonPressed.GetType() == typeof(ResumeGameButton))
                {

                    gameState = GameStates.States.Loading;
                }


            }


            else if (gameState == GameStates.States.CharacterSelection)
            {
                Texture2D selectedCharacter = UIHandler.HandleCharacterSelection();

                if (selectedCharacter != null)
                {
                    NewGameButton.selectedCharacterName = characterNameDictionary[selectedCharacter];
                    
                    selectedCharacterTexture = selectedCharacter;

                    UIHandler.BuildTraitSelectionButtons(graphicsManager, Trait.ButtonToString.Keys.ToList(), selectedCharacter);


                    gameState = GameStates.States.TraitSelection;

                }
                

            }

            else if (gameState == GameStates.States.TraitSelection)
            {
                Texture2D selectedTrait = UIHandler.HandleTraitSelection();
                
                if (selectedTrait != null)
                {
                    string selectedTraitString = Trait.ButtonToString[selectedTrait];
                    Button selectedButton = UIHandler.traitSelectionButtons.Find(b => b.buttonTexture == selectedTrait);



                    if (NewGameButton.selectedTraitNames.Contains(selectedTraitString)) //user unselects button
                    {

                        selectedButton.isSelected = false;

                        NewGameButton.selectedTraitNames.Remove(selectedTraitString);
                       



                    }
                    else //selects new button
                    {
                        NewGameButton.selectedTraitNames.Add(selectedTraitString);

                        selectedButton.isSelected = true;

                        if (NewGameButton.selectedTraitNames.Count == Trait.maxTraits)
                        {
                            NewGameButton.CreateNewGame();
                            gameState = GameStates.States.Loading;
                        }

                    }




                }


            }

            
            else if (gameState==GameStates.States.Loading && !isLoading)
            {
                
                isLoading = true;
                

                await Task.Run(async () => { await LoadGame(); } );

               

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

        /// <summary>
        /// : Using GameStates, the function will either draw a 2D menu or loading screen or, if the game is in play mode, the Draw function for each avatar in the game as well as any selection or toolbar buttons. Each 3D item displayed in the game will have its own avatar used to render it.
        /// </summary>
        /// <param name="gameTime"></param>
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


                #region Draw Path Points
                if (player.drawPathPoints)
                {
                    PathPoint.DrawPath(camera.view, camera.projection);
                }

                #endregion

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

            else if (gameState == GameStates.States.CharacterSelection)
            {
                spriteBatch.Begin();
               
                spriteBatch.Draw(characterSelectionBackground, position: new Vector2(0, -10), scale: new Vector2(0.7f, 0.8f));
               

                UIHandler.DrawCharacterSelectionButtons(spriteBatch);
                spriteBatch.End();


            }
            else if (gameState == GameStates.States.TraitSelection)
            {
                spriteBatch.Begin();
                spriteBatch.Draw(traitSelectionBackground, position: new Vector2(0, -10), scale: new Vector2(0.7f, 0.8f));
                spriteBatch.Draw(selectedCharacterTexture, position: new Vector2(20, 80));
               

                UIHandler.DrawTraitSelectionButtons(spriteBatch);
                spriteBatch.End();

            }




            base.Draw(gameTime);
        }
    }
}