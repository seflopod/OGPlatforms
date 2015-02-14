using UnityEngine;

[System.Serializable]
public class WeightedSpriteInfo : System.IComparable<WeightedSpriteInfo>, System.IComparable
{
	public int weight = 1;
	public Sprite obj;
	

	#region IComparable implementation
	public int CompareTo(WeightedSpriteInfo other)
	{
		if(other == null)
		{
			return 1;
		}
		return weight.CompareTo(other.weight);
	}
	#endregion
	
	#region IComparable implementation
	int System.IComparable.CompareTo(object obj)
	{
		if(obj is WeightedSpriteInfo)
		{
			WeightedSpriteInfo other = (WeightedSpriteInfo)obj;
			return this.CompareTo(other);
		}
		throw new System.ArgumentException("obj is not a WeightedObjectInfo");
	}
	#endregion
}
