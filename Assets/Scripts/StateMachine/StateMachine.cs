using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DEEP.StateMachine
{
    public class StateMachine <T>{

        public T Owner;
        public State <T> currentState{get; private set;}

        public StateMachine(T owner){
            this.Owner =owner;
            currentState =  null;
        }

        public void ChangeState(State<T> newState){
            
            if (newState != null)
            {
                if (currentState != null)
                    currentState.ExitState(Owner);
                currentState =  newState;
                currentState.EnterState(Owner);
            }
        }

        public void update(){
            if (currentState != null)
                currentState.UpdateState(Owner);
        }

    }

    public class State <T>{

        public abstract void EnterState(T owner);

        public abstract void ExitState(T owner);
        public abstract void UpdateState(T owner);
    }
}