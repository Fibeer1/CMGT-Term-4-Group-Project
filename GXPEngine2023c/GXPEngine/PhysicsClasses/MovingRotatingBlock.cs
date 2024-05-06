using System;
using GXPEngine;
using GXPEngine.Core; // For Collision and Vector2

class MovingRotatingBlock : Sprite {
	Vec2 position;
	Vec2 velocity;
	Vec2 acceleration;

	float angularVelocity = 0; // Rotation change, in radians per frame
	float torque = 0; // "angular acceleration": can be used for e.g. "steering"

	// Physics properties (see below):
	float bounciness = 0.8f;
	float inverseMass = 1;
	float inverseMomentOfInertia = 0.01f;

	public MovingRotatingBlock(
		Vec2 pVelocity=new Vec2(), Vec2 pAcceleration=new Vec2(), float pRotation=0) : base("colors.png") {

		velocity=pVelocity;
		acceleration=pAcceleration;

		rotation=pRotation;

		SetOrigin(width/2, height/2);
		scaleX=0.5f;

		ComputeMassInertia(1);
		Console.WriteLine("Mass: {0} Moment of Interia: {1}",1/inverseMass,1/inverseMomentOfInertia);
	}

	void ComputeMassInertia(float density) {
		float mass = width * height * density;
		// Deduced from general triangle / polygon formula:
		//   (https://www.codeproject.com/Articles/1215961/Making-a-D-Physics-Engine-Mass-Inertia-and-Forces)
		// See also: https://en.wikipedia.org/wiki/List_of_moments_of_inertia (ignore depth)
		float inertia = mass * (width*width + height*height)/12;
		inverseMass = 1 / mass;
		inverseMomentOfInertia = 1 / inertia;
	}

	void RandomReset() {
		velocity = new Vec2(Utils.Random(-15, 16), 0);
		rotation = Utils.Random(0,90);
		position = new Vec2(100 + Utils.Random(0, 600), 0);

		angularVelocity=0;
		torque=0;
	}

	void UpdateScreenPosition() {
		x=position.x;
		y=position.y;
	}

	// Press Left or Right to make the block rotate:
	void Steering() {
		if (Input.GetKey(Key.LEFT)) {
			torque = -0.01f;
		} else if (Input.GetKey(Key.RIGHT)) {
			torque = 0.01f;
		} else {
			torque = 0;
		}
	}

	void Update() {
		Steering();

		// Standard Euler integration (for position):
		velocity += acceleration;
		position += velocity;

		// Doing something very similar for rotation - don't forget the radians to degrees conversion:
		angularVelocity += torque;
		rotation += Vec2.Rad2Deg(angularVelocity);

		// In this case we cannot compute point of impact perfectly and efficiently,
		// so we just use a "simply position reset" for every object we collide with. 
		// This might cause overlaps with other objects, but we'll have to accept that...

		// Discrete collision detection:
		GameObject[] overlaps = GetCollisions();

		// Collision resolve (position, velocity and angular velocity):
		foreach (GameObject other in overlaps) {
			if (other!=this) {
				ResolveCollision(other);
			}
		}
		UpdateScreenPosition();

		if (y>game.height || Input.GetKeyDown(Key.SPACE)) {
			RandomReset();
		}
	}

	void ResolveCollision(GameObject other) {
		// A GXPEngine method for finding all kinds of useful info about collisions (=overlaps):
		Collision colInfo = collider.GetCollisionInfo(other.collider);

		//// Translate from GXPEngine.Core.Vector2 to our own Vec2:
		// collision normal:
		Vec2 normal = new Vec2(colInfo.normal);
		// The exact collision point: 
		// (This might be a corner of this sprite, or a corner of the other sprite)
		Vec2 point = new Vec2(colInfo.point);

		// Resolve collision - the position reset part:
		// (Move until they are not overlapping anymore.)
		position += normal * colInfo.penetrationDepth;

		// Compute some vectors related to the exact collision point:

		Vec2 r1 = point - position; // from center of this to collision point
		Vec2 r1perp = new Vec2 (-r1.y, r1.x); // the normal vector of r1 (not unit!)

		// Compute the velocity of the actual collision point.
		// TODO: take angular velocity into account!
		// HINTS:
		// Add a vector that corresponds to the rotational movement of the collision point.
		// -The *direction* of this vector is equal to the direction that the point moves when rotating
		//    only a little bit (e.g. 1 degree). (Also called the "tangent" of the circle)
		// -The *length* of this vector is equal to the length this point moves along the circle, when applying
		//    the rotation (recall the circle circumference formula from lecture 1). 
		//    If the point is further from the center of this block (which is position), 
		//    the length becomes bigger.
		Vec2 pointVelocity = velocity;

		// See Chris Hecker, article 3, p.5, from
		// https://www.chrishecker.com/Rigid_Body_Dynamics
		// or 
		// Dunn & Parberry, 3D Math Primer for Graphics and Game Development, 2nd edition, 2011
		// p.620, for the full formula (with two moving bodies) and explanation:
		// (Impulse should be positive)
		float impulse =
			-(1+bounciness) * (pointVelocity.Dot(normal)) /
			(
				normal.Dot(normal) * inverseMass + 
				r1perp.Dot(normal) * r1perp.Dot(normal) * inverseMomentOfInertia
			);
		// No need to understand the details of this formula.
		// Please also don't ask your lab teacher to explain it. :-)
		if (impulse<0) {
			Console.WriteLine ("Impulse: {0}",impulse);
			return;
		}

		// Finally, resolve the velocity & angular velocity part of the collision, by
		// applying the impulse: 
		// (see p.619 Dunn & Parberry - but our normal is from two to one)
		// (See also Chris Hecker, eq. 8a and 8b)
		velocity += (impulse*inverseMass)*normal;
		angularVelocity += r1perp.Dot (normal) * (impulse * inverseMomentOfInertia);
	}
}
