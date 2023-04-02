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
    /// Child Store class inherits from Building parent class; implements store in town. 
    /// </summary>
    class Store :Building
    {
        /// <summary>
        /// Constructor for new store object. Sets model name, names of careers available at store, EmployedAtConditionString, and calls DefineActions() method. 
        /// </summary>
        public Store()
        {
            modelName = "Store";
            
            CareerNames.Add(StoreClerk.Name);
            EmployedAtConditionString = "employed at store";
            DefineActions();

        }


        /// <summary>
        /// Defines GOAP actions for store building and adds action label to describe action in button display to actionLabels list. 
        /// </summary>
        public override void DefineActions()
        {

            WorkAtStoreAction workAtStore = new WorkAtStoreAction(this);
            GOAPAction workAtStoreGOAP = workAtStore.DefineGOAPAction();
            GOAPActions.Add(workAtStoreGOAP);
            actionLabels.Add("work at store", workAtStoreGOAP);

            GetJobAtStoreAction getJobAtStore = new GetJobAtStoreAction(this);
            GOAPAction getJobAtStoreGOAP = getJobAtStore.DefineGOAPAction();
            GOAPActions.Add(getJobAtStoreGOAP);
            actionLabels.Add("get job at store", getJobAtStoreGOAP);

            QuitJobAtStoreAction quitJobAtStore = new QuitJobAtStoreAction(this);
            GOAPAction quitJobAtStoreGOAP = quitJobAtStore.DefineGOAPAction();
            GOAPActions.Add(quitJobAtStoreGOAP);
            actionLabels.Add("quit job at store", quitJobAtStoreGOAP);

            BuyGroceriesAction buyGroceries = new BuyGroceriesAction(this);
            GOAPAction buyGroceriesGOAP = buyGroceries.DefineGOAPAction();
            GOAPActions.Add(buyGroceriesGOAP);
            actionLabels.Add("buy groceries", buyGroceriesGOAP);




        }


    }
}
