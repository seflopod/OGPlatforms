using UnityEngine;
using System.Collections;

public class WeaponPickup : AbstractPickup
{
	public WeaponInfo Info;

	public override void Apply(PlayerManager player)
	{
		if(!HasApplied)
		{
			collider2D.enabled = false;
			renderer.enabled = false;
			HasApplied = true;
			WeaponComponent wc = player.transform.GetComponentInChildren<WeaponComponent>();
			wc.CurrentWeapon = Info;
			GameManager.Instance.AddToDestroyQueue(gameObject);
		}
	}
}
