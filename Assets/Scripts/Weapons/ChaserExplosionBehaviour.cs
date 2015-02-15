using UnityEngine;
using System.Collections;

public class ChaserExplosionBehaviour: MonoBehaviour
{
	public float ForceOfExplosion = 500;
	public int ExplosionDamange = 25;

	public void OnAnimationEnd()
	{
		renderer.enabled = false;
		collider2D.enabled = false;
		GameManager.Instance.AddToDestroyQueue(gameObject);
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		Vector2 dir = (Vector2)(other.transform.position - transform.position).normalized;
		other.rigidbody2D.AddForceAtPosition(ForceOfExplosion*dir, other.transform.position, ForceMode2D.Impulse);
		HealthComponent hc = other.gameObject.GetComponent<HealthComponent>();
		if(hc != null)
		{
			hc.CurrentHealth -= ExplosionDamange;
		}
	}
}
