using Game1.GOAP;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.Actions
{
    internal class SleepAction: ActionAbstract
    {

        public SleepAction(Item _item)
        {
            Name = "sleep";
            Item = _item;
            minActionTime = 15;
            rateOfNeedIncrease = 15f;
            ActionMethod = Sleep;
            NeedAffected = NeedNames.Sleep;


        }

        public override GOAPAction DefineGOAPAction()
        {
            GOAPAction = new GOAPAction(this);
            GOAPAction.SetPrecondition(GOAPPerson.LowSleep, true);
            
            GOAPAction.SetPostcondition(GOAPPerson.LowSleep, false);
            GOAPAction.item = this.Item;
            return GOAPAction;
        }

        public void Sleep(GameTime timer, Dictionary<NeedNames, Need> needs)
        {
           
            needs[NeedAffected].Update(rate: rateOfNeedIncrease, gameTime: timer);
            actionTimeElapsed += timer.ElapsedGameTime.TotalSeconds;
            EstTimeToFinish = Duration - (float)actionTimeElapsed;

            Console.WriteLine("sleeping");

            if (needs[NeedAffected].IsFulfilled() && actionTimeElapsed > minActionTime)
            {
                CompleteAction();
            }





            
            
            
            
            //actionTimeElapsed += timer.ElapsedGameTime.TotalSeconds;

            //if (actionTimeElapsed >= actionTime)
            //{
            //    actionComplete = true;
            //    actionTimeElapsed = 0;
            //    return;
            //}

            //Console.WriteLine("sleeping");
            //interactingWithPerson.goapPerson.goapPersonState.personState.Sleep++;

        }




    }
}
