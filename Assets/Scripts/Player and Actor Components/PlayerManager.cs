using UnityEngine;
using OgreToast.Utility;

[RequireComponent(typeof(MovementComponent), typeof(HealthComponent))]
public class PlayerManager : MonoBehaviour
{
	public float HitStunTime = 0.1f;
	public float HitPushForce = 100f;
	public float TimeToRespawn = 3f;

	private MovementComponent _mover;
	private WeaponComponent _weapon;
	private HealthComponent _health;
	private Animator _animator;

	private SimpleTimer _stunTimer;
	private Vector3 _spawnPos;
	private bool _isDespawned = false;
	private SimpleTimer _respawnTimer;

	#region monobehaviour
	private void Start()
	{
		_mover = GetComponent<MovementComponent>();
		_weapon = transform.FindChild("gun").GetComponent<WeaponComponent>();
		_stunTimer = new SimpleTimer(HitStunTime);
		_health = GetComponent<HealthComponent>();
		_health.Dead += OnDead;
		_health.Hit += OnHit;
		_spawnPos = transform.position;
		_respawnTimer = new SimpleTimer(TimeToRespawn);
		_animator = GetComponent<Animator>();
	}

	private void Update ()
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
					_mover.Run(xMod);
				}
				else
				{
					if(xMod < 0f)
					{
						transform.rotation = Quaternion.Euler(0f, 180f, 0f);
					}
					else if(xMod > 0f)
					{
						transform.rotation = Quaternion.Euler(0f, 0f, 0f);
					}
				}
				
				if(!_mover.IsJumping && Input.GetButtonDown("Jump"))
				{
					_mover.Jump();
				}
				else if(_mover.IsJumping && Input.GetButton("Jump"))
				{
					_mover.IsGliding = true;
				}
				
				if(Input.GetButtonUp("Jump"))
				{
					_mover.IsGliding = false;
				}

				Vector2 fireDir = (new Vector2(xMod, yMod)).normalized;
				_weapon.Aim(fireDir);

				if(Input.GetButton("Fire1"))
				{
					_weapon.Fire();
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
		if(!_mover.IsJumping && rigidbody2D.velocity.x != 0f && _animator != null && !_animator.GetBool("isRunning"))
		{
			_animator.SetBool("isRunning", true);
		}
		else if(!_mover.IsJumping && rigidbody2D.velocity.x == 0f && _animator != null && _animator.GetBool("isRunning"))
		{
			_animator.SetBool("isRunning", false);
		}
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		GameObject collisionGO = collision.gameObject;
		_mover.CheckCollisionEnter(collisionGO);
		_health.CheckCollisionEnter(collisionGO);
	}

	private void OnCollisionExit2D(Collision2D collision)
	{
		_mover.CheckCollisionExit(collision);
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
			_weapon.renderer.enabled = false;
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
			_weapon.renderer.enabled = true;
			collider2D.enabled = true;
			rigidbody2D.isKinematic = false;
			_weapon.Reset();
			_health.Reset();
			Vector3 cameraPos = Camera.main.transform.position;
			cameraPos.x = transform.position.x;
			cameraPos.y = transform.position.y;
			Camera.main.transform.position = cameraPos;
		}
	}
}
