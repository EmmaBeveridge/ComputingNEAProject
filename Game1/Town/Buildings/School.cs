using Game1.Actions;
using Game1.GOAP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game1.Careers;
namespace Game1.Town.Buildings
{
    /// <summary>
    /// Child School class inherits from Building parent class; implements school in town. 
    /// </summary>
    class School : Building
    {
        /// <summary>
        /// Constructor for new school object. Sets model name, names of careers available at school, EmployedAtConditionString, and calls DefineActions() method. 
        /// </summary>
        public School()
        {
            modelName = "School";

            CareerNames.Add(Teacher.Name);
            EmployedAtConditionString = "employed at school";
            DefineActions();

        }


        /// <summary>
        /// Defines GOAP actions for school building and adds action label to describe action in button display to actionLabels list.
        /// </summary>
        public override void DefineActions()
        {

            WorkAtSchoolAction workAtSchool = new WorkAtSchoolAction(this);
            GOAPAction workAtSchoolGOAP = workAtSchool.DefineGOAPAction();
            GOAPActions.Add(workAtSchoolGOAP);
            actionLabels.Add("work at school", workAtSchoolGOAP);

            GetJobAtSchoolAction getJobAtSchool = new GetJobAtSchoolAction(this);
            GOAPAction getJobAtSchoolGOAP = getJobAtSchool.DefineGOAPAction();
            GOAPActions.Add(getJobAtSchoolGOAP);
            actionLabels.Add("get job at school", getJobAtSchoolGOAP);

            QuitJobAtSchoolAction quitJobAtSchool = new QuitJobAtSchoolAction(this);
            GOAPAction quitJobAtSchoolGOAP = quitJobAtSchool.DefineGOAPAction();
            GOAPActions.Add(quitJobAtSchoolGOAP);
            actionLabels.Add("quit job at school", quitJobAtSchoolGOAP);

            


        }


    }
}
