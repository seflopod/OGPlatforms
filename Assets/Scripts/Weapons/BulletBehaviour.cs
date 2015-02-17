using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D), typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
public class BulletBehaviour : MonoBehaviour
{
	public int DamageValue = 0;

	private bool _isDying = false;
	private Animator _animator;

	private void Start()
	{
		_animator = GetComponent<Animator>();
	}

	private void OnBecameInvisible()
	{
		if(!_isDying && Application.isPlaying)
		{
			Kill(false);
		}
	}

	public void Kill(bool useImpact)
	{
		if(!_isDying)
		{
			_isDying = true;
			rigidbody2D.velocity = Vector2.zero;
			collider2D.enabled = false;

			if(useImpact)
			{
				_animator.SetTrigger("do_impact");
			}
		}
	}

	public void OnImpactAnimationDone()
	{
		renderer.enabled = false;
		gameObject.SetActive(false);
	}
}
