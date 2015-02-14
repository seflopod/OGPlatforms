using UnityEngine;
using System.Collections;

public class BulletStart : MonoBehaviour
{
	private void OnDrawGizmos()
	{
		Color gizColor = Gizmos.color;
		Gizmos.color = Color.yellow;
		Gizmos.DrawSphere(transform.position, 2f);
		Gizmos.color = gizColor;
	}
}
