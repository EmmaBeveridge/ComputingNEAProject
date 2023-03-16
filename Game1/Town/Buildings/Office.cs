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
    class Office:Building
    {
        public Office()
        {
            modelName = "Office";
            
            CareerNames.Add(OfficeWorker.Name);
            EmployedAtConditionString = "employed at office";
            DefineActions();

        }

        public override void DefineActions()
        {

            WorkAtOfficeAction workAtOffice = new WorkAtOfficeAction(this);
            GOAPAction workAtOfficeGOAP = workAtOffice.DefineGOAPAction();
            GOAPActions.Add(workAtOfficeGOAP);
            actionLabels.Add("work at office", workAtOfficeGOAP);

            GetJobAtOfficeAction getJobAtOffice = new GetJobAtOfficeAction(this);
            GOAPAction getJobAtOfficeGOAP = getJobAtOffice.DefineGOAPAction();
            GOAPActions.Add(getJobAtOfficeGOAP);
            actionLabels.Add("get job at office", getJobAtOfficeGOAP);

            QuitJobAtOfficeAction quitJobAtOffice = new QuitJobAtOfficeAction(this);
            GOAPAction quitJobAtOfficeGOAP = quitJobAtOffice.DefineGOAPAction();
            GOAPActions.Add(quitJobAtOfficeGOAP);
            actionLabels.Add("quit job at office", quitJobAtOfficeGOAP);




        }


    }
}
