using UnityEngine;
using System.Collections;

public class HealthPickup : AbstractPickup
{
	public int AmountHealed = 25;

	public override void Apply(PlayerManager player)
	{
		if(!HasApplied)
		{
			collider2D.enabled = false;
			renderer.enabled = false;
			HasApplied = true;
			player.gameObject.GetComponent<HealthComponent>().CurrentHealth += AmountHealed;
			Destroy(gameObject);
		}
	}
}
