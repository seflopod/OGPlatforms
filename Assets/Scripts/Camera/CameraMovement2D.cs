using UnityEngine;
using System.Collections;
using OgreToast.Utility;

public class CameraMovement2D : MonoBehaviour
{
	[Header("Target Following")]
	public float dampTime = 0.15f;
	private Vector3 velocity = Vector3.zero;
	public Transform target;
	public bool UseViewportCushion = false;
	public Vector2 TargetViewportMinimum = Vector2.zero;
	public Vector2 TargetViewportMaximum = Vector2.zero;
	public bool ConstrainMovement = false;
	public Vector2 MaximumPoint = Vector2.zero;
	public Vector2 MinimumPoint = Vector2.zero;

	[Header("Shaking")]
	public bool ShakeOnTargetHit = true;
	public ShakeInfo TargetHitShake = new ShakeInfo();
	public bool ShakeOnExplosions = true;
	public ShakeInfo ExplosionShake = new ShakeInfo();
	public bool ShakeOnMachineGunFire = true;
	public ShakeInfo MachineGunFireShake = new ShakeInfo();
	public bool ShakeOnRocketLaunch = false;
	public ShakeInfo RocketLaunchShake = new ShakeInfo();
	public ShakeInfo MaxShakeInfo = new ShakeInfo();

	private bool _isShaking = false;
	private bool _startShake = false;
	private SimpleTimer _shakeTimer;
	private bool _doHitShake = false;
	private bool _doExplosionShake = false;
	private bool _doMachineGunShake = false;
	private bool _doRocketLaunchShake = false;

	private Vector3 _shakeOffset;
	private Vector3 _destination;

	private void Start()
	{
		_destination = transform.position;
		if(target == null)
		{
			GameObject go = GameObject.FindGameObjectWithTag("Player");
			if(go != null)
			{
				target = go.transform;
			}
		}

		HealthComponent hc = target.gameObject.GetComponent<HealthComponent>();
		if(hc != null)
		{
			hc.Hit += OnTargetHit;
		}
		_shakeTimer = new SimpleTimer();
	}
	
	private void Update() 
	{
		if(target != null)
		{
			Vector3 point = camera.WorldToViewportPoint(target.position);
			Vector3 viewportDest = new Vector3(0.5f, 0.5f, point.z);
			if(UseViewportCushion)
			{
				if(point.x >= TargetViewportMaximum.x)
				{
					viewportDest.x = TargetViewportMinimum.x;
				}
				else if(point.x <= TargetViewportMinimum.x)
				{
					viewportDest.x = TargetViewportMaximum.x;
				}

				if(point.y >= TargetViewportMaximum.y)
				{
					viewportDest.y = TargetViewportMinimum.y;
				}
				else if(point.y <= TargetViewportMinimum.y)
				{
					viewportDest.y = TargetViewportMaximum.y;
				}
			}

			if(!UseViewportCushion || viewportDest.x != 0.5f || viewportDest.y != 0.5f)
			{
				Vector3 delta = target.position - camera.ViewportToWorldPoint(viewportDest); //(new Vector3(0.5, 0.5, point.z));
				_destination = transform.position + delta;
			}

			if(_startShake)
			{
				_startShake = false;
				StartCoroutine(doShake());
			}


			Vector3 newPos = Vector3.SmoothDamp(transform.position, _destination, ref velocity, dampTime);

			if(_isShaking)
			{
				newPos += _shakeOffset;
			}

			if(ConstrainMovement)
			{
				newPos.x = Mathf.Min(Mathf.Max(MinimumPoint.x, newPos.x), MaximumPoint.x);
				newPos.y = Mathf.Min(Mathf.Max(MinimumPoint.y, newPos.y), MaximumPoint.y);
			}

			transform.position = newPos;
		}
		
	}

	private IEnumerator doShake()
	{
		ShakeInfo shakeInfo = new ShakeInfo();
		if(_doHitShake)
		{
			shakeInfo += TargetHitShake;
			_doHitShake = false;
		}

		if(_doExplosionShake)
		{
			shakeInfo += ExplosionShake;
			_doExplosionShake = false;
		}

		if(_doMachineGunShake)
		{
			shakeInfo += MachineGunFireShake;
			_doMachineGunShake = false;
		}

		if(_doRocketLaunchShake)
		{
			shakeInfo += RocketLaunchShake;
			_doRocketLaunchShake = false;
		}

		shakeInfo.ShakeTime = Mathf.Min (shakeInfo.ShakeTime, MaxShakeInfo.ShakeTime);
		shakeInfo.ShakeMagnitude.x = Mathf.Min(shakeInfo.ShakeMagnitude.x, MaxShakeInfo.ShakeMagnitude.x);
		shakeInfo.ShakeMagnitude.y = Mathf.Min(shakeInfo.ShakeMagnitude.y, MaxShakeInfo.ShakeMagnitude.y);
		_shakeTimer.TargetTime = shakeInfo.ShakeTime;

		_shakeTimer.Start();

		_isShaking = true;
		while(!_shakeTimer.IsExpired)
		{
			if(_doHitShake)
			{
				shakeInfo += TargetHitShake;
				_doHitShake = false;
			}
			
			if(_doExplosionShake)
			{
				shakeInfo += ExplosionShake;
				_doExplosionShake = false;
			}
			
			if(_doMachineGunShake)
			{
				shakeInfo += MachineGunFireShake;
				_doMachineGunShake = false;
			}
			
			if(_doRocketLaunchShake)
			{
				shakeInfo += RocketLaunchShake;
				_doRocketLaunchShake = false;
			}
			
			shakeInfo.ShakeTime = Mathf.Min (shakeInfo.ShakeTime, MaxShakeInfo.ShakeTime);
			shakeInfo.ShakeMagnitude.x = Mathf.Min(shakeInfo.ShakeMagnitude.x, MaxShakeInfo.ShakeMagnitude.x);
			shakeInfo.ShakeMagnitude.y = Mathf.Min(shakeInfo.ShakeMagnitude.y, MaxShakeInfo.ShakeMagnitude.y);
			_shakeTimer.TargetTime = shakeInfo.ShakeTime;
			float pct = _shakeTimer.CurrentTime / _shakeTimer.TargetTime;
			float xMag = Mathf.Lerp(shakeInfo.ShakeMagnitude.x, 0f, pct);
			float yMag = Mathf.Lerp(shakeInfo.ShakeMagnitude.y, 0f, pct);
			_shakeOffset.x = Random.Range(-1f, 1f) * xMag;
			_shakeOffset.y = Random.Range(-1f, 1f) * yMag;
			yield return null;
		}
		_isShaking = false;
		_shakeOffset = Vector2.zero;
	}

	private void OnTargetHit(object sender, Vector2 dir)
	{
		_doHitShake = ShakeOnTargetHit;
		_startShake = !_isShaking && _doHitShake;
	}

	private void OnExplosion(object sender, System.EventArgs e)
	{
		_doExplosionShake = ShakeOnExplosions;
		_startShake = !_isShaking && _doExplosionShake;
	}

	private void OnMachineGunFire(object sender, System.EventArgs e)
	{
		_doMachineGunShake = ShakeOnMachineGunFire;
		_startShake = !_isShaking && _doMachineGunShake;
	}

	private void OnRocketLaunch(object sender, System.EventArgs e)
	{
		_doRocketLaunchShake = ShakeOnRocketLaunch;
		_startShake = !_isShaking && _doRocketLaunchShake;
	}
}