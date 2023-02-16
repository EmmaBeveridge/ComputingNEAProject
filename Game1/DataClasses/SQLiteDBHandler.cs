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




        public static bool DBExists()
        {
            return File.Exists(DBName);
        }



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
                        command.Parameters.AddWithValue("@Career", person.Career==null?"":person.Career.ToString());
                        command.Parameters.AddWithValue("@PersonID", person.DBID);

                        command.ExecuteNonQuery();



                        foreach (Trait trait in person.Traits)
                        {
                            command.CommandText = "INSERT OR IGNORE INTO Trait(TraitHolderID, TraitID) VALUES (@TraitHolderID, @TraitID)";
                            command.Parameters.AddWithValue("@TraitHolderID", person.DBID);
                            command.Parameters.AddWithValue("@TraitID", trait.GetID());




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





                    }


                }

                
            }

            
            //save item inventory


        }




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



        public Dictionary<NeedNames, Need> GetNeeds(DBPerson person, List<Trait> traits)
        {

            Dictionary<NeedNames, Need> needs = new Dictionary<NeedNames, Need>();

            List<NeedNames> prioritisedNeeds = Trait.GetNeedsPrioritised(traits);
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

                        Need need = new Need(_name: Name, _currentNeed: Score, generateNeedBar: person.IsPlayer, _prioritised: prioritisedNeeds.Contains(Name));

                        needs.Add(Name, need);


                    }

                }
            }
            return needs;
        }





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





        public void AddPeople(string playerModelName, List<string> playerTraits)
        {

            Dictionary<int, string[]> PeopleTraits = new Dictionary<int, string[]> { { 1, new string[] { "Lazy", "Gourmand" } } };


            
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    using (SQLiteCommand insertCommand = connection.CreateCommand())
                    {

                        insertCommand.CommandText = "INSERT OR IGNORE INTO People (Name, HouseNum, Career, ModelName, IsPlayer) VALUES (@Name, @HouseNum, @Career, @ModelName, @IsPlayer)";

                        insertCommand.Parameters.AddWithValue("@Name", "Jane Doe");
                        insertCommand.Parameters.AddWithValue("@HouseNum", 2);
                        insertCommand.Parameters.AddWithValue("@Career", "store clerk");
                        insertCommand.Parameters.AddWithValue("@ModelName", "WomanYellow");
                        insertCommand.Parameters.AddWithValue("@IsPlayer", 0);

                        insertCommand.ExecuteNonQuery();

                        insertCommand.Parameters.AddWithValue("@Name", "player");
                        insertCommand.Parameters.AddWithValue("@HouseNum", 1);
                        insertCommand.Parameters.AddWithValue("@Career", "");
                        insertCommand.Parameters.AddWithValue("@ModelName", playerModelName);
                        insertCommand.Parameters.AddWithValue("@IsPlayer", 1);

                        insertCommand.ExecuteNonQuery();

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
                                    AddTraits(dbid, PeopleTraits[dbid], connection);
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

                }
            }

            SetTraitIDs();



        }


        /// <summary>
        /// Adds persons traits to Trait table in db
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



                        command.CommandText = "CREATE TABLE IF NOT EXISTS Trait(TraitID INTEGER NOT NULL UNIQUE, TraitHolderID INTEGER NOT NULL, TraitNumber INTEGER NOT NULL, PRIMARY KEY(TraitID AUTOINCREMENT), FOREIGN KEY(TraitHolderID) REFERENCES People(PersonID), FOREIGN KEY(TraitNumber) REFERENCES TraitLookup(TraitNumber))";

                        command.ExecuteNonQuery();


                        command.CommandText = "CREATE TABLE IF NOT EXISTS TraitLookup(TraitNumber INTEGER NOT NULL UNIQUE, TraitName TEXT NOT NULL, PRIMARY KEY(TraitNumber AUTOINCREMENT))";
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

