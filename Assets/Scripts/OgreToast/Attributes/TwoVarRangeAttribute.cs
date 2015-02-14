using UnityEngine;

namespace OgreToast.Attributes
{
	public class TwoVarRangeAttribute : PropertyAttribute
	{
		public readonly string name;
		public readonly float minValue;
		public readonly float maxValue;
		public readonly string tooltip;

		public TwoVarRangeAttribute (string name, string tooltip, float minValue, float maxValue)
		{
			this.name = name;
			this.minValue = minValue;
			this.maxValue = maxValue;
			this.tooltip = tooltip;
		}
		

		public TwoVarRangeAttribute(string name, float minValue, float maxValue) : this (name, "", minValue, maxValue)
		{}
	}
}
