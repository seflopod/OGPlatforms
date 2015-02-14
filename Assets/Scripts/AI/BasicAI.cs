﻿using UnityEngine;
using OgreToast.Utility;

[RequireComponent(typeof(MovementComponent), typeof(SpriteRenderer), typeof(HealthComponent))]
public class BasicAI : BaseAI<BasicAI>
{
	#region patrol_public_vars
	[Header("Patrolling")]
	public bool DoPatrolAtStart = true;
	public bool ShowNodesInEditor = true;
	public Vector3 PatrolNode1;
	public Vector3 PatrolNode2;
	public float PatrolSpeed = 25f;
	public float PauseAtNodeTime = 0.5f;
	#endregion

	[Space(16)]

	#region sensing_public_vars
	[Header("Sensing")]
	public bool CheckForSomething = true;
	public bool ShowSensorInEditor = true;
	public Vector2 SensorCenter = Vector2.zero;
	public float SensorRadius = 0.5f;
	public int MaxObjectsToFind = 1;
	public LayerMask SensorLayersToCheck = 0;
	#endregion

	[Space(16)]

	#region attacking_public_vars
	[Header("Fighting")]
	public bool CanAttack = true;
	public float TimeBeforeAttackExpires = 10f;
	#endregion

	private Vector3 _moveTarget;
	private MovementComponent _moveComponent;
	private SimpleTimer _pauseTimer;

	private Collider2D[] _sensedColliders;
	private Vector2 _realSensorCenter;
	private SimpleTimer _attackExpireTimer;
	private WeaponComponent _weapon = null;
	private HealthComponent _health;

	private void Start()
	{
		_moveTarget = PatrolNode2;
		_moveComponent = GetComponent<MovementComponent>();
		_pauseTimer = new SimpleTimer(PauseAtNodeTime);
		_pauseTimer.Start();
		if(DoPatrolAtStart)
		{
			currentState = PatrolState;
		}

		if(MaxObjectsToFind <= 0)
		{
			MaxObjectsToFind = 1;
		}
		_sensedColliders = new Collider2D[MaxObjectsToFind];
		_realSensorCenter = (Vector2)transform.position + SensorCenter;

		_attackExpireTimer = new SimpleTimer(TimeBeforeAttackExpires);
		_weapon = transform.FindChild("gun").gameObject.GetComponent<WeaponComponent>();
		_health = GetComponent<HealthComponent>();
		_health.Dead += OnDead;
	}

	protected void OnDrawGizmosSelected()
	{
		if(ShowNodesInEditor)
		{
			Gizmos.color = new Color(1f, 0.5f, 0f, 0.5f);
			SpriteRenderer sr = GetComponent<SpriteRenderer>();
			Gizmos.DrawCube(PatrolNode1, sr.bounds.size);
			Gizmos.DrawLine(PatrolNode1, PatrolNode2);
			Gizmos.DrawCube(PatrolNode2, sr.bounds.size);
		}

		if(ShowSensorInEditor)
		{
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(transform.position + (Vector3)SensorCenter, SensorRadius);
		}
	}

	protected void PatrolState()
	{
		if(Mathf.Abs(transform.position.x - _moveTarget.x) < 1f && !_pauseTimer.IsRunning)
		{
			_pauseTimer.Start();
			_moveTarget = (_moveTarget == PatrolNode1) ? PatrolNode2 : PatrolNode1;
		}
		else if(_pauseTimer.IsExpired)
		{
			_pauseTimer.Stop();
			_moveComponent.BaseSpeed = PatrolSpeed;
			float xMod = (transform.position.x < _moveTarget.x) ? 1f : -1f;
			if((SensorCenter.x < 0 && xMod > 0) || (SensorCenter.x > 0 && xMod < 0))
			{
				SensorCenter.x *= -1;
			}
			_moveComponent.Run(xMod);
			_realSensorCenter = (Vector2)transform.position + SensorCenter;
		}

		//check for player and change state as needed.
		if(HasSensedSomething() && CanAttack)
		{
			rigidbody2D.velocity = Vector2.zero;
			currentState = AttackState;
		}
	}

	protected void AttackState()
	{
		//keep checking for player
		if(!HasSensedSomething())
		{
			if(!_attackExpireTimer.IsRunning)
			{
				_attackExpireTimer.Start();
			}
			else if(_attackExpireTimer.IsExpired)
			{
				currentState = PatrolState;
				_attackExpireTimer.Stop();
			}
		}
		else
		{
			_attackExpireTimer.Stop();
			//aim and then fire
			Transform target = _sensedColliders[0].transform;
			if(transform.rotation.y == 0f && target.position.x < transform.position.x)
			{
				transform.rotation = Quaternion.Euler(0f, 180f, 0f);
			}
			else if(transform.rotation.y == 180f && target.position.x > transform.position.x)
			{
				transform.rotation = Quaternion.Euler(0f, 0f, 0f);
			}

			Vector2 dirToTarget = (target.position - transform.position).normalized;
			if(dirToTarget.y > 0.7f && dirToTarget.x != 0f && !_moveComponent.IsJumping)
			{
				Debug.Log("Jump");
				_moveComponent.Jump();
				dirToTarget = (target.position - transform.position).normalized;
			}
			dirToTarget.x = Mathf.RoundToInt(dirToTarget.x);
			dirToTarget.y = Mathf.RoundToInt(dirToTarget.y);
			if(_weapon != null)
			{
				_weapon.Aim(dirToTarget);
				_weapon.Fire();
			}
		}
	}

	protected bool HasSensedSomething()
	{
		if(CheckForSomething)
		{
			int numSensed = Physics2D.OverlapCircleNonAlloc(_realSensorCenter, SensorRadius, _sensedColliders, SensorLayersToCheck.value);
			return numSensed != 0;
		}
		return false;
	}

	protected void OnDead(object sender, System.EventArgs e)
	{
		Debug.Log("AI Dead");
	}
}
