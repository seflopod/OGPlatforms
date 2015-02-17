using UnityEngine;
using System.Collections;
using OgreToast.Utility;

public class OrloMover : AbstractMover
{
	[Range(0f, 20f)]
	public float GlideForce = 9.81f;
	public float MaxGlideTime = 1f;

	private SimpleTimer _glideTimer;
}
