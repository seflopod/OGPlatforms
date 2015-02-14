using UnityEngine;
using System.Collections;
using OgreToast.Attributes;

namespace OgreToast.Utility
{
	public class MultiSpriteObject : MonoBehaviour
	{
		#region serialized_fields
		[SerializeField]
		[HideInInspector]
		private Bounds bndSpriteBounds = new Bounds(Vector3.zero, Vector3.zero);
		#endregion
		
		#region other_fields
		private SpriteRenderer[] mSprites = new SpriteRenderer[0];
		private bool bIsVisible = true;
		#endregion
		
		#region monobehaviour
		protected virtual void Awake()
		{
			mSprites = GetComponentsInChildren<SpriteRenderer>();
			bndSpriteBounds = calculateSpriteBounds();
		}
		#endregion
		
		/// <summary>
		/// Calculates the sprite bounds.
		/// </summary>
		/// <returns>The sprite bounds.</returns>
		/// <description>
		/// This will go through every SpriteRenderer in every child of the object (but not their children)
		/// and use the size of the Sprites to expand the bounds as needed.
		/// </description>
		protected Bounds calculateSpriteBounds()
		{
			if (mSprites.Length == 0)
			{
				mSprites = GetComponentsInChildren<SpriteRenderer>();
			}
			Vector3 size = Vector3.zero;
			for (int i=0; i<mSprites.Length; ++i)
			{
				//since the sprite might not be at dead center on the parent, we
				//need to adjust the size by that offset.  Size is non-negative,
				//so the offset is also non-negative.
				Vector3 offset = mSprites[i].gameObject.transform.localPosition;
				offset.x = Mathf.Abs(offset.x);
				offset.y = Mathf.Abs(offset.y);
				
				Vector3 curSize = mSprites[i].bounds.size + offset;
				if(curSize.x > size.x)
				{
					size.x = curSize.x;
				}
				if(curSize.y > size.y)
				{
					size.y = curSize.y;
				}
			}
			Vector3 pos = transform.position;
			pos.x = size.x / 2;
			pos.y = size.y / 2;
			return new Bounds (pos, size);
		}
		
		public void DisableSprites()
		{
			for (int i=0; i<mSprites.Length; ++i) 
			{
				mSprites[i].enabled = false;
			}
			bIsVisible = false;
		}
		
		public void EnableSprites()
		{
			for (int i=0; i<mSprites.Length; ++i) 
			{
				mSprites[i].enabled = true;
			}
			bIsVisible = true;
		}
		
		public Bounds SpriteBounds
		{
			get
			{
				if(bndSpriteBounds.size == Vector3.zero)
				{
					bndSpriteBounds = calculateSpriteBounds();
				}
				return bndSpriteBounds;
			}
		}
		
		public bool IsVisible
		{
			get { return bIsVisible; }
			set
			{
				bIsVisible = value;
				if(bIsVisible)
				{
					EnableSprites();
				}
				else
				{
					DisableSprites();
				}
			}
		}
	}
}
