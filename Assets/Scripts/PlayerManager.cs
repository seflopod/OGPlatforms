using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MovementComponent), typeof(HealthComponent))]
public class PlayerManager : MonoBehaviour
{
	private MovementComponent _mover;
	private WeaponComponent _weapon;
	private HealthComponent _health;

	private void Start()
	{
		_mover = GetComponent<MovementComponent>();
		_weapon = transform.FindChild("gun").GetComponent<WeaponComponent>();
		_health = GetComponent<HealthComponent>();
		_health.Dead += OnDead;
	}

	private void Update ()
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



	private void OnDead(object sender, System.EventArgs e)
	{
		Debug.Log("Dead!");
	}
}
