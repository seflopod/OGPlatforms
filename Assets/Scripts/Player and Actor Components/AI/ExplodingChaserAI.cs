using UnityEngine;
using System.Collections;
using OgreToast.Utility;

[RequireComponent(typeof(MovementComponent), typeof(SpriteRenderer), typeof(HealthComponent))]
public class ExplodingChaserAI : AbstractAI
{
	public bool ShowExplosionRadiusInEditor = true;
	public float ExplosionRadius = 16f;
	public float TimeBeforeExplode = 3f;
	public LayerMask ExplodeWhenHitLayers = 0;
	public GameObject ExplosionPrefab;
	
	private MovementComponent _mover;
	private SimpleTimer _explosionTimer = new SimpleTimer();

	#region monobehaviour
	protected override void Start()
	{
		base.Start();
		_mover = GetComponent<MovementComponent>();
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

	protected override void OnDrawGizmosSelected ()
	{
		base.OnDrawGizmosSelected ();
		if(ShowExplosionRadiusInEditor)
		{
			Gizmos.color = new Color(0.75f, 0.75f, 0f, 0.4f);
			Gizmos.DrawSphere(transform.position, ExplosionRadius);
		}
	}
	#endregion

	#region states
	protected override void IdleState()
	{
		base.IdleState();
		if(HasSensedSomething() && CanAttack)
		{
			ChangeState(AttackState);
			_explosionTimer.TargetTime = TimeBeforeExplode;
			_explosionTimer.Start();
		}
	}

	protected override void AttackState()
	{
		Transform target = _sensedColliders[0].transform;
		Vector3 moveTarget = _sensedColliders[0].transform.position;
		if(Physics2D.OverlapCircle(transform.position, ExplosionRadius, ExplodeWhenHitLayers.value) != null || _explosionTimer.IsExpired)
		{
			GetComponent<HealthComponent>().CurrentHealth = 0;
		}
		else
		{
			if(target.rigidbody2D != null && Random.value < 0.3f)
			{
				moveTarget += (Vector3)target.rigidbody2D.velocity * Time.deltaTime;
			}
			_mover.Run((moveTarget.x < transform.position.x) ? -1 : 1);
		}
	}

	protected override void DeadState()
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
			GameObject.Instantiate(ExplosionPrefab, transform.position, Quaternion.identity);
			GameManager.Instance.AddToDestroyQueue(gameObject);
		}
	}
	#endregion
}
