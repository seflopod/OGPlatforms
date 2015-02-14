using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D), typeof(SpriteRenderer))]
public class BulletBehaviour : MonoBehaviour
{
	public int DamageValue = 0;

	private bool _isDying = false;

	private void OnBecameInvisible()
	{
		if(!_isDying)
		{
			Kill();
		}
	}

	public void Kill()
	{
		if(!_isDying)
		{
			_isDying = true;
			rigidbody2D.velocity = Vector2.zero;
			collider2D.enabled = false;
			renderer.enabled = false;
			gameObject.SetActive(false);
		}
	}
}
