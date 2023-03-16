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

    class QuitJobAtTownHallAction : ActionAbstract
    {
        public QuitJobAtTownHallAction(Building _building)
        {
            Name = "quit job at town hall";
            Building = _building;
            minActionTime = 5;
            //rateOfNeedIncrease = 1f;
            ActionMethod = QuitJobAtTownHall;


        }

        public override GOAPAction DefineGOAPAction()
        {
            GOAPAction = new GOAPActionWithBuilding(this);
            GOAPAction.SetPrecondition(Building.EmployedAtConditionString, true); //must be employed before can quit
            GOAPAction.SetPostcondition(Building.EmployedAtConditionString, false);
            GOAPAction.building = this.Building;
            return GOAPAction;

        }



        public override void BeginAction(People person)
        {
            ActionComplete = false;
            actionTimeElapsed = 0;
        }


        public override void CompleteAction()
        {
            ActionComplete = true;
        }

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





