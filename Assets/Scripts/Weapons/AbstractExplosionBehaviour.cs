using UnityEngine;
using System.Collections;

public abstract class AbstractExplosionBehaviour : MonoBehaviour
{
	public static event System.EventHandler Explosion;

	public float ForceOfExplosion = 500;
	public int ExplosionDamange = 25;

	private bool _hasExploded = false;

	public abstract void OnAnimationEnd();

	protected virtual void OnTriggerEnter2D(Collider2D other)
	{
		if(!_hasExploded)
		{
			_hasExploded = true;
			System.EventHandler handler = Explosion;
			if(handler != null)
			{
				Explosion(this, null);
			}

			Vector2 dir = (Vector2)(other.transform.position - transform.position).normalized;
			other.rigidbody2D.AddForceAtPosition(ForceOfExplosion*dir, other.transform.position, ForceMode2D.Impulse);
			HealthComponent hc = other.gameObject.GetComponent<HealthComponent>();
			if(hc != null)
			{
				hc.CurrentHealth -= ExplosionDamange;
			}
		}
	}
}
