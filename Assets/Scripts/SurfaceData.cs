using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Surface" , menuName = "Surface")]
public class SurfaceData : ScriptableObject
{
	[Tooltip("Variable that slow player if it is < 1 player is moving faster")]
	public float Slowing = 1f;
	[Tooltip("Angle from which player start slip")]
	public float SlipAngle;
	[Tooltip("Like friction but without physics")]
	public float SharpnessOnGround;
	
}
