using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.Traits
{
    public abstract class Trait
    {


       public static string TraitString;

        public static Dictionary<Texture2D, string> ButtonToString = new Dictionary<Texture2D, string>();
        public static int maxTraits = 2;

        public static int DBID;

        public List<NeedNames> needsPrioritised = new List<NeedNames>();
        public List<NeedNames> needsAcceleratedDepletion = new List<NeedNames>();
        public List<NeedNames> needsDeceleratedDepletion = new List<NeedNames>();


        public Trait()
        {

        }



        public virtual int GetID() { return DBID; }


        public static void SetTraitID(string TraitString, int TraitID)
        {
            switch (TraitString)
            {
                case "Lazy":
                    TraitLazy.DBID = TraitID;
                    break;
                case "Gourmand":
                    TraitGourmand.DBID = TraitID;
                    break;
                case "Sociable":
                    TraitSociable.DBID = TraitID;
                    break;
                case "Clean":
                    TraitClean.DBID = TraitID;
                    break;
                case "FunLoving":
                    TraitFunLoving.DBID = TraitID;
                    break;
                case "Loner":
                    TraitLoner.DBID = TraitID;
                    break;


                default:
                    break;
            }

        }

        public static int GetTraitID(string TraitString)
        {
            switch (TraitString)
            {
                case "Lazy":
                    return TraitLazy.DBID;
                case "Gourmand":
                    return TraitGourmand.DBID;
                case "Sociable":
                    return TraitSociable.DBID;
                case "Clean":
                    return TraitClean.DBID;
                case "FunLoving":
                    return TraitFunLoving.DBID;
                case "Loner":
                    return TraitLoner.DBID;
                default:
                    break;
            }
            return -1;
        }


        /// <summary>
        /// Returns list of all needs that are prioritised by the specified traits
        /// </summary>
        /// <param name="traits">List of person's trait</param>
        /// <returns></returns>
        public static List<NeedNames> GetNeedsPrioritised(List<Trait> traits)
        {
            List<NeedNames> prioritised = new List<NeedNames>();

            foreach (Trait trait in traits)
            {
                prioritised.AddRange(trait.needsPrioritised);
            }

            prioritised = prioritised.Distinct().ToList();

            return prioritised;


        }

        /// <summary>
        /// Returns list of all needs that have an accelerated depletion rate due to specified traits
        /// </summary>
        /// <param name="traits">List of person's traits</param>
        /// <returns></returns>
        public static List<NeedNames> GetNeedsAcceleratedDepletion(List<Trait> traits)
        {
            List<NeedNames> accelerated = new List<NeedNames>();

            foreach (Trait trait in traits)
            {
                accelerated.AddRange(trait.needsAcceleratedDepletion);
            }

            accelerated = accelerated.Distinct().ToList();

            return accelerated;


        }

        public static List<NeedNames> GetNeedsDeceleratedDepletion(List<Trait> traits)
        {
            List<NeedNames> decelerated = new List<NeedNames>();

            foreach (Trait trait in traits)
            {
                decelerated.AddRange(trait.needsDeceleratedDepletion);
            }

            decelerated = decelerated.Distinct().ToList();

            return decelerated;


        }


        /// <summary>
        /// Returns trait object given string name of trait
        /// </summary>
        /// <param name="TraitString">string name of trait</param>
        /// <returns></returns>

        public static Trait GetTraitFromString (string TraitString)
        {
            switch (TraitString)
            {
                case "Lazy":
                    return new TraitLazy();
                    break;

                case "Gourmand":
                    return new TraitGourmand();
                    break;
                case "Sociable":
                    return new TraitSociable();
                    break;
                case "Clean":
                    return new TraitClean();
                    break;
                case "FunLoving":
                    return new TraitFunLoving();
                    break;
                case "Loner":
                    return new TraitLoner();
                    break;

                default:
                    return null;
                    break;
            }



        }





    }
}
