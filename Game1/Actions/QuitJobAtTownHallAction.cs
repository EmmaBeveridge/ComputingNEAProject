using Game1.Careers;
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
    /// Inherits from ActionAbstract to implement action to quit a job at the town hall
    /// </summary>
    class QuitJobAtTownHallAction : ActionAbstract
    {

        /// <summary>
        /// Constructor to create a new QuitJobAtTownHallAction object. Sets action name, building, minActionTime, and ActionMethod (set to QuitJobAtTownHall method) 
        /// </summary>
        /// <param name="_building"></param>
        public QuitJobAtTownHallAction(Building _building)
        {
            Name = "quit job at town hall";
            Building = _building;
            minActionTime = 5;
            //rateOfNeedIncrease = 1f;
            ActionMethod = QuitJobAtTownHall;


        }

        /// <summary>
        /// Overrides virtual method in parent class. Creates and returns new GOAPActionWithBuilding with instance as parameter. Sets preconditions of building’s EmployedAtConditionString as true and sets postconditions of building’s EmployedAtConditionString as false. Sets GOAPAction.building to instance’s building. 
        /// </summary>
        /// <returns></returns>
        public override GOAPAction DefineGOAPAction()
        {
            GOAPAction = new GOAPActionWithBuilding(this);
            GOAPAction.SetPrecondition(Building.EmployedAtConditionString, true); //must be employed before can quit
            GOAPAction.SetPostcondition(Building.EmployedAtConditionString, false);
            GOAPAction.building = this.Building;
            return GOAPAction;

        }


        /// <summary>
        /// Overrides virtual method in parent class. Resets actionComplete and actionTimeElapsed variables. 
        /// </summary>
        /// <param name="person"></param>
        public override void BeginAction(People person)
        {
            ActionComplete = false;
            actionTimeElapsed = 0;
        }

        /// <summary>
        ///  Overrides virtual method in parent class. Sets ActionComplete to true. 
        /// </summary>
        public override void CompleteAction()
        {
            ActionComplete = true;
        }

        /// <summary>
        /// Called each update frame for which action is ongoing. Simulates quitting a job at town hall. Increments actionTimeElapsed and reduces EstTimeToFinish. If the elapsed action time is greater than the minimum action time and the affected need is fulfilled, Career attribute of person set to null and CompleteAction method is called. 
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="needs"></param>
        /// <param name="person"></param>
        public void QuitJobAtTownHall(GameTime gameTime, Dictionary<NeedNames, Need> needs, People person = null)
        {


            actionTimeElapsed += gameTime.ElapsedGameTime.TotalSeconds;
            EstTimeToFinish = Duration - (float)actionTimeElapsed;

            Console.WriteLine("quitting job at town hall");

            if (actionTimeElapsed > minActionTime)
            {
                person.Career = null;
                CompleteAction();
            }

        }


    }
}





