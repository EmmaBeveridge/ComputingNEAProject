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
    class School : Building
    {
        public School()
        {
            modelName = "School";

            CareerNames.Add(Teacher.Name);
            EmployedAtConditionString = "employed at school";
            DefineActions();

        }

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
