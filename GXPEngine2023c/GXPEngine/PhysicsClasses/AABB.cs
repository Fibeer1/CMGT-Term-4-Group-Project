using System;
using GXPEngine;

namespace Physics {
	// Example collider: an Axis-Aligned Bounding Box (=non rotated rectangle)
	// Currently it only implements GetEarliestCollision for fully horizontal or vertical movement
	class AABB : Collider {
		public readonly float halfWidth;
		public readonly float halfHeight;

		const float epsilon = 0.001f; // error range for floating point rounding errors

		public AABB(GameObject owner, Vec2 startPosition, float pHalfWidth, float pHalfHeight) : base(owner, startPosition) {
			halfWidth=pHalfWidth;
			halfHeight=pHalfHeight;
		}

		public override CollisionInfo GetEarliestCollision(Collider other, Vec2 velocity) {
			// TODO: add other things, like circles and rotated line segments here!
			if (other is AABB) {
				return GetEarliestCollision((AABB)other, velocity);
			} else if (other is HorizontalLineSegment) {
				return GetEarliestCollision((HorizontalLineSegment)other, velocity);
			} else {
				throw new NotImplementedException();
			}
		}

		CollisionInfo GetEarliestCollision(AABB other, Vec2 velocity) {
			// Keeping it simple for now - only horizontal and vertical movement allowed
			// (For the general case, see and solve Assignment 3.3)
			if (velocity.x==0) { // moving vertically

				// If their "x-axis projections" are not overlapping, it will never be a collision:
				if (other.position.x-other.halfWidth >= position.x+halfWidth-epsilon ||
					other.position.x+other.halfWidth <= position.x-halfWidth+epsilon)
					return null;

				float sign = Mathf.Sign(velocity.y);
				// If velocity.y is positive, this expression gives
				//   top of other minus bottom of this.
				// Otherwise, it gives
				//   bottom of other minus top of this:
				float startDistance =
					other.position.y - sign * other.halfHeight -
					(position.y + sign * halfHeight);
				float TOI = startDistance/velocity.y;

				if (TOI>=0 && TOI<1) {
					// case: not overlapping yet, but will be this frame:
					return new CollisionInfo(new Vec2(0, Mathf.Sign(velocity.y)), other, TOI);
				} else if (TOI<0 && Mathf.Abs(startDistance) < halfHeight + other.halfHeight) {
					// case: already overlapping, but moving towards deeper overlap: return TOI = 0.
					return new CollisionInfo(new Vec2(0, Mathf.Sign(velocity.y)), other, 0);
				}
				return null;
			} else if (velocity.y==0) { // moving horizontally

				// If their "y-axis projections" are not overlapping, it will never be a collision:
				if (other.position.y-other.halfHeight >= position.y+halfHeight-epsilon ||
					other.position.y+other.halfHeight <= position.y-halfHeight+epsilon)
					return null;

				float sign = Mathf.Sign(velocity.x);
				// If velocity.x is positive, this expression gives
				//   left of other minus right of this.
				// Otherwise, it gives
				//   right of other minus left of this:
				float startDistance =
					other.position.x - sign * other.halfWidth -
					(position.x + sign * halfWidth);
				float TOI = startDistance/velocity.x;

				if (TOI>=0 && TOI<1) {
					// case: not overlapping yet, but will be this frame:
					return new CollisionInfo(new Vec2(Mathf.Sign(velocity.x), 0), other, TOI);
				} else if (TOI<0 && Mathf.Abs(startDistance) < halfWidth + other.halfWidth) {
					// case: already overlapping, but moving towards deeper overlap: return TOI = 0.
					return new CollisionInfo(new Vec2(Mathf.Sign(velocity.x), 0), other, 0);
				}
				return null;
			} else {
				throw new NotImplementedException(); // For now...
			}
		}

		CollisionInfo GetEarliestCollision(HorizontalLineSegment other, Vec2 velocity) {
			if (velocity.y * other.normal.y>=0) {
				return null; // Moving away
			}
			if (velocity.x!=0) {
				throw new NotImplementedException();
			}

			// If their "x-axis projections" are not overlapping, it will never be a collision:
			if (other.leftPoint.x  >= position.x+halfWidth-epsilon ||
				other.rightPoint.x <= position.x-halfWidth+epsilon)
				return null;

			float sign = Mathf.Sign(velocity.y);
			// If velocity.y is positive, this expression gives
			//   line y minus bottom of this.
			// Otherwise, it gives
			//   line y minus top of this:
			float startDistance =
				other.position.y -
				(position.y + sign * halfHeight);
			float TOI = startDistance/velocity.y;

			if (TOI>=0 && TOI<1) {
				// case: not overlapping yet, but will be this frame:
				return new CollisionInfo(new Vec2(0, Mathf.Sign(velocity.y)), other, TOI);
			} else if (TOI<0 && Mathf.Abs(startDistance) < 0.1f) {
				// case: already slightly overlapping, but moving towards deeper overlap: return TOI = 0.
				return new CollisionInfo(new Vec2(0, Mathf.Sign(velocity.y)), other, 0);
			}
			return null;
		}

		public override bool Overlaps(Collider other) {
			if (other is AABB) {
				return Overlaps((AABB)other);
			} else if (other is HorizontalLineSegment) {
				return Overlaps((HorizontalLineSegment)other);
			} else {
				throw new NotImplementedException();
			}
		}

		public bool Overlaps(AABB other) {
			return
				other.position.x - other.halfWidth  < position.x + halfWidth &&
				other.position.x + other.halfWidth  > position.x - halfWidth &&
				other.position.y - other.halfHeight < position.y + halfHeight &&
				other.position.y + other.halfHeight > position.y - halfHeight;
		}

		public bool Overlaps(HorizontalLineSegment other) {
			return
				other.position.y   < position.y+halfHeight &&
				other.position.y   > position.y-halfHeight &&
				other.rightPoint.x > position.x-halfWidth &&
				other.leftPoint.x  < position.x+halfWidth;
		}
	}
}
