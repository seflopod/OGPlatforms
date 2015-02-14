using UnityEngine;
using System;
using OgreToast.Attributes;

public class HealthComponent : MonoBehaviour 
{
	public delegate void HitEventHandler(object sender, Vector2 hitDirection);

	public event EventHandler Dead;
	public event HitEventHandler Hit;

	[SerializeField]
	[PrivateIntRange("Health", 0, 9999)]
	private int _health = 100;

	public LayerMask TakeDamageFromLayers = 0;

	public int Health
	{
		get { return _health; }
		set { _health = value; }
	}

	public void CheckCollisionEnter(GameObject collisionGO)
	{
		if(((1 << collisionGO.layer) & TakeDamageFromLayers.value) != 0)
		{
			BulletBehaviour bb = collisionGO.GetComponent<BulletBehaviour>();
			if(bb != null)
			{
				_health -= bb.DamageValue;
				if(_health <= 0)
				{
					OnDead(null);
				}
				else
				{
					OnHit(bb.rigidbody2D.velocity.normalized);
				}
				bb.Kill();
			}
		}
	}

	private void OnDead(EventArgs e)
	{
		EventHandler handler = Dead;
		if(handler != null)
		{
			handler(this, e);
		}
	}

	private void OnHit(Vector2 hitDirection)
	{
		HitEventHandler handler = Hit;
		if(handler != null)
		{
			handler(this, hitDirection);
		}
	}
}
