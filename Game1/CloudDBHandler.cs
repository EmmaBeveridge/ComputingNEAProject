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
using Game1.Town;
using Game1.Town.Districts;
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

        public async Task CreateTownInDBAsync()
        {
            string query = ReadInQuery("CreateTownCypher6.txt");

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



        public async Task<List<Town.Town>> GetTownsInDB()
        {
            var session = driver.AsyncSession();
            List<Town.Town> towns = new List<Town.Town>();

            string query = $"MATCH (t:Town) RETURN t";

            try
            {
                var readResults = await session.ReadTransactionAsync(async tx =>
                {
                    var result = await tx.RunAsync(query);
                    return (await result.ToListAsync());
                });

                foreach (var result in readResults)
                {
                    var townTemp = JsonConvert.SerializeObject(result[0].As<INode>().Properties);
                    Town.Town town = JsonConvert.DeserializeObject<Town.Town>(townTemp);
                    towns.Add(town);


                }

                return towns;

            }

            catch (Neo4jException ex)
            {
                Console.WriteLine($"{query} - {ex}");
                throw;
            }
            finally
            {
                Console.WriteLine("Got towns");
                await session.CloseAsync();
            }

        }





        public async Task<List<District>> GetDistrictsInTown(Town.Town town)
        {
            var session = driver.AsyncSession();
            List<District> districts = new List<District>();

            string query = $"MATCH (t:Town)-[:CONTAINS]-(d:District) WHERE t.id='{town.id}' RETURN d";

            try
            {
                var readResults = await session.ReadTransactionAsync(async tx =>
                {
                    var result = await tx.RunAsync(query);
                    return (await result.ToListAsync());
                });

                foreach (var result in readResults)
                {
                    var districtTemp = JsonConvert.SerializeObject(result[0].As<INode>().Properties);
                    District districtUncast = JsonConvert.DeserializeObject<District>(districtTemp);

                    switch (districtUncast.districtClass)
                    {
                        case "Residential":
                            districts.Add(JsonConvert.DeserializeObject<Residential>(districtTemp));
                            break;
                        default:
                            districts.Add(districtUncast);
                            break;


                    }


                }

                return districts;

            }

            catch (Neo4jException ex)
            {
                Console.WriteLine($"{query} - {ex}");
                throw;
            }
            finally
            {
                Console.WriteLine("got districts");
                await session.CloseAsync();
            }

        }




        public async Task<List<Street>> GetStreetsInDistrict(District district)
        {
            var session = driver.AsyncSession();
            List<Street> streets = new List<Street>();

            string query = $"MATCH (d:District)-[:CONTAINS]-(s:Street) WHERE d.id='{district.id}' RETURN s";

            try
            {
                var readResults = await session.ReadTransactionAsync(async tx =>
                {
                    var result = await tx.RunAsync(query);
                    return (await result.ToListAsync());
                });

                foreach (var result in readResults)
                {

                    var streetTemp = JsonConvert.SerializeObject(result[0].As<INode>().Properties);
                    Street street = JsonConvert.DeserializeObject<Street>(streetTemp);
                    streets.Add(street);
                    
                }

                return streets;

            }

            catch (Neo4jException ex)
            {
                Console.WriteLine($"{query} - {ex}");
                throw;
            }
            finally
            {
                Console.WriteLine("got streets");
                await session.CloseAsync();
            }

        }

        public async Task<List<Street>> SetStreetPointers(List<Street> streets)
        {
            

            foreach (Street street in streets)
            {
                var sessionParent = driver.AsyncSession();


                string getParentQuery = $"MATCH (p:Street)-[:PARENT]-(s:Street) WHERE s.id='{street.id}' RETURN p.id";
                try
                {
                    var readResults = await sessionParent.ReadTransactionAsync(async tx =>
                    {
                        var result = await tx.RunAsync(getParentQuery);
                        return (await result.ToListAsync());
                    });

                    foreach (var result in readResults)
                    {


                        string parentID = result.ToString();

                        if (streets.Exists(s => s.id == parentID))
                        {
                            street.parent = streets.Find(s => s.id == parentID);

                        }

                        

                        

                    }

                    

                }

                catch (Neo4jException ex)
                {
                    Console.WriteLine($"{getParentQuery} - {ex}");
                    throw;
                }
                finally
                {
                    await sessionParent.CloseAsync();
                }

                var sessionChildren = driver.AsyncSession();
                string getChildrenQuery = $"MATCH (s:Street)-[:CHILD]-(c:Street) WHERE s.id='{street.id}' RETURN c.id";

                try
                {
                    var readResults = await sessionChildren.ReadTransactionAsync(async tx =>
                    {
                        var result = await tx.RunAsync(getChildrenQuery);
                        return (await result.ToListAsync());
                    });

                    foreach (var result in readResults)
                    {


                        string childID = result.ToString();

                        if (streets.Exists(s => s.id == childID))
                        {
                            street.children.AddRange(streets.FindAll(s => s.id == childID));

                        }


                        



                    }

                    

                }

                catch (Neo4jException ex)
                {
                    Console.WriteLine($"{getParentQuery} - {ex}");
                    throw;
                }
                finally
                {
                    Console.WriteLine("set pointers");
                    await sessionChildren.CloseAsync();
                }
                
                
               

            }


             return streets;


        }


        public async Task<List<House>> GetHousesOnStreet(Street street)
        {
            var session = driver.AsyncSession();
            List<House> houses = new List<House>();

            string query = $"MATCH (s:Street)-[:CONTAINS]-(h:House) WHERE s.id='{street.id}' RETURN h";

            try
            {
                var readResults = await session.ReadTransactionAsync(async tx =>
                {
                    var result = await tx.RunAsync(query);
                    return (await result.ToListAsync());
                });

                foreach (var result in readResults)
                {

                    var houseTemp = JsonConvert.SerializeObject(result[0].As<INode>().Properties);
                    House house = JsonConvert.DeserializeObject<House>(houseTemp);
                    house.rotation = street.rotation;
                    house.street = street;
                    houses.Add(house);

                }

                return houses;
            }

            catch (Neo4jException ex)
            {
                Console.WriteLine($"{query} - {ex}");
                throw;
            }
            finally
            {
                Console.WriteLine("got houses");
                await session.CloseAsync();
            }

        }

        public async Task<List<Building>> GetBuildingsOnStreet(Street street)
        {
            var session = driver.AsyncSession();
            List<Building> buildings = new List<Building>();

            string query = $"MATCH (s:Street)-[:CONTAINS]-(b:Building) WHERE s.id='{street.id}' RETURN b";

            try
            {
                var readResults = await session.ReadTransactionAsync(async tx =>
                {
                    var result = await tx.RunAsync(query);
                    return (await result.ToListAsync());
                });

                foreach (var result in readResults)
                {

                    var buildingTemp = JsonConvert.SerializeObject(result[0].As<INode>().Properties);
                    Building buildingUncast = JsonConvert.DeserializeObject<Building>(buildingTemp);
                    Building building;

                    switch (buildingUncast.buildingClass)
                    {
                        
                        default:
                            building = buildingUncast;
                            break;


                    }

                    building.rotation = street.rotation;
                    building.street = street;
                    buildings.Add(building);

                }

                return buildings;
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

        public async Task<List<Room>> GetRoomsInHouse(House house)
        {
            var session = driver.AsyncSession();
            List<Room> rooms = new List<Room>();

            string query = $"MATCH (h:House)-[:CONTAINS]-(r:Room) WHERE h.id='{house.id}' RETURN r";

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


            string query = $"MATCH(r: Room{{class:'{room.roomClass}'}})-[:CONTAINS]-(i:Item) WHERE r.id='{room.id}' RETURN i";

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
                        case "Shower":
                            items.Add(JsonConvert.DeserializeObject<Shower>(itemTemp));
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
