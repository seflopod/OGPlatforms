using UnityEngine;
using System.Collections;

public class SmoothCamera2D : MonoBehaviour
{
	public float dampTime = 0.15f;
	private Vector3 velocity = Vector3.zero;
	public Transform target;
	public bool UseViewportCushion = false;
	public Vector2 TargetViewportMinimum = Vector2.zero;
	public Vector2 TargetViewportMaximum = Vector2.zero;
	public bool ConstrainMovement = false;
	public Vector2 MaximumPoint = Vector2.zero;
	public Vector2 MinimumPoint = Vector2.zero;

	private Vector3 _destination;

	private void Start()
	{
		_destination = transform.position;
	}

	// Update is called once per frame
	void Update () 
	{
		if (target)
		{
			Vector3 point = camera.WorldToViewportPoint(target.position);
			Vector3 viewportDest = new Vector3(0.5f, 0.5f, point.z);
			if(UseViewportCushion)
			{
				if(point.x >= TargetViewportMaximum.x)
				{
					viewportDest.x = TargetViewportMinimum.x;
				}
				else if(point.x <= TargetViewportMinimum.x)
				{
					viewportDest.x = TargetViewportMaximum.x;
				}

				if(point.y >= TargetViewportMaximum.y)
				{
					viewportDest.y = TargetViewportMinimum.y;
				}
				else if(point.y <= TargetViewportMinimum.y)
				{
					viewportDest.y = TargetViewportMaximum.y;
				}
			}

			if(!UseViewportCushion || viewportDest.x != 0.5f || viewportDest.y != 0.5f)
			{
				Vector3 delta = target.position - camera.ViewportToWorldPoint(viewportDest); //(new Vector3(0.5, 0.5, point.z));
				_destination = transform.position + delta;
			}
			Vector3 newPos = Vector3.SmoothDamp(transform.position, _destination, ref velocity, dampTime);

			if(ConstrainMovement)
			{
				newPos.x = Mathf.Min(Mathf.Max(MinimumPoint.x, newPos.x), MaximumPoint.x);
				newPos.y = Mathf.Min(Mathf.Max(MinimumPoint.y, newPos.y), MaximumPoint.y);
			}

			transform.position = newPos;
		}
		
	}
}