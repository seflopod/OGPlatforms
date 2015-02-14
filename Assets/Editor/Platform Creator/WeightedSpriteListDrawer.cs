using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomPropertyDrawer(typeof(WeightedSpriteList))]
public class WeightedSpriteListDrawer : PropertyDrawer
{
	private bool _bIsVisible = false;
	private bool _hasWeightChanged = false;
	
	//Define the height
	public override float GetPropertyHeight(SerializedProperty prop, GUIContent label)
	{
		prop.Next(true);
		prop.Next(true);
		SerializedProperty listProp = prop; //prop.FindPropertyRelative("_fallingObjectList");
		
		//if the list is visible, we need to display the whole thing plus the foldout. 
		//if it is invisible, then only the foldout is shown, which is a single line.
		if(_bIsVisible)
		{
			int minLines = (listProp.arraySize > 0) ? 3 : 2;
			return EditorGUIUtility.singleLineHeight * (minLines + listProp.arraySize);
		}
		return EditorGUIUtility.singleLineHeight;
	}
	
	public override void OnGUI(Rect pos, SerializedProperty prop, GUIContent label)
	{
		//setup the styles look
		GUIStyle foldoutStyle = new GUIStyle(EditorStyles.foldout);
		foldoutStyle.fontStyle = FontStyle.Bold;
		
		GUIStyle headerStyle = new GUIStyle(EditorStyles.label);
		headerStyle.fontStyle = FontStyle.Italic;

		SerializedProperty totalWeightProp = prop.FindPropertyRelative("_totWeight");
		prop.Next(true);
		prop.Next(true);
		SerializedProperty listProp = prop;

		//show the foldout
		Rect foldoutRect = new Rect(pos.x, pos.y, pos.width, EditorGUIUtility.singleLineHeight);
		_bIsVisible = EditorGUI.Foldout(foldoutRect, _bIsVisible, "Sprites Used", foldoutStyle);
		if(_bIsVisible)
		{
			Rect btnRect = pos;
			if(listProp.arraySize > 0)
			{	//only show labels if we have elements
				//make column labels
				Rect weightLblRect = new Rect(pos.x, pos.y+EditorGUIUtility.singleLineHeight, pos.width/6-3, EditorGUIUtility.singleLineHeight);
				EditorGUI.LabelField(weightLblRect, "Weight", headerStyle);
				
				Rect prefabLblRect = weightLblRect;
				prefabLblRect.x += weightLblRect.width + 6;
				prefabLblRect.width = 5 * pos.width / 6;
				EditorGUI.LabelField(prefabLblRect, "Sprite", headerStyle);
				btnRect = drawList(pos, listProp);
			}
			else
			{	//if we have no list elements we hand-set the position of the button
				btnRect.y += EditorGUIUtility.singleLineHeight;
				btnRect.height = EditorGUIUtility.singleLineHeight;
			}
			
			//button to add a new element
			if(GUI.Button(btnRect,"Add",EditorStyles.miniButton))
			{
				listProp.InsertArrayElementAtIndex(listProp.arraySize);
				listProp.GetArrayElementAtIndex(listProp.arraySize-1).FindPropertyRelative("weight").intValue = 1;
				_hasWeightChanged = true;
			}
		}

		if(_hasWeightChanged)
		{
			totalWeightProp.intValue = getTotalWeight(listProp);
			_hasWeightChanged = false;
		}
	}
	
	private Rect drawList(Rect pos, SerializedProperty listProp)
	{
		Rect eleRect = new Rect(pos.x, pos.y+2*EditorGUIUtility.singleLineHeight, pos.width, EditorGUIUtility.singleLineHeight);
		for(int i=0;i<listProp.arraySize;++i)
		{
			SerializedProperty ele = listProp.GetArrayElementAtIndex(i);

			SerializedProperty weightProp = ele.FindPropertyRelative("weight");
			Rect weightRect = eleRect;
			weightRect.width = eleRect.width / 6 - 3;
			int newWeightVal = EditorGUI.IntField(weightRect, GUIContent.none, weightProp.intValue);

			if(newWeightVal <= 0)
			{
				_hasWeightChanged = true;
				weightProp.intValue = 1;
			}
			else if(newWeightVal != weightProp.intValue)
			{
				weightProp.intValue = newWeightVal;
				_hasWeightChanged = true;
			}
			
			Rect prefabRect = eleRect;
			prefabRect.x += weightRect.width + 3;
			prefabRect.width = 4 * eleRect.width / 6 - 6;
			EditorGUI.PropertyField(prefabRect, ele.FindPropertyRelative("obj"), GUIContent.none);
			
			//add a button to delete an element if so desired
			Rect delRect = eleRect;
			delRect.x += prefabRect.x + prefabRect.width;
			delRect.width = eleRect.width / 8 - 3;
			if(GUI.Button(delRect, "Del"))
			{
				listProp.DeleteArrayElementAtIndex(i);
				i--;
			}
			
			eleRect.y+=EditorGUIUtility.singleLineHeight;
		}
		
		//sort the list if needed
		if(listProp.arraySize > 1)
		{
			sortSerializedList(listProp);
		}
		
		return eleRect;
	}
	
	//uses selection sort b/c we'll probably have a lower number of elements
	private void sortSerializedList(SerializedProperty listProp)
	{
		int minWeight, minIdx;
		for(int i=0;i<listProp.arraySize-1;++i)
		{
			//we want the actual element property (the WeightedObjectInfo)
			SerializedProperty eleI = listProp.GetArrayElementAtIndex(i);
			
			//this is what we'll use for comparison
			SerializedProperty weightPropI = eleI.FindPropertyRelative("weight");
			
			minWeight = weightPropI.intValue;
			minIdx = i;
			
			for(int j=i+1;j<listProp.arraySize;++j)
			{
				SerializedProperty eleJ = listProp.GetArrayElementAtIndex(j);
				SerializedProperty weightPropJ = eleJ.FindPropertyRelative("weight");
				int val = weightPropJ.intValue;
				
				if(val < minWeight)
				{
					minWeight = val;
					minIdx = j;
				}
			}
			
			if(weightPropI.intValue != minWeight)
			{
				//since we have to worry about carrying the whole property along, we can't just swap weight values
				listProp.MoveArrayElement(minIdx, i);
				
				#pragma warning disable 0219
				//this is assigning the reference for the SerializedProeprty eleI in the SerializedProperty at minIdx
				SerializedProperty prop = listProp.GetArrayElementAtIndex(minIdx);
				prop = eleI;
				#pragma warning restore 0219
			}
		}
	}

	private int getTotalWeight(SerializedProperty listProp)
	{
		int totWeight = 0;
		for(int i=0;i<listProp.arraySize;++i)
		{
			totWeight += listProp.GetArrayElementAtIndex(i).FindPropertyRelative("weight").intValue;
		}
		return totWeight;
	}
}