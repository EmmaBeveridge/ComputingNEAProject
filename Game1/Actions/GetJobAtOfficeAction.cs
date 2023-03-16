﻿using Game1.Careers;
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
    class GetJobAtOfficeAction:ActionAbstract
    {

        public GetJobAtOfficeAction(Building _building)
        {
            Name = "get job at office";
            Building = _building;
            minActionTime = 5;
            //rateOfNeedIncrease = 1f;
            ActionMethod = GetJobAtOffice;


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

        public void GetJobAtOffice(GameTime gameTime, Dictionary<NeedNames, Need> needs, People person = null)
        {


            actionTimeElapsed += gameTime.ElapsedGameTime.TotalSeconds;
            EstTimeToFinish = Duration - (float)actionTimeElapsed;

            Console.WriteLine("getting job at office");

            if (actionTimeElapsed > minActionTime)
            {
                person.Career = new OfficeWorker();
                CompleteAction();
            }

        }


    }
}