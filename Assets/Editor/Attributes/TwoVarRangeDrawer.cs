using UnityEngine;
using UnityEditor;

namespace OgreToast.Attributes
{
	[CustomPropertyDrawer(typeof(TwoVarRangeAttribute))]
	public class TwoVarRangeDrawer : PropertyDrawer
	{
		//Define the height
		public override float GetPropertyHeight(SerializedProperty prop, GUIContent label)
		{
			return 3 * EditorGUIUtility.singleLineHeight;
		}

		public override void OnGUI(Rect pos, SerializedProperty prop, GUIContent label)
		{
			if(isValidType(prop))
			{
				drawControl(pos, prop);
			}
			else
			{
				EditorGUI.LabelField(pos,"Invalid variable type for TwoVarRangeAttribute!");
			}
		}

		private bool isValidType(SerializedProperty prop)
		{
			return (prop.propertyType == SerializedPropertyType.Vector2);
		}

		private void drawControl(Rect pos, SerializedProperty prop)
		{
			//temp variables for ref passing
			float curMin = prop.vector2Value.x;
			float curMax = prop.vector2Value.y;

			EditorGUI.LabelField(pos, new GUIContent(twoVarRangeAttribute.name, twoVarRangeAttribute.tooltip), EditorStyles.label);
			pos.y += EditorGUIUtility.singleLineHeight;

			//setup positioning rectangles for the control
			Rect minPos = pos;
			minPos.width = EditorGUIUtility.currentViewWidth / 6 - 10;
			minPos.height = EditorGUIUtility.singleLineHeight;

			Rect maxPos = minPos;
			maxPos.x += 5 * EditorGUIUtility.currentViewWidth / 6;

			Rect sliderPos = pos;
			sliderPos.y += EditorGUIUtility.singleLineHeight;
			sliderPos.x += EditorGUIUtility.currentViewWidth / 6 - 5;
			sliderPos.width = 2*EditorGUIUtility.currentViewWidth / 3;
			sliderPos.height = EditorGUIUtility.singleLineHeight;


			//draw min info
			EditorGUI.LabelField(minPos, "Min", EditorStyles.miniLabel);
			minPos.y += EditorGUIUtility.singleLineHeight;
			curMin = EditorGUI.FloatField(minPos, curMin);

			//draw slider
			EditorGUI.MinMaxSlider(sliderPos, ref curMin, ref curMax, twoVarRangeAttribute.minValue, twoVarRangeAttribute.maxValue);

			//draw max info
			EditorGUI.LabelField(maxPos, "Max", EditorStyles.miniLabel);
			maxPos.y += EditorGUIUtility.singleLineHeight;
			curMax = EditorGUI.FloatField(maxPos, curMax);

			prop.vector2Value = new Vector2(curMin, curMax);
		}

		//property for easier internal access to the attribute
		private TwoVarRangeAttribute twoVarRangeAttribute { get { return (TwoVarRangeAttribute)attribute; } }
	}
}
