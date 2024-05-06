using System;
using GXPEngine;

namespace Physics {

	/// <summary>
	/// A superclass for all shapes in our physics engine (like circles, lines, AABBs, ...)
	/// </summary>
	public class Collider {
		public readonly GameObject owner;
		public Vec2 position;

		public Collider(GameObject pOwner, Vec2 startPosition) {
			owner=pOwner;
			position=startPosition;
		}

		// Implement this method in subclasses:
		public virtual CollisionInfo GetEarliestCollision(Collider other, Vec2 velocity) {
			throw new NotImplementedException();
		}

		// Implement this method in subclasses:
		public virtual bool Overlaps(Collider other) {
			throw new NotImplementedException();
		}
	}
}
