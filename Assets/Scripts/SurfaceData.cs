using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Surface" , menuName = "Surface")]
public class SurfaceData : ScriptableObject
{
	public FloorTypes FloorType; //Dont know why I need this but think it can be useful  
	public float MaxSpeed = 1f;
	public float Friction = 1f;
	public float SlipAngle;

	public enum FloorTypes
	{
		Wood, Ice, Send
	}
}
