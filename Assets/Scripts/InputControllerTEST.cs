using UnityEngine;
using System.Collections;

public class InputControllerTEST : MonoBehaviour
{
	MovementComponent mover;
	WeaponComponent gun;

	private void Start()
	{
		mover = GetComponent<MovementComponent>();
		WeaponComponent[] weapons = GetComponentsInChildren<WeaponComponent>();
		for(int i=0;i < weapons.Length;++i)
		{
			if(weapons[i].gameObject.name == "gun")
			{
				gun = weapons[i];
			}
		}
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
			mover.Run(xMod);
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
		
		if(!mover.IsJumping && Input.GetButtonDown("Jump"))
		{
			mover.Jump();
		}
		else if(mover.IsJumping && Input.GetButton("Jump"))
		{
			mover.IsGliding = true;
		}
		
		if(Input.GetButtonUp("Jump"))
		{
			mover.IsGliding = false;
		}

		Vector2 fireDir = (new Vector2(xMod, yMod)).normalized;
		gun.Aim(fireDir);

		if(Input.GetButton("Fire1"))
		{
			gun.Fire();
		}

	}
}
