using UnityEngine;
using UnityEditor;

namespace OgreToast.Attributes
{
	[CustomPropertyDrawer(typeof(PrivateSerializedAttribute))]
	public class PrivateSerializedDrawer : PropertyDrawer
	{
		//Define the height
		public override float GetPropertyHeight(SerializedProperty prop, GUIContent label)
		{
			return base.GetPropertyHeight(prop, new GUIContent(privateSeralizedAttribute.name));
		}

		public override void OnGUI(Rect pos, SerializedProperty prop, GUIContent label)
		{
			EditorGUI.PropertyField(pos, prop, new GUIContent(privateSeralizedAttribute.name, privateSeralizedAttribute.tooltip), false);
		}

		//property for easier internal access to the attribute
		private PrivateSerializedAttribute privateSeralizedAttribute { get { return (PrivateSerializedAttribute)attribute; } }
	}
}
