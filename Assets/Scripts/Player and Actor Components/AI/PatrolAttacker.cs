using UnityEngine;

public class PatrolAttacker : AbstractAttacker
{
	public Transform GunTransform;

	protected override void Start()
	{
		base.Start();
		if(GunTransform == null)
		{
			Debug.LogError("Missing GunTransform for " + gameObject.name + " weapon");
		}
	}

	public override void Aim(Vector2 direction)
	{
		base.Aim (direction);
		int angle = Mathf.RoundToInt(Mathf.Rad2Deg * Mathf.Atan2(_aimDirection.y, Mathf.Abs(_aimDirection.x)));
		angle = ((angle + (int)Mathf.Sign(angle) * 44) / 45) * 45; //round to nearest multiple of 45 for 8-direction aiming
		GunTransform.rotation = Quaternion.Euler(0f, 0f, angle);
	}
}