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
    /// Inherits from ActionAbstract to implement action to read book 
    /// </summary>
    class ReadBookAction : ActionAbstract
    {

        /// <summary>
        /// Constructor for new ReadBookAction object. Sets action name, item, minActionTime, rateOfNeedIncrease, NeedAffected (Fun), and ActionMethod (set to ReadBook method) 
        /// </summary>
        /// <param name="_item"></param>
        public ReadBookAction(Item _item)
        {
            Name = "read book";
            Item = _item;
            minActionTime = 5;
            rateOfNeedIncrease = 20f;
            ActionMethod = ReadBook;
            NeedAffected = NeedNames.Fun;


        }


        /// <summary>
        /// Overrides virtual method in parent class. Creates and returns new GOAPAction with ReadBookAction instance as parameter. Sets preconditions of lowFun as true and sets postconditions of lowFun as false. Sets GOAPAction.item to instance’s item.
        /// </summary>
        /// <returns></returns>
        public override GOAPAction DefineGOAPAction()
        {
            GOAPAction = new GOAPActionWithItem(this);
            GOAPAction.SetPrecondition(GOAPPerson.LowFun, true);

            GOAPAction.SetPostcondition(GOAPPerson.LowFun, false);
            GOAPAction.item = this.Item;
            return GOAPAction;
        }


        /// <summary>
        /// Called each update frame for which action is ongoing. Simulates reading book action. Increments actionTimeElapsed and reduces EstTimeToFinish. Calls Update on affected need. If the elapsed action time is greater than the minimum action time and the affected need is fulfilled, CompleteAction method is called. 
        /// </summary>
        /// <param name="timer"></param>
        /// <param name="needs"></param>
        /// <param name="person"></param>
        public void ReadBook(GameTime timer, Dictionary<NeedNames, Need> needs, People person = null)
        {

            needs[NeedAffected].Update(rate: rateOfNeedIncrease, gameTime: timer);
            actionTimeElapsed += timer.ElapsedGameTime.TotalSeconds;
            EstTimeToFinish = Duration - (float)actionTimeElapsed;

            Console.WriteLine("reading book");

            if (needs[NeedAffected].IsFulfilled() && actionTimeElapsed > minActionTime)
            {
                CompleteAction();
            }






        }


    }
}
