using UnityEngine;

namespace OgreToast.Attributes
{
	public class TwoVarIntRangeAttribute : PropertyAttribute
	{
		public readonly string name;
		public readonly int minValue;
		public readonly int maxValue;
		public readonly string tooltip;

		public TwoVarIntRangeAttribute (string name, string tooltip, int minValue, int maxValue)
		{
			this.name = name;
			this.minValue = minValue;
			this.maxValue = maxValue;
			this.tooltip = tooltip;
		}
		

		public TwoVarIntRangeAttribute(string name, int minValue, int maxValue) : this (name, "", minValue, maxValue)
		{}
	}
}
