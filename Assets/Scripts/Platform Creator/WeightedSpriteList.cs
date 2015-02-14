using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

//a facade for List<WeightedObjectInfo> used to add automatic sorting and Unity Editor functionality
[System.Serializable]
public class WeightedSpriteList : IList<WeightedSpriteInfo>, IList
{
	[SerializeField]
	private List<WeightedSpriteInfo> _weightedObjectList = new List<WeightedSpriteInfo>();

	[SerializeField]
	private int _totWeight;
	
	#region Generic.IList implementation
	public int IndexOf(WeightedSpriteInfo item)
	{
		return _weightedObjectList.IndexOf(item);
	}
	
	public void Insert(int index, WeightedSpriteInfo item)
	{
		_weightedObjectList.Insert(index, item);
		_weightedObjectList.Sort();
		RecalculateWeight();
	}
	
	public void RemoveAt(int index)
	{
		_weightedObjectList.RemoveAt(index);
		RecalculateWeight();
	}
	
	public WeightedSpriteInfo this[int index]
	{
		get
		{
			return _weightedObjectList[index];
		}
		set
		{
			_weightedObjectList[index] = value;
			_weightedObjectList.Sort();
			RecalculateWeight();
		}
	}
	#endregion
	
	#region Generic.ICollection implementation
	public void Add(WeightedSpriteInfo item)
	{
		_weightedObjectList.Add(item);
		_weightedObjectList.Sort();
		RecalculateWeight();
	}
	
	public void Clear()
	{
		_weightedObjectList.Clear();
		_totWeight = 0;
	}
	
	public bool Contains(WeightedSpriteInfo item)
	{
		return _weightedObjectList.Contains(item);
	}
	
	public void CopyTo(WeightedSpriteInfo[] array, int arrayIndex)
	{
		_weightedObjectList.CopyTo(array, arrayIndex);
	}
	
	public bool Remove(WeightedSpriteInfo item)
	{
		bool ret = _weightedObjectList.Remove(item);
		RecalculateWeight();
		return ret;
	}
	
	public int Count
	{
		get
		{
			return _weightedObjectList.Count;
		}
	}
	
	//it just isn't going to be readonly
	public bool IsReadOnly
	{
		get
		{
			return false;
		}
	}
	#endregion
	
	#region Generic_IEnumerable implementation
	public IEnumerator<WeightedSpriteInfo> GetEnumerator()
	{
		return _weightedObjectList.GetEnumerator();
	}
	#endregion
	
	#region IEnumerable implementation
	IEnumerator IEnumerable.GetEnumerator()
	{
		return this.GetEnumerator();
	}
	#endregion
	
	#region IList implementation
	int IList.Add(object value)
	{
		int ret = ((IList)_weightedObjectList).Add(value);
		_weightedObjectList.Sort();
		RecalculateWeight();
		return ret;
	}
	
	bool IList.Contains(object value)
	{
		return ((IList)_weightedObjectList).Contains(value);
	}
	
	int IList.IndexOf(object value)
	{
		return ((IList)_weightedObjectList).IndexOf(value);
	}
	
	void IList.Insert(int index, object value)
	{
		((IList)_weightedObjectList).Insert(index, value);
		_weightedObjectList.Sort();
		RecalculateWeight();
	}
	
	void IList.Remove(object value)
	{
		((IList)_weightedObjectList).Remove(value);
		RecalculateWeight();
	}
	
	object IList.this[int index]
	{
		get
		{
			return ((IList)_weightedObjectList)[index];
		}
		set
		{
			((IList)_weightedObjectList)[index] = value;
			_weightedObjectList.Sort();
			RecalculateWeight();
		}
	}
	
	bool IList.IsFixedSize
	{
		get
		{
			return false;
		}
	}
	
	#endregion
	
	#region ICollection implementation
	void ICollection.CopyTo(Array array, int index)
	{
		((ICollection)_weightedObjectList).CopyTo(array, index);
	}
	
	object ICollection.SyncRoot
	{
		get
		{
			return ((ICollection)_weightedObjectList).SyncRoot;
		}
	}
	
	bool ICollection.IsSynchronized
	{
		get
		{
			return ((ICollection)_weightedObjectList).IsSynchronized;
		}
	}
	#endregion
	
	public void Sort()
	{
		_weightedObjectList.Sort();
	}
	
	public void RecalculateWeight()
	{
		_totWeight = 0;
		foreach(WeightedSpriteInfo foi in _weightedObjectList)
		{
			_totWeight += foi.weight;
		}
	}

	public WeightedSpriteInfo GetRandomFromList()
	{
		if(_weightedObjectList.Count == 0)
		{
			return null;
		}

		int roll = UnityEngine.Random.Range(0, _totWeight);
		foreach(WeightedSpriteInfo woi in _weightedObjectList)
		{
			if(roll < woi.weight)
			{
				return woi;
			}
			roll -= woi.weight;
		}
		return _weightedObjectList[0];
	}
	
	public int TotalWeight
	{
		get { return _totWeight; }
	}
}
