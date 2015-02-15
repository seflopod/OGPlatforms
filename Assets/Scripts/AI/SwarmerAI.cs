using UnityEngine;
using System.Collections;
using OgreToast.Utility;
using OgreToast.Attributes;

[RequireComponent(typeof(FlyingComponent))]
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
	public float PauseAtNodeTime = 0.5f;
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
		rigidbody2D.gravityScale = 0f;
	}

	private void OnDrawGizmosSelected()
	{
		if(ShowEndPointsInEditor)
		{
			float midPct = (MaxScreenPct - MinScreenPct) / 2f;
			Vector3 leftCenter = Camera.main.ViewportToWorldPoint(new Vector3(MinScreenPct, midPct, Camera.main.transform.position.z));
			Vector3 rightCenter = Camera.main.ViewportToWorldPoint(new Vector3(MaxScreenPct, midPct, Camera.main.transform.position.z));
			Vector3 size = new Vector3(collider2D.bounds.size.x, (MaxScreenPct - MinScreenPct) * Screen.height, 1f);
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

	protected virtual void PatrolState()
	{
		if(Mathf.Abs(transform.position.x - _moveTarget.x) < 1f && !_pauseTimer.IsRunning)
		{
			_pauseTimer.Start();

			//always switch between moving up and moving down
			IsMovingUp = !IsMovingUp;
			Vector3 viewportPos;
			if(IsMovingRight)
			{
				viewportPos = Camera.main.WorldToViewportPoint(transform.position + collider2D.bounds.extents.x * Vector3.right);
				IsMovingRight = viewportPos.x >= MaxScreenPct;
			}
			else
			{
				viewportPos = Camera.main.WorldToViewportPoint(transform.position - collider2D.bounds.extents.x * Vector3.right);
				IsMovingRight = viewportPos.x <= MinScreenPct;
			}
			Vector3 moveDelta = new Vector3(((IsMovingRight)?1:-1) * Random.Range(HorizontalDistRange.x, HorizontalDistRange.y),
			                                ((IsMovingUp)?1:-1) * Random.Range(VerticalDistRange.x, VerticalDistRange.y),
			                                0f);
			viewportPos = Camera.main.WorldToViewportPoint(transform.position + moveDelta);
			viewportPos.x = Mathf.Min(Mathf.Max(viewportPos.x, MinScreenPct), MaxScreenPct);
			viewportPos.y = Mathf.Min(Mathf.Max(viewportPos.y, MinScreenPct), MaxScreenPct);
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
	}
}
