using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GXPEngine.Core;

namespace GXPEngine
{
    class MimicBox : Sprite
    {
        //Movement variables           
        public Vec2 position
        {
            get
            {
                return _position;
            }
        }
        private Vec2 velocity;
        private Vec2 _position;
        private Vec2 acceleration;
        private float angularVelocity = 0;
        private float frictionCoefficient = 0.01f;
        private float _gravity = 0.1f;
        private float angularDampCoefficient = 0.9f;

        //Physics variables
        private float bounciness = 0f;
        private float inverseMass = 0.5f;
        private float inverseMomentOfInertia = 0.01f;
        private string gravityDirection; //Can be Up, Right, Left, Down

        //Other variables
        public Player player;
        private float teleportCD;
        private float teleportCDDuration = 3;
        private Vec2 normalSize;

        public MimicBox(float xPos, float yPos) : base("MimicBox.png")
        {
            SetOrigin(width / 2, height / 2);
            _position = new Vec2(xPos, yPos);
            acceleration = new Vec2(0, _gravity);
            gravityDirection = "Down";
            normalSize = new Vec2(scaleX, scaleY);
            ComputeMassInertia(3f);
        }

        private void ComputeMassInertia(float density)
        {
            float mass = width * height * density;
            float inertia = mass * (width * width + height * height) / 12;
            inverseMass = 1 / mass;
            inverseMomentOfInertia = 1 / inertia;
        }

        private void Update()
        {
            HandleTeleportCD();
            HandleGravityDirection();
            HandleMovement();
            HandleCollisions();
        }

        private void HandleTeleportCD()
        {
            if (scaleX < normalSize.x)
            {
                scaleX += 0.05f;
            }
            if (scaleY < normalSize.y)
            {
                scaleY += 0.05f;
            }
            if (teleportCD > 0)
            {
                teleportCD -= 0.0175f;
            }
        }

        private void HandleGravityDirection()
        {
            float diff = position.Distance(new Vec2(player.camera.x, player.camera.y));
            if (diff < 600)
            {
                acceleration = player.acceleration;
                gravityDirection = player.gravityDirection;
            }           
        }

        private void HandleMovement()
        {
            // Standard Euler integration (for position):
            velocity += acceleration;
            _position += velocity;

            angularVelocity *= angularDampCoefficient;
            rotation += Vec2.Rad2Deg(angularVelocity);

            UpdateScreenPosition();
        }

        private void UpdateScreenPosition()
        {
            x = position.x;
            y = position.y;
        }

        private void HandleCollisions()
        {
            //Continuous collision detection:
            // Interpolate position
            Vec2 intermediatePosition = position + velocity;

            // Check collisions at intermediate position
            GameObject detectedCollision = CheckCollisionsAt(intermediatePosition);

            if (detectedCollision != null)
            {
                ResolveCollision(detectedCollision);
            }

            //Discrete collision detection (Needed because the continuous one bugs sometimes):
            GameObject[] overlaps = GetCollisions();

            // Resolve collisions
            foreach (GameObject other in overlaps)
            {
                if (other != this)
                {
                    ResolveCollision(other);
                }
                if (other is Collectable)
                {
                    player.ConsumeBlob();                    
                    (other as Collectable).CollectBlob();
                }
                if (other is ButtonObject && !(other as ButtonObject).isPushing)
                {
                    (other as ButtonObject).isPushing = true;
                }
                if (other is TeleportingTile && teleportCD <= 0)
                {
                    _position = (other as TeleportingTile).pairTile.position;
                    teleportCD = teleportCDDuration;
                    SetScaleXY(0.1f, 0.1f);
                }
            }
            UpdateScreenPosition();
        }

        private GameObject CheckCollisionsAt(Vec2 collisionPos)
        {
            Vec2 originalPosition = position;
            _position = collisionPos;

            // Check collisions with other objects
            GameObject[] overlaps = GetCollisions();

            // Check if the mimic box is colliding with any other object
            if (overlaps.Length > 1 && Mathf.Abs(velocity.x) > 10 || Mathf.Abs(velocity.y) > 10)
            {
                angularVelocity *= 0.01f;
            }

            // Move object back to original position
            _position = originalPosition;


            // Check if any collisions were detected
            foreach (GameObject other in overlaps)
            {
                if (other != this)
                {
                    return other;
                }
            }
            return null;
        }

        private void ResolveCollision(GameObject other)
        {
            if ((other is Collectable) ||
                (other is ObjectDeathEffect) ||
                (other is FireEmitter) ||
                (other is TeleportingTile) ||
                (other is FireParticle))
            {
                return;
            }

            // A GXPEngine method for finding all kinds of useful info about collisions (=overlaps):
            Collision colInfo = collider.GetCollisionInfo(other.collider);

            // Translate from GXPEngine.Core.Vector2 to our own Vec2:
            // collision normal:
            Vec2 normal = new Vec2(colInfo.normal);
            // The exact collision point: 
            // (This might be a corner of this sprite, or a corner of the other sprite)
            Vec2 point = new Vec2(colInfo.point);

            // Resolve collision - the position reset part:
            // (Move until they are not overlapping anymore.)
            _position += normal * colInfo.penetrationDepth;

            // Compute some vectors related to the exact collision point:

            Vec2 r1 = point - position; // from center of this to collision point
            Vec2 r1perp = new Vec2(-r1.y, r1.x); // the normal vector of r1 (not unit!)

            Vec2 pointVelocity = velocity + r1perp * angularVelocity;

            float impulseMagnitude =
                -(1 + bounciness) * (pointVelocity.Dot(normal)) /
                (
                    normal.Dot(normal) * inverseMass +
                    r1perp.Dot(normal) * r1perp.Dot(normal) * inverseMomentOfInertia
                );

            if (impulseMagnitude < 0)
            {
                //Console.WriteLine("Impulse: {0}", impulse);
                return;
            }

            // Apply friction
            Vec2 relativeVelocity = velocity + r1perp * angularVelocity; // Relative velocity at collision point
            Vec2 friction = relativeVelocity * frictionCoefficient; // Friction force
            Vec2 impulse = impulseMagnitude * normal;

            float torque = r1perp.Dot(friction);
            angularVelocity -= torque * inverseMomentOfInertia;

            // Dampen linear and angular velocities upon collision
            float linearDamping = 0.5f;
            float angularDamping = 0.5f;
            velocity *= linearDamping;
            angularVelocity *= angularDamping;

            velocity += (impulse + friction) * inverseMass;
            angularVelocity += r1perp.Dot(normal) * impulseMagnitude * inverseMomentOfInertia;
        }
    }
}
