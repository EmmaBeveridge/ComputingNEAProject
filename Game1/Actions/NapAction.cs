using Game1.GOAP;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.Actions
{
    internal class NapAction:ActionAbstract
    {


        public NapAction(Item _item)
        {
            Name = "nap";
            Item = _item;
            minActionTime = 15;
            rateOfNeedIncrease = 1f;
            ActionMethod = Nap;
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


        public void Nap(GameTime gameTime, Dictionary<NeedNames, Need> needs)
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
