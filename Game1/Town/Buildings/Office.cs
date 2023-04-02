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
    /// Child Office class inherits from Building parent class; implements office in town.
    /// </summary>
    class Office :Building
    {
        /// <summary>
        /// Constructor for new office object. Sets model name, names of careers available at office, EmployedAtConditionString, and calls DefineActions() method. 
        /// </summary>
        public Office()
        {
            modelName = "Office";
            
            CareerNames.Add(OfficeWorker.Name);
            EmployedAtConditionString = "employed at office";
            DefineActions();

        }


        /// <summary>
        ///  Defines GOAP actions for office building and adds action label to describe action in button display to actionLabels list. 
        /// </summary>
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
