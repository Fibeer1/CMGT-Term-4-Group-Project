using System;
using GXPEngine;

namespace Physics {
	// Example collider:
	// A simple horizontal, one-way collision line segment collider.
	class HorizontalLineSegment : Collider {
		public readonly float deltaX;
		public readonly Vec2 normal;

		public Vec2 leftPoint {
			get {
				if (deltaX<0) {
					return position - new Vec2(deltaX, 0);
				} else {
					return position;
				}
			}
		}
		public Vec2 rightPoint {
			get {
				if (deltaX<0) {
					return position;
				} else {
					return position + new Vec2(deltaX, 0);
				}
			}
		}

		// Create a line segment between position and position + (pDeltaX,0).
		// Convention: the normal points up if and only if pDeltaX is positive.
		public HorizontalLineSegment(GameObject owner, Vec2 position, float pDeltaX) : base(owner, position) {
			deltaX=pDeltaX;
			normal = new Vec2(0, -Mathf.Sign(deltaX));
		}

		// TODO: If you also want to call GetOverlaps and GetEarliestCollision for line segments (moving line segments?),
		// Implement those methods here!
	}
}