using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.Skills
{
    public class Cooking:Skill
    {
        public new static string SkillString = "Cooking";
        public new static int DBID;


        /// <summary>
        /// Constructor to create new cooking skill; setting SkillType and rate of skill gain for cooking skill 
        /// </summary>
        public Cooking()
        {
            SkillType = SkillTypes.Cooking;
           
            RateOfSkillGain = 0.1f;

        }

        /// <summary>
        ///  Returns DBID for Cooking skill class
        /// </summary>
        /// <returns></returns>
        public override int GetID() { return DBID; }

        /// <summary>
        /// Returns SkillString for Cooking skill class 
        /// </summary>
        /// <returns></returns>
        public override string GetSkillString() { return SkillString; }




    }
}
