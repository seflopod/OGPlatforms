using UnityEngine;

[System.Serializable]
public class WeaponInfo
{
	public string Name = "";
	public Sprite WeaponSprite;
	public float RateOfFire = 0.1f;
	public int DamageDealt = 10;
	public Sprite BulletSprite;
	public float BulletSpeed = 20f;
	public bool HasAmmo = false;
	public int StartAmmo = 10;
}