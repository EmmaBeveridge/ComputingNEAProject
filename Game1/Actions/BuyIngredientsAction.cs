using Game1.GOAP;
using Game1.Town;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.Actions
{

    /// <summary>
    /// Inherits from ActionAbstract to implement action to buy groceries 
    /// </summary>
    class BuyGroceriesAction : ActionAbstract
    {
        public static string HasGroceriesConditionString = "has groceries";

        public BuyGroceriesAction(Building _building)
        {
            Name = "buy groceries";
            Building = _building;
            minActionTime = 5;
            rateOfNeedIncrease = 1f;
            ActionMethod = BuyGroceries;


        }


        /// <summary>
        /// Overrides virtual method in parent class. Creates and returns new GOAPAction with BuyGroceriesAction instance as parameter. Sets postcodition identified by HasGroceriesConditionString to true, sets GOAPAction.building to instance’s building. 
        /// </summary>
        /// <returns></returns>
        public override GOAPAction DefineGOAPAction()
        {
            GOAPAction = new GOAPActionWithBuilding(this);
            GOAPAction.SetPostcondition(HasGroceriesConditionString, true);
            GOAPAction.building = this.Building;
            return GOAPAction;

        }


        /// <summary>
        /// Overrides virtual method in parent class. Resets actionComplete and actionTimeElapsed variables.
        /// </summary>
        /// <param name="person"></param>
        public override void BeginAction(People person)
        {
            ActionComplete = false;
            actionTimeElapsed = 0;
        }

        /// <summary>
        /// Overrides virtual method in parent class. Sets ActionComplete to true. 
        /// </summary>
        public override void CompleteAction()
        {
            ActionComplete = true;
        }


        /// <summary>
        /// Called each update frame for which action is ongoing. Simulates buying groceries action. Increments actionTimeElapsed and reduces EstTimeToFinish. If the elapsed action time is greater than the minimum action time, CompleteAction method is called. 
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="needs"></param>
        /// <param name="person"></param>
        public void BuyGroceries(GameTime gameTime, Dictionary<NeedNames, Need> needs, People person = null)
        {


            actionTimeElapsed += gameTime.ElapsedGameTime.TotalSeconds;
            EstTimeToFinish = Duration - (float)actionTimeElapsed;

            Console.WriteLine("buying groceries");

            if (actionTimeElapsed > minActionTime)
            {
                CompleteAction();
            }

        }


    }
}
