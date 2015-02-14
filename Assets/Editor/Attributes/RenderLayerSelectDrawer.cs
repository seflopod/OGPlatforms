using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Reflection;

namespace OgreToast.Attributes
{
	[CustomPropertyDrawer(typeof(RenderLayerSelectAttribute))]
	public class RenderLayerSelectDrawer : PropertyDrawer
	{
		public override float GetPropertyHeight(SerializedProperty prop, GUIContent label)
		{
			return base.GetPropertyHeight(prop, new GUIContent(renderLayerSelectAttribute.label));
		}

		public override void OnGUI(Rect pos, SerializedProperty prop, GUIContent label)
		{
			GUIContent lbl = new GUIContent(renderLayerSelectAttribute.label);
			GUIContent[] popupOptions = getSortingLayerNames();
			GUIStyle style = new GUIStyle();
			style.richText = true;
			float width = EditorStyles.label.CalcSize(lbl).x;
			Rect lblRect = new Rect(pos.x, pos.y, width, pos.height);
			Rect popupRect = lblRect;
			popupRect.x += lblRect.width + 3;
			popupRect.width = pos.width - lblRect.width - 3;

			EditorGUI.LabelField(lblRect, lbl);
			int selection = EditorGUI.Popup(popupRect, currentSelection(prop, popupOptions), popupOptions);
			prop.stringValue = popupOptions[selection].text;
		}

		private GUIContent[] getSortingLayerNames()
		{
			PropertyInfo sortingLayersProp = typeof(InternalEditorUtility).GetProperty("sortingLayerNames", BindingFlags.Static | BindingFlags.NonPublic);
			string[] names = (string[])sortingLayersProp.GetValue(null, new object[0]);
			GUIContent[] ret = new GUIContent[names.Length];
			for(int i=0;i<names.Length;++i)
			{
				ret[i] = new GUIContent(names[i]);
			}
			return ret;
		}

		private int currentSelection(SerializedProperty prop, GUIContent[] layerNames)
		{
			for(int i=0;i<layerNames.Length;++i)
			{
				if(layerNames[i].text == prop.stringValue)
				{
					return i;
				}
			}
			return 0;
		}

		//property for easier internal access to the attribute
		private RenderLayerSelectAttribute renderLayerSelectAttribute { get { return (RenderLayerSelectAttribute)attribute; } }
	}
}