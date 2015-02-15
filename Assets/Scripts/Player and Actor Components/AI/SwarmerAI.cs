using UnityEngine;
using System.Collections;
using OgreToast.Utility;
using OgreToast.Attributes;

[RequireComponent(typeof(FlyingComponent), typeof(SpriteRenderer), typeof(HealthComponent))]
public class SwarmerAI : BaseAI<SwarmerAI>
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
	
	#region sensing_public_vars
	[Header("Sensing")]
	public bool CheckForSomething = true;
	public bool ShowSensorInEditor = true;
	public Vector2 SensorCenter = Vector2.zero;
	public float SensorRadius = 100f;
	public int MaxObjectsToFind = 1;
	public LayerMask SensorLayersToCheck = 0;
	#endregion
	
	#region attacking_public_vars
	[Header("Fighting")]
	public bool CanAttack = true;
	public float TimeBeforeAttackExpires = 10f;
	public float HitStunTime = 0.1f;
	public float HitPushForce = 100f;
	#endregion

	private Vector3 _moveTarget;
	private FlyingComponent _mover;
	private SimpleTimer _pauseTimer;
	
	private Collider2D[] _sensedColliders;
	private Vector2 _realSensorCenter;
	private SimpleTimer _attackExpireTimer;
	private WeaponComponent _weapon = null;
	private HealthComponent _health;
	private SimpleTimer _stunTimer;

	protected bool _isDying = false;

	private void Start()
	{
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
			currentState = PatrolAndAttackState;
		}
		
		if(MaxObjectsToFind <= 0)
		{
			MaxObjectsToFind = 1;
		}
		_sensedColliders = new Collider2D[MaxObjectsToFind];
		_realSensorCenter = (Vector2)transform.position + SensorCenter;
		
		_attackExpireTimer = new SimpleTimer(TimeBeforeAttackExpires);
		_weapon = transform.FindChild("gun").gameObject.GetComponent<WeaponComponent>();
		_stunTimer = new SimpleTimer(HitStunTime);
		_health = GetComponent<HealthComponent>();
		_health.Dead += OnDead;
		_health.Hit += OnHit;
		
		if(Mathf.RoundToInt(Mathf.Cos(transform.rotation.eulerAngles.y * Mathf.Deg2Rad)) == -1)
		{
			SensorCenter.x *= -1;
		}

		rigidbody2D.gravityScale = 0f;
	}

	private void OnDrawGizmosSelected()
	{
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

		if(ShowSensorInEditor)
		{
			Vector3 center = SensorCenter;
			if(Mathf.RoundToInt(Mathf.Cos(transform.rotation.eulerAngles.y * Mathf.Deg2Rad)) == -1 && !Application.isPlaying)
			{
				center.x = -1 * SensorCenter.x;
			}
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(transform.position + center, SensorRadius);
		}
	}

	protected virtual void OnCollisionEnter2D(Collision2D collision)
	{
		GameObject collisionGO = collision.gameObject;
		_mover.CheckCollisionEnter(collisionGO);
		_health.CheckCollisionEnter(collisionGO);
	}
	
	protected virtual void OnCollisionExit2D(Collision2D collision)
	{
		_mover.CheckCollisionExit(collision);
	}

	protected virtual void PatrolAndAttackState()
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

	protected virtual void DeadState()
	{

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
	
	protected virtual void OnHit(object sender, Vector2 hitDirection)
	{
		if(!_stunTimer.IsRunning)
		{
			rigidbody2D.AddForce(HitPushForce * hitDirection, ForceMode2D.Impulse);
			_stunTimer.Stop();
			_stunTimer.TargetTime = HitStunTime;
			_stunTimer.Start();
		}
	}
	
	protected virtual void OnDead(object sender, System.EventArgs e)
	{
		if(!_isDying)
		{
			_isDying = true;
			currentState = DeadState;
			rigidbody2D.velocity = Vector2.zero;
			Renderer[] renderers = GetComponentsInChildren<Renderer>();
			Collider2D[] colliders = GetComponentsInChildren<Collider2D>();
			for(int i=0;i<renderers.Length;++i)
			{
				renderers[i].enabled = false;
			}
			for(int i=0;i<colliders.Length;++i)
			{
				colliders[i].enabled = false;
			}
			GameManager.Instance.AddToDestroyQueue(gameObject);
		}
	}
}
