using GXPEngine; // For GameObject

namespace Physics {
	public class CollisionInfo {
		public readonly Vec2 normal;
		public readonly Collider other;
		public readonly float timeOfImpact;

		public CollisionInfo(Vec2 pNormal, Collider pOther, float pTimeOfImpact) {
			normal = pNormal;
			other = pOther;
			timeOfImpact = pTimeOfImpact;
		}
	}
}