using Game1.GOAP;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.Actions
{
    class ReadBookAction : ActionAbstract
    {

        public ReadBookAction(Item _item)
        {
            Name = "read book";
            Item = _item;
            minActionTime = 5;
            rateOfNeedIncrease = 20f;
            ActionMethod = ReadBook;
            NeedAffected = NeedNames.Fun;


        }


        public override GOAPAction DefineGOAPAction()
        {
            GOAPAction = new GOAPActionWithItem(this);
            GOAPAction.SetPrecondition(GOAPPerson.LowFun, true);

            GOAPAction.SetPostcondition(GOAPPerson.LowFun, false);
            GOAPAction.item = this.Item;
            return GOAPAction;
        }

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
