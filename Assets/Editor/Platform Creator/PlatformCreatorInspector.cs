using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(PlatformCreator))]
public class PlatformCreatorInspector : Editor
{
	List<GameObject> _innerPieces;
	Transform _rightTransform;
	Transform _leftTransform;
	int _desired;
	SerializedProperty _innerSpritePool;
	
	private void OnEnable()
	{
		Transform baseTrans = null;
		_innerPieces = new List<GameObject>();
		baseTrans = ((MonoBehaviour)serializedObject.targetObject).gameObject.transform;
		Transform piecesParent = baseTrans.transform.Find("Pieces");
		_rightTransform = baseTrans.Find("Pieces/RIGHT");
		_leftTransform = baseTrans.Find("Pieces/LEFT");
		for(int i=0;i<piecesParent.childCount;++i)
		{
			if(piecesParent.GetChild(i).gameObject.name == "INNER_PIECE")
			{
				_innerPieces.Add(piecesParent.GetChild(i).gameObject);
			}
		}
		_desired = _innerPieces.Count;
		_innerSpritePool = serializedObject.FindProperty("_middlePieceSprites");
	}

	public override void OnInspectorGUI ()
	{
		serializedObject.Update();

		EditorGUILayout.LabelField("Number of Inner Pieces");
		_desired = EditorGUILayout.IntSlider(_desired, 0, 100);
		adjustNumberOfPieces();

		EditorGUILayout.PropertyField(_innerSpritePool);

		serializedObject.ApplyModifiedProperties();
	}

	private void adjustNumberOfPieces()
	{
		Transform lastTrans = null;
		while(_desired > _innerPieces.Count)
		{
			GameObject newGO = null;
			if(_innerPieces.Count == 0)
			{
				newGO = createInnerPiece();
				lastTrans = _leftTransform;
			}
			else
			{
				newGO = (GameObject)Instantiate(_innerPieces[0]);
				newGO.GetComponent<SpriteRenderer>().sprite = getRandomInnerSprite();
				lastTrans = _innerPieces[_innerPieces.Count-1].transform;
			}
			float baseXOffset = lastTrans.gameObject.GetComponent<SpriteRenderer>().sprite.bounds.size.x;
			newGO.transform.parent = lastTrans.parent;
			newGO.transform.localScale = lastTrans.localScale;
			newGO.transform.localPosition = lastTrans.localPosition + baseXOffset * Vector3.right;
			newGO.transform.localRotation = lastTrans.localRotation;
			newGO.name = "INNER_PIECE";
			_innerPieces.Add(newGO);
		}
		while(_desired < _innerPieces.Count)
		{
			GameObject oldGO = _innerPieces[_innerPieces.Count - 1];
			_innerPieces.RemoveAt(_innerPieces.Count - 1);
			DestroyImmediate(oldGO);
		}

		if(_innerPieces.Count != 0)
		{
			_rightTransform.localPosition = _innerPieces[_innerPieces.Count-1].transform.localPosition + _innerPieces[_innerPieces.Count-1].gameObject.GetComponent<SpriteRenderer>().sprite.bounds.size.x * Vector3.right;
		}
		else
		{
			_rightTransform.localPosition = _leftTransform.localPosition + _leftTransform.gameObject.GetComponent<SpriteRenderer>().sprite.bounds.size.x*Vector3.right;
		}

	}

	private GameObject createInnerPiece()
	{
		GameObject ret = new GameObject("INNER_PIECE");
		ret.AddComponent<SpriteRenderer>().sprite = getRandomInnerSprite();
		return ret;
	}

	private Sprite getRandomInnerSprite()
	{
		_innerSpritePool = serializedObject.FindProperty("_middlePieceSprites");
		updateWeight();
		int totalWeight = _innerSpritePool.FindPropertyRelative("_totWeight").intValue;
		int roll = Random.Range(0,totalWeight);
		SerializedProperty listProp = _innerSpritePool.FindPropertyRelative("_weightedObjectList");
		if(listProp.arraySize == 0)
		{
			throw new System.IndexOutOfRangeException("Need at least one element for the inner piece sprites");
			//return null;
		}
		for(int i=0;i<listProp.arraySize;++i)
		{
			SerializedProperty woiProp = listProp.GetArrayElementAtIndex(i);
			int weight = woiProp.FindPropertyRelative("weight").intValue;
			if(roll < weight)
			{
				return (Sprite)woiProp.FindPropertyRelative("obj").objectReferenceValue;
			}
			roll -= weight;
		}
		return null;
	}

	private void updateWeight()
	{
		int totalWeight = 0;
		SerializedProperty listProp = _innerSpritePool.FindPropertyRelative("_weightedObjectList");
		if(listProp.arraySize == 0)
		{
			throw new System.IndexOutOfRangeException("Need at least one element for the inner piece sprites");
			//return null;
		}
		for(int i=0;i<listProp.arraySize;++i)
		{
			SerializedProperty woiProp = listProp.GetArrayElementAtIndex(i);
			totalWeight += woiProp.FindPropertyRelative("weight").intValue;
		}
		_innerSpritePool.FindPropertyRelative("_totWeight").intValue = totalWeight;
	}
}
