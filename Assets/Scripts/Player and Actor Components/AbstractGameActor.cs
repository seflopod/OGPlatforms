using UnityEngine;
using System.Collections;

public abstract class AbstractGameActor : MonoBehaviour
{
	protected AbstractMover _mover;
	protected AbstractAttacker _attacker;
	protected AbstractAI _ai;

	#region monobehaviour
	protected virtual void Start()
	{
		_mover = GetComponent<AbstractMover>();
		_ai = GetComponent<AbstractAI>();
		_attacker = GetComponent<AbstractAttacker>();
	}
	#endregion
}
