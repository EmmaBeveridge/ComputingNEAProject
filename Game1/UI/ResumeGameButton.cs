﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game1.DataClasses;
using Game1.Skills;
using Game1.Traits;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game1.UI
{
    class ResumeGameButton:Button
    {
        public ResumeGameButton(string argLabel, Vector2 argPosition, Texture2D argTexture) : base(argLabel, argPosition, argTexture)
        {


        }



        /// <summary>
        ///  Creates a new SQLiteDBHandler object and uses to load in game data from SQLite database. Receives list of DBPerson objects from SQliteDBHandler GetPeople method. By iterating over these DBPeople, uses handler to get traits, skills, and relationships from database to create People and player objects. 
        /// </summary>
        /// <param name="game"></param>
        public static void ResumeGame(Game1 game)
        {

            SQLiteDBHandler handler = new SQLiteDBHandler();



            List<DBPerson> dbPeople = handler.GetPeople();
            List<People> people = new List<People>();

            foreach (DBPerson dbPerson in dbPeople) //builds people/player objects
            {
                Model model = game.modelDictionary[dbPerson.ModelName];
                Texture2D icon = game.iconDictionary[dbPerson.ModelName];
                Vector3 position = dbPerson.House.TownLocation;


                List<Trait> traits = handler.GetTraits(dbPerson);
                List<Skill> skills = handler.GetSkills(dbPerson);
                Dictionary<NeedNames, Need> needs = handler.GetNeeds(dbPerson, traits);



                if (dbPerson.IsPlayer)
                {

                    Player player = new Player(model, position, Town.Town.navMesh, game.towns[0], game, icon, dbPerson.DBID, dbPerson.Name, dbPerson.House, dbPerson.Career, traits, needs, skills);
                    game.player = player;
                    people.Add(player);
                    
                }

                else
                {
                    People person = new People(model, position, Town.Town.navMesh, game.towns[0], game, icon, dbPerson.DBID, dbPerson.Name, dbPerson.House, dbPerson.Career, traits, needs, skills, false);
                    people.Add(person);
                }


            }

            foreach (People person1 in people)
            {

               person1.Relationships = handler.GetRelationships(person1, people);
                



            }


            game.people = people;







        }





    }
}
