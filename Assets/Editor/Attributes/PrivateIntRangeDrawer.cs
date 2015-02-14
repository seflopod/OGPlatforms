using UnityEngine;
using UnityEditor;

namespace OgreToast.Attributes
{
	[CustomPropertyDrawer(typeof(PrivateIntRangeAttribute))]
	public class PrivateIntRangeDrawer : PropertyDrawer
	{
		public override float GetPropertyHeight(SerializedProperty prop, GUIContent label)
		{
			return base.GetPropertyHeight(prop, new GUIContent(privateRangeAttribute.name));
		}
		
		public override void OnGUI(Rect pos, SerializedProperty prop, GUIContent label)
		{
			GUIStyle editorStyle = new GUIStyle();
			editorStyle.richText = true;
			GUIContent lbl = new GUIContent(privateRangeAttribute.name, privateRangeAttribute.tooltip);
			Vector2 lblSize = editorStyle.CalcSize(lbl);
			Rect contentRect = new Rect(pos.x, pos.y, Mathf.Min(lblSize.x, pos.width/2), Mathf.Min(pos.height, base.GetPropertyHeight(prop, lbl)));
			EditorGUI.LabelField(contentRect, lbl, editorStyle);
			contentRect.x += lblSize.x + 3;
			contentRect.width = pos.width - Mathf.Min(lblSize.x, pos.width/2) + 3;

			prop.intValue = EditorGUI.IntSlider(contentRect, prop.intValue, privateRangeAttribute.minValue, privateRangeAttribute.maxValue);
		}
		
		//property for easier internal access to the attribute
		private PrivateIntRangeAttribute privateRangeAttribute { get { return (PrivateIntRangeAttribute)attribute; } }
	}
}