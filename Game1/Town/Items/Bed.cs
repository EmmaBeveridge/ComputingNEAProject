using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game1.GOAP;
using Microsoft.Xna.Framework;

namespace Game1.Items
{
    class Bed : Item
    {
        
        public Bed(string argId, int[] argLocation, string argRoomClass) : base(argId, argLocation, argRoomClass)
        {
            modelName = "Bed";

            DefineActions();

            


            

        }

        public override void DefineActions()
        {
            
            var sleep = new GOAPAction("sleep");
            sleep.SetPrecondition(GOAPPerson.IsTired, true);
            sleep.SetPrecondition("Available", true);
            sleep.SetPostcondition(GOAPPerson.IsTired, false);
            sleep.item = this;
            GOAPActions.Add(sleep);
            actionLabels.Add("sleep", sleep);
            

            var nap = new GOAPAction("nap");
            nap.SetPrecondition(GOAPPerson.IsTired, true);
            nap.SetPostcondition(GOAPPerson.IsTired, false); 
            nap.item = this;
            GOAPActions.Add(nap);
            actionLabels.Add("nap", nap);
        }


        public override Action<GameTime> BeginAction(string actionName, People person)
        {
            actionComplete = false;

            interactingWithPerson = person;
            Action<GameTime> currentAction = null;
            switch (actionName)
            {
                case "sleep":
                    currentAction = sleep;
                   
                    break;

                case "nap":
                    currentAction = nap;
                    
                    break;


                default:
                    break;
            }

            return currentAction;

        }

        public void sleep(GameTime timer) 
        {
            const float actionTime = 10;
            
            actionTimeElapsed += timer.ElapsedGameTime.TotalSeconds;

            if (actionTimeElapsed >= actionTime)
            {
                actionComplete = true;
                actionTimeElapsed = 0;
                return;
            }

            Console.WriteLine("sleeping");
            interactingWithPerson.goapPerson.goapPersonState.personState.Sleep++;
        
        }

        public void nap(GameTime timer) 
        { 
            const float actionTime = 3;
            actionTimeElapsed += timer.ElapsedGameTime.TotalSeconds;

            if (actionTimeElapsed >= actionTime)
            {
                actionComplete = true;
                actionTimeElapsed = 0;
                return;
            }


            Console.WriteLine("napping");
            interactingWithPerson.goapPerson.goapPersonState.personState.Sleep++;
        }




    }
}
