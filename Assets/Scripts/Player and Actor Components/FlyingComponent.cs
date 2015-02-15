using UnityEngine;
using System.Collections;

public class FlyingComponent : MovementComponent
{
	protected override void Start()
	{

	}

	protected override void FixedUpdate ()
	{

	}

	public override void CheckCollisionEnter (GameObject collisionGO)
	{

	}

	public override void CheckCollisionExit (Collision2D collision)
	{

	}

	public virtual void Fly(float xMod, float yMod)
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

		vel.y = yMod * BaseSpeed;

		rigidbody2D.velocity = vel;
	}

	public override void Jump ()
	{

	}
}
