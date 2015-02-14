using UnityEngine;
using System.Collections;
using OgreToast.Utility;

public class GameManager : Singleton<GameManager>
{
	public int BulletPoolSize = 300;
	public GameObject BulletPrefab;

	private GameObjectPool _bulletPool;

	private void Start()
	{
		_bulletPool = GameObjectPool.CreateNewPool(BulletPrefab, BulletPoolSize);
	}

	public GameObject GetBullet()
	{
		return _bulletPool.GetGameObject();
	}
}