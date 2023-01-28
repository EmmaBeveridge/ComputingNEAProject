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
                        float score = dataReader.GetInt32(dataReader.GetOrdinal("Score"));


                        relationships.Add(person2, score);



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
                    SQLiteDataReader dataReader;

                    command.CommandText = "SELECT TraitName FROM People INNER JOIN Trait ON TraitHolderID = PersonID WHERE PersonId = @PersonID ";
                    command.Parameters.AddWithValue("@PersonID", person.DBID);
                    dataReader = command.ExecuteReader();

                    while (dataReader.Read())
                    {
                        Trait trait = Trait.GetTraitFromString(dataReader.GetString(dataReader.GetOrdinal("TraitName")));
                        traits.Add(trait);


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
                        float Score = dataReader.GetInt32(dataReader.GetOrdinal("Score"));

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

                        bool isPlayer = name.ToLower() == "player" ? true : false;


                        DBPeople.Add(new DBPerson(dbid, name, isPlayer, career, house, modelName));


                    }
                }
            }

            return DBPeople;


        }





        public void AddPeople()
        {
            
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    using (SQLiteCommand insertCommand = connection.CreateCommand())
                    {
                        
                        insertCommand.CommandText = "INSERT OR IGNORE INTO People (Name, HouseNum, Career, ModelName) VALUES (@Name, @HouseNum, @Career, @ModelName)";

                        insertCommand.Parameters.AddWithValue("@Name", "Robin Steele");
                        insertCommand.Parameters.AddWithValue("@HouseNum", 2);
                        insertCommand.Parameters.AddWithValue("@Career", "theif");
                        insertCommand.Parameters.AddWithValue("@ModelName", "WomanYellow");
                        insertCommand.ExecuteNonQuery();

                        insertCommand.Parameters.AddWithValue("@Name", "player");
                        insertCommand.Parameters.AddWithValue("@HouseNum", 1);
                        insertCommand.Parameters.AddWithValue("@Career", "");
                        insertCommand.Parameters.AddWithValue("@ModelName", "WomanPurple"); //need to change for character customisation
                        insertCommand.ExecuteNonQuery();



                        using (SQLiteCommand readCommand = connection.CreateCommand())
                        {
                            SQLiteDataReader dataReader;

                            readCommand.CommandText = "SELECT PersonID FROM People";

                            dataReader = readCommand.ExecuteReader();
                            while (dataReader.Read())
                            {
                                int dbid = dataReader.GetInt32(dataReader.GetOrdinal("PersonID"));



                                AddNeeds(dbid, connection);






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

                        command.CommandText = "DROP TABLE IF EXISTS InventoryIndex";
                        command.ExecuteNonQuery();

                        command.CommandText = "DROP TABLE IF EXISTS InventoryItems";
                        command.ExecuteNonQuery();


                        command.CommandText = "CREATE TABLE IF NOT EXISTS People(PersonID INTEGER NOT NULL UNIQUE, Name TEXT NOT NULL UNIQUE, HouseNum INTEGER, Career TEXT, ModelName TEXT NOT NULL, PRIMARY KEY(PersonID AUTOINCREMENT))";
                        command.ExecuteNonQuery();


                        command.CommandText = "CREATE TABLE IF NOT EXISTS Relationship (RelationshipID INTEGER NOT NULL UNIQUE, Person1ID INTEGER NOT NULL, Person2ID INTEGER NOT NULL, Score INTEGER NOT NULL, PRIMARY KEY(RelationshipID AUTOINCREMENT), FOREIGN KEY(Person1ID) REFERENCES People(PersonID), FOREIGN KEY(Person2ID) REFERENCES People(PersonID))";

                        command.ExecuteNonQuery();

                        command.CommandText = "CREATE TABLE IF NOT EXISTS Need(NeedID INTEGER NOT NULL UNIQUE, PersonID INTEGER NOT NULL, NeedName TEXT NOT NULL, Score INTEGER NOT NULL, PRIMARY KEY(NeedID AUTOINCREMENT), FOREIGN KEY(PersonID) REFERENCES People(PersonID))";
                        command.ExecuteNonQuery();



                        command.CommandText = "CREATE TABLE IF NOT EXISTS Trait(TraitID INTEGER NOT NULL UNIQUE, TraitHolderID INTEGER NOT NULL, TraitName TEXT NOT NULL, PRIMARY KEY(TraitID AUTOINCREMENT), FOREIGN KEY(TraitHolderID) REFERENCES People(PersonID))";

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

