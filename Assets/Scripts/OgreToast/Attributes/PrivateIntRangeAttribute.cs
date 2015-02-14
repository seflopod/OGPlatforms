using UnityEngine;

namespace OgreToast.Attributes
{
	public class PrivateIntRangeAttribute : PropertyAttribute
	{
		public readonly string name;
		public readonly string tooltip;
		public readonly int minValue;
		public readonly int maxValue;

		public PrivateIntRangeAttribute (string name, string tooltip, int minValue, int maxValue)
		{
			this.name = name;
			this.minValue = minValue;
			this.maxValue = maxValue;
			this.tooltip = tooltip;
		}
		
		public PrivateIntRangeAttribute(string name, int minValue, int maxValue) : this(name, "", minValue, maxValue)
		{}
	}
}
