using UnityEngine;
using System.Collections;

namespace OgreToast.Utility
{
	public static class Tweens
	{
		//http://www.robertpenner.com/easing/penner_chapter7_tweening.pdf
		public delegate float TweenFunc(float currentTime, float beginValue, float totalValueChange, float duration);
		public delegate Vector3 Vector3TweenFunc(float currentTime, Vector3 beginVec, Vector3 totalChangeVec, float duration);
		
		public static float LinearTween(float currentTime, float beginValue, float totalValueChange, float duration)
		{
			return totalValueChange * currentTime / duration + beginValue;
		}
		
		public static float QuadraticEaseInTween(float currentTime, float beginValue, float totalValueChange, float duration)
		{
			float t = currentTime / duration;
			return totalValueChange * t * t + beginValue;
		}

		public static Vector3 Vector3Linear(float currentTime, Vector3 beginVec, Vector3 totalChangeVec, float duration)
		{
			return currentTime / duration * totalChangeVec + beginVec;
		}

		public static Vector3 Vector3QuadraticEaseIn(float currentTime, Vector3 beginVec, Vector3 totalChangeVec, float duration)
		{
			float t = currentTime / duration;
			return totalChangeVec * t * t + beginVec;
		}
	}
}