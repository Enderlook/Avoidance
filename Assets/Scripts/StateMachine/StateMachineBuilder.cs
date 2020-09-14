﻿using System;
using System.Collections.Generic;

namespace Avoidance
{
    /// <summary>
    /// Builder of an state machine.
    /// </summary>
    /// <typeparam name="TState">Type that determines states.</typeparam>
    /// <typeparam name="TEvent">Type that determines events.</typeparam>
    public class StateMachineBuilder<TState, TEvent>
        where TState : IComparable
        where TEvent : IComparable
    {
        private Dictionary<TState, StateBuilder<TState, TEvent>> states = new Dictionary<TState, StateBuilder<TState, TEvent>>();
        private bool hasInitialState;
        private TState initialState;

        /// <summary>
        /// Add a new state.
        /// </summary>
        /// <param name="state">State to add.</param>
        /// <returns>State builder.</returns>
        /// <exception cref="ArgumentException">Thrown when the <paramref name="state"/> was already registered.</exception>
        public StateBuilder<TState, TEvent> In(TState state)
        {
            if (states.ContainsKey(state))
                throw new ArgumentException($"The state {state} was already registered.");

            StateBuilder<TState, TEvent> builder = new StateBuilder<TState, TEvent>(this, state);
            states.Add(state, builder);
            return builder;
        }

        /// <summary>
        /// Determines the initial state of the state machine.
        /// </summary>
        /// <param name="state">Initial state.</param>
        /// <returns><see cref="this"/>.</returns>
        /// <exception cref="InvalidOperationException">Throw when the initial state was already registered.</exception>
        public StateMachineBuilder<TState, TEvent> SetInitialState(TState state)
        {
            if (hasInitialState)
                throw new InvalidOperationException("Already has a registered initial state.");
            hasInitialState = true;
            initialState = state;
            return this;
        }

        /// <summary>
        /// Convert the builder into an immutable state machinee.
        /// </summary>
        /// <returns>Immutable state machine.</returns>
        /// <exception cref="InvalidOperationException">Thrown when there is no registered initial state.<br/>
        /// Or when there are no registered states.<br/>
        /// Or when a transition refers to a non-registered state.</exception>
        public StateMachine<TState, TEvent> Build()
        {
            if (!hasInitialState)
                throw new InvalidOperationException("The state machine builder doesn't have registered an initial state.");

            if (this.states.Count == 0)
                throw new InvalidOperationException("The state machine builder doesn't have registered any state.");
            Dictionary<TState, int> statesMap = new Dictionary<TState, int>();
            int i = 0;
            foreach (KeyValuePair<TState, StateBuilder<TState, TEvent>> kv in this.states)
                statesMap.Add(kv.Key, i++);

            List<State<TState, TEvent>> states = new List<State<TState, TEvent>>();
            ListSlot<Transition<TState, TEvent>> transitions = new ListSlot<Transition<TState, TEvent>>(new List<Transition<TState, TEvent>>());
            foreach ((TState state, StateBuilder<TState, TEvent> builder) in this.states)
                states.Add(builder.ToState(state, transitions, statesMap));

            return new StateMachine<TState, TEvent>(TryGetStateIndex(initialState, statesMap), states, transitions.Extract());
        }

        /// <summary>
        /// Extract the index of an state.
        /// </summary>
        /// <param name="state">State to query.</param>
        /// <param name="statesMap">Possible states.</param>
        /// <returns>Index of the given state.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the state <paramref name="state"/> is not registered.</exception>
        internal static int TryGetStateIndex(TState state, Dictionary<TState, int> statesMap)
        {
            if (statesMap.TryGetValue(state, out int index))
                return index;
            throw new InvalidOperationException("Transition has a goto to an unregistered state.");
        }
    }
}