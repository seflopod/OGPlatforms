using UnityEngine;
using System.Collections;

public class OrloAttacker : AbstractAttacker
{
	public Transform GunTransform;

	private WeaponInfo _baseWeapon;

	protected override void Start()
	{
		base.Start();
		if(GunTransform == null)
		{
			Debug.LogError("Missing GunTransform for " + gameObject.name + " weapon");
		}
		_baseWeapon = CurrentWeapon;
	}

	public override void Aim(Vector2 direction)
	{
		base.Aim (direction);

		//we want the angle of aiming for rotation's sake, but it should only be in the forward direction (x >= 0)
		//any flipping is taken care of elsewhere.
		int angle = Mathf.RoundToInt(Mathf.Rad2Deg * Mathf.Atan2(_aimDirection.y, Mathf.Abs(_aimDirection.x)));
		angle = ((angle + (int)Mathf.Sign(angle) * 44) / 45) * 45; //round to nearest multiple of 45 for 8-direction aiming
		_aimDirection = new Vector2(Mathf.Cos(Mathf.Deg2Rad * angle), Mathf.Sin(Mathf.Deg2Rad * angle));
		GunTransform.rotation = Quaternion.Euler(0f, 0f, angle);
	}

	public override void Fire()
	{
		base.Fire();
		if(CurrentWeapon.HasAmmo && AmmoRemaining <= 0)
		{
			ChangeWeapon(_baseWeapon);
		}
	}

	public void ResetWeapon()
	{
		ChangeWeapon(_baseWeapon);
	}
}
