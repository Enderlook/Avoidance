using System;
using System.Collections.Generic;

namespace Avoidance
{
    internal struct State<TState, TEvent>
        where TState : IComparable
        where TEvent : IComparable
    {
        public TState state;
        public Delegate onEntry;
        public Delegate onExit;
        public Dictionary<TEvent, int> transitions;

        public State(TState state, Delegate onEntry, Delegate onExit, Dictionary<TEvent, int> transitions)
        {
            this.state = state;
            this.onEntry = onEntry;
            this.onExit = onExit;
            this.transitions = transitions;
        }
    }
}