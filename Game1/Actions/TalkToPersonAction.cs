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
    /// Inherits from ActionAbstract to implement action to talk to another person. Each person gets their own talk to person (i.e talk to them) interaction. Each person assigns themself as PersonToInteractWith for their TalkToPersonAction- someone else can then pick the action which causes the someone else to start talking to the person. 
    /// </summary>
    class TalkToPersonAction : ActionAbstract
    {
        
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



        /// <summary>
        ///  Overrides base class method. Sets initiatorReachedGoal attribute to true on non-initiator's ActionAbstract object to inform them that the initiator has reached them. 
        /// </summary>
        public override void NotifyInitiatorReachedGoal()
        {
            ActionAbstract actionToUpdate = this.PersonToInteractWith.town.GOAPActions.Find(a => a.Action.PersonToInteractWith == this.initiator).Action;

            actionToUpdate.initiatorReachedGoal = true;



        }

        /// <summary>
        /// Overrides virtual method in parent class. Creates and returns new GOAPActionWithPerson with instance as parameter. Sets preconditions of lowSocial as true and sets postconditions of lowSocial as false. Sets GOAPAction.interactionPerson to instance’s PersonToInteractWith. 
        /// </summary>
        /// <returns></returns>
        public override GOAPAction DefineGOAPAction()
        {
            GOAPAction = new GOAPActionWithPerson(this);
            GOAPAction.SetPrecondition(GOAPPerson.LowSocial, true);

            GOAPAction.SetPostcondition(GOAPPerson.LowSocial, false);
            //GOAPAction.item = this.Item;
            GOAPAction.interactionPerson = PersonToInteractWith;

            return GOAPAction;
        }




        /// <summary>
        /// Called each update frame for which action is ongoing. Simulates autonomous interaction between characters. Increments actionTimeElapsed and reduces EstTimeToFinish. Calls Update on affected need. Calls UpdateRelationsAutonomous method on person interacting with PersonToInteractWith, sending PersonToInteractWith as an argument. If the elapsed action time is greater than the minimum action time and the affected need is fulfilled, CompleteAction method is called. 
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


        /// <summary>
        /// Overrides virtual method in parent class. Resets ActionComplete and actionTimeElapsed variables. Sets PersonToInteractWith’s availability to false.
        /// </summary>
        /// <param name="person"></param>
        public override void BeginAction(People person)
        {
            ActionComplete = false;
            actionTimeElapsed = 0;
            PersonToInteractWith.IsAvailable = false;




        }

        /// <summary>
        /// Overrides virtual method in parent class. Ends and resets action: sets ActionComplete to true, PersonToInteractWith as being available, sets initiatorReachedGoal to false, and initiator to null. 
        /// </summary>
        public override void CompleteAction()
        {
            ActionComplete = true;
            PersonToInteractWith.IsAvailable = true;
            initiatorReachedGoal = false;
            initiator = null;
        }



    }
}