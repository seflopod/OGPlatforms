using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class MovementComponent : MonoBehaviour
{
	public float BaseSpeed = 10f;
	public float JumpForce = 10f;
	public float JumpVelocityThreshold = 5f;

	[Range(0f, 20f)]
	public float GlideForce = 9.81f;
	public float MaxGlideTime = 1f;
	public LayerMask PlatformLayers = -1;

	public bool IsJumping { get; private set; }

	public bool IsGliding
	{
		get { return _isGliding; }
		set
		{
			if(value && IsJumping)
			{
				_glideAccumulator = 0f;
				_isGliding = true;
			}
			else if(!value)
			{
				_isGliding = false;
			}
		}
	}

	private bool _isGliding = false;
	private float _glideAccumulator = 0f;

	private void Start()
	{
		IsJumping = true;
	}

	private void FixedUpdate()
	{
		if(rigidbody2D.velocity.y > 0f && rigidbody2D.velocity.y < JumpVelocityThreshold)
		{
			Vector2 vel = rigidbody2D.velocity;
			vel.y = 0f;
			rigidbody2D.velocity = vel;
		}
		else if(rigidbody2D.velocity.y < 0 && rigidbody2D.velocity.y > -JumpVelocityThreshold)
		{
			Vector2 vel = rigidbody2D.velocity;
			vel.y = -JumpVelocityThreshold;
			rigidbody2D.velocity = vel;
		}

//		if(rigidbody2D.velocity.y < 3f)
//		{
//			rigidbody2D.gravityScale = 20f;
//		}
//		else
//		{
//			rigidbody2D.gravityScale = 10f;
//		}

		if(_isGliding && _glideAccumulator < MaxGlideTime)
		{
			rigidbody2D.AddForce(Mathf.Sign(Physics2D.gravity.y) * rigidbody2D.mass * GlideForce * -Vector2.up);
			_glideAccumulator += Time.fixedDeltaTime;
		}
	}

	public void CheckCollisionEnter(GameObject collisionGO)
	{
		if(((1 << collisionGO.layer)  & PlatformLayers.value) != 0 && (Physics2D.gravity.y < 0f && collisionGO.transform.position.y < transform.position.y) || (Physics2D.gravity.y > 0f && collisionGO.transform.position.y > transform.position.y))
		{
			IsJumping = false;
			_isGliding = false;
		}
	}

	public void CheckCollisionExit(Collision2D collision)
	{
		if(((1 << collision.gameObject.layer) & PlatformLayers.value) != 0)
		{
			Collider2D platformCheck = null;
			if(Physics2D.gravity.y < 0f)
			{
				platformCheck = Physics2D.OverlapArea(collider2D.bounds.min, new Vector2(collider2D.bounds.max.x, collider2D.bounds.min.y - 1f), PlatformLayers.value);
			}
			else if(Physics2D.gravity.y > 0f)
			{
				platformCheck = Physics2D.OverlapArea(collider2D.bounds.max, new Vector2(collider2D.bounds.max.x, collider2D.bounds.max.y + 1f), PlatformLayers.value);
			}

			if(platformCheck == null)
			{
				IsJumping = true;
			}
		}
	}

	public void Run(float xMod)
	{
		Vector2 vel = rigidbody2D.velocity;
		vel.x = xMod * BaseSpeed;
		if(xMod < 0f)
		{
			transform.rotation = Quaternion.Euler(0f, 180f, 0f);
		}
		else if(xMod > 0f)
		{
			transform.rotation = Quaternion.Euler(0f, 0f, 0f);
		}
		rigidbody2D.velocity = vel;
	}

	public void Jump()
	{
		if(IsJumping)
		{
			return;
		}
		rigidbody2D.AddForce(JumpForce * Vector2.up, ForceMode2D.Impulse);
		IsJumping = true;
	}
}
