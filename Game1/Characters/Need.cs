using Game1.UI;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1
{
    public enum NeedLevel {def, High, Mid, Low}
    public enum NeedPriority { High, Mid, Low}

    public enum NeedNames
    {
        Null, Hunger, Sleep, Toilet, Social, Hygiene, Fun
    }


    public class Need
    {
        private static float DepletionRateHunger = 0.4f;
        private static float DepletionRateToilet = 0.6f;
        private static float DepletionRateSleep = 0.1f;
        private static float DepletionRateHygiene = 0.2f;
        private static float DepletionRateFun = 0.4f;
        private static float DepletionRateSocial = 0.3f;


        public NeedNames Name;
        public float CurrentNeed;
        public int MaxNeed;
        public NeedLevel Level;
        public float DepletionRate;
        
        public NeedBar NeedBar = null;
        public float Percent;
        public bool Prioritised;



        public Need(NeedNames _name, int _maxNeed = 100, float _currentNeed = 10, bool generateNeedBar = false, bool _prioritised = false, bool _accelerated = false, bool _decelerated = false)
        {
            Name = _name;
            SetDepletionRate(Name, _accelerated, _decelerated);
            MaxNeed = _maxNeed;
            CurrentNeed = _currentNeed;
            Prioritised = _prioritised;
            if (generateNeedBar) { NeedBar = new NeedBar(this); }
            Update();

        }


        private void SetDepletionRate(NeedNames name, bool accelerated, bool decelerated)
        {
            float multiplier = accelerated ? 1.3f : (decelerated ? 0.7f : 1f);
            

            switch (name)
            {
                case NeedNames.Null:
                    break;
                case NeedNames.Hunger:
                    DepletionRate = DepletionRateHunger;
                    break;
                case NeedNames.Sleep:
                    DepletionRate = DepletionRateSleep;
                    break;
                case NeedNames.Toilet:
                    DepletionRate = DepletionRateToilet;
                    break;
                case NeedNames.Social:
                    DepletionRate = DepletionRateSocial;
                    break;
                case NeedNames.Hygiene:
                    DepletionRate = DepletionRateHygiene;
                    break;
                case NeedNames.Fun:
                    DepletionRate = DepletionRateFun;
                    break;
                default:
                    break;
            }


            DepletionRate *= multiplier;

        }


        public void DepleteNeed(GameTime gameTime)
        {
            float decrement = (float)gameTime.ElapsedGameTime.TotalSeconds * DepletionRate;

            Update(-decrement);




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
