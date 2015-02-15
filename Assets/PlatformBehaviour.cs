using UnityEngine;
using System.Collections;

public class PlatformBehaviour : MonoBehaviour
{
	private void OnCollisionEnter2D(Collision2D collision)
	{
		BulletBehaviour bb = collision.gameObject.GetComponent<BulletBehaviour>();
		if(bb != null)
		{
			bb.Kill();
		}
	}
}
