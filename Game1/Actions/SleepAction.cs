using Game1.GOAP;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.Actions
{

    /// <summary>
    /// Inherits from ActionAbstract to implement action to sleeping 
    /// </summary>
    internal class SleepAction: ActionAbstract
    {
        /// <summary>
        /// Constructor for new SleepAction object. Sets action name, item, minActionTime, rateOfNeedIncrease, NeedAffected (Sleep), and ActionMethod (set to Sleep method) 
        /// </summary>
        /// <param name="_item"></param>
        public SleepAction(Item _item)
        {
            Name = "sleep";
            Item = _item;
            minActionTime = 15;
            rateOfNeedIncrease = 15f;
            ActionMethod = Sleep;
            NeedAffected = NeedNames.Sleep;


        }



        /// <summary>
        /// Overrides virtual method in parent class. Creates and returns new GOAPAction with SleepAction instance as parameter. Sets preconditions of lowSleep as true and sets postconditions of lowSleep as false. Sets GOAPAction.item to instance’s item. 
        /// </summary>
        /// <returns></returns>
        public override GOAPAction DefineGOAPAction()
        {
            GOAPAction = new GOAPActionWithItem(this);
            GOAPAction.SetPrecondition(GOAPPerson.LowSleep, true);
            
            GOAPAction.SetPostcondition(GOAPPerson.LowSleep, false);
            GOAPAction.item = this.Item;
            return GOAPAction;
        }


        /// <summary>
        ///  Called each update frame for which action is ongoing. Simulates sleeping action. Increments actionTimeElapsed and reduces EstTimeToFinish. Calls Update on affected need. If the elapsed action time is greater than the minimum action time and the affected need is fulfilled, CompleteAction method is called. 
        /// </summary>
        /// <param name="timer"></param>
        /// <param name="needs"></param>
        /// <param name="person"></param>
        public void Sleep(GameTime timer, Dictionary<NeedNames, Need> needs, People person = null)
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
