using UnityEngine;

namespace OgreToast.Attributes
{
	public class PrivateSerializedAttribute : PropertyAttribute
	{
		public readonly string name;
		public readonly string tooltip;

		public PrivateSerializedAttribute(string name, string tooltip)
		{
			this.name = name;
			this.tooltip = tooltip;
		}

		public PrivateSerializedAttribute(string name) : this(name, "")
		{}
	}
}