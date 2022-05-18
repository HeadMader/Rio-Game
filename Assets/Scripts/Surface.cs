using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Surface : MonoBehaviour
{
	public float SlipAngle;
	public float Friction;
	public float MaxSpeed;
	public SurfaceData surfaceData;
	private void Awake()
	{
		MaxSpeed = surfaceData.MaxSpeed;
		Friction = surfaceData.Friction;
		SlipAngle = surfaceData.SlipAngle;
	}
}
