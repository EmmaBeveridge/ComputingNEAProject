using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
namespace Game1.Skills
{

    public enum SkillTypes
    {
        Null,
        Cooking
        
    }


    public class Skill
    {
        public SkillTypes SkillType; 
        public static string SkillString;
        public static int DBID;
        public float Score=0;
        protected float MaxScore = 100;
        public int Level=0;
        protected float RateOfSkillGain;

        public virtual int GetID() { return DBID; }
        public static void SetSkillID(string SkillString, int SkillID)
        {
            switch (SkillString)
            {
                case "Cooking":
                    Cooking.DBID = SkillID;
                    break;
               
                default:
                    break;
            }

        }


        public virtual string GetSkillString() { return SkillString; }


        public static int GetSkillID(string SkillString)
        {
            switch (SkillString)
            {
                case "Cooking":
                    return Cooking.DBID;
                
                default:
                    break;
            }
            return -1;
        }

        public static Skill GetSkillFromString(string SkillName)
        {
            switch (SkillName)
            {

                case "Cooking":
                    return new Cooking();
                default:
                    return null;
                    break;
            }

        }


        public static Skill GetNewSkill(SkillTypes type)
        {
            switch (type)
            {
                
                case SkillTypes.Cooking:
                    return new Cooking();
                default:
                    return null;
                    break;
            }

        }

        private float CalculateIncrementScore(GameTime gameTime)
        {
            float increment = (float)gameTime.ElapsedGameTime.TotalSeconds * RateOfSkillGain;

            return increment;

        }


        public void UpdateSkill(GameTime gameTime)
        {
            Score += CalculateIncrementScore(gameTime);
            Score = Math.Max(Score, 0);
            Score = Math.Min(Score, MaxScore);


            float percent = (float)Score / (float)MaxScore;

            Level = (int)(percent*100 / 10f);
        }





    }
}
