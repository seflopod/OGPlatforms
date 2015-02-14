using UnityEngine;
using System;
using OgreToast.Attributes;

public class HealthComponent : MonoBehaviour 
{
	public event EventHandler Dead;

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
				bb.Kill();
				if(_health <= 0)
				{
					OnDead(null);
				}
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
}
