using UnityEngine;

namespace OgreToast.Attributes
{
	public class ForcePositiveAttribute : PropertyAttribute
	{
		public readonly string name;
		public readonly string tooltip;

		public ForcePositiveAttribute(string name, string tooltip)
		{
			this.name = name;
			this.tooltip = tooltip;
		}
		
		public ForcePositiveAttribute(string name) : this(name, "")
		{}
	}
}
