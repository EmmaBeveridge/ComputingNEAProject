using Game1.GOAP;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.Actions
{
    class TakeShowerAction : ActionAbstract
    {
        public TakeShowerAction(Item _item)
        {
            Name = "take shower";
            Item = _item;
            minActionTime = 7;
            rateOfNeedIncrease = 20f;
            ActionMethod = TakeShower;
            NeedAffected = NeedNames.Hygiene;


        }

        public override GOAPAction DefineGOAPAction()
        {
            GOAPAction = new GOAPAction(this);
            GOAPAction.SetPrecondition(GOAPPerson.LowHygiene, true);

            GOAPAction.SetPostcondition(GOAPPerson.LowHygiene, false);
            GOAPAction.item = this.Item;
            return GOAPAction;
        }

        public void TakeShower(GameTime timer, Dictionary<NeedNames, Need> needs)
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
