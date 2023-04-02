using Game1.GOAP;
using Game1.Skills;
using Game1.Town;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1
{
    public abstract class ActionAbstract 
    {

        public string Name;
        public Item Item = null;
        public People PersonToInteractWith = null;
        public Building Building = null;
        public Action<GameTime, Dictionary<NeedNames, Need>, People> ActionMethod;
        protected float minActionTime;
        protected float rateOfNeedIncrease;
        protected double actionTimeElapsed;
        public GOAPAction GOAPAction;
        public NeedNames NeedAffected;
        public SkillTypes SkillAffected = SkillTypes.Null;
        public bool ActionComplete;
        public float EstTimeToFinish;
        public float Duration;


        /// <summary>
        /// Person who selected the action via GOAP planner
        /// </summary>
        public People initiator;
        /// <summary>
        /// stores if action initiator has reached persontointeractwith location so conversation can begin
        /// </summary>
        public bool initiatorReachedGoal = false;



        /// <summary>
        ///   Virtual method. Base method has no method body. Overridden in TalkToPersonAction class. 
        /// </summary>
        public virtual void NotifyInitiatorReachedGoal()
        {

        }

        /// <summary>
        /// Returns an estimated cost (time) for the action. First determines the estimated amount of time needed to fulfil the need by dividing the difference between the current need level and the maximum need level by the rate of need increase for that action. This is then compared to the minimum action time, the larger of these two values being assigned to Duration and EstTimeToFinish variables and returned.
        /// </summary>
        /// <param name="need"></param>
        /// <returns></returns>
        public virtual float Cost(Need need)
        {
           
            float estTimeToFulfil = (need.MaxNeed - need.CurrentNeed) / rateOfNeedIncrease;

            

            Duration=  Math.Max(estTimeToFulfil, minActionTime);
            EstTimeToFinish = Duration;
            return Duration;


        }

        /// <summary>
        /// Virtual method. Called once when person begins doing the action. Resets actionComplete and actionTimeElapsed variables and sets item availability to false. If the action affects a skill and the person does not currently possess this skill, a new Skill object of the affected type is added to person.Skills using Skill.GetNewSkill static method. 
        /// </summary>
        /// <param name="person"></param>
        public virtual void BeginAction(People person)
        {
            ActionComplete = false;
            actionTimeElapsed = 0;
            Item.IsAvailable = false;

            if (SkillAffected!= SkillTypes.Null)
            {
                if (!person.Skills.Any(s => s.SkillType == SkillAffected))
                {
                    person.Skills.Add(Skill.GetNewSkill(SkillAffected));
                }


            }


        }


        /// <summary>
        /// Virtual method. Called once when action is finished. Sets ActionComplete and item availability to true. 
        /// </summary>
        public virtual void CompleteAction()
        {
            ActionComplete = true;
            Item.IsAvailable = true;
        }


        /// <summary>
        /// Abstract method implemented in child classes. Creates and returns a new GOAPAction with pre/post- conditions set and building/item/interactionPerson attributes assigned. 
        /// </summary>
        /// <returns>GOAPAction for action</returns>
        public abstract GOAPAction DefineGOAPAction();




    }
}
