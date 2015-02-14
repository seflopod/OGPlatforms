using UnityEngine;
using OgreToast.Utility;

public class WeaponComponent : MonoBehaviour
{
	[SerializeField]
	private WeaponInfo _currentWeapon;

	private Transform _bulletStartTrans;
	private SimpleTimer _fireTimer;
	private Vector2 _aimDir = Vector2.right;

	public WeaponInfo CurrentWeapon
	{
		get { return _currentWeapon; }
	}

	private void Start()
	{
		_bulletStartTrans = transform.FindChild("bullet start");
		_fireTimer = new SimpleTimer(_currentWeapon.RateOfFire);


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

	public void Fire()
	{
		if(!_fireTimer.IsRunning || _fireTimer.IsExpired)
		{
			_fireTimer.Stop();
			GameObject bullet = GameManager.Instance.GetBullet();
			bullet.SetActive(true);
			bullet.transform.position = _bulletStartTrans.position;
			bullet.renderer.enabled = true;
			bullet.GetComponent<SpriteRenderer>().sprite = _currentWeapon.BulletSprite;
			bullet.collider2D.enabled = true;
			BoxCollider2D box = bullet.collider2D as BoxCollider2D;
			box.size = _currentWeapon.BulletSprite.bounds.size;
			box.center = bullet.transform.position;

			bullet.rigidbody2D.velocity = _currentWeapon.BulletSpeed * _aimDir;
			_fireTimer.Start();
		}
	}

	public void ChangeWeapon(WeaponInfo newInfo)
	{
		_currentWeapon = newInfo;
		_fireTimer.TargetTime = _currentWeapon.RateOfFire;
	}
}
