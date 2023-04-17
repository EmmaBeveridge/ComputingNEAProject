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
    /// Inherits from ActionAbstract to implement action to nap 
    /// </summary>
    internal class NapAction:ActionAbstract
    {

        /// <summary>
        /// Constructor for new NapAction object. Sets action name, item, minActionTime, rateOfNeedIncrease, NeedAffected,  and ActionMethod (set to Nap method) 
        /// </summary>
        /// <param name="_item"></param>
        public NapAction(Item _item)
        {
            Name = "nap";
            Item = _item;
            minActionTime = 15;
            rateOfNeedIncrease = 1f;
            ActionMethod = Nap;
            NeedAffected = NeedNames.Sleep;

        }


        /// <summary>
        /// Overrides virtual method in parent class. Creates and returns new GOAPAction with NapAction instance as parameter. Sets preconditions of lowSleep as true and sets postconditions of lowSleep as false. Sets GOAPAction.item to instance’s item. 
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
        /// Called each update frame for which action is ongoing. Simulates napping action. Increments actionTimeElapsed and reduces EstTimeToFinish. Calls Update on affected need. If the elapsed action time is greater than the minimum action time and the affected need is fulfilled, CompleteAction method is called. 
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="needs"></param>
        /// <param name="person"></param>
        public void Nap(GameTime gameTime, Dictionary<NeedNames, Need> needs, People person = null)
        {

            needs[NeedAffected].Update(rate: rateOfNeedIncrease, gameTime: gameTime);
            actionTimeElapsed += gameTime.ElapsedGameTime.TotalSeconds;
            EstTimeToFinish = Duration - (float)actionTimeElapsed;

            Console.WriteLine("napping");

            if (needs[NeedAffected].IsFulfilled() && actionTimeElapsed > minActionTime)
            {
                CompleteAction();
            }

        }



    }
}
