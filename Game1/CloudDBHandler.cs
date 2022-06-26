using Neo4j.Driver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Neo4jClient;
using Newtonsoft.Json;
using Game1.Rooms;
using Game1.Items;

namespace Game1
{
    public class CloudDBHandler : IDisposable
    {
        private bool _disposed = false;
        private readonly IDriver driver;
        //string uri = "neo4j+s://58d02373.databases.neo4j.io";
        string user = "neo4j";
        string password = "4nY0E1-_YOtZ-8Lx2iuxRNp8lTDUVTixHzx_wETJTpI";
        private GraphClient client;
        Uri uri = new Uri("neo4j+s://58d02373.databases.neo4j.io");

        ~CloudDBHandler() => Dispose(false);

        public CloudDBHandler()
        {
            driver = GraphDatabase.Driver(uri, AuthTokens.Basic(user, password));
            //client = new GraphClient(uri);
        }

        public async Task CreateTownAsync()
        {
            string query = ReadInQuery("CreateTownCypher3.txt");

            var session = driver.AsyncSession();
            try
            {
                
                var writeResults = await session.WriteTransactionAsync(async tx =>
                {
                    var result = await tx.RunAsync(query);
                    
                    return (await result.ToListAsync());
                });


            }
            
            catch (Neo4jException ex)
            {
                Console.WriteLine($"{query} - {ex}");
                throw;
            }
            finally
            {
                await session.CloseAsync();
       
            }
        }














        public async Task<List<Room>> GetRoomsInHouse()
        {
            var session = driver.AsyncSession();
            List<Room> rooms = new List<Room>();

            string query = "MATCH (h:House)-[:CONTAINS]-(r:Room) RETURN r";

            try
            {
                var readResults = await session.ReadTransactionAsync(async tx =>
                {
                    var result = await tx.RunAsync(query);
                    return (await result.ToListAsync());
                });

                foreach (var result in readResults)
                {
                    var roomTemp = JsonConvert.SerializeObject(result[0].As<INode>().Properties);
                    Room roomUncast = JsonConvert.DeserializeObject<Room>(roomTemp);

                    switch (roomUncast.roomClass)
                    {
                        case "Kitchen":
                            rooms.Add(JsonConvert.DeserializeObject<Kitchen>(roomTemp));
                            break;
                        case "Bathroom":
                            rooms.Add(JsonConvert.DeserializeObject<Bathroom>(roomTemp));
                            break;
                        case "Bedroom":
                            rooms.Add(JsonConvert.DeserializeObject<Bedroom>(roomTemp));
                            break;
                        case "LivingRoom":
                            rooms.Add(JsonConvert.DeserializeObject<LivingRoom>(roomTemp));
                            break;
                        default:
                            rooms.Add(roomUncast);
                            break;


                    }


                }

                return rooms;

            }

            catch (Neo4jException ex)
            {
                Console.WriteLine($"{query} - {ex}");
                throw;
            }
            finally
            {
                await session.CloseAsync();
            }
        }



        public async Task<List<Item>> GetItemsInRoom(Room room)
        {
            var session = driver.AsyncSession();
            List<Item> items = new List<Item>();


            string query = $"MATCH(r: Room{{class:'{room.roomClass}'}})-[:CONTAINS]-(i:Item) RETURN i";

            try
            {
                var readResults = await session.ReadTransactionAsync(async tx =>
                {
                    var result = await tx.RunAsync(query);
                    return (await result.ToListAsync());
                });

                foreach (var result in readResults)
                {
                    var itemTemp = JsonConvert.SerializeObject(result[0].As<INode>().Properties);
                    Item itemUncast = JsonConvert.DeserializeObject<Item>(itemTemp);

                    switch (itemUncast.itemClass)
                    {
                        case "Fridge":
                            items.Add(JsonConvert.DeserializeObject<Fridge>(itemTemp));
                            break;
                        case "Oven":
                            items.Add(JsonConvert.DeserializeObject<Oven>(itemTemp));
                            break;
                        case "Chair":
                            items.Add(JsonConvert.DeserializeObject<Chair>(itemTemp));
                            break;
                        case "Table":
                            items.Add(JsonConvert.DeserializeObject<Table>(itemTemp));
                            break;
                        case "Countertop":
                            items.Add(JsonConvert.DeserializeObject<Countertop>(itemTemp));
                            break;
                        case "CountertopSink":
                            items.Add(JsonConvert.DeserializeObject<CountertopSink>(itemTemp));
                            break;
                        case "Bin":
                            items.Add(JsonConvert.DeserializeObject<Bin>(itemTemp));
                            break;
                        case "Bed":
                            items.Add(JsonConvert.DeserializeObject<Bed>(itemTemp));
                            break;
                        case "Bookcase":
                            items.Add(JsonConvert.DeserializeObject<Bookcase>(itemTemp));
                            break;
                        case "Dresser":
                            items.Add(JsonConvert.DeserializeObject<Dresser>(itemTemp));
                            break;
                        case "EndTable":
                            items.Add(JsonConvert.DeserializeObject<EndTable>(itemTemp));
                            break;
                        case "Sink":
                            items.Add(JsonConvert.DeserializeObject<Sink>(itemTemp));
                            break;
                        case "Sofa":
                            items.Add(JsonConvert.DeserializeObject<Sofa>(itemTemp));
                            break;
                        case "Toilet":
                            items.Add(JsonConvert.DeserializeObject<Toilet>(itemTemp));
                            break;
                        case "TV":
                            items.Add(JsonConvert.DeserializeObject<TV>(itemTemp));
                            break;
                        default:
                            items.Add(itemUncast);
                            break;


                    }




                }

                Console.WriteLine("Success");

                return items;

            }
            
            catch (Neo4jException ex)
            {
                Console.WriteLine($"{query} - {ex}");
                throw;
            }
            finally
            {
                await session.CloseAsync();
            }
        }




        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                driver?.Dispose();
            }

            _disposed = true;
        }




        private string ReadInQuery(string fileName)
        {
            string URI = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent.Parent.FullName;
            URI = URI + "/GraphDatabase/" + fileName;

            string query = File.ReadAllText(URI);

            

            return query;



        }






        




       
    }
}
