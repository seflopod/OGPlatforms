using UnityEngine;

[System.Serializable]
public class WeaponInfo
{
	public string Name = "";
	public Sprite WeaponSprite;
	public Sprite UserInterfaceSprite;
	public Sprite UserInterfaceBG;
	public float RateOfFire = 0.1f;
	public int DamageDealt = 10;
	public Sprite BulletSprite;
	public float BulletSpeed = 20f;
	public int BulletImpactPoolIndex = 0;
	public bool HasAmmo = false;
	public int StartAmmo = 10;
	public Vector3 BulletStartOffset = Vector3.zero;
	public bool OverrideBulletLayer = false;
	public string BulletLayerName = "";
	public AudioClip FireSound;
}