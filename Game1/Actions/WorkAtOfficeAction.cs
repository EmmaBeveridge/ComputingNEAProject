using Game1.GOAP;
using Game1.Town;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.Actions
{
    /// <summary>
    /// Inherits from ActionAbstract to implement action to work at the office
    /// </summary>
    class WorkAtOfficeAction : ActionAbstract
    {
        int WorkScore = 0;

        /// <summary>
        ///  Constructor to create a new WorkAtOfficeAction object. Sets action name, building, minActionTime, and ActionMethod (set to WorkAtOffice method) 
        /// </summary>
        /// <param name="_building"></param>
        public WorkAtOfficeAction(Building _building)
        {
            Name = "work at office";
            Building = _building;
            minActionTime = 15;
            rateOfNeedIncrease = 1f;
            ActionMethod = WorkAtOffice;


        }

        /// <summary>
        ///  Overrides virtual method in parent class. Creates and returns new GOAPActionWithBuilding with instance as parameter. Sets preconditions of building’s EmployedAtConditionString as true. Sets GOAPAction.building to instance’s building. 
        /// </summary>
        /// <returns></returns>
        public override GOAPAction DefineGOAPAction()
        {
            GOAPAction = new GOAPActionWithBuilding(this);
            GOAPAction.SetPrecondition(Building.EmployedAtConditionString, true);
            GOAPAction.building = this.Building;
            return GOAPAction;

        }


        /// <summary>
        /// Overrides virtual method in parent class. Resets actionComplete and actionTimeElapsed variables. Resets WorkScore to 0
        /// </summary>
        /// <param name="person"></param>
        public override void BeginAction(People person)
        {
            ActionComplete = false;
            actionTimeElapsed = 0;
            WorkScore = 0;
        }

        /// <summary>
        /// Overrides virtual method in parent class. Sets ActionComplete to true.  
        /// </summary>
        public override void CompleteAction()
        {
            ActionComplete = true;
        }


        /// <summary>
        /// Called each update frame for which action is ongoing. Simulates working at office. Increments actionTimeElapsed and reduces EstTimeToFinish. If the elapsed action time is greater than the minimum action time and the affected need is fulfilled, if the person is the player, then worker feedback is determined based on workScore and person.DisplayCareerFeedback method used to display feedback to user. CompleteAction method is then called. 
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="needs"></param>
        /// <param name="person"></param>
        public void WorkAtOffice(GameTime gameTime, Dictionary<NeedNames, Need> needs, People person = null)
        {


            actionTimeElapsed += gameTime.ElapsedGameTime.TotalSeconds;
            EstTimeToFinish = Duration - (float)actionTimeElapsed;

            Console.WriteLine("working at office");

            if (actionTimeElapsed > minActionTime)
            {
                if (person.isPlayer) //displaying career feedback
                {
                    double averageScore = WorkScore / actionTimeElapsed;
                    if (averageScore < 0) { person.DisplayCareerFeedback(UI.FeedbackScore.Bad); }
                    else if (averageScore > 0.5) { person.DisplayCareerFeedback(UI.FeedbackScore.Good); }
                    else { person.DisplayCareerFeedback(UI.FeedbackScore.Average); }

                }
                CompleteAction();
            }

        }


    }
}
