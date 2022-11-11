using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game1.Actions;
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

            SleepAction sleep = new SleepAction(this);
            GOAPAction sleepGOAP = sleep.DefineGOAPAction();
            GOAPActions.Add(sleepGOAP);
            actionLabels.Add("sleep", sleepGOAP);

            NapAction nap = new NapAction(this);
            GOAPAction napGOAP = nap.DefineGOAPAction();
            GOAPActions.Add(napGOAP);
            actionLabels.Add("nap", napGOAP);

        }


        //public override Action<GameTime> BeginAction(string actionName, People person)
        //{
        //    actionComplete = false;

        //    interactingWithPerson = person;
        //    Action<GameTime> currentAction = null;
        //    switch (actionName)
        //    {
        //        case "sleep":
        //            currentAction = Sleep;
                   
        //            break;

        //        case "nap":
        //            currentAction = Nap;
                    
        //            break;


        //        default:
        //            break;
        //    }

        //    return currentAction;

        //}







        //public void Sleep(GameTime timer) 
        //{
        //    const float minActionTime = 20;
            
        //    actionTimeElapsed += timer.ElapsedGameTime.TotalSeconds;

        //    if (actionTimeElapsed >= actionTime)
        //    {
        //        actionComplete = true;
        //        actionTimeElapsed = 0;
        //        return;
        //    }

        //    Console.WriteLine("sleeping");
        //    interactingWithPerson.goapPerson.goapPersonState.personState.Sleep++;
        
        //}

        //public void Nap(GameTime timer) 
        //{ 
        //    const float actionTime = 3;
        //    actionTimeElapsed += timer.ElapsedGameTime.TotalSeconds;

        //    if (actionTimeElapsed >= actionTime)
        //    {
        //        actionComplete = true;
        //        actionTimeElapsed = 0;
        //        return;
        //    }


        //    Console.WriteLine("napping");
        //    interactingWithPerson.goapPerson.goapPersonState.personState.Sleep++;
        //}




    }
}
