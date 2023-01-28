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


        public GOAPActionWithPerson(ActionAbstract _action)
        {


            Action = _action;
            interactionPerson = Action.PersonToInteractWith;
            Name = _action.Name;



        }

        public GOAPActionWithPerson(ActionAbstract _action, int cost) : this(_action)
        {
            this.Cost = cost;
        }




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






        public override void UpdateCost(People person, Dictionary<NeedNames, Need> needs)
        {
            //Cost = (float)Math.Sqrt(Math.Pow(MathHelper.Distance(person.position.X, item.townLocation.X), 2) + Math.Pow(MathHelper.Distance(person.position.Z, item.townLocation.Z), 2));
            Cost = (interactionPerson.position - person.position).Length();
            var selectedNeed = from need in needs
                               where need.Key == Action.NeedAffected
                               select need.Value;

            Cost += Action.Cost(selectedNeed.FirstOrDefault<Need>());


            Cost/= person.Relationships.ContainsKey(interactionPerson)? person.Relationships[interactionPerson]: 50;


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
            


        }
    }
}
