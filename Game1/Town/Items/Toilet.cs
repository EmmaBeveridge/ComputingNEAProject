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
    class Toilet : Item
    {


       
        public Toilet(string argId, int[] argLocation, string argRoomClass) : base(argId, argLocation, argRoomClass)
        {
            modelName = "Toilet";
            DefineActions();
            
        }

        public override void DefineActions()
        {

            ToiletAction toilet = new ToiletAction(this);
            GOAPAction toiletGOAP = toilet.DefineGOAPAction();
            GOAPActions.Add(toiletGOAP);
            actionLabels.Add("use toilet", toiletGOAP);

        }



        //public override Action<GameTime> BeginAction(string actionName, People person)
        //{
        //    interactingWithPerson = person;
        //    Action<GameTime> currentAction = null;
        //    switch (actionName)
        //    {
        //        case "useToilet":
        //            currentAction = UseToilet;
        //            break;

        //        default:
        //            break;
        //    }
        //    return currentAction;
        //}


        //public void UseToilet(GameTime timer)
        //{
        //    const float actionTime = 2;
        //    actionTimeElapsed += timer.ElapsedGameTime.TotalSeconds;

        //    if (actionTimeElapsed >= actionTime)
        //    {
        //        actionComplete = true;
        //        actionTimeElapsed = 0;
        //        Console.WriteLine("finished with toilet");
        //        return;
        //    }

        //    Console.WriteLine("using toilet");
        //    interactingWithPerson.goapPerson.goapPersonState.personState.Toilet++;

           
        //}


    }
}