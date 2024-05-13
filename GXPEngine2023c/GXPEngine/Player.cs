using System;
using System.Collections.Generic;
using GXPEngine;
using Physics;
using GXPEngine.Core;

public class Player : Sprite
{
    //General variables
    public int score;
    public int healthPoints = 5;
    private int maxHealth;
    private int healthTaken = 2;
    private bool isDead = false;
    private bool spawnedDeathParticle = false;
    private float deadTimer = 1;

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
    float bounciness = 0f;
    float inverseMass = 0.5f;
    float inverseMomentOfInertia = 0.01f;

    //Color Indicator variables
    private float[] colorIndicationRGB = new float[3];
    private float colorIndicatorTimer = 0.1f;       
    private bool showColorIndicator;

    //Mechanics variables
    private bool isCollidingWithBlock;
    private string gravityDirection; //Can be Up, Right, Left, Down

    //Other variables
    public Camera camera;
    private Vec2 spawnPosition;
    bool standingStill => velocity.x <= 1 && velocity.y <= 1 && velocity.x >= -1 && velocity.y >= -1;

    public Player() : base("PowerupBox.png")
    {
        SetOrigin(width / 2, height / 2);
        maxHealth = healthPoints;
        SetColor(0.75f, 0, 0);
        acceleration = new Vec2(0, _gravity);
        gravityDirection = "Down";
        ComputeMassInertia(1f);
    }

    void ComputeMassInertia(float density)
    {
        float mass = width * height * density;
        float inertia = mass * (width * width + height * height) / 12;
        inverseMass = 1 / mass;
        inverseMomentOfInertia = 1 / inertia;
    }

    private void Update()
    {
        if (!isDead)
        {
            HandleGravityDirection();
            HandleMovement();
            HandleCameraMovement();
            HandleCollisions();
            HandleColorIndication();
        }
        else
        {
            HandleDying();
        }
    }

    private void HandleGravityDirection()
    {
        if (isCollidingWithBlock)
        {            
            if (Input.GetKeyDown(Key.LEFT) && standingStill)
            {
                isCollidingWithBlock = false;
                acceleration = new Vec2(-_gravity, 0);
                gravityDirection = "Left";
            }
            else if (Input.GetKeyDown(Key.RIGHT) && standingStill)
            {
                isCollidingWithBlock = false;
                acceleration = new Vec2(_gravity, 0);
                gravityDirection = "Right";
            }
            else if (Input.GetKeyDown(Key.UP) && standingStill)
            {
                isCollidingWithBlock = false;
                acceleration = new Vec2(0, -_gravity);
                gravityDirection = "Up";
            }
            else if (Input.GetKeyDown(Key.DOWN) && standingStill)
            {
                isCollidingWithBlock = false;
                acceleration = new Vec2(0, _gravity);
                gravityDirection = "Down";
            }
        }
    }

    private void HandleCameraMovement()
    {
        Vec2 camPos = new Vec2(camera.x, camera.y);
        Vec2 desiredCamPos = Vec2.Lerp(camPos, position, 0.125f);
        camera.SetXY(desiredCamPos.x, desiredCamPos.y);
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
            isCollidingWithBlock = true;
            ResolveCollision(detectedCollision);
        }

        //Discrete collision detection (Needed because the continuous one bugs sometimes):
        GameObject[] overlaps = GetCollisions();

        // Resolve collisions
        foreach (GameObject other in overlaps)
        {
            if (other != this)
            {
                isCollidingWithBlock = true;
                ResolveCollision(other);
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

        // Check if the player is colliding with any other object
        if (overlaps.Length > 0)
        {
            isCollidingWithBlock = true;
        }
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

    private void PickupHealth(GameObject healthBox)
    {
        healthPoints += healthTaken;
        showColorIndicator = true;
        colorIndicationRGB[0] = 0;
        colorIndicationRGB[1] = 0.75f;
        colorIndicationRGB[2] = 0;
        if (healthPoints > maxHealth)
        {
            healthPoints = maxHealth;
        }
        healthBox.LateDestroy();
    }

    private void POIResolveCollision()
    {
        float oldDistance = (position - (position - velocity)).Length();
        float newDistance = (position - (position + velocity)).Length();
        float timeOfImpact = oldDistance / newDistance;

        // Calculate point of impact
        Vec2 pointOfImpact = position - timeOfImpact * velocity;
        _position = pointOfImpact;
    }

    private void HandleColorIndication()
    {
        //The player changes color for a part of a second when taking damage or being healed
        if (showColorIndicator)
        {
            SetColor(colorIndicationRGB[0], colorIndicationRGB[1], colorIndicationRGB[2]);
            if (healthPoints <= 0)
            {
                isDead = true;
            }
            colorIndicatorTimer -= 0.01f;
            if (colorIndicatorTimer <= 0)
            {
                SetColor(0.75f, 0, 0);
                colorIndicatorTimer = 0.1f;
                showColorIndicator = false;
            }
        }
    }
    public void SetSpawnPoint()
    {
        spawnPosition = position;
    }
    public void SetPosition(Vec2 pos)
    {
        _position = pos;
    }
    void SetSpawnPosition()
    {
        _position = spawnPosition;
    }

    private void HandleDying()
    {
        if (isDead)
        {
            if (!spawnedDeathParticle)
            {
                spawnedDeathParticle = true;
                ObjectDeathEffect deathEffect = new ObjectDeathEffect(position, velocity);
                game.LateAddChild(deathEffect);
            }         
            alpha = 0;
            deadTimer -= 0.0175f;
            if (deadTimer <= 0)
            {
                game.FindObjectOfType<MyGame>().EndGame();
            }
        }
    }
}
