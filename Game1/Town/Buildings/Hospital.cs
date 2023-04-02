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
    /// Child Hospital class inherits from Building parent class; implements hospital in town. 
    /// </summary>
    class Hospital : Building
    {
        /// <summary>
        /// Constructor for new hospital object. Sets model name, names of careers available at hospital, EmployedAtConditionString, and calls DefineActions() method. 
        /// </summary>
        public Hospital()
        {
            modelName = "Hospital";

            CareerNames.Add(Doctor.Name);
            EmployedAtConditionString = "employed at hospital";
            DefineActions();

        }


        /// <summary>
        /// Defines GOAP actions for hospital building and adds action label to describe action in button display to actionLabels list. 
        /// </summary>
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
