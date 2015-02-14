using UnityEngine;

namespace OgreToast.Attributes
{
	public class PrivateFloatRangeAttribute : PropertyAttribute
	{
		public readonly string name;
		public readonly string tooltip;
		public readonly float minValue;
		public readonly float maxValue;

		public PrivateFloatRangeAttribute (string name, string tooltip, float minValue, float maxValue)
		{
			this.name = name;
			this.minValue = minValue;
			this.maxValue = maxValue;
			this.tooltip = tooltip;
		}

		public PrivateFloatRangeAttribute(string name, float minValue, float maxValue) : this(name, "", minValue, maxValue)
		{}
	}
}
