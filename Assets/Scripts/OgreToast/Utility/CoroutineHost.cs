using UnityEngine;
using System.Collections;

namespace OgreToast.Utility
{
	public class CoroutineHost : Singleton<CoroutineHost>
	{
		public delegate IEnumerator CoroutineMethodParams(object[] paramArray);
		public delegate IEnumerator CoroutineMethodNoParams();

		#region coroutine_hosting
		IEnumerator RunCoroutine(object[] paramArray)
		{
			if(paramArray.Length != 1)
			{
				return ((CoroutineMethodParams)paramArray[0])(paramArray);
			}
			else
			{
				return ((CoroutineMethodNoParams)paramArray[0])();
			}
		}

		public void StartCoroutineDelegate(CoroutineMethodNoParams coroutineMethod)
		{
			object[] paramArray = new object[1];
			paramArray[0] = coroutineMethod;
			StartCoroutine("RunCoroutine", paramArray);
		}

		public void StartCoroutineDelegate(CoroutineMethodParams coroutineMethod, object[] extraParams)
		{
			object[] paramArray = new object[1];
			if(extraParams != null)
			{
				paramArray = new object[1+extraParams.Length];
			}

			paramArray[0] = coroutineMethod;
			System.Array.Copy(extraParams, 0, paramArray, 1, extraParams.Length);
			StartCoroutine("RunCoroutine", paramArray);
		}
		#endregion
	}
}
