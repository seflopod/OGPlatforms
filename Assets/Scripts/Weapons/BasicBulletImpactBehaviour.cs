using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
public class BasicBulletImpactBehaviour : MonoBehaviour
{
	public void OnAnimationEnd()
	{
		renderer.enabled = false;
		gameObject.SetActive(false);
	}
}
