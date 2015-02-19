using UnityEngine;
using OgreToast.Utility;

[RequireComponent(typeof(BasicMover), typeof(SpriteRenderer), typeof(PatrolAttacker))]
public class PatrolAI : AbstractAI
{
	public bool ShowViewInEditor = true;
	public Vector3 ViewingStart = Vector3.zero;
	public float ViewAngle = 75f;
	public float ViewDistance = 2f;
	public LayerMask TargetLayerMask = 0;

	private PatrolAttacker _attacker;
	private BasicMover _mover;
	private Transform _target;

	#region monobehaviour
	private void OnDrawGizmosSelected()
	{
		if(ShowViewInEditor)
		{
			Gizmos.color = new Color(0f, 1f, 0f, 0.75f);
			Vector3 viewStart = transform.TransformPoint(ViewingStart);
			for(float i=-ViewAngle/2;i<ViewAngle/2;i+=1)
			{
				Gizmos.DrawLine(viewStart, viewStart + Mathf.Sign(transform.localScale.x) * ViewDistance * (new Vector3(Mathf.Cos(Mathf.Deg2Rad * i), Mathf.Sin(Mathf.Deg2Rad * i), transform.position.z)));
			}
		}
	}
	#endregion

	#region states
	public override void IdleState()
	{
		base.IdleState();
		bool whatevs = canSeePlayer();
	}

	public override void DeathState()
	{
		base.DeathState();
	}

	public void PatrolState()
	{

	}

	public void AttackState()
	{
		float yMod = 0f;
		if(_target.position.y > transform.position.y)
		{
			yMod = 1f;
		}
		else if(_target.position.y < transform.position.y)
		{
			yMod = -1f;
		}

		float xMod = 0f;
		if(_target.position.x > transform.position.x)
		{
			xMod = 1f;
		}
		else if(_target.position.x < transform.position.x)
		{
			xMod = -1f;
		}
	}

	private bool canSeePlayer()
	{
		Vector3 center = transform.TransformPoint(ViewingStart);
		Collider2D player = Physics2D.OverlapCircle(transform.TransformPoint(ViewingStart), ViewDistance);
		if(player != null)
		{
			Vector2 myDirection = Mathf.Sign(transform.localScale.x) * Vector2.right;
			Vector2 playerDir = (player.transform.position - transform.position).normalized;
			float angle = Mathf.Acos(Vector2.Dot(myDirection, playerDir));
			Debug.Log(Mathf.Rad2Deg * angle);
		}
		return true;
	}
	#endregion
//	#region patrol_public_vars
//	[Header("Patrolling")]
//	public bool DoPatrolAtStart = true;
//	public bool ShowNodesInEditor = true;
//	public Vector3 PatrolNode1;
//	public Vector3 PatrolNode2;
//	public float PatrolSpeed = 25f;
//	public float PauseAtNodeTime = 0.5f;
//	#endregion
//
//	private Vector3 _moveTarget;
//	private MovementComponent _mover;
//	private SimpleTimer _pauseTimer;
//
//	#region monobehaviour
//	protected override void Start()
//	{
//		base.Start();
//		_moveTarget = PatrolNode2;
//		_mover = GetComponent<MovementComponent>();
//		_pauseTimer = new SimpleTimer(PauseAtNodeTime);
//		_pauseTimer.Start();
//		if(DoPatrolAtStart)
//		{
//			ChangeState(PatrolState);
//		}
//		else
//		{
//			ChangeState(IdleState);
//		}
//	}
//
//	protected override void OnDrawGizmosSelected()
//	{
//		base.OnDrawGizmosSelected();
//		if(ShowNodesInEditor)
//		{
//			Gizmos.color = new Color(1f, 0.5f, 0f, 0.5f);
//			SpriteRenderer sr = GetComponent<SpriteRenderer>();
//			Gizmos.DrawCube(PatrolNode1, sr.bounds.size);
//			Gizmos.DrawLine(PatrolNode1, PatrolNode2);
//			Gizmos.DrawCube(PatrolNode2, sr.bounds.size);
//		}
//	}
//
//	protected override void OnCollisionEnter2D(Collision2D collision)
//	{
//		_mover.CheckCollisionEnter(collision.gameObject);
//		base.OnCollisionEnter2D(collision);
//	}
//	
//	protected override void OnCollisionExit2D(Collision2D collision)
//	{
//		_mover.CheckCollisionExit(collision);
//		base.OnCollisionEnter2D(collision);
//	}
//	#endregion
//
//	#region states
//	protected override void IdleState()
//	{
//		//check for player and change state as needed.
//		if(HasSensedSomething() && CanAttack)
//		{
//			rigidbody2D.velocity = Vector2.zero;
//			ChangeState(AttackState);
//		}
//	}
//
//	protected virtual void PatrolState()
//	{
//		if(Mathf.Abs(transform.position.x - _moveTarget.x) < 1f && !_pauseTimer.IsRunning)
//		{
//			_pauseTimer.Start();
//			_moveTarget = (_moveTarget == PatrolNode1) ? PatrolNode2 : PatrolNode1;
//		}
//		else if(_pauseTimer.IsExpired)
//		{
//			_pauseTimer.Stop();
//			_mover.BaseSpeed = PatrolSpeed;
//			float xMod = (transform.position.x < _moveTarget.x) ? 1f : -1f;
//			if((SensorCenter.x < 0 && xMod > 0) || (SensorCenter.x > 0 && xMod < 0))
//			{
//				SensorCenter.x *= -1;
//			}
//			_mover.Run(xMod);
//			_realSensorCenter = (Vector2)transform.position + SensorCenter;
//		}
//
//		//check for player and change state as needed.
//		if(HasSensedSomething() && CanAttack)
//		{
//			rigidbody2D.velocity = Vector2.zero;
//			ChangeState(AttackState);
//		}
//	}
//
//	protected override void AttackState()
//	{
//		//keep checking for player
//		if(!HasSensedSomething())
//		{
//			if(!_attackExpireTimer.IsRunning)
//			{
//				_attackExpireTimer.Start();
//			}
//			else if(_attackExpireTimer.IsExpired)
//			{
//				ChangeState(PatrolState);
//				_attackExpireTimer.Stop();
//			}
//		}
//		else
//		{
//			_attackExpireTimer.Stop();
//			//aim and then fire
//			Transform target = _sensedColliders[0].transform;
//			if(transform.rotation.y == 0f && target.position.x < transform.position.x)
//			{
//				transform.rotation = Quaternion.Euler(0f, 180f, 0f);
//			}
//			else if(transform.rotation.y == 180f && target.position.x > transform.position.x)
//			{
//				transform.rotation = Quaternion.Euler(0f, 0f, 0f);
//			}
//
//			Vector2 dirToTarget = (target.position - transform.position).normalized;
//			if(dirToTarget.y > 0.7f && dirToTarget.x != 0f && !_mover.IsJumping)
//			{
//				_mover.Jump();
//				dirToTarget = (target.position - transform.position).normalized;
//			}
//			dirToTarget.x = Mathf.RoundToInt(dirToTarget.x);
//			dirToTarget.y = Mathf.RoundToInt(dirToTarget.y);
//			if(_weapon != null)
//			{
//				_weapon.Aim(dirToTarget);
//				_weapon.Fire(rigidbody2D.velocity);
//			}
//		}
//	}
//	#endregion
}
