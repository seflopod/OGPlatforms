using UnityEngine;

[System.Serializable]
public class ShakeInfo
{
	public float ShakeTime = 0f;
	public Vector2 ShakeMagnitude = Vector2.zero;

	public ShakeInfo()
	{}

	public ShakeInfo(float shakeTime, Vector2 shakeMagnitude)
	{
		ShakeTime = shakeTime;
		ShakeMagnitude = shakeMagnitude;
	}

	public static ShakeInfo operator +(ShakeInfo si1, ShakeInfo si2)
	{
		return new ShakeInfo(si1.ShakeTime + si2.ShakeTime, si1.ShakeMagnitude + si2.ShakeMagnitude);
	}

	public static ShakeInfo operator -(ShakeInfo si1, ShakeInfo si2)
	{
		return new ShakeInfo(si1.ShakeTime - si2.ShakeTime, si1.ShakeMagnitude - si2.ShakeMagnitude);
	}
}
