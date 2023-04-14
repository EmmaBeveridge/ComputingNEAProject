using Game1.UI;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1
{
    /// <summary>
    /// Enumeration to store possible levels of a given need. Used in colouring of need bars. 
    /// </summary>
    public enum NeedLevel {def, High, Mid, Low}

    /// <summary>
    /// Enumeration to store possible need types. 
    /// </summary>
    public enum NeedNames
    {
        Null, Hunger, Sleep, Toilet, Social, Hygiene, Fun
    }


    public class Need
    {
        private static float DepletionRateHunger = 0.1f;
        private static float DepletionRateToilet = 0.3f;
        private static float DepletionRateSleep = 0.05f;
        private static float DepletionRateHygiene = 0.2f;
        private static float DepletionRateFun = 0.2f;
        private static float DepletionRateSocial = 0.05f;


        public NeedNames Name;
        public float CurrentNeed;
        public int MaxNeed;
        public NeedLevel Level;
        public float DepletionRate;
        
        public NeedBar NeedBar = null;
        public float Percent;
        public bool Prioritised;


        /// <summary>
        /// Constructor to create new Need object. Calls method to set depletion rate of need. If character is player, then a corresponding need bar will also be generated in order to display in UI. 
        /// </summary>
        /// <param name="_name"></param>
        /// <param name="_maxNeed"></param>
        /// <param name="_currentNeed"></param>
        /// <param name="generateNeedBar"></param>
        /// <param name="_prioritised"></param>
        /// <param name="_accelerated"></param>
        /// <param name="_decelerated"></param>
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

        /// <summary>
        /// Sets rate at which need depletes depending on type of Need e.g. hunger decays at different rate to sleep. Standard decay rates for each of the needs are altered depending on if the character possesses traits accelerating/decelerating decay.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="accelerated"></param>
        /// <param name="decelerated"></param>
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

        /// <summary>
        ///  Calculates need decrement using decay rate and elapsed game time since last decay. Calls update method to decrement. 
        /// </summary>
        /// <param name="gameTime"></param>
        public void DepleteNeed(GameTime gameTime)
        {
            float decrement = (float)gameTime.ElapsedGameTime.TotalSeconds * DepletionRate;

            Update(-decrement);




        }


        /// <summary>
        /// Alters current need score using supplied increment, ensuring need value does not exceed maximum need score or falls below 0. Calculates a percentage for new need score and updates need level accordingly. 
        /// </summary>
        /// <param name="increment"></param>
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

        /// <summary>
        /// If float rate and gametime parameters supplied, an increment value is calculated and then Update is called using increment value 
        /// </summary>
        /// <param name="rate"></param>
        /// <param name="gameTime"></param>
        public void Update (float rate, GameTime gameTime)
        {
            float increment =  (float)(rate * gameTime.ElapsedGameTime.TotalSeconds);
            this.Update(increment);
        }

        /// <summary>
        /// Returns if current need score is equal to maximum need score. 
        /// </summary>
        /// <returns></returns>
        public bool IsFulfilled()
        {
            return this.CurrentNeed == this.MaxNeed;
        }


        /// <summary>
        /// For use with SQLiteDBHandler class to convert need name from string as stored in database to NeedNames enum value for use in code.
        /// </summary>
        /// <param name="nameString"></param>
        /// <returns></returns>
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
