using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using OgreToast.Utility;

public class GameManager : Singleton<GameManager>
{
	public GameObject BulletPrefab;
	public GameObject[] BulletImpactPrefabs;
	public int BulletImpactPoolSize = 50;

	private GameObjectPool[] _bulletImpactPools;
	private Queue<GameObject> _destructionQueue;
	
	private bool _isDestroying = false;
	private int _queueSize = 50;

	private void Start()
	{
		_bulletImpactPools = new GameObjectPool[BulletImpactPrefabs.Length];
		for(int i=0;i<_bulletImpactPools.Length;++i)
		{
			_bulletImpactPools[i] = GameObjectPool.CreateNewPool(BulletImpactPrefabs[i], BulletImpactPoolSize);
		}
		_destructionQueue = new Queue<GameObject>(_queueSize);
	}

	private void LateUpdate()
	{
		if(!_isDestroying && _destructionQueue.Count >= _queueSize / 2)
		{
			_isDestroying = true;
			StartCoroutine(DestroyInQueue());
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

	public void AddToDestroyQueue(GameObject toDestroy)
	{
		_destructionQueue.Enqueue(toDestroy);
	}

	public IEnumerator DestroyInQueue()
	{
		_isDestroying = true;
		while(_destructionQueue.Count > 0)
		{
			GameObject toDestroy = _destructionQueue.Dequeue();
			GameObject.Destroy(toDestroy);
			yield return null;
		}
		_isDestroying = false;
	}

}