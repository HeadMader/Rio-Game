using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	[Header("Movement")]
	[SerializeField] private float gravityDownForce = 20f;
	[SerializeField] private float maxSpeedOnGround = 10f;
	[SerializeField] private float movementSharpnessOnGround = 15;
	[SerializeField] private float maxSpeedInAir = 10f;
	[SerializeField] private float accelerationSpeedInAir = 25f;

	[SerializeField] private float rotationSpeed = 150.0f;
	[Range(0.1f, 1f)]
	public float _rotationMultiplier = 0.4f;

	[Header("Jump")]
	public float jumpForce = 9f;

	[Header("LayerMask")]
	[SerializeField] private LayerMask layerMask;

	private PlayerInput playerInput;
	public Vector3 characterVelocity { get; set; }
	public bool isGrounded { get; private set; }
	public bool isDead { get; private set; }


	public float RotationMultiplier = 1f;

	private CharacterController controller;

	private Vector3 groundNormal;
	private Surface surface;
	private float lastTimeJumped = 0f;
	public float groundCheckDistance;
	const float k_JumpGroundingPreventionTime = 0.2f;
	const float k_GroundCheckDistanceInAir = 0.07f;
	private bool isVisible;
	private Vector3 SnowballVelocity;
	#region Init
	private void Awake()
	{
		playerInput = new PlayerInput();
	}
	private void OnEnable()
	{
		playerInput.Enable();
	}
	private void OnDisable()
	{
		playerInput.Disable();
	}
	void Start()
	{
		controller = GetComponent<CharacterController>();
	}
	#endregion
	void Update()
	{

		GroundCheck();
		HandleCharacterMovement();
	}
	void GroundCheck()
	{
		float chosenGroundCheckDistance = isGrounded ? (controller.skinWidth + groundCheckDistance) : k_GroundCheckDistanceInAir;

		isGrounded = false;
		groundNormal = Vector3.up;

		if (Time.time >= lastTimeJumped + k_JumpGroundingPreventionTime)
		{
			if (Physics.CapsuleCast(GetCapsuleBottomHemisphere(), GetCapsuleTopHemisphere(), controller.radius, Vector3.down, out RaycastHit hit, chosenGroundCheckDistance, layerMask))
			{
				groundNormal = hit.normal;
				surface = hit.transform.GetComponent<Surface>();
				if (surface != null)
				{
					if (Vector3.Dot(hit.normal, transform.up) > 0f &&
						IsNormalUnderSlopeLimit(groundNormal))
					{
						//Debug.Log(Vector3.Dot(hit.normal, transform.up));
						isGrounded = true;
						if (hit.distance > controller.skinWidth)
						{
							controller.Move(Vector3.down * hit.distance);
						}
					}
				}
				else
				{
					Debug.LogError("GameObject doesnt have Surface component");
				}
			}
		}
	}

	private void HandleCharacterMovement()
	{

		float speedModifier = 1f;
		Vector3 moveInput = ConvertVecto2ToVector3(playerInput.Player.Move.ReadValue<Vector2>());
		Vector3 worldspaceMoveInput = transform.TransformVector(moveInput);

		if (isGrounded)
		{
			
				Vector3 targetVelocity = worldspaceMoveInput * maxSpeedOnGround / surface.Slowing;
				targetVelocity = GetDirectionReorientedOnSlope(targetVelocity.normalized, groundNormal) * targetVelocity.magnitude;
				characterVelocity = Vector3.Lerp(characterVelocity, targetVelocity, surface.SharpnessOnGround * Time.deltaTime);

			if (Vector3.Angle(transform.up, groundNormal) >= surface.SlipAngle)
			{
				Vector3 gravitation = Vector3.down * gravityDownForce * maxSpeedOnGround * Time.deltaTime;    //Player  doesnt accelerate for more predictable behavior
				characterVelocity += Vector3.ProjectOnPlane(gravitation,groundNormal);
			}
		}
		else
		{
			characterVelocity += worldspaceMoveInput * accelerationSpeedInAir * Time.deltaTime;
			float verticalVelocity = characterVelocity.y;
			Vector3 horizontalVelocity = Vector3.ProjectOnPlane(characterVelocity, Vector3.up);
			horizontalVelocity = Vector3.ClampMagnitude(horizontalVelocity, maxSpeedInAir * speedModifier);
			characterVelocity = horizontalVelocity + (Vector3.up * verticalVelocity);
			characterVelocity += Vector3.down * gravityDownForce * Time.deltaTime;
		}

		Vector3 capsuleBottomBeforeMove = GetCapsuleBottomHemisphere();
		Vector3 capsuleTopBeforeMove = GetCapsuleTopHemisphere();
		controller.Move(characterVelocity * Time.deltaTime);

		if (Physics.CapsuleCast(capsuleBottomBeforeMove, capsuleTopBeforeMove, controller.radius, characterVelocity.normalized, out RaycastHit hit, characterVelocity.magnitude * Time.deltaTime))
		{
			characterVelocity = Vector3.ProjectOnPlane(characterVelocity, hit.normal);
		}
	}

	private Vector3 ConvertVecto2ToVector3(Vector2 vector2)
	{
		return new Vector3(vector2.x, 0, vector2.y);
	}

	private Vector3 GetCapsuleBottomHemisphere()
	{
		return transform.position + (controller.center - transform.up * (controller.height / 2 - controller.radius));
	}
	private Vector3 GetCapsuleTopHemisphere()
	{
		return transform.position + (controller.center + transform.up * (controller.height / 2 - controller.radius));
	}
	public Vector3 GetDirectionReorientedOnSlope(Vector3 direction, Vector3 slopeNormal)
	{
		Vector3 directionRight = Vector3.Cross(direction, Vector3.up);
		return Vector3.Cross(slopeNormal, directionRight).normalized;
	}
	bool IsNormalUnderSlopeLimit(Vector3 normal)
	{
		return Vector3.Angle(transform.up, normal) <= controller.slopeLimit;
	}
	private void OnTriggerStay(Collider other)
	{
		if (other.gameObject.layer == 7) //SnowBall layer
		{
			Rigidbody SnowballRigidbody = other.transform.GetComponent<Rigidbody>();
			SnowballVelocity = SnowballRigidbody.velocity;
			//Search angle betwwen snowball velocity vector and vector from snowball to player 
			float angle = Vector3.Angle(Vector3.ProjectOnPlane(SnowballVelocity, Vector3.up),
				Vector3.ProjectOnPlane(transform.position - other.transform.position, Vector3.up));
			if (angle <= 30f)
			{
				transform.parent = other.transform;
				transform.localPosition = Vector3.zero;
				controller.enabled = false;
			}
		}
	}

	private void Fall()
	{

	}

	private void ShowHideCoursor()
	{
		if (!isVisible)
		{
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
			isVisible = true;
		}
		else
		{
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
			isVisible = false;
		}
	}

	//private void OnDrawGizmos()
	//{
	//	Gizmos.color = Color.red;
	//	Gizmos.DrawSphere(GetCapsuleTopHemisphere(), 0.1f);
	//	Gizmos.DrawSphere(GetCapsuleBottomHemisphere(), 0.1f);
	//}

}
