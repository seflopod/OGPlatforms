using UnityEngine;
using System.Collections;

namespace OgreToast.Utility
{
	//this could probably benefit from implementing IEnumerable
	public class GameObjectPool
	{
		#region pool_factory
		public delegate void ExtraPoolInitMethod(GameObject go);

		/// <summary>
		/// Factory for creating new pools.
		/// </summary>
		/// <returns>The new pool.</returns>
		/// <param name="prefab">Prefab to use for the pool.</param>
		/// <param name="size">The pool size.</param>
		public static GameObjectPool CreateNewPool(GameObject prefab, int size, ExtraPoolInitMethod extraInit)
		{
			//if we don't have something to host the coroutine, then we're fucked.

			GameObjectPool ret = new GameObjectPool(size);
			GameObject poolObj = new GameObject("Pool - " + prefab.name);
			poolObj.transform.position = Vector3.zero;

			object[] paramArray = new object[4] { ret, prefab, poolObj.transform, extraInit };

			//the preference is for a coroutine here, but in the case where we can't use it we still
			//need the pool to generate.
			if(CoroutineHost.Instance == null)
			{
				initPoolNonCoroutine(paramArray);
			}
			else
			{
				CoroutineHost.Instance.StartCoroutineDelegate(initPool, paramArray);
			}

			return ret;
		}

		//convenient overload for if we do not have extra initialization to do.
		public static GameObjectPool CreateNewPool(GameObject prefab, int size)
		{
			return CreateNewPool(prefab, size, null);
		}

		/// <summary>
		/// Initializes the pool in a coroutine.
		/// </summary>
		/// <returns>An IEnumerator indicating if there's more to init.</returns>
		/// <param name="paramArray">An array of objects used to pass parameters.  The second element is the GameObjectPool
		/// to use.  The third element should be the prefab to spawn.</param>
		private static IEnumerator initPool(object[] paramArray)
		{
			GameObjectPool pool = (GameObjectPool)paramArray[1];
			GameObject prefab = (GameObject)paramArray[2];
			Transform parent = (Transform)paramArray[3];
			ExtraPoolInitMethod extra = (ExtraPoolInitMethod)paramArray[4];

			for(int i=0;i<pool.mSize;++i)
			{
				//add object to the pool
				pool.aPool[i] = initGameObject(prefab, parent, extra);
				yield return null;
			}
		}

		//not sure if this is necessary
		private static void initPoolNonCoroutine(object[] paramArray)
		{
			GameObjectPool pool = (GameObjectPool)paramArray[1];
			GameObject prefab = (GameObject)paramArray[2];
			Transform parent = (Transform)paramArray[3];
			ExtraPoolInitMethod extra = (ExtraPoolInitMethod)paramArray[4];
			
			for(int i=0;i<pool.mSize;++i)
			{
				//add object to the pool
				pool.aPool[i] = initGameObject(prefab, parent, extra);
			}
		}

		private static GameObject initGameObject(GameObject prefab, Transform parent, ExtraPoolInitMethod extraInit)
		{
			GameObject go = (GameObject)GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity);
			go.transform.parent = parent;

			//if there's an extra init function for an object (adding behaviours or whatnot)
			//this is where we invoke it.
			if(extraInit != null)
			{
				extraInit(go);
			}
			
			//turn off the usual things that would make the object meaningful in the scene.  These are the renderer
			//(visibility) and the colliders for either physics system (affect objects in scene).  It might be better
			//to just disable the GameObject, but I seem to recall that causing problems...
			if(go.renderer != null)
			{
				go.renderer.enabled = false;
			}
			if(go.collider != null)
			{
				go.collider.enabled = false;
			}
			if(go.collider2D != null)
			{
				go.collider2D.enabled = false;
			}
			go.SetActive(false);

			return go;
		}
		#endregion

		#region private_fields
		private int mSize;
		private GameObject[] aPool;
		private int mCurIdx = 0;
		#endregion

		private GameObjectPool(int size)
		{
			mSize = size;
			aPool = new GameObject[mSize];
		}

		public GameObject GetGameObject()
		{
			GameObject ret = aPool[mCurIdx++];
			if(ret == null)
			{
				mCurIdx--;
			}
			else if(mCurIdx >= mSize)
			{
				mCurIdx = 0;
			}
			return ret;
		}

		public int PoolSize
		{
			get { return mSize; }
		}
	}
}