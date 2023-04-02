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
    /// Child TownHall class inherits from Building parent class; implements town hall in town. 
    /// </summary>
    class TownHall : Building
    {
        /// <summary>
        /// Constructor for new town hall object. Sets model name, names of careers available at town hall, EmployedAtConditionString, and calls DefineActions() method.
        /// </summary>
        public TownHall()
        {
            modelName = "TownHall";

            CareerNames.Add(Politician.Name);
            EmployedAtConditionString = "employed at town hall";
            DefineActions();

        }

        /// <summary>
        /// Defines GOAP actions for town hall building and adds action label to describe action in button display to actionLabels list. 
        /// </summary>
        public override void DefineActions()
        {

            WorkAtTownHallAction workAtTownHall = new WorkAtTownHallAction(this);
            GOAPAction workAtTownHallGOAP = workAtTownHall.DefineGOAPAction();
            GOAPActions.Add(workAtTownHallGOAP);
            actionLabels.Add("work at town hall", workAtTownHallGOAP);

            GetJobAtTownHallAction getJobAtTownHall = new GetJobAtTownHallAction(this);
            GOAPAction getJobAtTownHallGOAP = getJobAtTownHall.DefineGOAPAction();
            GOAPActions.Add(getJobAtTownHallGOAP);
            actionLabels.Add("get job at town hall", getJobAtTownHallGOAP);

            QuitJobAtTownHallAction quitJobAtTownHall = new QuitJobAtTownHallAction(this);
            GOAPAction quitJobAtTownHallGOAP = quitJobAtTownHall.DefineGOAPAction();
            GOAPActions.Add(quitJobAtTownHallGOAP);
            actionLabels.Add("quit job at town hall", quitJobAtTownHallGOAP);

           



        }


    }
}
