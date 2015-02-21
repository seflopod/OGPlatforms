using UnityEngine;
using UnityEditor;

namespace OgreToast.Attributes
{
	[CustomPropertyDrawer(typeof(ForcePositiveAttribute))]
	public class ForcePositiveDrawer : PropertyDrawer
	{
		//Define the height
		public override float GetPropertyHeight(SerializedProperty prop, GUIContent label)
		{
			return base.GetPropertyHeight(prop, new GUIContent(forcePositiveAttribute.name));
		}
		
		public override void OnGUI(Rect pos, SerializedProperty prop, GUIContent label)
		{
			int intValue = -1;
			float floatValue = -1f;
			if(prop.propertyType == SerializedPropertyType.Float)
			{
				floatValue = prop.floatValue;
			}
			else if(prop.propertyType == SerializedPropertyType.Integer)
			{
				intValue = prop.intValue;
			}
			else
			{
				Debug.LogError("Attempting to positive on a non-number");
				return;
			}

			EditorGUI.PropertyField(pos, prop, new GUIContent(forcePositiveAttribute.name, forcePositiveAttribute.tooltip), false);

			if(prop.propertyType == SerializedPropertyType.Float && prop.floatValue < 0f)
			{
				 prop.floatValue = floatValue;
			}
			else if(prop.propertyType == SerializedPropertyType.Integer && prop.intValue < 0)
			{
				prop.intValue = intValue;
			}
		}
		
		//property for easier internal access to the attribute
		private ForcePositiveAttribute forcePositiveAttribute { get { return (ForcePositiveAttribute)attribute; } }
	}
}
