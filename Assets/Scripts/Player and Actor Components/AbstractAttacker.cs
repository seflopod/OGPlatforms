using UnityEngine;
using System.Collections;
using OgreToast.Utility;
using OgreToast.Attributes;

public delegate void WeaponFiredHandler(object sender, WeaponEventArgs e);

/// <summary>
/// Component used to indicate that a <see cref="AbstractGameActor"/> makes attacks.  The assumption here is that those
/// attacks are using some sort of weapon that is fired.
/// </summary>
public abstract class AbstractAttacker : MonoBehaviour
{
	public event WeaponFiredHandler WeaponFired;

	/// <summary>
	/// The size of the bullet pool.  This is used in conjunction with <see cref="OgreToast.Utility.GameObjectPool"/> to
	/// provide bullets for this object.
	/// </summary>
	public int BulletPoolSize = 100;

	/// <summary>
	/// Whether or not the bullet start position is shown in the Unity editor.
	/// </summary>
	public bool ShowBulletStartPosition = true;

	/// <summary>
	/// Where the bullet is spawned.  Stored and edited as local coordinates.
	/// </summary>
	public Vector3 BulletStartPosition = Vector3.zero;

	/// <summary>
	/// The current weapon.
	/// </summary>
	[SerializeField]
	//[PrivateSerialized("Starting Weapon")]
	private WeaponInfo _currentWeapon = new WeaponInfo();

	/// <summary>
	/// The <see cref="OgreToast.Utility.GameObjectPool"/> used for bullets.
	/// </summary>
	protected GameObjectPool _bulletPool;

	/// <summary>
	/// A cooldown timer for firing using <see cref="OgreToast.Utility.SimpleTimer"/>.
	/// </summary>
	protected SimpleTimer _fireCooldown;

	/// <summary>
	/// The direction the weapon is aimed.
	/// </summary>
	protected Vector2 _aimDirection = Vector2.right;

	/// <summary>
	/// Gets or sets the ammo remaining.  This is only useful if the current weapon HAS ammo.
	/// </summary>
	/// <value>The ammo remaining.</value>
	public int AmmoRemaining { get; protected set; }

	/// <summary>
	/// Gets the current weapon.
	/// </summary>
	/// <value>The current weapon.</value>
	public WeaponInfo CurrentWeapon { get { return _currentWeapon; } }

	/// <summary>
	/// Aim in the specified direction.
	/// </summary>
	/// <param name="direction">The direction to aim.</param>
	/// <description>
	/// This only sets the aim direction.  This should be overridden if there is a visual indication (like a weapon
	/// rotating) so that sprites can be updated appropriately.
	/// </description>
	public virtual void Aim(Vector2 direction)
	{
		//no direction really indicated with a null vector, so just skip it
		if(direction != Vector2.zero)
		{
			_aimDirection = (direction.sqrMagnitude == 1f) ? direction : direction.normalized;
		}
	}

	/// <summary>
	/// Fire the current weapon.  This will check ammo to see if firing is allowed.
	/// </summary>
	public virtual void Fire()
	{
		if((!_currentWeapon.HasAmmo || AmmoRemaining > 0) && _fireCooldown.IsExpired)
		{

			_fireCooldown.Stop();
			GameObject bullet = _bulletPool.GetGameObject();
			bullet.SetActive(true);
			Quaternion rot = Quaternion.Euler(0f, 0f, Mathf.Rad2Deg * Mathf.Atan2(_aimDirection.y, Mathf.Abs(_aimDirection.x)));
			bullet.transform.position = transform.TransformPoint(rot * BulletStartPosition);
			bullet.transform.localScale = transform.localScale;
			if(bullet.transform.localScale.x < 0f)
			{
				rot.z *= -1f;
			}
			bullet.transform.rotation = rot;
			bullet.renderer.enabled = true;
			bullet.GetComponent<SpriteRenderer>().sprite = _currentWeapon.BulletSprite;
			bullet.collider2D.enabled = true;
			BoxCollider2D box = bullet.collider2D as BoxCollider2D;
			box.size = _currentWeapon.BulletSprite.bounds.size;
			box.center = Vector2.zero;
			BulletBehaviour bb = bullet.GetComponent<BulletBehaviour>();
			bb.DamageValue = _currentWeapon.DamageDealt;
			bullet.layer = LayerMask.NameToLayer(_currentWeapon.BulletLayerName);
			bullet.rigidbody2D.velocity = _currentWeapon.BulletSpeed * new Vector2(_aimDirection.x * transform.localScale.x, _aimDirection.y) + ((rigidbody2D != null) ? rigidbody2D.velocity : Vector2.zero);
			
			--AmmoRemaining;
			_fireCooldown.Start();
			
			WeaponFiredHandler handler = WeaponFired;
			if(handler != null)
			{
				handler(this, new WeaponEventArgs(_currentWeapon, _aimDirection));
			}
		}
	}

	/// <summary>
	/// Changes the weapon.  Changes the time remaining on the cooldown timer and resets the ammo count, but makes no
	/// other chanes (e.g. this does not do any sprite swaps).
	/// </summary>
	/// <param name="newWeapon">The new weapon.</param>
	public virtual void ChangeWeapon(WeaponInfo newWeapon)
	{
		_currentWeapon = newWeapon;
		_fireCooldown.TargetTime = _currentWeapon.RateOfFire;
		if(_currentWeapon.HasAmmo)
		{
			AmmoRemaining = _currentWeapon.StartAmmo;
		}
	}

	#region monobehaviour
	protected virtual void Start()
	{
		_fireCooldown = new SimpleTimer(_currentWeapon.RateOfFire);
		_bulletPool = GameObjectPool.CreateNewPool(_currentWeapon.BulletPrefab, BulletPoolSize);
	}

	protected virtual void OnDrawGizmosSelected()
	{
		if(ShowBulletStartPosition)
		{
			Vector3 worldPos = transform.TransformPoint(BulletStartPosition);
			Gizmos.color = new Color(1f, 1f, 0f, 0.75f);
			Gizmos.DrawSphere(worldPos, 0.02f);
		}
	}
	#endregion
}
