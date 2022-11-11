using Game1.GOAP;
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
        public Item Item;
        public Action<GameTime, Dictionary<NeedNames, Need>> ActionMethod;
        protected float minActionTime;
        protected float rateOfNeedIncrease;
        protected double actionTimeElapsed;
        public GOAPAction GOAPAction;
        public NeedNames NeedAffected;
        public bool ActionComplete;
        public float EstTimeToFinish;
        public float Duration;

        

        public float Cost(Need need)
        {
           
            float estTimeToFulfil = (need.MaxNeed - need.CurrentNeed) / rateOfNeedIncrease;

            

            Duration=  Math.Max(estTimeToFulfil, minActionTime);
            EstTimeToFinish = Duration;
            return Duration;


        }


        public void BeginAction()
        {
            ActionComplete = false;
            actionTimeElapsed = 0;
            Item.IsAvailable = false;
        }



        public void CompleteAction()
        {
            ActionComplete = true;
            Item.IsAvailable = true;
        }


        public abstract GOAPAction DefineGOAPAction();




    }
}
