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
    class Store:Building
    {
        public Store()
        {
            modelName = "Store";
            
            CareerNames.Add(StoreClerk.Name);
            EmployedAtConditionString = "employed at store";
            DefineActions();

        }

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
