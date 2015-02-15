using UnityEngine;
using System.Collections;

public class RocketImpactBehaviour : AbstractExplosionBehaviour
{
	public override void OnAnimationEnd()
	{
		renderer.enabled = false;
		collider2D.enabled = false;
		gameObject.SetActive(false);
	}
}
