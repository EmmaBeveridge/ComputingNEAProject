using Game1.GOAP;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.Actions
{
    internal class ToiletAction:ActionAbstract
    {
        public ToiletAction(Item _item)
        {
            Name = "toilet";
            Item = _item;
            minActionTime = 2;
            rateOfNeedIncrease = 40f;
            ActionMethod = Toilet;
            NeedAffected = NeedNames.Toilet;



        }

        public override GOAPAction DefineGOAPAction()
        {
            GOAPAction = new GOAPActionWithItem(this);
            GOAPAction.SetPrecondition(GOAPPerson.LowToilet, true);
            
            GOAPAction.SetPostcondition(GOAPPerson.LowToilet, false);
            GOAPAction.item = this.Item;
            return GOAPAction;
        }

        public void Toilet(GameTime gameTime, Dictionary<NeedNames, Need> needs, People person = null)
        {

            needs[NeedAffected].Update(rate: rateOfNeedIncrease, gameTime: gameTime);
            actionTimeElapsed += gameTime.ElapsedGameTime.TotalSeconds;
            EstTimeToFinish = Duration - (float)actionTimeElapsed;


            Console.WriteLine("using toilet");

            if (needs[NeedAffected].IsFulfilled() && actionTimeElapsed > minActionTime)
            {
                CompleteAction();
            }




        }








    }
}
