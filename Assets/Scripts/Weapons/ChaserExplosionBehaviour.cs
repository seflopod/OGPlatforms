using UnityEngine;
using System.Collections;

public class ChaserExplosionBehaviour: AbstractExplosionBehaviour
{
	public override void OnAnimationEnd()
	{
		renderer.enabled = false;
		collider2D.enabled = false;
		GameManager.Instance.AddToDestroyQueue(gameObject);
	}
}
