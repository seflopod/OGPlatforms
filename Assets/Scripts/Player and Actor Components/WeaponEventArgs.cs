using UnityEngine;
using System.Collections;

public class WeaponEventArgs : System.EventArgs
{
	public WeaponInfo Weapon;
	public Vector2 FireDirection;
	
	public WeaponEventArgs (WeaponInfo weapon, Vector2 fireDirection)
	{
		this.Weapon = weapon;
		this.FireDirection = fireDirection;
	}
}
