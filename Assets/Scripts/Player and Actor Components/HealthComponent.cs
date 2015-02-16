using UnityEngine;
using System;
using OgreToast.Attributes;

public class HealthComponent : MonoBehaviour 
{
	public delegate void HitEventHandler(object sender, Vector2 hitDirection);

	public event EventHandler Dead;
	public event HitEventHandler Hit;

	[SerializeField]
	[PrivateIntRange("Max Health", 0, 9999)]
	private int _maxHealth = 100;

	[SerializeField]
	[PrivateIntRange("Health", 0, 9999)]
	private int _startHealth = 100;

	private int _curHealth = 0;
	public LayerMask TakeDamageFromLayers = 0;

	public int CurrentHealth
	{
		get { return _curHealth; }
		set
		{
			_curHealth = value;
			if(_curHealth > _maxHealth)
			{
				_curHealth = _maxHealth;
			}

			if(_curHealth <= 0)
			{
				OnDead(null);
			}
		}
	}

	public int MaxHealth { get { return _maxHealth; } }

	private void Start()
	{
		CurrentHealth = _startHealth;
	}

	public void CheckCollisionEnter(GameObject collisionGO)
	{
		if(((1 << collisionGO.layer) & TakeDamageFromLayers.value) != 0)
		{
			BulletBehaviour bb = collisionGO.GetComponent<BulletBehaviour>();
			if(bb != null)
			{
				_curHealth -= bb.DamageValue;
				if(_curHealth <= 0)
				{
					OnDead(null);
				}
				else
				{
					OnHit(bb.rigidbody2D.velocity.normalized);
				}
				bb.Kill(true);
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

	public void Reset()
	{
		CurrentHealth = _startHealth;
	}
}
