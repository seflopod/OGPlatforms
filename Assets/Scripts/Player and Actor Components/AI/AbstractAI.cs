using UnityEngine;
using System.Collections;
using OgreToast.Utility;

[RequireComponent(typeof(SpriteRenderer), typeof(HealthComponent))]
public abstract class AbstractAI : MonoBehaviour
{
	public delegate void stateFunc();
	public delegate void StateChangeHandler(string oldStateName, string newStateName);

	public event StateChangeHandler StateChange;

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

	//private to force children to use ChangeState
	private stateFunc _currentState;

	protected Collider2D[] _sensedColliders;
	protected Vector2 _realSensorCenter;
	protected SimpleTimer _attackExpireTimer;
	protected WeaponComponent _weapon = null;
	protected HealthComponent _health;
	protected SimpleTimer _stunTimer;

	protected bool isDying = false;

	#region monobehaviour
	protected virtual void Awake()
	{
		_currentState = IdleState;
	}

	protected virtual void Start()
	{
		if(MaxObjectsToFind <= 0)
		{
			MaxObjectsToFind = 1;
		}
		_sensedColliders = new Collider2D[MaxObjectsToFind];

		
		_attackExpireTimer = new SimpleTimer(TimeBeforeAttackExpires);
		Transform gunChild = transform.FindChild("gun");
		if(gunChild != null)
		{
			_weapon = gunChild.gameObject.GetComponent<WeaponComponent>();
		}

		_stunTimer = new SimpleTimer(HitStunTime);
		_health = GetComponent<HealthComponent>();
		_health.Dead += OnDead;
		_health.Hit += OnHit;
		
		if(Mathf.RoundToInt(Mathf.Cos(transform.rotation.eulerAngles.y * Mathf.Deg2Rad)) == -1)
		{
			SensorCenter.x *= -1;
		}
		_realSensorCenter = (Vector2)transform.position + SensorCenter;
	}

	protected virtual void Update()
	{
		if(!_stunTimer.IsRunning)
		{
			_currentState();
		}
	}

	protected virtual void LateUpdate()
	{
		if(_stunTimer.IsExpired)
		{
			_stunTimer.Stop();
		}
	}

	protected virtual void OnCollisionEnter2D(Collision2D collision)
	{
		GameObject collisionGO = collision.gameObject;
		_health.CheckCollisionEnter(collisionGO);
	}
	
	protected virtual void OnCollisionExit2D(Collision2D collision)
	{
		//do nothing, provided for symmetry
	}

	protected virtual void OnDrawGizmosSelected()
	{
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
	#endregion

	#region states
	public void ChangeState(stateFunc newStateFunc)
	{
		if(newStateFunc == null)
		{
			return;
		}
		string oldStateName = _currentState.Method.Name;
		_currentState = newStateFunc;
		StateChangeHandler handler = StateChange;
		if(handler != null)
		{
			StateChange(oldStateName, _currentState.Method.Name);
		}
	}

	protected virtual void IdleState()
	{
		//do nothing?
	}

	protected abstract void AttackState();

	protected virtual void DeadState()
	{
		if(!isDying)
		{
			isDying = true;
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

	protected bool HasSensedSomething()
	{
		if(CheckForSomething)
		{
			int numSensed = Physics2D.OverlapCircleNonAlloc(_realSensorCenter, SensorRadius, _sensedColliders, SensorLayersToCheck.value);
			return numSensed != 0;
		}
		return false;
	}
	#endregion

	#region event_handlers
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
		ChangeState(DeadState);
	}
	#endregion
}
