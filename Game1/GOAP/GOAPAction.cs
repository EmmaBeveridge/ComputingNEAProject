using Game1.Town;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.GOAP
{
    public abstract class GOAPAction
    {
        /// <summary>
        /// optional name for the Action. Used for debugging purposes
        /// </summary>
        public string Name;

        /// <summary>
        /// Estimated cost of performing the action for example the distance to the item or the length of the action etc. 
        /// </summary>
        public float Cost; ///esitmate so high accuracy not necessary


        ///<summary>
        ///The item with which the action occurs in instance of derived GOAPActionWithItem class 
        ///</summary>
        public Item item = null;

        /// <summary>
        /// Person who ‘owns’ the TalkToPersonAction (TalkToPersonAction.PersonToInteractWith) in instance of derived GOAPActionWithPerson class 
        /// </summary>
        public People interactionPerson = null;

        /// <summary>
        /// The building with which the action occurs in instance of derived GOAPActionWithBuilding class 
        /// </summary>
        public Building building = null;
        

        /// <summary>
        /// ActionAbstract object that implements a specific action. 
        /// </summary>
        public ActionAbstract Action;



        /// <summary>
        /// Set of conditions that must be met for the action to be possible. HashSet of tuples where first item in tuple is conditions string and second item is the Boolean state of the condition. 
        /// </summary>
        internal HashSet<Tuple<string, bool>> PreConditions = new HashSet<Tuple<string, bool>>();


        /// <summary>
        /// Set of conditions that will be fulfilled on the action’s completion. HashSet of tuples where first item in tuple is conditions string and second item is the Boolean state of the condition. 
        /// </summary>
        internal HashSet<Tuple<string, bool>> PostConditions = new HashSet<Tuple<string, bool>>();

        /// <summary>
        ///  Queue of people currently or waiting to do the action. 
        /// </summary>
        public Queue<People> doingAction = new Queue<People>();
        
        /// <summary>
        /// Constructor for new GOAPAction object
        /// </summary>
        public GOAPAction()
        { }


        /// <summary>
        /// Returns the value of the precondition with the name supplied as a parameter. If no precondition found with supplied name, it is assumed to be false.
        /// </summary>
        /// <param name="PreconditionName"></param>
        /// <returns></returns>
        public bool GetStateOfPrecondition(string PreconditionName)
        {
            foreach (Tuple<string, bool> precondtion in PreConditions)
            {
                if (precondtion.Item1 == PreconditionName)
                {
                    return precondtion.Item2;
                }

            }

            return false; //assumes false if no precondtition found
        }


        public GOAPAction(ActionAbstract _action)
        {


            Action = _action;
            Name = _action.Name;

            
               
        }

        /// <summary>
        /// Abstract method overriden in child classes to update the Cost attribute of the GOAPAction with estimated cost for completing action. 
        /// </summary>
        /// <param name="person"></param>
        /// <param name="needs"></param>
        public abstract void UpdateCost(People person, Dictionary<NeedNames, Need> needs);


        public GOAPAction(ActionAbstract _action, int cost) : this(_action)
        {
            this.Cost = cost;
        }

        /// <summary>
        /// Add precondition to set of preconditions. 
        /// </summary>
        /// <param name="conditionName"></param>
        /// <param name="value"></param>
        public void SetPrecondition(string conditionName, bool value)
        {
            this.PreConditions.Add(new Tuple<string, bool>(conditionName, value));
        }

        /// <summary>
        /// Add postcondition to set of postconditions.
        /// </summary>
        /// <param name="conditionName"></param>
        /// <param name="value"></param>
        public void SetPostcondition(string conditionName, bool value)
        {
            this.PostConditions.Add(new Tuple<string, bool>(conditionName, value));
        }


        /// <summary>
        /// called before the Planner does its planning. Gives the Action an opportunity to set its score or to opt out if it isnt of use.
        /// eg if action is to get food from fridge but no food in fridge, action is not considered by ActionPlanner
        /// </summary>
        public virtual bool Validate(People person)
        {

            //check if item is available herre???


           

            return true;

            
        }



        /// <summary>
        ///  Returns string representation of GOAPAction. Used for debugging purposes. 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"[Action] {this.Name} - cost: {this.Cost}";
        }

    }
}


