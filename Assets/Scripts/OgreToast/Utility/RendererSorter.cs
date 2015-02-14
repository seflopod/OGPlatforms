using UnityEngine;
using OgreToast.Attributes;

namespace OgreToast.Utility
{
	/// <summary>
	/// A fix for Unity rendering particles behind sprites.
	/// </summary>
	[ExecuteInEditMode]
	public class RendererSorter : MonoBehaviour
	{
		public bool isParticleSystem = false;

		[RenderLayerSelect("Render Layer")]
		public string renderLayer = "";

		public int sortingOrder = 0;

		private void Start()
		{
			if(!isParticleSystem)
			{
				renderer.sortingLayerName = renderLayer;
				renderer.sortingOrder = sortingOrder;
			}
			else
			{
				particleSystem.renderer.sortingLayerName = renderLayer;
				particleSystem.renderer.sortingOrder = sortingOrder;
			}
		}
	}
}