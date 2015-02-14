using UnityEngine;

namespace OgreToast.Attributes
{
	public class RenderLayerSelectAttribute : PropertyAttribute
	{
		public readonly string label;
		public readonly string tooltip;

		public RenderLayerSelectAttribute (string label) : this(label, "")
		{}

		public RenderLayerSelectAttribute (string label, string tooltip)
		{
			this.label = label;
			this.tooltip = tooltip;
		}
	}
}
