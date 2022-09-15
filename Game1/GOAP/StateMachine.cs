using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.GOAP
{
    public abstract class State<T>
    {
        protected StateMachine<T> Machine;
        protected T Context;
        public bool transitionOnNextTick;

        internal void SetMachineAndContext(StateMachine<T> machine, T context)
        {
            this.Machine = machine;
            this.Context = context;
            this.OnInitialized();
        }

        /// <summary>
        /// called directly after the machine and context are set allowing the state to do any required setup
        /// </summary>
        public virtual void OnInitialized()
        { }

        /// <summary>
        /// called when the state becomes the active state
        /// </summary>
        public virtual void Begin()
        { }

        /// <summary>
        /// called every frame this state is the active state
        /// </summary>
        public abstract void Update(GameTime gameTime);

        /// <summary>
        /// called when this state is no longer the active state
        /// </summary>
        public virtual void End()
        { }
    }


    public class StateMachine<T>
    {
        public event Action OnStateChanged;

        public State<T> PreviousState { get; private set; }

        public State<T> CurrentState { get; private set; }

        public State<T> NextState { get; private set; }

        protected T Context;

        private readonly Dictionary<Type, State<T>> states = new Dictionary<Type, State<T>>();

        public StateMachine(T context, State<T> initialState)
        {
            this.Context = context;

            // setup our initial state
            this.AddState(initialState);
            this.CurrentState = initialState;
            initialState.Begin();
        }

        /// <summary>
        /// adds the state to the machine
        /// </summary>
        public void AddState(State<T> state)
        {
            state.SetMachineAndContext(this, this.Context);
            this.states[state.GetType()] = state;
        }

        /// <summary>
        /// ticks the state machine with the provided delta time
        /// </summary>
        public void Tick(GameTime gameTime)
        {
            if (this.CurrentState == null)
            {
                return;
            }
            
            this.CurrentState.Update(gameTime);

            if (this.CurrentState.transitionOnNextTick)
            {
                // only call end if we have a currentState
                this.CurrentState?.End();

                // swap states and call begin
                this.PreviousState = this.CurrentState;
                this.CurrentState = this.NextState;
                this.NextState = null;
                this.CurrentState.Begin();

                // fire the changed event if we have a listener
                this.OnStateChanged?.Invoke();
            }

            
           
        }

        /// <summary>
        /// changes the current state
        /// </summary>
        public void ChangeState<TR>() where TR : State<T>
        {
            // avoid changing to the same state
            var newType = typeof(TR);
            if (this.CurrentState.GetType() == newType)
                return;

            this.NextState = this.states[newType];
        }

        public void SetNextState<TR>() where TR : State<T>
        {
            var nextType = typeof(TR);
            this.NextState = this.states[nextType];

        }
    }


}