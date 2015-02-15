using UnityEngine;
using System.Collections;
using OgreToast.Utility;
using OgreToast.Attributes;

[RequireComponent(typeof(FlyingComponent), typeof(SpriteRenderer), typeof(HealthComponent))]
public class SwarmerAI : AbstractAI
{
	#region patrol_public_vars
	[Header("Patrolling")]
	public bool DoPatrolAtStart = true;
	public bool ShowEndPointsInEditor = true;
	public bool IsMovingRight = true;
	public bool IsMovingUp = true;

	[TwoVarRange("Horizontal Dist Range", "The range of values allowed for determing the delta to the next movepoint on the x-axis", 16f, 128f)]
	public Vector2 HorizontalDistRange = new Vector2(16f, 128f);

	[TwoVarRange("Vertical Dist Range", "The range of values allowed for determing the delta to the next movepoint on the y-axis", 16f, 96f)]
	public Vector2 VerticalDistRange = new Vector2(16f, 64f);

	public float MinScreenPct = 0.1f;
	public float MaxScreenPct = 0.9f;
	public float PatrolSpeed = 25f;
	public float PauseTimeAtEdge = 0.1f;
	#endregion

	private Vector3 _moveTarget;
	private FlyingComponent _mover;
	private SimpleTimer _pauseTimer;

	#region monobehaviour
	protected override void Start()
	{
		base.Start();
		Vector3 moveDelta = new Vector3(((IsMovingRight)?1:-1) * Random.Range(HorizontalDistRange.x, HorizontalDistRange.y),
		                                ((IsMovingUp)?1:-1) * Random.Range(VerticalDistRange.x, VerticalDistRange.y),
		                                0f);
		Vector3 viewportPos = Camera.main.WorldToViewportPoint(transform.position + moveDelta);
		viewportPos.x = Mathf.Min(Mathf.Max(viewportPos.x, MinScreenPct), MaxScreenPct);
		viewportPos.y = Mathf.Min(Mathf.Max(viewportPos.y, MinScreenPct), MaxScreenPct);
		_moveTarget = Camera.main.ViewportToWorldPoint(viewportPos);
		_mover = GetComponent<FlyingComponent>();
		_pauseTimer = new SimpleTimer(PauseTimeAtEdge);
		_pauseTimer.Start();
		if(DoPatrolAtStart)
		{
			ChangeState(AttackState);
		}

		rigidbody2D.gravityScale = 0f;
	}

	protected override void OnDrawGizmosSelected()
	{
		base.OnDrawGizmosSelected();
		if(ShowEndPointsInEditor)
		{
			float midPct = MinScreenPct + (MaxScreenPct - MinScreenPct) / 2f;
			Vector3 leftCenter = Camera.main.ViewportToWorldPoint(new Vector3(MinScreenPct, midPct, Camera.main.transform.position.z));
			Vector3 rightCenter = Camera.main.ViewportToWorldPoint(new Vector3(MaxScreenPct, midPct, Camera.main.transform.position.z));
			Vector3 size = new Vector3(collider2D.bounds.size.x, (MaxScreenPct - MinScreenPct) * Camera.main.pixelHeight, 1f);
			Gizmos.color = new Color(1f, 0.5f, 0f, 0.5f);
			SpriteRenderer sr = GetComponent<SpriteRenderer>();
			Gizmos.DrawCube(leftCenter, size);
			Gizmos.DrawCube(rightCenter, size);
		}
	}

	protected override void OnCollisionEnter2D(Collision2D collision)
	{
		_mover.CheckCollisionEnter(collision.gameObject);
		base.OnCollisionEnter2D(collision);
	}
	
	protected override void OnCollisionExit2D(Collision2D collision)
	{
		_mover.CheckCollisionExit(collision);
		base.OnCollisionEnter2D(collision);
	}
	#endregion

	#region states
	protected override void AttackState()
	{
		if(Mathf.Abs(transform.position.x - _moveTarget.x) < 1f && !_pauseTimer.IsRunning)
		{
			_pauseTimer.Start();

			//always switch between moving up and moving down
			IsMovingUp = !IsMovingUp;
			Vector3 viewportPos;
			if(IsMovingRight)
			{
				viewportPos = Camera.main.WorldToViewportPoint(collider2D.bounds.max);
				IsMovingRight = viewportPos.x < MaxScreenPct;
			}
			else
			{
				viewportPos = Camera.main.WorldToViewportPoint(collider2D.bounds.min);
				IsMovingRight = viewportPos.x <= MinScreenPct;
			}
			Vector3 moveDelta = new Vector3(((IsMovingRight)?1:-1) * Random.Range(HorizontalDistRange.x, HorizontalDistRange.y),
			                                ((IsMovingUp)?1:-1) * Random.Range(VerticalDistRange.x, VerticalDistRange.y),
			                                0f);
			viewportPos = Camera.main.WorldToViewportPoint(transform.position + moveDelta);
			viewportPos.x = Mathf.Min(Mathf.Max(viewportPos.x, MinScreenPct), MaxScreenPct);
			viewportPos.y = Mathf.Min(Mathf.Max(viewportPos.y, MinScreenPct), MaxScreenPct);
			if(viewportPos.y == MaxScreenPct && IsMovingUp)
			{
				float otherBound = (MaxScreenPct-MinScreenPct)/2f;
				viewportPos.y = (otherBound < MinScreenPct) ? Random.Range(otherBound, MinScreenPct) : Random.Range(MinScreenPct, otherBound);
				IsMovingUp = false;
			}
			else if(viewportPos.y == MinScreenPct && !IsMovingUp)
			{
				float otherBound = (MaxScreenPct-MinScreenPct)/2f;
				viewportPos.y = (otherBound < MaxScreenPct) ? Random.Range(otherBound, MaxScreenPct) : Random.Range(MaxScreenPct, otherBound);
				IsMovingUp = true;
			}
			_moveTarget = Camera.main.ViewportToWorldPoint(viewportPos);
		}
		else if(_pauseTimer.IsExpired)
		{
			_pauseTimer.Stop();
			_mover.BaseSpeed = PatrolSpeed;
			float xMod = (IsMovingRight) ? 1f : -1f;
			float yMod = (IsMovingUp) ? 1f : -1f;
			_mover.Fly(xMod, yMod);
		}
		//check for player and change state as needed.
		if(HasSensedSomething() && CanAttack)
		{
			_attackExpireTimer.Stop();
			//aim and then fire
			Transform target = _sensedColliders[0].transform;
			Vector3 targetPos = target.position;
			if(target.rigidbody2D != null)
			{
				targetPos += (Vector3)target.rigidbody2D.velocity * Time.deltaTime;
			}
			
			Vector2 dirToTarget = (targetPos - transform.position).normalized;
			dirToTarget.x = Mathf.RoundToInt(dirToTarget.x);
			dirToTarget.y = Mathf.RoundToInt(dirToTarget.y);
			if(_weapon != null)
			{
				_weapon.Aim(dirToTarget);
				_weapon.Fire();
			}
		}
	}
	#endregion
}
