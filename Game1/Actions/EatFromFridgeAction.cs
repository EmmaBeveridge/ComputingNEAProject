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
    /// Inherits from ActionAbstract to implement action to eat from fridge 
    /// </summary>
    class EatFromFridgeAction : ActionAbstract
    {
        /// <summary>
        /// Constructor for new EatFromFridgeAction object. Sets action name, item, minActionTime, rateOfNeedIncrease, NeedAffected, SkillAffected, and ActionMethod (set to EatFromFridge method)
        /// </summary>
        /// <param name="_item"></param>
        public EatFromFridgeAction(Item _item)
        {
            Name = "eat from fridge";
            Item = _item;
            minActionTime = 5;
            rateOfNeedIncrease = 20f;
            ActionMethod = EatFromFridge;
            NeedAffected = NeedNames.Hunger;
            SkillAffected = Skills.SkillTypes.Cooking;


        }

        /// <summary>
        /// Overrides virtual method in parent class. Creates and returns new GOAPAction with EatFromFridgeAction instance as parameter. Sets preconditions of low hunger and has groceries and sets postconditions of lowHunger and has groceries as false. Sets GOAPAction.item to instance’s item. 
        /// </summary>
        /// <returns></returns>
        public override GOAPAction DefineGOAPAction()
        {
            GOAPAction = new GOAPActionWithItem(this);
            GOAPAction.SetPrecondition(GOAPPerson.LowHunger, true);
            GOAPAction.SetPrecondition(BuyGroceriesAction.HasGroceriesConditionString, true);

            GOAPAction.SetPostcondition(GOAPPerson.LowHunger, false);
            GOAPAction.SetPostcondition(BuyGroceriesAction.HasGroceriesConditionString, false);

            GOAPAction.item = this.Item;
            return GOAPAction;
        }

        /// <summary>
        /// Called each update frame for which action is ongoing. Simulates eating from fridge action. Increments actionTimeElapsed and reduces EstTimeToFinish. Calls Update on affected need, and UpdateSkill method on affected skill. If the elapsed action time is greater than the minimum action time and the affected need is fulfilled, CompleteAction method is called. 
        /// </summary>
        /// <param name="timer"></param>
        /// <param name="needs"></param>
        /// <param name="person"></param>
        public void EatFromFridge(GameTime timer, Dictionary<NeedNames, Need> needs, People person = null)
        {

            needs[NeedAffected].Update(rate: rateOfNeedIncrease, gameTime: timer);
            person.Skills.Find(s => s.SkillType == SkillAffected).UpdateSkill(timer);
            actionTimeElapsed += timer.ElapsedGameTime.TotalSeconds;
            EstTimeToFinish = Duration - (float)actionTimeElapsed;

            Console.WriteLine("eating from fridge");

            if (needs[NeedAffected].IsFulfilled() && actionTimeElapsed > minActionTime)
            {
                CompleteAction();
            }








        }
    }
}
