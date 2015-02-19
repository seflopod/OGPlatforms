using UnityEngine;
using OgreToast.Utility;

[RequireComponent(typeof(OrloMover), typeof(OrloAttacker), typeof(HealthComponent))]
public class PlayerManager : MonoBehaviour
{
	public float HitStunTime = 0.1f;
	public float HitPushForce = 100f;
	public float TimeToRespawn = 3f;
	
	private HealthComponent _health;
	private Animator _animator;

	private SimpleTimer _stunTimer;
	private Vector3 _spawnPos;
	private bool _isDespawned = false;
	private SimpleTimer _respawnTimer;
	private OrloMover _mover;
	private OrloAttacker _attacker;

	#region monobehaviour
	protected void Start()
	{
		_mover = GetComponent<OrloMover>();
		_attacker = GetComponent<OrloAttacker>();
		_stunTimer = new SimpleTimer(HitStunTime);
		_health = GetComponent<HealthComponent>();
		_health.Dead += OnDead;
		_health.Hit += OnHit;
		_spawnPos = transform.position;
		_respawnTimer = new SimpleTimer(TimeToRespawn);
		//_animator = GetComponent<Animator>();
	}

	private void Update()
	{
		if(Input.GetButtonDown("Jump"))
		{
			_mover.Jump();
		}
	}

	private void FixedUpdate ()
	{
		if(!_isDespawned)
		{
			if(!_stunTimer.IsRunning)
			{
				float xMod = Input.GetAxis("Horizontal");
				if(xMod < 0f)
				{
					xMod = -1f;
				}
				else if(xMod > 0f)
				{
					xMod = 1f;
				}

				float yMod = Input.GetAxis("Vertical");
				if(yMod < 0f)
				{
					yMod = -1f;
				}
				else if(yMod > 0f)
				{
					yMod = 1f;
				}

				bool aimMode = Input.GetButton("Fire2");

				if(!aimMode)
				{
					_mover.Move(xMod, 0f);

					Vector3 newScale = transform.localScale;
					if((rigidbody2D.velocity.x < 0f && newScale.x > 0f) || (rigidbody2D.velocity.x > 0f && newScale.x < 0f))
					{
						newScale.x *= -1f;
						transform.localScale = newScale;
					}
				}
				else
				{
					Vector3 newScale = transform.localScale;
					if((xMod < 0f && newScale.x > 0f) || (xMod > 0f && newScale.x < 0f))
					{
						newScale.x *= -1f;
						transform.localScale = newScale;
					}
				}

				Vector2 fireDir = (new Vector2(xMod, yMod)).normalized;
				_attacker.Aim(fireDir);

				if(Input.GetButton("Fire1"))
				{
					_attacker.Fire();
				}
			}
		}
		else if(_respawnTimer.IsExpired)
		{
			Respawn();
		}
	}

	private void LateUpdate()
	{
		if(_stunTimer.IsExpired)
		{
			_stunTimer.Stop();
		}

		//doing animations
//		if(!_mover.IsJumping && rigidbody2D.velocity.x != 0f && _animator != null && !_animator.GetBool("isRunning"))
//		{
//			_animator.SetBool("isRunning", true);
//		}
//		else if(!_mover.IsJumping && rigidbody2D.velocity.x == 0f && _animator != null && _animator.GetBool("isRunning"))
//		{
//			_animator.SetBool("isRunning", false);
//		}
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		GameObject collisionGO = collision.gameObject;
		//_mover.CheckCollisionEnter(collisionGO);
		_health.CheckCollisionEnter(collisionGO);
	}

	private void OnCollisionExit2D(Collision2D collision)
	{
		//_mover.CheckCollisionExit(collision);
	}

	//don't need to broadcast to other components since none of them use triggers
	private void OnTriggerEnter2D(Collider2D other)
	{
		if(LayerMask.LayerToName(other.gameObject.layer) == "Pickups")
		{
			AbstractPickup ap = other.gameObject.GetComponent<AbstractPickup>();
			if(ap != null && !ap.HasApplied)
			{
				ap.Apply(this);
			}
		}
	}
	#endregion

	#region events
	private void OnHit(object sender, Vector2 hitDirection)
	{
		if(!_stunTimer.IsRunning)
		{
			rigidbody2D.AddForce(HitPushForce * hitDirection, ForceMode2D.Impulse);
			_stunTimer.Stop();
			_stunTimer.TargetTime = HitStunTime;
			_stunTimer.Start();
		}
	}

	private void OnDead(object sender, System.EventArgs e)
	{
		Despawn();
	}
	#endregion

	public void Despawn()
	{
		if(!_isDespawned)
		{
			rigidbody2D.velocity = Vector2.zero;
			rigidbody2D.isKinematic = true;
			collider2D.enabled = false;
			renderer.enabled = false;
			_isDespawned = true;
			_respawnTimer.TargetTime = TimeToRespawn;
			_respawnTimer.Start();
		}
	}

	public void Respawn()
	{
		if(_isDespawned)
		{
			_isDespawned = false;
			transform.position = _spawnPos;
			renderer.enabled = true;
			_attacker.renderer.enabled = true;
			collider2D.enabled = true;
			rigidbody2D.isKinematic = false;
			(_attacker as OrloAttacker).ResetWeapon();
			_health.Reset();
			Vector3 cameraPos = Camera.main.transform.position;
			cameraPos.x = transform.position.x;
			cameraPos.y = transform.position.y;
			Camera.main.transform.position = cameraPos;
		}
	}
}
