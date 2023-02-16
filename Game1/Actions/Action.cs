using Game1.GOAP;
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
        /// Updates variable for non-initiators action object so they know initiator has reached them
        /// </summary>
        public virtual void NotifyInitiatorReachedGoal()
        {

        }

        public virtual float Cost(Need need)
        {
           
            float estTimeToFulfil = (need.MaxNeed - need.CurrentNeed) / rateOfNeedIncrease;

            

            Duration=  Math.Max(estTimeToFulfil, minActionTime);
            EstTimeToFinish = Duration;
            return Duration;


        }


        public virtual void BeginAction()
        {
            ActionComplete = false;
            actionTimeElapsed = 0;
            Item.IsAvailable = false;
        }



        public virtual void CompleteAction()
        {
            ActionComplete = true;
            Item.IsAvailable = true;
        }


        public abstract GOAPAction DefineGOAPAction();




    }
}
