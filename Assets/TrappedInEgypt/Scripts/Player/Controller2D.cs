using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider2D))]
public class Controller2D : MonoBehaviour {

	const float skinWidth = 0.015f;

	public LayerMask collisionMask;
	[Range(1,89)] public float maxClimbAngle = 80;
	[Range(1,89)] public float maxDescendAngle = 75;
	public int horizontalRayCount = 4;
	public int verticalRayCount = 4;
	public bool viewRays = false;

	private BoxCollider2D boxCollider;
	private RaycastOrigins raycastOrigins;
	public CollisionInfo collisions;
	private float horizontalRaySpacing;
	private float verticalRaySpacing;
	private Vector2 moveAmountOld;

	void Start () {
		boxCollider = GetComponent<BoxCollider2D>();
		CalculateRaySpacing ();
	}

	// Called to move the character
	public void Move(Vector2 moveAmount) {
		UpdateRaycastOrigins ();
		collisions.Reset ();
		moveAmountOld = moveAmount;

		if (moveAmount.y < 0) {
			DescendSlope (ref moveAmount);
		}
		if (moveAmount.x != 0) {
			HorizontalCollisions (ref moveAmount);
		}
		if (moveAmount.y != 0) {
			VerticalCollisions (ref moveAmount);
		}

		transform.Translate (moveAmount);
	}

