using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D), typeof(SpriteRenderer))]
public abstract class AbstractPickup : MonoBehaviour
{
	public float DropSpeed = -10f;
	public bool HasApplied { get; protected set; }

	protected SpriteRenderer _spriteRenderer;
	protected Vector2 _velocity = Vector2.zero;
	protected Collider2D[] _preAllocCollider = new Collider2D[1];

	public abstract void Apply(PlayerManager player);

	protected virtual void Start()
	{
		HasApplied = false;
		_spriteRenderer = GetComponent<SpriteRenderer>();
		_velocity = DropSpeed * Vector2.up;
	}

	protected virtual void Update()
	{
		doDrop();
	}

	protected void doDrop()
	{
		if(_velocity != Vector2.zero)
		{
			int size = Physics2D.OverlapAreaNonAlloc(_spriteRenderer.bounds.min, new Vector2(_spriteRenderer.bounds.max.x, _spriteRenderer.bounds.min.y - 1f), _preAllocCollider, 1 << LayerMask.NameToLayer("Platform"));
			if(size != 0)
			{
				_velocity = Vector2.zero;
				transform.position = new Vector3(transform.position.x, _preAllocCollider[0].bounds.max.y + collider2D.bounds.extents.y, transform.position.z);
			}
			else
			{
				transform.Translate(Time.deltaTime * _velocity);
			}

		}
	}

}
