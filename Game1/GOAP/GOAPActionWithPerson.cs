using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.GOAP
{
    class GOAPActionWithPerson : GOAPAction
    {

        public GOAPActionWithPerson()
        { }

        /// <summary>
        /// Constructor for new GOAPActionWithPerson object.
        /// </summary>
        /// <param name="_action"></param>
        public GOAPActionWithPerson(ActionAbstract _action)
        {


            Action = _action;
            interactionPerson = Action.PersonToInteractWith;
            Name = _action.Name;



        }

        /// <summary>
        /// Constructor for new GOAPActionWithPerson object.
        /// </summary>
        /// <param name="_action"></param>
        /// <param name="cost"></param>
        public GOAPActionWithPerson(ActionAbstract _action, int cost) : this(_action)
        {
            this.Cost = cost;
        }



        /// <summary>
        ///  Overrides method in parent class. Action not valid, returns false, if the interactionPerson is the same as the initiator (prevents person choosing to talk to themselves) or if the interactionPerson has already decided to talk to the initiator. If not retruned false, base Validate method called and returned. 
        /// </summary>
        /// <param name="person"></param>
        /// <returns></returns>
        public override bool Validate(People person)
        {
            if (person == interactionPerson)
            {
                return false;

            }

            People interactionPersonTalkingTo = interactionPerson.town.GOAPActions.Find(a => a.interactionPerson == person).Action.initiator; //something like this then check if thats interactionPerson

            if (interactionPerson == interactionPersonTalkingTo) //if interactionPerson already decided to talk to person
            {
                return false;
            }
            return base.Validate(person);
        }





        /// <summary>
        ///  Overrides method in parent class. Estimated cost of completing the action is calculated as the sum of the Euclidean distance from the person to the interactionPerson, the estimated cost (time) to fulfil the need affected by the value, and the estimated time taken for person to reach front of queue for action by  multiplying the duration of the action by the number of the rest of the people in the queue. As the person should prioritise interactions with people with whom they have a good relationship; the cost is then divided by the score of the relationship (or default relationship value if no relationship exists). This results in higher scoring relationships having a lower estimated cost and therefore are preferred by the planner. 
        /// </summary>
        /// <param name="person"></param>
        /// <param name="needs"></param>
        public override void UpdateCost(People person, Dictionary<NeedNames, Need> needs)
        {
            //Cost = (float)Math.Sqrt(Math.Pow(MathHelper.Distance(person.position.X, item.townLocation.X), 2) + Math.Pow(MathHelper.Distance(person.position.Z, item.townLocation.Z), 2));
            Cost = (interactionPerson.position - person.position).Length();
            var selectedNeed = from need in needs
                               where need.Key == Action.NeedAffected
                               select need.Value;

            Cost += Action.Cost(selectedNeed.FirstOrDefault<Need>());


            

            //if (doingAction.FirstOrDefault() != person)
            //{
            //    Cost += Action.EstTimeToFinish + Action.Duration * (doingAction.Count - 1);
            //} ALREADY IN PERSON ACTION STACK IF OTHER PEOPLE GOING TO TALK TO PERSON???
            if (interactionPerson.goapPerson.goapPersonState != null  && interactionPerson.goapPerson.goapPersonState.actionPlan!=null)
            {
                foreach (GOAPAction action in interactionPerson.goapPerson.goapPersonState.actionPlan)
                {


                    Cost += action.Action.Duration;

                }

            }

            Cost/= person.Relationships.ContainsKey(interactionPerson)? person.Relationships[interactionPerson]: 50;



        }
    }
}
