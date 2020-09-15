﻿using System;
using System.Collections.Generic;

namespace Avoidance
{
    /// <summary>
    /// Builder of a concrete state.
    /// </summary>
    /// <typeparam name="TState">Type that determines states.</typeparam>
    /// <typeparam name="TEvent">Type that determines events.</typeparam>
    public class StateBuilder<TState, TEvent>
        where TState : IComparable
        where TEvent : IComparable
    {
        private Delegate onEntry;
        private Delegate onExit;
        internal TState State { get; }

        private Dictionary<TEvent, TransitionBuilder<TState, TEvent>> transitions = new Dictionary<TEvent, TransitionBuilder<TState, TEvent>>();
        private StateMachineBuilder<TState, TEvent> parent;

        internal StateBuilder(StateMachineBuilder<TState, TEvent> parent, TState state)
        {
            this.parent = parent;
            State = state;
        }

        /// <inheritdoc cref="StateMachineBuilder{TState, TEvent}.In(TState)"/>
        public StateBuilder<TState, TEvent> In(TState state) => parent.In(state);

        /// <inheritdoc cref="StateMachineBuilder{TState, TEvent}.Build"/>
        public StateMachine<TState, TEvent> Build() => parent.Build();

        /// <summary>
        /// Determines an action to execute on entry to this state.
        /// </summary>
        /// <param name="action">Action to execute.</param>
        /// <returns><see cref="this"/>.</returns>
        /// <exception cref="InvalidOperationException">Thrown when this state already has a registered entry action.</exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="action"/> is <see langword="null"/>.</exception>
        public StateBuilder<TState, TEvent> ExecuteOnEntry(Action<object> action)
        {
            if (!(onEntry is null))
                throw new InvalidOperationException("Already has a registered entry action");
            if (action is null)
                throw new ArgumentNullException(nameof(action));

            onEntry = action;
            return this;
        }

        /// <summary>
        /// Determines an action to execute on exit of this state.
        /// </summary>
        /// <param name="action">Action to execute.</param>
        /// <returns><see cref="this"/>.</returns>
        /// <exception cref="InvalidOperationException">Thrown when this state already has a registered exit action.</exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="action"/> is <see langword="null"/>.</exception>
        public StateBuilder<TState, TEvent> ExecuteOnExit(Action<object> action)
        {
            if (!(onExit is null))
                throw new InvalidOperationException("Already has a registered exit action");
            if (action is null)
                throw new ArgumentNullException(nameof(action));

            onExit = action;
            return this;
        }

        /// <summary>
        /// Add a behaviour that is executed on an event.
        /// </summary>
        /// <param name="event">Raised event.</param>
        /// <returns>Transition builder.</returns>
        /// <exception cref="ArgumentException">Thrown when this state already has registered <paramref name="event"/>.</exception>
        public MasterTransitionBuilder<TState, TEvent> On(TEvent @event)
        {
            if (transitions.ContainsKey(@event))
                throw new ArgumentException($"The event {transitions} was already registered for this state.");

            MasterTransitionBuilder<TState, TEvent> builder = new MasterTransitionBuilder<TState, TEvent>(this);
            transitions.Add(@event, builder);
            return builder;
        }

        /// <summary>
        /// Ignores an event.
        /// </summary>
        /// <param name="event">Event to ignore.</param>
        /// <returns><see cref="this"/>.</returns>
        /// <exception cref="ArgumentException">Thrown when this state already has registered <paramref name="event"/>.</exception>
        public StateBuilder<TState, TEvent> Ignore(TEvent @event)
        {
            if (transitions.ContainsKey(@event))
                throw new ArgumentException($"The event {transitions} was already registered for this state.");

            transitions.Add(@event, null);
            return this;
        }

        internal State<TState, TEvent> ToState(TState state, ListSlot<Transition<TState, TEvent>> transitions, Dictionary<TState, int> statesMap)
        {
            Dictionary<TEvent, int> trans = new Dictionary<TEvent, int>();

            foreach ((TEvent @event, TransitionBuilder<TState, TEvent> builder) in this.transitions)
            {
                int slot = transitions.Reserve();
                trans.Add(@event, slot);
                transitions.Store(builder.ToTransition(transitions, statesMap), slot);
            }

            return new State<TState, TEvent>(state, onEntry, onExit, trans);
        }
    }
}