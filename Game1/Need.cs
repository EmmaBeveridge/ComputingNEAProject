using Game1.UI;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1
{
    public enum NeedLevel { High, Mid, Low}
    public enum NeedPriority { High, Mid, Low}

    public enum NeedNames
    {
        Hunger, Sleep, Toilet, Social, Hygiene, Fun
    }


    public class Need
    {
        public NeedNames Name;
        public float CurrentNeed;
        public int MaxNeed;
        public NeedLevel Level;
        public NeedPriority Priority;
        public NeedBar NeedBar = null;
        public float Percent;



        public Need(NeedNames _name, NeedPriority _priority, int _maxNeed = 100, int _currentNeed = 10, bool generateNeedBar = false)
        {
            Name = _name;
            Priority = _priority;
            MaxNeed = _maxNeed;
            CurrentNeed = _currentNeed;
            if (generateNeedBar) { NeedBar = new NeedBar(this); }
            Update();

        }

        public void Update(float increment = 0)
        {
            CurrentNeed += increment;
            CurrentNeed = Math.Max(CurrentNeed, 0);
            CurrentNeed = Math.Min(CurrentNeed, MaxNeed);


            Percent = (float) CurrentNeed / (float)MaxNeed;

            if (Percent < 0.2) { Level = NeedLevel.Low; }
            else if (Percent >= 0.75) { Level = NeedLevel.High; }
            else { Level = NeedLevel.Mid; }

        }

        public void Update (float rate, GameTime gameTime)
        {
            float increment =  (float)(rate * gameTime.ElapsedGameTime.TotalSeconds);
            this.Update(increment);
        }


        public bool IsFulfilled()
        {
            return this.CurrentNeed == this.MaxNeed;
        }










    }
}
