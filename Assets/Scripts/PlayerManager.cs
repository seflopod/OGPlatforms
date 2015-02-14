using UnityEngine;
using OgreToast.Utility;

[RequireComponent(typeof(MovementComponent), typeof(HealthComponent))]
public class PlayerManager : MonoBehaviour
{
	public float HitStunTime = 0.1f;
	public float HitPushForce = 100f;

	private MovementComponent _mover;
	private WeaponComponent _weapon;
	private HealthComponent _health;

	private SimpleTimer _stunTimer;

	private void Start()
	{
		_mover = GetComponent<MovementComponent>();
		_weapon = transform.FindChild("gun").GetComponent<WeaponComponent>();
		_stunTimer = new SimpleTimer(HitStunTime);
		_health = GetComponent<HealthComponent>();
		_health.Dead += OnDead;
		_health.Hit += OnHit;
	}

	private void Update ()
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

	private void LateUpdate()
	{
		if(_stunTimer.IsExpired)
		{
			_stunTimer.Stop();
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
		Debug.Log("Dead!");
	}
}
