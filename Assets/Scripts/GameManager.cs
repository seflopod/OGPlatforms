using UnityEngine;
using System.Collections;
using OgreToast.Utility;

public class GameManager : Singleton<GameManager>
{
	public GameObject BulletPrefab;
	public GameObject[] BulletImpactPrefabs;
	public int BulletImpactPoolSize = 50;

	private GameObjectPool[] _bulletImpactPools;

	private void Start()
	{
		_bulletImpactPools = new GameObjectPool[BulletImpactPrefabs.Length];
		for(int i=0;i<_bulletImpactPools.Length;++i)
		{
			_bulletImpactPools[i] = GameObjectPool.CreateNewPool(BulletImpactPrefabs[i], BulletImpactPoolSize);
		}
	}

	public GameObject GetBulletImpact(int poolIdx)
	{
		if(poolIdx >= _bulletImpactPools.Length)
		{
			return null;
		}
		return _bulletImpactPools[poolIdx].GetGameObject();
	}

}