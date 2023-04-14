using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
namespace Game1.Skills
{
    /// <summary>
    /// Enumeration to store possible skill types 
    /// </summary>
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
        public int Level { get { return (int)(Score / 10f); } }
        protected float RateOfSkillGain;


        /// <summary>
        ///  Returns ID for skill type used in SQLite database skill lookup system, where each skill type is assigned and identified by an integer value
        /// </summary>
        /// <returns></returns>
        public virtual int GetID() { return DBID; }

        /// <summary>
        /// Static method to assign database ID for each skill type to static DBID attribute of each derived Skill class. Appropriate skill class identified by string supplied as parameter.
        /// </summary>
        /// <param name="SkillString"></param>
        /// <param name="SkillID"></param>
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



        /// <summary>
        /// Returns SkillString attribute for skill class.
        /// </summary>
        /// <returns></returns>
        public virtual string GetSkillString() { return SkillString; }

        /// <summary>
        /// Returns DBID attribute for skill class specified by string parameter. 
        /// </summary>
        /// <param name="SkillString"></param>
        /// <returns></returns>
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



        /// <summary>
        /// Returns new child skill object of correct derived type given string name of skill.
        /// </summary>
        /// <param name="SkillName"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Returns new child skill object of correct derived type given parameter of SkillType 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>

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


        /// <summary>
        ///  Calculates and returns amount by which Score of skill should increase given rate of skill gain and elapsed game time.
        /// </summary>
        /// <param name="gameTime"></param>
        /// <returns></returns>
        private float CalculateIncrementScore(GameTime gameTime)
        {
            float increment = (float)gameTime.ElapsedGameTime.TotalSeconds * RateOfSkillGain;

            return increment;

        }

        /// <summary>
        /// Updates Score, ensuring it does not exceed maximum score. Calculates score as percentage of maximum score and updates skill level using this percentage. 
        /// </summary>
        /// <param name="gameTime"></param>
        public void UpdateSkill(GameTime gameTime)
        {
            Score += CalculateIncrementScore(gameTime);
            
            Score = Math.Min(Score, MaxScore);


        }





    }
}
