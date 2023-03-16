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

    class GetJobAtTownHallAction : ActionAbstract
    {
        public GetJobAtTownHallAction(Building _building)
        {
            Name = "get job at town hall";
            Building = _building;
            minActionTime = 5;
            //rateOfNeedIncrease = 1f;
            ActionMethod = GetJobAtTownHall;


        }

        public override GOAPAction DefineGOAPAction()
        {
            GOAPAction = new GOAPActionWithBuilding(this);
            GOAPAction.SetPrecondition(Building.EmployedAtConditionString, false);
            GOAPAction.SetPostcondition(Building.EmployedAtConditionString, true);
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

        public void GetJobAtTownHall(GameTime gameTime, Dictionary<NeedNames, Need> needs, People person = null)
        {


            actionTimeElapsed += gameTime.ElapsedGameTime.TotalSeconds;
            EstTimeToFinish = Duration - (float)actionTimeElapsed;

            Console.WriteLine("getting job at town hall");

            if (actionTimeElapsed > minActionTime)
            {
                person.Career = new Politician();
                CompleteAction();
            }

        }


    }
}





