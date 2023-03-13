using Game1.GOAP;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.Actions
{
    class TalkToPersonAction : ActionAbstract
    {
        //each person gets their own talk to person (i.e talk to them) interaction i.e. each person assigns themself as PersonToInteractWith for their TalkToPersonAction- someone else can then pick the action which causes the someone else to start talking to the person
        //but think should look from perspective of you are other person - need to push action where other person is PersonToInteractWith onto PersonToInteractWith action stack






        public TalkToPersonAction(People _person)
        {
            Name = "talk";
            PersonToInteractWith = _person;
            minActionTime = 4;
            rateOfNeedIncrease = 20f;
            ActionMethod = TalkToPerson;
            NeedAffected = NeedNames.Social;


        }




        public override void NotifyInitiatorReachedGoal()
        {
            ActionAbstract actionToUpdate = this.PersonToInteractWith.town.GOAPActions.Find(a => a.Action.PersonToInteractWith == this.initiator).Action;

            actionToUpdate.initiatorReachedGoal = true;



        }


        public override GOAPAction DefineGOAPAction()
        {
            GOAPAction = new GOAPActionWithPerson(this);
            GOAPAction.SetPrecondition(GOAPPerson.LowSocial, true);

            GOAPAction.SetPostcondition(GOAPPerson.LowSocial, false);
            GOAPAction.item = this.Item;
            return GOAPAction;
        }




        /// <summary>
        /// Simulates autonomous interaction
        /// </summary>
        /// <param name="timer"></param>
        /// <param name="needs">Needs of person interacting with PersonToInteractWith</param>
        /// <param name="person">Person interacting with PersonToInteractWith</param>
        public void TalkToPerson(GameTime timer, Dictionary<NeedNames, Need> needs, People person)
        {

            needs[NeedAffected].Update(rate: rateOfNeedIncrease, gameTime: timer);
            actionTimeElapsed += timer.ElapsedGameTime.TotalSeconds;
            EstTimeToFinish = Duration - (float)actionTimeElapsed;



            person.UpdateRelationsAutonomous(PersonToInteractWith);



            Console.WriteLine("talking");

            if (needs[NeedAffected].IsFulfilled() && actionTimeElapsed > minActionTime)
            {
                CompleteAction();
            }

        }

        public override void BeginAction(People person)
        {
            ActionComplete = false;
            actionTimeElapsed = 0;
            PersonToInteractWith.IsAvailable = false;




        }


        public override void CompleteAction()
        {
            ActionComplete = true;
            PersonToInteractWith.IsAvailable = true;
            initiatorReachedGoal = false;
            initiator = null;
        }



    }
}