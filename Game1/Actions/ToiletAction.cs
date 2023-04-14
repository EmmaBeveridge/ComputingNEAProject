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
    /// Inherits from ActionAbstract to implement action to use toilet 
    /// </summary>
    internal class ToiletAction:ActionAbstract
    {


        /// <summary>
        /// Constructor for new ToiletAction object. Sets action name, item, minActionTime, rateOfNeedIncrease, NeedAffected (Toilet), and ActionMethod (set to Toilet method) 
        /// </summary>
        /// <param name="_item"></param>
        public ToiletAction(Item _item)
        {
            Name = "toilet";
            Item = _item;
            minActionTime = 2;
            rateOfNeedIncrease = 40f;
            ActionMethod = Toilet;
            NeedAffected = NeedNames.Toilet;



        }


        /// <summary>
        /// Overrides virtual method in parent class. Creates and returns new GOAPAction with ToiletAction instance as parameter. Sets preconditions of lowToilet as true and sets postconditions of lowToilet as false. Sets GOAPAction.item to instance’s item. 
        /// </summary>
        /// <returns></returns>
        public override GOAPAction DefineGOAPAction()
        {
            GOAPAction = new GOAPActionWithItem(this);
            GOAPAction.SetPrecondition(GOAPPerson.LowToilet, true);
            
            GOAPAction.SetPostcondition(GOAPPerson.LowToilet, false);
            GOAPAction.item = this.Item;
            return GOAPAction;
        }


        /// <summary>
        /// Called each update frame for which action is ongoing. Simulates using toilet action. Increments actionTimeElapsed and reduces EstTimeToFinish. Calls Update on affected need. If the elapsed action time is greater than the minimum action time and the affected need is fulfilled, CompleteAction method is called. 
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="needs"></param>
        /// <param name="person"></param>
        public void Toilet(GameTime gameTime, Dictionary<NeedNames, Need> needs, People person = null)
        {

            needs[NeedAffected].Update(rate: rateOfNeedIncrease, gameTime: gameTime);
            actionTimeElapsed += gameTime.ElapsedGameTime.TotalSeconds;
            EstTimeToFinish = Duration - (float)actionTimeElapsed;


            //££Console.WriteLine("using toilet");

            if (needs[NeedAffected].IsFulfilled() && actionTimeElapsed > minActionTime)
            {
                CompleteAction();
            }




        }








    }
}
