﻿using Game1.UI;
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
        Null, Hunger, Sleep, Toilet, Social, Hygiene, Fun
    }


    public class Need
    {
        public NeedNames Name;
        public float CurrentNeed;
        public int MaxNeed;
        public NeedLevel Level;
        
        public NeedBar NeedBar = null;
        public float Percent;
        public bool Prioritised;



        public Need(NeedNames _name, int _maxNeed = 100, float _currentNeed = 10, bool generateNeedBar = false, bool _prioritised = false)
        {
            Name = _name;
            
            MaxNeed = _maxNeed;
            CurrentNeed = _currentNeed;
            Prioritised = _prioritised;
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



        public static NeedNames GetNeedNamefromString (string nameString)
        {
            switch (nameString.ToLower())
            {
                case "hunger":
                    return NeedNames.Hunger;
                case "toilet":
                    return NeedNames.Toilet;
                case "sleep":
                    return NeedNames.Sleep;
                case "social":
                    return NeedNames.Social;
                case "fun":
                    return NeedNames.Fun;
                case "hygiene":
                    return NeedNames.Hygiene;

                default:
                    return NeedNames.Null;
            }
        }






    }
}
