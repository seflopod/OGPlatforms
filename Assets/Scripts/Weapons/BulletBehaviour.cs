using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D), typeof(SpriteRenderer))]
public class BulletBehaviour : MonoBehaviour
{
	public int DamageValue = 0;
	public int ImpactPoolIndex = 0;

	private bool _isDying = false;

	private void OnBecameInvisible()
	{
		if(!_isDying && Application.isPlaying)
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

			GameObject impactGO = GameManager.Instance.GetBulletImpact(ImpactPoolIndex);
			impactGO.SetActive(true);
			impactGO.transform.position = transform.position;
			if(transform.rotation.y != 0f)
			{
				Quaternion newRot = transform.rotation;
				newRot.y *= -1f;
				newRot.w *= -1f;
				impactGO.transform.rotation = newRot;
			}
			else
			{
				impactGO.transform.rotation = transform.rotation;
			}

			impactGO.renderer.enabled = true;

			gameObject.SetActive(false);
		}
	}
}
