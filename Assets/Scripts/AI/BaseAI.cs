using UnityEngine;
using System.Collections;

public class BaseAI<T> : MonoBehaviour where T : MonoBehaviour
{
	public delegate void stateFunc();

	protected stateFunc currentState;

	protected virtual void Awake()
	{
		currentState = IdleState;
	}

	protected virtual void Update()
	{
		currentState();
	}

	public void ChangeState(stateFunc newStateFunc)
	{
		currentState = newStateFunc;
	}

	protected virtual void IdleState()
	{
		//do nothing?
	}
}
