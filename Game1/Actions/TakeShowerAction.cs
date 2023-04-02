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
    /// Inherits from ActionAbstract to implement action to take a shower 
    /// </summary>
    class TakeShowerAction : ActionAbstract
    {

        /// <summary>
        /// Constructor for new TakeShowerAction object. Sets action name, item, minActionTime, rateOfNeedIncrease, NeedAffected (Hygiene), and ActionMethod (set to TakeShower method
        /// </summary>
        /// <param name="_item"></param>
        public TakeShowerAction(Item _item)
        {
            Name = "take shower";
            Item = _item;
            minActionTime = 7;
            rateOfNeedIncrease = 20f;
            ActionMethod = TakeShower;
            NeedAffected = NeedNames.Hygiene;


        }


        /// <summary>
        ///  Overrides virtual method in parent class. Creates and returns new GOAPAction with TakesShowerAction instance as parameter. Sets preconditions of lowHygiene as true and sets postconditions of lowHygiene as false. Sets GOAPAction.item to instance’s item. 
        /// </summary>
        /// <returns></returns>
        public override GOAPAction DefineGOAPAction()
        {
            GOAPAction = new GOAPActionWithItem(this);
            GOAPAction.SetPrecondition(GOAPPerson.LowHygiene, true);

            GOAPAction.SetPostcondition(GOAPPerson.LowHygiene, false);
            GOAPAction.item = this.Item;
            return GOAPAction;
        }



        /// <summary>
        /// Called each update frame for which action is ongoing. Simulates taking shower action. Increments actionTimeElapsed and reduces EstTimeToFinish. Calls Update on affected need. If the elapsed action time is greater than the minimum action time and the affected need is fulfilled, CompleteAction method is called. 
        /// </summary>
        /// <param name="timer"></param>
        /// <param name="needs"></param>
        /// <param name="person"></param>
        public void TakeShower(GameTime timer, Dictionary<NeedNames, Need> needs, People person = null)
        {

            needs[NeedAffected].Update(rate: rateOfNeedIncrease, gameTime: timer);
            actionTimeElapsed += timer.ElapsedGameTime.TotalSeconds;
            EstTimeToFinish = Duration - (float)actionTimeElapsed;

            Console.WriteLine("taking shower");

            if (needs[NeedAffected].IsFulfilled() && actionTimeElapsed > minActionTime)
            {
                CompleteAction();
            }

        }


     }

}
