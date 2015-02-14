using UnityEngine;
using System.Collections;

namespace OgreToast.Utility
{
	public class MeshScaleEffect : MonoBehaviour
	{
		public Vector3 startScale = Vector3.one;
		public Vector3 endScale = Vector3.one;
		public float duration = 1f;

		private Tweens.Vector3TweenFunc _tween = Tweens.Vector3QuadraticEaseIn;

		private bool _isScaling = false;
		private bool _doScale = false;

		private void Update()
		{
			if(_doScale && !_isScaling)
			{
				StartCoroutine(tweenScale());
			}
		}

		public void StartScaling()
		{
			if(!_isScaling)
			{
				_doScale = true;
			}
		}

		private IEnumerator tweenScale()
		{
			_isScaling = true;
			Vector3 deltaScale = endScale - startScale;
			transform.localScale = startScale;
			float time = 0f;
			while(time < duration)
			{
				time += Time.deltaTime;
				transform.localScale = _tween(time, startScale, deltaScale, duration);
				yield return null;
			}
			_doScale = false;
			_isScaling = false;
			renderer.enabled = false;
			gameObject.SetActive(false);
		}

		public bool IsScaling { get { return _isScaling; } }
	}
}
