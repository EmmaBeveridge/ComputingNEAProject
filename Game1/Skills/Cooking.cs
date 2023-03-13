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

        public Cooking()
        {
            SkillType = SkillTypes.Cooking;
           
            RateOfSkillGain = 0.1f;

        }


        public override int GetID() { return DBID; }
        public override string GetSkillString() { return SkillString; }




    }
}
