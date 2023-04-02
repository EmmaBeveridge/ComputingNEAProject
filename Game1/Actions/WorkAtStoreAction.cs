﻿using Game1.GOAP;
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
    /// Inherits from ActionAbstract to implement action to work at the store
    /// </summary>
    class WorkAtStoreAction : ActionAbstract
    {

        /// <summary>
        ///  Constructor to create a new WorkAtStoreAction object. Sets action name, building, minActionTime, and ActionMethod (set to WorkAtStore method) 
        /// </summary>
        /// <param name="_building"></param>
        public WorkAtStoreAction(Building _building)
        {
            Name = "work at store";
            Building = _building;
            minActionTime = 15;
            rateOfNeedIncrease = 1f;
            ActionMethod = WorkAtStore;
            

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
        /// Overrides virtual method in parent class. Resets actionComplete and actionTimeElapsed variables.
        /// </summary>
        /// <param name="person"></param>
        public override void BeginAction(People person)
        {
            ActionComplete = false;
            actionTimeElapsed = 0;
        }

        /// <summary>
        /// Overrides virtual method in parent class. Sets ActionComplete to true.  
        /// </summary>
        public override void CompleteAction()
        {
            ActionComplete = true;
        }


        /// <summary>
        /// Called each update frame for which action is ongoing. Simulates working at store. Increments actionTimeElapsed and reduces EstTimeToFinish. If the elapsed action time is greater than the minimum action time and the affected need is fulfilled, CompleteAction method is called. 
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="needs"></param>
        /// <param name="person"></param>

        public void WorkAtStore(GameTime gameTime, Dictionary<NeedNames, Need> needs, People person = null)
        {

            
            actionTimeElapsed += gameTime.ElapsedGameTime.TotalSeconds;
            EstTimeToFinish = Duration - (float)actionTimeElapsed;

            Console.WriteLine("working at store");

            if (actionTimeElapsed > minActionTime)
            {
                CompleteAction();
            }

        }


    }
}
