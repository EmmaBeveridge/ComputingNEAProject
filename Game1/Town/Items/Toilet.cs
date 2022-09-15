using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game1.GOAP;
using Microsoft.Xna.Framework;

namespace Game1.Items
{
    class Toilet : Item
    {


       
        public Toilet(string argId, int[] argLocation, string argRoomClass) : base(argId, argLocation, argRoomClass)
        {
            modelName = "Toilet";
            DefineActions();
            
        }

        public override void DefineActions()
        {

            var useToilet = new GOAPAction("useToilet");
            useToilet.SetPrecondition(GOAPPerson.NeedsToilet, true);
            useToilet.SetPostcondition(GOAPPerson.NeedsToilet, false);
            useToilet.item = this;
            GOAPActions.Add(useToilet);
            actionLabels.Add("use toilet", useToilet);



        }



        public override Action<GameTime> BeginAction(string actionName, People person)
        {
            interactingWithPerson = person;
            Action<GameTime> currentAction = null;
            switch (actionName)
            {
                case "useToilet":
                    currentAction = useToilet;
                    break;

                default:
                    break;
            }
            return currentAction;
        }


        public void useToilet(GameTime timer)
        {
            const float actionTime = 2;
            actionTimeElapsed += timer.ElapsedGameTime.TotalSeconds;

            if (actionTimeElapsed >= actionTime)
            {
                actionComplete = true;
                actionTimeElapsed = 0;
                Console.WriteLine("finished with toilet");
                return;
            }

            Console.WriteLine("using toilet");
            interactingWithPerson.goapPerson.goapPersonState.personState.Toilet++;

           
        }


    }
}