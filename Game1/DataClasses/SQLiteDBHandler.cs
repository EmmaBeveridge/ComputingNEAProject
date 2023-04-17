using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.IO;
using Game1.Town;
using Game1.Careers;
using Game1.Traits;
using Game1.Skills;

namespace Game1.DataClasses
{
    public class SQLiteDBHandler
    {

        SQLiteConnection connection;
        SQLiteCommand command;

        static string DBName = "Database.db";
        string connectionString = $"Data Source={DBName}; Version = 3; New = True; Compress = True; ";
        public SQLiteDBHandler()
        {


           

        }





        public void CloseConnection()
        {
            
            connection.Close();

        }



        /// <summary>
        /// Returns if database file for game exists 
        /// </summary>
        /// <returns></returns>
        public static bool DBExists()
        {
            return File.Exists(DBName);
        }


        /// <summary>
        /// Saves game state for each person. Updates career, trait, relationship, need, and skill data using SQLite queries. 
        /// </summary>
        /// <param name="people">People to save state for</param>
        public void SaveGame(List<People> people)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                using (SQLiteCommand command = connection.CreateCommand())
                {
                    foreach (People person in people) //save needs, relationships, career
                    {
                        command.CommandText = "UPDATE People SET Career = @Career WHERE PersonID = @PersonID";
                        command.Parameters.AddWithValue("@Career", person.Career==null?"":person.Career.CareerName);
                       
                        command.Parameters.AddWithValue("@PersonID", person.DBID);

                        command.ExecuteNonQuery();



                        foreach (Trait trait in person.Traits)
                        {
                            command.CommandText = "INSERT OR IGNORE INTO Trait(TraitHolderID, TraitNumber) VALUES (@TraitHolderID, @TraitNumber)";
                            command.Parameters.AddWithValue("@TraitHolderID", person.DBID);
                            command.Parameters.AddWithValue("@TraitNumber", trait.GetID());


                            command.ExecuteNonQuery();


                        }




                        foreach (KeyValuePair<People, float> relationship in person.Relationships)
                        {
                            //adds relationship if doesn't already exist
                            command.CommandText = "INSERT OR IGNORE INTO Relationship(Person1ID, Person2ID, Score) VALUES (@Person1ID, @Person2ID, @Score)";
                            command.Parameters.AddWithValue("@Person1ID", person.DBID);
                            command.Parameters.AddWithValue("@Person2ID", relationship.Key.DBID);
                            command.Parameters.AddWithValue("@Score", relationship.Value);

                            command.ExecuteNonQuery();


                            //updates existing relationship which now must exist in table
                            command.CommandText="UPDATE Relationship SET Score = @Score WHERE Person1ID = @Person1ID AND Person2ID = @Person2ID";

                            command.Parameters.AddWithValue("@Person1ID", person.DBID);
                            command.Parameters.AddWithValue("@Person2ID", relationship.Key.DBID);
                            command.Parameters.AddWithValue("@Score", relationship.Value);

                            command.ExecuteNonQuery();

                        }

                        foreach (KeyValuePair<NeedNames, Need> need in person.Needs)
                        {
                            command.CommandText = "UPDATE Need SET Score = @Score WHERE PersonID = @PersonID AND NeedName = @NeedName";
                            command.Parameters.AddWithValue("@Score", need.Value.CurrentNeed);
                            command.Parameters.AddWithValue("@PersonID", person.DBID);
                            command.Parameters.AddWithValue("@NeedName", need.Key.ToString());

                            command.ExecuteNonQuery();

                        }

                        foreach (Skill skill in person.Skills)
                        {
                            command.CommandText = "INSERT INTO Skill(SkillHolderID, SkillScore, SkillNumber) VALUES (@SkillHolderID, @SkillScore, @SkillNumber) ON CONFLICT (SkillHolderID, SkillNumber) DO UPDATE SET SkillScore = @SkillScore WHERE SkillHolderID = @SkillHolderID AND SkillNumber = @SkillNumber";

                           
                            command.Parameters.AddWithValue("@SkillScore", skill.Score);
                            command.Parameters.AddWithValue("@SkillHolderID", person.DBID);
                            command.Parameters.AddWithValue("@SkillNumber", skill.GetID());

                            command.ExecuteNonQuery();

                        }





                    }


                }

                
            }

            
            //save item inventory


        }



        /// <summary>
        ///  Uses Trait.SetTraitID method to set static attribute DBID for each child trait class to match TraitNumber field stored in database.
        /// </summary>
        public void SetTraitIDs()
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                using (SQLiteCommand command = connection.CreateCommand())
                {
                    SQLiteDataReader dataReader;
                    command.CommandText = "SELECT * FROM TraitLookup";
                    dataReader = command.ExecuteReader();



                    while (dataReader.Read())
                    {
                        int TraitID = dataReader.GetInt32(dataReader.GetOrdinal("TraitNumber"));
                        string TraitName = dataReader.GetString(dataReader.GetOrdinal("TraitName"));


                        Trait.SetTraitID(TraitName, TraitID);

                    }



                }

            }
        }


        /// <summary>
        /// Uses Skill.SetSkillID method to set static attribute DBID for each child skill class to match SkillNumber field stored in database.
        /// </summary>
        public void SetSkillIDs()
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                using (SQLiteCommand command = connection.CreateCommand())
                {
                    SQLiteDataReader dataReader;
                    command.CommandText = "SELECT * FROM SkillLookup";
                    dataReader = command.ExecuteReader();



                    while (dataReader.Read())
                    {
                        int SkillID = dataReader.GetInt32(dataReader.GetOrdinal("SkillNumber"));
                        string SKillName = dataReader.GetString(dataReader.GetOrdinal("SkillName"));


                        Skill.SetSkillID(SKillName, SkillID);

                    }



                }

            }
        }




        /// <summary>
        /// Populates relationships dictionary for person from database. 
        /// </summary>
        /// <param name="person1">Person to set relationship for</param>
        /// <param name="people2">List of people with whom person1 may have relationship i.e. all people</param>
        /// <returns></returns>
        public Dictionary<People, float> GetRelationships(People person1, List<People> people2)
        {
            Dictionary<People, float> relationships = new Dictionary<People, float>();

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                using (SQLiteCommand command = connection.CreateCommand())
                {
                    SQLiteDataReader dataReader;
                    command.CommandText = "SELECT Person2ID, Score FROM People INNER JOIN Relationship ON Person1ID = PersonId WHERE PersonId = @PersonID ";
                    command.Parameters.AddWithValue("@PersonID", person1.DBID);
                    dataReader = command.ExecuteReader();

                    while (dataReader.Read())
                    {
                        People person2 = people2.Find(p => p.DBID == dataReader.GetInt32(dataReader.GetOrdinal("Person2ID")));
                        float score = dataReader.GetFloat(dataReader.GetOrdinal("Score"));


                        relationships[person2] = score;

                        


                    }


                }

            }

            return relationships;


        }

        /// <summary>
        /// Uses trait lookup system to return name of trait possessed by person. Adds corresponding trait object to person’s list of traits using Trait.GetTraitFromString method.
        /// </summary>
        /// <param name="person">DB person to get traits for</param>
        /// <returns></returns>
        public List<Trait> GetTraits (DBPerson person)
        {
            List<Trait> traits = new List<Trait>();
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (SQLiteCommand command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT TraitName FROM People INNER JOIN Trait ON PersonID = Trait.TraitHolderID JOIN TraitLookup ON TraitLookup.TraitNumber = Trait.TraitNumber WHERE PersonID = @PersonID";
                    command.Parameters.AddWithValue("@PersonID", person.DBID);

                    using (SQLiteDataReader dataReader = command.ExecuteReader())
                    {


                        while (dataReader.Read())
                        {
                            Trait trait = Trait.GetTraitFromString(dataReader.GetString(dataReader.GetOrdinal("TraitName")));
                            traits.Add(trait);


                        }
                    }
                }
            }

            return traits;



        }

        /// <summary>
        /// Uses skill lookup system to return name of skill possessed by person. Adds corresponding skill object to person’s list of skills using Skill.GetSkillFromString method.
        /// </summary>
        /// <param name="person">DB person to get skills for</param>
        /// <returns></returns>
        public List<Skill> GetSkills(DBPerson person)
        {
            List<Skill> skills = new List<Skill>();
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (SQLiteCommand command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT SkillName, SkillScore FROM People INNER JOIN Skill ON PersonID = Skill.SkillHolderID JOIN SkillLookup ON SkillLookup.SkillNumber = Skill.SkillNumber WHERE PersonID = @PersonID";
                    command.Parameters.AddWithValue("@PersonID", person.DBID);

                    using (SQLiteDataReader dataReader = command.ExecuteReader())
                    {


                        while (dataReader.Read())
                        {
                            Skill skill = Skill.GetSkillFromString(dataReader.GetString(dataReader.GetOrdinal("SkillName")));
                            skill.Score = dataReader.GetFloat(dataReader.GetOrdinal("SkillScore"));
                            skills.Add(skill);


                        }
                    }
                }
            }

            return skills;



        }


        /// <summary>
        /// Gets person’s need data and adds new need object to needs dictionary. Given list of traits as a parameter which is used with static methods in Trait class to determine which needs are prioritised, have accelerated/decelerated depletion for player.
        /// </summary>
        /// <param name="person">Person to get needs for</param>
        /// <param name="traits">Person's traits</param>
        /// <returns></returns>
        public Dictionary<NeedNames, Need> GetNeeds(DBPerson person, List<Trait> traits)
        {

            Dictionary<NeedNames, Need> needs = new Dictionary<NeedNames, Need>();

            List<NeedNames> prioritisedNeeds = Trait.GetNeedsPrioritised(traits);
            List<NeedNames> acceleratedDepletionNeeds = Trait.GetNeedsAcceleratedDepletion(traits);
            List<NeedNames> deceleratedDepletionNeeds = Trait.GetNeedsDeceleratedDepletion(traits);
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (SQLiteCommand command = connection.CreateCommand())
                {
                    SQLiteDataReader dataReader;


                    command.CommandText = "SELECT NeedName, Score FROM People INNER JOIN Need ON People.PersonID = Need.PersonID WHERE People.PersonId = @PersonID ";
                    command.Parameters.AddWithValue("@PersonID", person.DBID);
                    dataReader = command.ExecuteReader();


                    while (dataReader.Read())
                    {

                        NeedNames Name = Need.GetNeedNamefromString(dataReader.GetString(dataReader.GetOrdinal("NeedName")));
                        float Score = dataReader.GetFloat(dataReader.GetOrdinal("Score"));

                        Need need = new Need(_name: Name, _currentNeed: Score, generateNeedBar: person.IsPlayer, _prioritised: prioritisedNeeds.Contains(Name), _accelerated: acceleratedDepletionNeeds.Contains(Name), _decelerated: deceleratedDepletionNeeds.Contains(Name));

                        needs.Add(Name, need);


                    }

                }
            }
            return needs;
        }




        /// <summary>
        /// Returns list of DBPeople which contain people data read from People table
        /// </summary>
        /// <returns></returns>
        public List<DBPerson> GetPeople()
        {
            List<DBPerson> DBPeople = new List<DBPerson>();
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (SQLiteCommand command = connection.CreateCommand())
                {
                    SQLiteDataReader dataReader;
                    command.CommandText = "SELECT * FROM People";
                    dataReader = command.ExecuteReader();

                    while (dataReader.Read())
                    {
                        int dbid = dataReader.GetInt32(dataReader.GetOrdinal("PersonID"));

                        string name = dataReader.GetString(dataReader.GetOrdinal("Name"));

                        House house = House.GetHouseFromNumber(dataReader.GetInt32(dataReader.GetOrdinal("HouseNum")));

                        Career career = Career.GetCareerFromString(dataReader.GetString(dataReader.GetOrdinal("Career")));

                        string modelName = dataReader.GetString(dataReader.GetOrdinal("ModelName"));

                        bool isPlayer = dataReader.GetInt16(dataReader.GetOrdinal("IsPlayer")) == 1 ? true : false;


                        DBPeople.Add(new DBPerson(dbid, name, isPlayer, career, house, modelName));


                    }
                }
            }

            return DBPeople;


        }



        /// <summary>
        /// Container for NPC data. Convenient way to store/format attribute data for NPCs so they can be added to the SQLite database
        /// </summary>
        struct NPC
        {
            public int DBID;
            public string Name;
            public int HouseNum;
            public string Career;
            public string ModelName;
            public string[] Traits;


            /// <summary>
            /// Constructor for new NPC object
            /// </summary>
            /// <param name="name"></param>
            /// <param name="houseNum"></param>
            /// <param name="career"></param>
            /// <param name="modelName"></param>
            /// <param name="traits"></param>
            public NPC(int dbid, string name, int houseNum, string career, string modelName, string[] traits)
            {
                DBID = dbid;
                Name = name;
                HouseNum = houseNum;
                Career = career;
                ModelName = modelName;
                Traits = traits; 

            }

        }


        /// <summary>
        /// Adds basic player and NPC data to tables when new game created. Calls methods to add needs and traits to database for each person. 
        /// </summary>
        /// <param name="playerModelName">String name of selected player model set</param>
        /// <param name="playerTraits">String list of selected traits for player</param>
        public void AddPeople(string playerModelName, List<string> playerTraits)
        {
            #region Data for NPCs
            List<NPC> NPCs = new List<NPC>();

            NPCs.Add(new NPC(2, "Fred", 2, "store clerk","ManRed", new string[] { "Sociable", "FunLoving" } ));
            NPCs.Add(new NPC(3, "Daphne", 3, "teacher", "WomanPurple", new string[] { "Sociable", "Clean" }));
            NPCs.Add(new NPC(4, "Velma", 4, "doctor", "WomanYellow", new string[] { "Loner", "FunLoving" }));
            NPCs.Add(new NPC(5, "Shaggy", 5, "office worker", "ManGreen", new string[] { "Lazy", "Gourmand" }));
            NPCs.Add(new NPC(6, "Scooby", 6, "", "ManGreen", new string[] { "Gourmand", "FunLoving" }));
            #endregion

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    using (SQLiteCommand insertCommand = connection.CreateCommand())
                    {
                        
                        insertCommand.CommandText = "INSERT OR IGNORE INTO People (Name, HouseNum, Career, ModelName, IsPlayer) VALUES (@Name, @HouseNum, @Career, @ModelName, @IsPlayer)";
                        insertCommand.Parameters.AddWithValue("@Name", "player");
                        insertCommand.Parameters.AddWithValue("@HouseNum", 1);
                        insertCommand.Parameters.AddWithValue("@Career", "");
                        insertCommand.Parameters.AddWithValue("@ModelName", playerModelName);
                        insertCommand.Parameters.AddWithValue("@IsPlayer", 1);

                        insertCommand.ExecuteNonQuery();




                        foreach (NPC npc in NPCs)
                        {
                            insertCommand.Parameters.AddWithValue("@Name", npc.Name);
                            insertCommand.Parameters.AddWithValue("@HouseNum", npc.HouseNum);
                            insertCommand.Parameters.AddWithValue("@Career", npc.Career);
                            insertCommand.Parameters.AddWithValue("@ModelName", npc.ModelName);
                            insertCommand.Parameters.AddWithValue("@IsPlayer", 0);

                            insertCommand.ExecuteNonQuery();

                        }

                        

                        

                    }

                    using (SQLiteCommand readCommand = connection.CreateCommand())
                    {
                        readCommand.CommandText = "SELECT PersonID, IsPlayer FROM People";
                        using (SQLiteDataReader dataReader = readCommand.ExecuteReader())
                        {


                            
                            while (dataReader.Read())
                            {
                                int dbid = dataReader.GetInt32(dataReader.GetOrdinal("PersonID"));
                                bool isPlayer = dataReader.GetInt16(dataReader.GetOrdinal("IsPlayer")) == 1 ? true : false;





                                AddNeeds(dbid, connection);


                                if (isPlayer)
                                {
                                    AddTraits(dbid, playerTraits.ToArray(), connection);
                                }
                                else
                                {
                                    AddTraits(dbid, NPCs.Find(npc => npc.DBID == dbid).Traits, connection);
                                }




                            }

                        }
                    }







                    





                }







            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex.ToString());

            }

        }


        /// <summary>
        ///  Populates SkillLookup table with records for each skill, where SkillNumber is autoincremented and SkillName is obtained from static attribute SkillString from child skill classes. After populating table, calls SetSkillIDs().
        /// </summary>
        public void CreateSkillLookupTable()
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                using (SQLiteCommand insertCommand = connection.CreateCommand())
                {
                    insertCommand.CommandText = "INSERT OR IGNORE INTO SkillLookup (SkillName) VALUES (@SkillName)";

                    insertCommand.Parameters.AddWithValue("@SkillName", Skills.Cooking.SkillString);
                    insertCommand.ExecuteNonQuery();

                    

                }
            }

            SetSkillIDs();


        }



        /// <summary>
        /// Populates TraitLookup table with records for each trait, where TraitNumber is autoincremented and TraitName is obtained from static attribute TraitString from child trait classes. After populating table, calls SetTraitIDs().
        /// </summary>
        public void CreateTraitLookupTable()
        {

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                using (SQLiteCommand insertCommand = connection.CreateCommand())
                {
                    insertCommand.CommandText = "INSERT OR IGNORE INTO TraitLookup (TraitName) VALUES (@TraitName)";

                    insertCommand.Parameters.AddWithValue("@TraitName", TraitLazy.TraitString);
                    insertCommand.ExecuteNonQuery();

                    insertCommand.Parameters.AddWithValue("@TraitName", TraitGourmand.TraitString);
                    insertCommand.ExecuteNonQuery();

                    insertCommand.Parameters.AddWithValue("@TraitName", TraitSociable.TraitString);
                    insertCommand.ExecuteNonQuery();

                    insertCommand.Parameters.AddWithValue("@TraitName", TraitClean.TraitString);
                    insertCommand.ExecuteNonQuery();

                    insertCommand.Parameters.AddWithValue("@TraitName", TraitFunLoving.TraitString);
                    insertCommand.ExecuteNonQuery();

                    insertCommand.Parameters.AddWithValue("@TraitName", TraitLoner.TraitString);
                    insertCommand.ExecuteNonQuery();

                }
            }

            SetTraitIDs();



        }


        /// <summary>
        /// Adds traits for person to Trait table in database. String representation of trait supplied in a list as a parameter is converted to the correct trait numbers as used in the database using  Trait.GetTraitID static method. 
        /// </summary>
        /// <param name="PersonID">Id of trait holder</param>
        /// <param name="TraitNames">Array of string names of traits of person</param>
        /// <param name="connection">Db connection</param>
        public void AddTraits(int PersonID, string[] TraitNames, SQLiteConnection connection)
        {
            
                using (SQLiteCommand command = connection.CreateCommand())
                {
                    command.CommandText = "INSERT OR IGNORE INTO Trait (TraitHolderID, TraitNumber) VALUES (@TraitHolderID, @TraitNumber)";

                    foreach (string TraitName in TraitNames)
                    {
                        command.Parameters.AddWithValue("@TraitHolderID", PersonID);
                        command.Parameters.AddWithValue("@TraitNumber", Trait.GetTraitID(TraitName));
                        command.ExecuteNonQuery();

                    }
                    
                }

            
        }

        /// <summary>
        /// Adds skills for person to Skill table in database. String representation of skill supplied in a list as a parameter is converted to the correct skill numbers as used in the database using  Skill.GetSkillID static method.
        /// </summary>
        /// <param name="PersonID">ID of skill holder</param>
        /// <param name="SkillsNames">Array of string names of skills for person</param>
        /// <param name="connection">db connection</param>
        public void AddSkills(int PersonID, string[] SkillsNames, SQLiteConnection connection)
        {

            using (SQLiteCommand command = connection.CreateCommand())
            {
                command.CommandText = "INSERT OR IGNORE INTO Skill (SkillHolderID, SkillNumber, SkillScore) VALUES (@SkillHolderID, @SkillNumber, @SkillScore)";

                foreach (string SkillName in SkillsNames)
                {
                    command.Parameters.AddWithValue("@SkillsHolderID", PersonID);
                    command.Parameters.AddWithValue("@SkillNumber", Skill.GetSkillID(SkillName));
                    command.ExecuteNonQuery();

                }

            }


        }


        /// <summary>
        /// Adds record for each of person’s needs to Need table in database. Default need score set as 50.
        /// </summary>
        /// <param name="PersonID">ID of need holder</param>
        /// <param name="connection">db connection</param>
        public void AddNeeds(int PersonID, SQLiteConnection connection)
        {
           
            using (SQLiteCommand insertCommand = connection.CreateCommand())
            {

                foreach (NeedNames needName in Enum.GetValues(typeof(NeedNames)))
                {

                    if (needName == NeedNames.Null)
                    {
                        continue;

                    }
                        
                    insertCommand.CommandText = "INSERT INTO Need(PersonID, NeedName, Score) VALUES (@PersonID, @NeedName, @Score)";

                    insertCommand.Parameters.AddWithValue("@PersonID", PersonID);
                    insertCommand.Parameters.AddWithValue("@NeedName", needName.ToString());
                    insertCommand.Parameters.AddWithValue("@Score", 50); //50 is default score value

                    insertCommand.ExecuteNonQuery();


                }






            }
            

        }

        /// <summary>
        /// Called when beginning a new game. Drops existing tables and creates new tables according to schema. Creates: People, Need, Relationship, Trait, TraitLookup, Skill, SkillLookup, InventoryIndex, InventoryItems. 
        /// </summary>
        public void CreateTables()
        {


            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    using (SQLiteCommand command = connection.CreateCommand())
                    {

                        command.CommandText="DROP TABLE IF EXISTS People";
                        command.ExecuteNonQuery();
                        
                        command.CommandText = "DROP TABLE IF EXISTS Need";
                        command.ExecuteNonQuery();

                        command.CommandText = "DROP TABLE IF EXISTS Relationship";
                        command.ExecuteNonQuery();

                        command.CommandText = "DROP TABLE IF EXISTS Trait";
                        command.ExecuteNonQuery();

                        command.CommandText = "DROP TABLE IF EXISTS TraitLookup";
                        command.ExecuteNonQuery();

                        command.CommandText = "DROP TABLE IF EXISTS SkillLookup";
                        command.ExecuteNonQuery();

                        command.CommandText = "DROP TABLE IF EXISTS Skill";
                        command.ExecuteNonQuery();


                        command.CommandText = "DROP TABLE IF EXISTS InventoryIndex";
                        command.ExecuteNonQuery();

                        command.CommandText = "DROP TABLE IF EXISTS InventoryItems";
                        command.ExecuteNonQuery();


                        command.CommandText = "CREATE TABLE IF NOT EXISTS People(PersonID INTEGER NOT NULL UNIQUE, Name TEXT NOT NULL UNIQUE, HouseNum INTEGER, Career TEXT, ModelName TEXT NOT NULL, IsPlayer INT NOT NULL,  PRIMARY KEY(PersonID AUTOINCREMENT))";
                        command.ExecuteNonQuery();


                        command.CommandText = "CREATE TABLE IF NOT EXISTS Relationship (RelationshipID INTEGER NOT NULL UNIQUE, Person1ID INTEGER NOT NULL, Person2ID INTEGER NOT NULL, Score INTEGER NOT NULL, PRIMARY KEY(RelationshipID AUTOINCREMENT), FOREIGN KEY(Person1ID) REFERENCES People(PersonID), FOREIGN KEY(Person2ID) REFERENCES People(PersonID))";

                        command.ExecuteNonQuery();

                        command.CommandText = "CREATE TABLE IF NOT EXISTS Need(NeedID INTEGER NOT NULL UNIQUE, PersonID INTEGER NOT NULL, NeedName TEXT NOT NULL, Score INTEGER NOT NULL, PRIMARY KEY(NeedID AUTOINCREMENT), FOREIGN KEY(PersonID) REFERENCES People(PersonID))";
                        command.ExecuteNonQuery();



                        command.CommandText = "CREATE TABLE IF NOT EXISTS Trait(TraitID INTEGER NOT NULL UNIQUE, TraitHolderID INTEGER NOT NULL, TraitNumber INTEGER NOT NULL, PRIMARY KEY(TraitID AUTOINCREMENT), FOREIGN KEY(TraitHolderID) REFERENCES People(PersonID) ON DELETE CASCADE, FOREIGN KEY(TraitNumber) REFERENCES TraitLookup(TraitNumber) ON DELETE CASCADE); CREATE UNIQUE INDEX idx2 ON Trait(TraitHolderID, TraitNumber);";

                        command.ExecuteNonQuery();


                        command.CommandText = "CREATE TABLE IF NOT EXISTS TraitLookup(TraitNumber INTEGER NOT NULL UNIQUE, TraitName TEXT NOT NULL, PRIMARY KEY(TraitNumber AUTOINCREMENT))";
                        command.ExecuteNonQuery();


                        command.CommandText = "CREATE TABLE IF NOT EXISTS Skill(SkillID INTEGER NOT NULL UNIQUE, SkillHolderID INTEGER NOT NULL, SkillNumber INTEGER NOT NULL, SkillScore INTEGER NOT NULL, PRIMARY KEY(SkillID AUTOINCREMENT), FOREIGN KEY(SkillHolderID) REFERENCES People(PersonID), FOREIGN KEY(SkillNumber) REFERENCES SkillLookup(SkillNumber)); CREATE UNIQUE INDEX idx ON Skill(SkillHolderID, SkillNumber);";

                        command.ExecuteNonQuery();

                        command.CommandText = "CREATE TABLE IF NOT EXISTS SkillLookup(SkillNumber INTEGER NOT NULL UNIQUE, SkillName TEXT NOT NULL, PRIMARY KEY(SkillNumber AUTOINCREMENT))";
                        command.ExecuteNonQuery();



                        command.CommandText = "CREATE TABLE IF NOT EXISTS InventoryIndex(InventoryID INTEGER NOT NULL UNIQUE, Item TEXT NOT NULL, Room TEXT, House INTEGER, PRIMARY KEY(InventoryID AUTOINCREMENT))";

                        command.ExecuteNonQuery();


                        command.CommandText = "CREATE TABLE IF NOT EXISTS InventoryItems(InventoryItemID INTEGER NOT NULL UNIQUE, InventoryID INTEGER NOT NULL, ItemName TEXT NOT NULL, Quantity INTEGER NOT NULL, PRIMARY KEY(InventoryItemID AUTOINCREMENT), FOREIGN KEY(InventoryID) REFERENCES InventoryIndex(InventoryID))";

                        command.ExecuteNonQuery();
                    }
                }

            }
            catch (Exception ex) { }

        }
        }

    }

