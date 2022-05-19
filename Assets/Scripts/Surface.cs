using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Surface : MonoBehaviour
{
	[Tooltip("Variable that slow player if it is < 1 player is moving faster")]
	public float Slowing;
	[Tooltip("Angle from which player start slip")]
	public float SlipAngle;
	[Tooltip("Like friction but without physics")]
	public float SharpnessOnGround;
	[Tooltip("Creat this in Creat/Surfaces")]
	public SurfaceData surfaceData;
	private void Awake()
	{
		Slowing = surfaceData.Slowing;
		SlipAngle = surfaceData.SlipAngle;
		SharpnessOnGround = surfaceData.SharpnessOnGround;
	}
}
