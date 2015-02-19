using UnityEngine;
using System.Collections;
using OgreToast.Utility;

[RequireComponent(typeof(Rigidbody2D))]
/// <summary>
/// The base class for all movement components.
/// </summary>
public abstract class AbstractMover : MonoBehaviour
{
	/// <summary>
	/// The base move speed.
	/// </summary>
	public Vector2 BaseMoveSpeed = Vector2.zero;

	/// <summary>
	/// The force added when jumping.
	/// </summary>
	public float JumpForce = 200f;

	/// <summary>
	/// Whether or not to display the gizmo for the GroundCheck variables when in the Unity editor.
	/// </summary>
	public bool ShowGroundCheckInEditor = true;

	/// <summary>
	/// The center of the cirle used for checking to see if the mover is on the ground.  Stored as world coordinates,
	/// but edited as local coordinates.
	/// </summary>
	public Vector3 GroundCheckPosition = Vector3.zero;

	/// <summary>
	/// The radius of the circle used for checking to see if the mover is on the ground.
	/// </summary>
	public float GroundCheckRadius = 0.04f;

	/// <summary>
	/// A layer mask used to determine which layers constitute the ground for this object.
	/// </summary>
	public LayerMask GroundMask = -1;

	/// <summary>
	/// Gets or sets a value indicating whether this instance on the ground.
	/// </summary>
	/// <value><c>true</c> if this instance is grounded; otherwise, <c>false</c>.</value>
	public bool IsGrounded { get; protected set; }


	/// <summary>
	/// Jumps by adding <see cref="AbstractMover.JumpForce"/> in the up direction to the rigidbody if
	/// <see cref="AbstractMover.IsGrounded"/> is <c>true</c>.
	/// </summary>
	public virtual void Jump()
	{
		if(!IsGrounded)
		{
			rigidbody2D.AddForce(JumpForce * Vector2.up);
		}
	}

	/// <summary>
	/// Set velocity based on the specified <c>xModifier</c> and <c>yModifier</c> as well as the
	/// <see cref="AbstractMover.BaseMoveSpeed"/>.
	/// </summary>
	/// <param name="xModifier">X modifier.</param>
	/// <param name="yModifier">Y modifier.</param>
	/// <description>
	/// For the basic implementation here, <c>yModifier</c> is ignored.
	/// </description>
	public virtual void Move(float xModifier, float yModifier)
	{
		Vector2 curVel = rigidbody2D.velocity;
		//fast direction changes
		curVel.x = xModifier * BaseMoveSpeed.x;

		rigidbody2D.velocity = curVel;
	}

	#region monobehaviour
	protected virtual void FixedUpdate()
	{
		IsGrounded = Physics2D.OverlapCircle(GroundCheckPosition, GroundCheckRadius, GroundMask);
	}

	protected virtual void OnDrawGizmosSelected()
	{
		if(ShowGroundCheckInEditor)
		{
			Vector3 worldPos = transform.TransformPoint(GroundCheckPosition);
			Gizmos.color = new Color(0.75f, 0.375f, 0f, 0.75f);
			Gizmos.DrawSphere(worldPos, GroundCheckRadius);
		}
	}
	#endregion

}
