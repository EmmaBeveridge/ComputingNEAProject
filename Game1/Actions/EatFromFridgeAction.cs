using Game1.GOAP;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.Actions
{
    class EatFromFridgeAction : ActionAbstract
    {
        public EatFromFridgeAction(Item _item)
        {
            Name = "eat from fridge";
            Item = _item;
            minActionTime = 5;
            rateOfNeedIncrease = 20f;
            ActionMethod = EatFromFridge;
            NeedAffected = NeedNames.Hunger;


        }

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

        public void EatFromFridge(GameTime timer, Dictionary<NeedNames, Need> needs, People person = null)
        {

            needs[NeedAffected].Update(rate: rateOfNeedIncrease, gameTime: timer);
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
