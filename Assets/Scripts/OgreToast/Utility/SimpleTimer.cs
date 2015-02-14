using UnityEngine;
using System.Collections;

namespace OgreToast.Utility
{
	public class SimpleTimer
	{
		private float _curTime = 0f;
		private float _startTime = 0f;
		private bool _isPaused = false;
		private bool _isRunning = false;

		public SimpleTimer() : this(0f)
		{}

		public SimpleTimer(float targetTime)
		{
			TargetTime = targetTime;
		}

		public void Start()
		{
			if(_isRunning)
			{
				Stop();
			}
			_startTime = Time.unscaledTime - ((_isPaused) ? _curTime : 0f);
			_isPaused = false;
			_isRunning = true;
		}

		public void Pause()
		{
			_curTime = CurrentTime;
			_isPaused = true;
		}

		public void Stop()
		{
			_curTime = 0f;
			_isPaused = false;
			_isRunning = false;
		}

		public float TargetTime { get; set; }

		public float CurrentTime
		{
			get
			{
				return Time.unscaledTime - _startTime;
			}
		}

		public bool IsExpired
		{
			get
			{
				return (Time.unscaledTime - _startTime >= TargetTime);
			}
		}

		public bool IsRunning
		{
			get { return _isRunning; }
		}

		public bool IsPaused
		{
			get { return _isPaused; }
		}
	}
}