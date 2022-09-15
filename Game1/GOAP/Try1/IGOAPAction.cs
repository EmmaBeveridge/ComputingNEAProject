using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.GOAP
{

    public struct GOAPActionStack<T, W>
    {
        public GOAPState<T, W> currentState;
        public GOAPState<T, W> goalState;
        public GOAPAgent<T, W> agent;
        public IGOAPAction<T, W> next;
        public GOAPState<T, W> settings;
    }
    public interface IGOAPAction<T, W>
    {
        List<GOAPState<T, W>> GetSettings(GOAPActionStack<T, W> stackData);
        void Run(IGOAPAction<T, W> previousAction, IGOAPAction<T, W> nextAction, GOAPState<T, W> settings, GOAPState<T, W> goalState, Action<IGOAPAction<T, W>> done, Action<IGOAPAction<T, W>> fail);
        void PlanEnter(IGOAPAction<T, W> previousAction, IGOAPAction<T, W> nextAction, GOAPState<T, W> settings, GOAPState<T, W> goalState);
        void PlanExit(IGOAPAction<T, W> previousAction, IGOAPAction<T, W> nextAction, GOAPState<T, W> settings, GOAPState<T, W> goalState);

        void Exit(IGOAPAction<T, W> nextAction);

        string GetName();
        bool isActive();
        bool isInterruptable();
        void AskForInterruption();


        GOAPState<T, W> GetPreconditions(GOAPActionStack<T, W> stackData);
        GOAPState<T, W> GetEffects(GOAPActionStack<T, W> stackData);
        float GetCosts(GOAPActionStack<T, W> stackData);
        bool CheckProceduralCondition(GOAPActionStack<T, W> stackData);
        void Precalculations(GOAPActionStack<T, W> stackData);
        


        



    }
}
