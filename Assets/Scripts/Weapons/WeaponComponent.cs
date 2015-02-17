using UnityEngine;
using OgreToast.Utility;

[RequireComponent(typeof(SpriteRenderer), typeof(AudioSource))]
public class WeaponComponent : MonoBehaviour
{
	public delegate void WeaponFireHandler(object sender, WeaponInfo weaponInfo);
	public event WeaponFireHandler FireWeapon;

	public int BulletPoolSize = 300;
	[SerializeField]
	private WeaponInfo _currentWeapon;

	private Transform _bulletStartTrans;
	private SimpleTimer _fireTimer;
	private Vector2 _aimDir = Vector2.right;
	private GameObjectPool _bulletPool;

	private WeaponInfo _baseWeapon;
	private int _currentAmmo = 0;

	public WeaponInfo CurrentWeapon
	{
		get { return _currentWeapon; }
		set
		{
			_currentWeapon = value;
			//need to swap weapon sprite and maybe animations?
			GetComponent<SpriteRenderer>().sprite = _currentWeapon.WeaponSprite;
			_fireTimer.TargetTime = _currentWeapon.RateOfFire;
			if(_currentWeapon.HasAmmo)
			{
				_currentAmmo = _currentWeapon.StartAmmo;
			}
		}
	}

	public int CurrentAmmo
	{
		get { return _currentAmmo; }
	}

	private void Awake()
	{
		_baseWeapon = _currentWeapon;
	}

	private void Start()
	{
		_bulletStartTrans = transform.FindChild("bullet start");
		_fireTimer = new SimpleTimer(_currentWeapon.RateOfFire);

		_bulletPool = GameObjectPool.CreateNewPool(GameManager.Instance.BulletPrefab, BulletPoolSize, delegate(GameObject go) {
			if(transform.parent.gameObject.layer == LayerMask.NameToLayer("Enemies") || transform.parent.gameObject.layer == LayerMask.NameToLayer("Flying_Enemies"))
			{
				go.layer = LayerMask.NameToLayer("enemy_bullets");
			}
			else if(transform.parent.gameObject.layer == LayerMask.NameToLayer("Player"))
			{
				go.layer = LayerMask.NameToLayer("player_bullets");
			}
		});

		AudioSource asrc = GetComponent<AudioSource>();
		asrc.playOnAwake = false;
		asrc.loop = false;
	}

	public void Aim(Vector2 dir)
	{
		if(dir == Vector2.zero)
		{
			dir = _aimDir;
		}
		else
		{
			_aimDir = dir;
		}
		
		float angle = 0f;
		if(dir.x != 0f)
		{
			if(dir.y < 0f)
			{
				angle = -45f;
			}
			else if(dir.y > 0f)
			{
				angle = 45f;
			}
			else
			{
				angle = 0f;
			}
		}
		else
		{
			if(dir.y < 0f)
			{
				angle = -90f;
			}
			else if(dir.y > 0f)
			{
				angle = 90f;
			}
			else
			{
				angle = 0f;
			}
		}
		transform.localRotation = Quaternion.Euler(0f, 0f, angle);
	}

	public void Fire(Vector2 currentVelocity)
	{
		if((!_currentWeapon.HasAmmo || _currentAmmo > 0) && (!_fireTimer.IsRunning || _fireTimer.IsExpired))
		{
			_fireTimer.Stop();
			GameObject bullet = _bulletPool.GetGameObject();
			bullet.SetActive(true);
			bullet.transform.position = _bulletStartTrans.position;
			bullet.transform.rotation = transform.rotation;

			bullet.renderer.enabled = true;
			bullet.GetComponent<SpriteRenderer>().sprite = _currentWeapon.BulletSprite;
			bullet.collider2D.enabled = true;
			BoxCollider2D box = bullet.collider2D as BoxCollider2D;
			box.size = _currentWeapon.BulletSprite.bounds.size;
			box.center = Vector2.zero;
			BulletBehaviour bb = bullet.GetComponent<BulletBehaviour>();
			bb.DamageValue = _currentWeapon.DamageDealt;

			if(_currentWeapon.OverrideBulletLayer)
			{
				bullet.layer = LayerMask.NameToLayer(_currentWeapon.BulletLayerName);
			}
			bullet.rigidbody2D.velocity = _currentWeapon.BulletSpeed * _aimDir + currentVelocity;

			if(_currentWeapon.FireSound != null)
			{
				audio.PlayOneShot(_currentWeapon.FireSound);
			}

			--_currentAmmo;
			_fireTimer.Start();

			WeaponFireHandler handler = FireWeapon;
			if(handler != null)
			{
				handler(this, _currentWeapon);
			}
		}

		if(_currentWeapon.HasAmmo && _currentAmmo <= 0)
		{
			CurrentWeapon = _baseWeapon;
		}
	}

	public void ChangeWeapon(WeaponInfo newInfo)
	{
		_currentWeapon = newInfo;
		_fireTimer.TargetTime = _currentWeapon.RateOfFire;
	}

	public void Reset()
	{
		ChangeWeapon(_baseWeapon);
	}
}
