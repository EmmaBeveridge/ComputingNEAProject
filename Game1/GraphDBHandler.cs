using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Neo4j.Driver;
using Neo4j.Driver.V1;

namespace Game1
{
    class GraphDBHandler: IDisposable
    {
        private IDriver _driver;


        private bool _disposed = false;
        ~GraphDBHandler() => Dispose(false);
        public GraphDBHandler(string uri, string user, string password)
        {
            _driver = GraphDatabase.Driver(uri, AuthTokens.Basic(user, password), Config.Builder.WithEncryptionLevel(EncryptionLevel.None).ToConfig());
        }

        private void RunQuery(string query)
        {
            using (var session = _driver.Session())
            {
                var transactionResult = session.WriteTransaction(tx =>
                {
                    var result = tx.Run(query);

                    if (!result.Any())
                    {

                        return null;

                    }

                    return result.Single()[0].As<string>();
                });
                Console.WriteLine(transactionResult);
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
                _driver?.Dispose();
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






        public void CreateTown()
        {
            string createTownQuery = ReadInQuery("CreateTownCypher.txt");

            RunQuery(createTownQuery);

        }










        //public static void Main()
        //{
        //    using (var greeter = new Neo4jConnect("bolt://localhost:7687/neo4j", "Emma", "password"))
        //    {
        //        greeter.PrintGreeting("hello, world");
        //    }
        //}


















        //public GraphDBHandler(string argURI, string argUserName, string argPassword )
        //{
        //    driver = GraphDatabase.Driver(argURI, AuthTokens.Basic(argUserName, argPassword));

        //}

        //public void Dispose()
        //{
        //    driver.Dispose();
        //}


        //public void CreateTown()
        //{
        //    var session = driver.AsyncSession();
        //    string query = ReadInQuery("CreateTownCypher.txt");



        //    Task task = RunQuery(query);


        //}


        //public async Task RunQuery(string query)
        //{




        //    using (var session = driver.AsyncSession)
        //    {

        //            session.


        //    }

        //}


       


    }
}
