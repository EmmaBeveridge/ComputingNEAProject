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
    class Hospital : Building
    {
        public Hospital()
        {
            modelName = "Hospital";

            CareerNames.Add(Doctor.Name);
            EmployedAtConditionString = "employed at hospital";
            DefineActions();

        }

        public override void DefineActions()
        {

            WorkAtHospitalAction workAtHospital = new WorkAtHospitalAction(this);
            GOAPAction workAtHospitalGOAP = workAtHospital.DefineGOAPAction();
            GOAPActions.Add(workAtHospitalGOAP);
            actionLabels.Add("work at hospital", workAtHospitalGOAP);

            GetJobAtHospitalAction getJobAtHospital = new GetJobAtHospitalAction(this);
            GOAPAction getJobAtHospitalGOAP = getJobAtHospital.DefineGOAPAction();
            GOAPActions.Add(getJobAtHospitalGOAP);
            actionLabels.Add("get job at hospital", getJobAtHospitalGOAP);

            QuitJobAtHospitalAction quitJobAtHospital = new QuitJobAtHospitalAction(this);
            GOAPAction quitJobAtHospitalGOAP = quitJobAtHospital.DefineGOAPAction();
            GOAPActions.Add(quitJobAtHospitalGOAP);
            actionLabels.Add("quit job at hospital", quitJobAtHospitalGOAP);




        }


    }
}
