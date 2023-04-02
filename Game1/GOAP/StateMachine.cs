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


    public class StateMachine<T>//In this system, T is an object of the GOAPPersonState class. 
    {
        public event Action OnStateChanged;

        public State<T> PreviousState { get; private set; }

        public State<T> CurrentState { get; private set; }

        public State<T> NextState { get; private set; }

        protected T Context;

        /// <summary>
        ///  Dictionary containing all of machine’s possible finite states. 
        /// </summary>
        private readonly Dictionary<Type, State<T>> states = new Dictionary<Type, State<T>>();


        /// <summary>
        ///  Constructor for new finite state machine. Constructor assigns context of machine and sets up initial state provided as argument, adding it to the machine, setting it as the current state and calling the Begin method on the state. 
        /// </summary>
        /// <param name="context">GOAP Person State object</param>
        /// <param name="initialState"> initial machine state</param>
        public StateMachine(T context, State<T> initialState)
        {
            this.Context = context;

            // setup our initial state
            this.AddState(initialState);
            this.CurrentState = initialState;
            initialState.Begin();
        }

        /// <summary>
        /// Adds a state to the state machine. Calls SetMachineAndContext method on state, sending FSM and FSM context as arguments. Adds new state to machine’s states dictionary where the key is the type of the state object and the value is the state object itself. As keys in a dictionary must be unique – only one state of each state type can be added e.g. machine cannot have two instances of the Idle state. 
        /// </summary>
        public void AddState(State<T> state)
        {
            state.SetMachineAndContext(this, this.Context);
            this.states[state.GetType()] = state;
        }

        /// <summary>
        /// Ticks the state machine using the game time supplied as a parameter. Calls the update method on the current state. If the transitionOnNextTick boolean property of the current state is true, the machine will then transition into its next state and call the Begin method on this new state. 
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
        ///  Changes machine state to state with type supplied as type paramater. Checks if and prevents attempting to change to the same state. Sets this as the machine’s NextState. 
        /// </summary>
        public void ChangeState<TR>() where TR : State<T>
        {
            // avoid changing to the same state
            var newType = typeof(TR);
            if (this.CurrentState.GetType() == newType)
                return;

            this.NextState = this.states[newType];
        }


        /// <summary>
        /// Sets the NextState attribute of the machine. This is state into which the machine will transition into next. 
        /// </summary>
        /// <typeparam name="TR"></typeparam>
        public void SetNextState<TR>() where TR : State<T>
        {
            var nextType = typeof(TR);
            this.NextState = this.states[nextType];

        }
    }


}