	// Modify the moveAmount based on horizontal collisions
	private void HorizontalCollisions(ref Vector2 moveAmount) {
		float directionX = Mathf.Sign (moveAmount.x);
		float rayLength = Mathf.Abs (moveAmount.x) + skinWidth;

		for (int i = 0; i < horizontalRayCount; i++) {
			Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
			rayOrigin += Vector2.up * (horizontalRaySpacing * i);
			RaycastHit2D hit = Physics2D.Raycast (rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

			// Draw the rays
			if (viewRays) {
				Debug.DrawRay (rayOrigin, Vector2.right * directionX, Color.red);
			}

			if (hit) {
				// Calculate the angle of ground slope
				float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

				// Climb the slope
				if (i == 0 && slopeAngle <= maxClimbAngle) {
					if (collisions.descendingSlope) {
						// Fix bug transitioning from downward slope to upward slope
						collisions.descendingSlope = false;
						moveAmount = moveAmountOld;
					}

					float distanceToSlopeStart = 0;
					if (slopeAngle != collisions.slopeAngleOld) {
						// Encountering the new slope angle
						distanceToSlopeStart = hit.distance - skinWidth;
						// reduce x distance from moveAmount so that it only climbs when it reaches the slope
						moveAmount.x -= distanceToSlopeStart * directionX;
					}

					ClimbSlope (ref moveAmount, slopeAngle);

					// add back x distance to the moveAmount
					moveAmount.x += distanceToSlopeStart * directionX;
				}

				if (!collisions.climbingSlope || slopeAngle > maxClimbAngle) {
					// Set x moveAmount equals to the amount that is needed to move from current position to the intersected position of the ray
					moveAmount.x = (hit.distance - skinWidth) * directionX;
					rayLength = hit.distance;

					// Fix for bug when hitting floating obstacles on the slope
					if (collisions.climbingSlope) {
						moveAmount.y = Mathf.Tan (collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs (moveAmount.x);
					}

					collisions.left = directionX == -1;
					collisions.right = directionX == 1;
				}
			}
		}
	}

	// Modify the moveAmount based on vertical collisions
	private void VerticalCollisions(ref Vector2 moveAmount) {
		float directionY = Mathf.Sign (moveAmount.y);
		float rayLength = Mathf.Abs (moveAmount.y) + skinWidth;

		for (int i = 0; i < verticalRayCount; i++) {
			Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
			rayOrigin += Vector2.right * (verticalRaySpacing * i + moveAmount.x);
			RaycastHit2D hit = Physics2D.Raycast (rayOrigin, Vector2.up * directionY, rayLength, collisionMask);

			// Draw the rays
			if (viewRays) {
				Debug.DrawRay (rayOrigin, Vector2.up * directionY, Color.red);
			}
			 
			if (hit) {
				// Set y moveAmount equals to the amount that is needed to move from current position to the intersected position of the ray
				moveAmount.y = (hit.distance - skinWidth) * directionY;
				rayLength = hit.distance;

				// Fix for bug when hitting ceiling on the slope
				if (collisions.climbingSlope) {
					moveAmount.x = moveAmount.y / Mathf.Tan (collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign (moveAmount.x);
				}

				collisions.below = directionY == -1;
				collisions.above = directionY == 1;
			}
		}

		// Fix for bug 1 frame delay on changing slope
		if (collisions.climbingSlope) {
			float directionX = Mathf.Sign (moveAmount.x);
			rayLength = Mathf.Abs (moveAmount.y) + skinWidth;
			Vector2 rayOrigin = ((directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight) + Vector2.up * moveAmount.y;
			RaycastHit2D hit = Physics2D.Raycast (rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

			if (hit) {
				float slopeAngle = Vector2.Angle (hit.normal, Vector2.up);
				if (slopeAngle != collisions.slopeAngle) {
					moveAmount.x = (hit.distance - skinWidth) * directionX;
					collisions.slopeAngle = slopeAngle;
				}
			}
		}
	}

	// Modify the moveAmount when climbing up the slope
	private void ClimbSlope(ref Vector2 moveAmount, float slopeAngle) {
		float moveDistance = Mathf.Abs (moveAmount.x);
		float climbMoveAmountY = Mathf.Sin (slopeAngle * Mathf.Deg2Rad) * moveDistance;

		if (moveAmount.y <= climbMoveAmountY) {
			// Assume we are standing on the slope
			moveAmount.y = Mathf.Sin (slopeAngle * Mathf.Deg2Rad) * moveDistance;
			moveAmount.x = Mathf.Cos (slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign (moveAmount.x);

			collisions.below = true;
			collisions.climbingSlope = true;
			collisions.slopeAngle = slopeAngle;
		} // otherwise we are jupimping so don't modify the y moveAmount
	}

	// Modify the moveAmount when descending down the slope
	private void DescendSlope(ref Vector2 moveAmount) {
		float directionX = Mathf.Sign (moveAmount.x);
		Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomRight : raycastOrigins.bottomLeft;
		RaycastHit2D hit = Physics2D.Raycast (rayOrigin, Vector2.down, Mathf.Infinity, collisionMask);

		if (hit) {
			float slopeAngle = Vector2.Angle (hit.normal, Vector2.up);
			if (slopeAngle != 0 && slopeAngle <= maxDescendAngle) {
				// Check the direction moving is really descending the slope
				if (Mathf.Sign (hit.normal.x) == directionX) {
					// If the distance down is less then how far we need to move
					if (hit.distance - skinWidth <= Mathf.Tan (slopeAngle * Mathf.Deg2Rad) * Mathf.Abs (moveAmount.x)) {
						float moveDistance = Mathf.Abs (moveAmount.x);
						float descendMoveAmountY = Mathf.Sin (slopeAngle * Mathf.Deg2Rad) * moveDistance;
						moveAmount.x = Mathf.Cos (slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign (moveAmount.x);
						moveAmount.y -= descendMoveAmountY;

						collisions.below = true;
						collisions.descendingSlope = true;
						collisions.slopeAngle = slopeAngle;
					}
				}
			}
		}
	}

	// Set the distance between each ray
	private void CalculateRaySpacing() {
		Bounds bounds = boxCollider.bounds;
		bounds.Expand(skinWidth * -2);

		horizontalRayCount = Mathf.Clamp (horizontalRayCount, 2, int.MaxValue);
		verticalRayCount = Mathf.Clamp (verticalRayCount, 2, int.MaxValue);

		horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
		verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
	}

	// Update the value of raycastOrigins
	private void UpdateRaycastOrigins() {
		Bounds bounds = boxCollider.bounds;
		bounds.Expand(skinWidth * -2);

		raycastOrigins.bottomLeft = new Vector2 (bounds.min.x, bounds.min.y);
		raycastOrigins.bottomRight = new Vector2 (bounds.max.x, bounds.min.y);
		raycastOrigins.topLeft = new Vector2 (bounds.min.x, bounds.max.y);
		raycastOrigins.topRight = new Vector2 (bounds.max.x, bounds.max.y);
	}

	struct RaycastOrigins {
		public Vector2 topLeft, topRight;
		public Vector2 bottomLeft, bottomRight;
	}

	public struct CollisionInfo {
		public bool above, below;
		public bool right, left;

		public bool climbingSlope, descendingSlope;
		public float slopeAngle, slopeAngleOld;

		public void Reset() {
			above = below = false;
			right = left = false;
			climbingSlope = false;
			descendingSlope = false;

			slopeAngleOld = slopeAngle;
			slopeAngle = 0;
		}
	}
}
