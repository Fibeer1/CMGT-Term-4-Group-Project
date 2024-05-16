using System;
using System.Collections.Generic;
using GXPEngine;
using Physics;
using GXPEngine.Core;

public class Player : AnimationSprite
{
    //General variables
    public int score;
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
    public Vec2 acceleration;
    private float angularVelocity = 0;
    private float frictionCoefficient = 0f;
    private float _gravity = 0.1f;
    private float angularDampCoefficient = 0.9f;

    //Physics variables
    float bounciness = 0f;
    float inverseMass = 0.5f;
    float inverseMomentOfInertia = 0.01f;

    //Mechanics variables
    private bool isCollidingWithBlock;
    public string gravityDirection; //Can be Up, Right, Left, Down
    private float fireCD;
    private float fireCDDuration = 1;
    private bool canChangeGravity;

    //Other variables
    public Camera camera;
    private Vec2 spawnPosition;
    public Vec2 normalSize;
    private Vec2 maximumSize = new Vec2(1, 1);
    private Vec2 minimumSize = new Vec2(0.7f, 0.7f);

    //Sound variables
    private SoundChannel audioSource;
    private SoundChannel wallAudioSource;
    private SoundChannel burnAudioSource;
    private SoundChannel levelFinishAudioSource;
    Sound impactSound = new Sound("SlimeImpact.wav");
    Sound movingWallSound = new Sound("MovingWall.wav");
    Sound burnSound = new Sound("SlimeBurning.wav");
    Sound collectableSound = new Sound("CollectBlob.wav");
    Sound levelFinishSound = new Sound("LevelFinish.wav");
    private bool shouldPlayImpactSound = true;


    bool standingStill => velocity.x <= 0.25f && velocity.y <= 0.25f && velocity.x >= -0.25f && velocity.y >= -0.25f;

    bool movingQuick => velocity.x > 5f || velocity.y > 5f || velocity.x < -5f || velocity.y < -5f;

    public Player() : base("Slime.png", 9, 4)
    {
        SetOrigin(width / 2, height / 2);
        SetScaleXY(minimumSize.x, minimumSize.y);
        acceleration = new Vec2(0, _gravity);
        gravityDirection = "Down";
        ComputeMassInertia(1);
        normalSize = new Vec2(scaleX, scaleY);
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
        if (!isDead)
        {
            if (Input.GetKeyDown(Key.SPACE))
            {
                HandleResizing();
            }
            HandleAnimations();
            HandleSounds();
            HandleFireCD();
            HandleGravityDirection();
            HandleMovement();
            HandleCameraMovement();
            HandleCollisions();
        }
        HandleDying();
    }

    private void HandleAnimations()
    {
        float animSpeed = 0.5f;
        if (gravityDirection == "Down")
        {
            if (velocity.y > 0.5f) //Down flying
            {
                animSpeed = 0.15f;
                if (currentFrame != 3)
                {
                    SetCycle(0, 4);
                }
                else
                {
                    SetCycle(3, 1);
                }
            }
            if (velocity.y <= 0.5f && canChangeGravity) //Down impact
            {
                animSpeed = 0.5f;
                if (currentFrame != 8)
                {
                    SetCycle(4, 5);
                }
                else
                {
                    SetCycle(8, 1);
                }
            }
        }
        if (gravityDirection == "Up")
        {
            if (velocity.y < -0.5f) //Up flying
            {
                animSpeed = 0.15f;
                if (currentFrame != 12)
                {
                    SetCycle(9, 4);
                }
                else
                {
                    SetCycle(12, 1);
                }
            }
            if (velocity.y >= -0.5f && velocity.y < 0 && canChangeGravity) //Up impact
            {
                animSpeed = 0.5f;
                if (currentFrame != 17)
                {
                    SetCycle(13, 5);
                }
                else
                {
                    SetCycle(17, 1);
                }
            }
        }
        if (gravityDirection == "Right")
        {
            if (velocity.x > 0.5f) //Right flying
            {
                animSpeed = 0.15f;
                if (currentFrame != 21)
                {
                    SetCycle(18, 4);
                }
                else
                {
                    SetCycle(21, 1);
                }
            }
            if (velocity.x <= 0.5f && canChangeGravity) //Right impact
            {
                animSpeed = 0.5f;
                if (currentFrame != 26)
                {
                    SetCycle(22, 5);
                }
                else
                {
                    SetCycle(26, 1);
                }
            }
        }
        if (gravityDirection == "Left")
        {
            if (velocity.x < -0.5f) //Left flying
            {
                animSpeed = 0.15f;
                if (currentFrame != 30)
                {
                    SetCycle(27, 4);
                }
                else
                {
                    SetCycle(30, 1);
                }
            }
            if (velocity.x >= -0.5f && velocity.x < 0 && canChangeGravity) //Left impact
            {
                animSpeed = 0.5f;
                if (currentFrame != 35)
                {
                    SetCycle(31, 5);
                }
                else
                {
                    SetCycle(35, 1);
                }
            }
        }

        Animate(animSpeed);
    }

    private void HandleSounds()
    {
        if (movingQuick)
        {
            shouldPlayImpactSound = true;
        }
    }

    private void HandleResizing()
    {
        if (normalSize.x > minimumSize.x + 0.1f)
        {
            ObjectDeathEffect slimeEffect = new ObjectDeathEffect(position, velocity);
            game.LateAddChild(slimeEffect);
            SetScaleXY(scaleX - 0.1f, scaleY - 0.1f);
            normalSize = new Vec2(scaleX, scaleY);
        }
    }

    private void HandleFireCD()
    {
        if (fireCD > 0)
        {
            fireCD -= 0.0175f;
        }
    }

    public void ConsumeBlob()
    {
        if (scaleX < maximumSize.x)
        {
            SetScaleXY(scaleX + 0.1f, scaleY + 0.1f);
        }
        audioSource = collectableSound.Play();
        normalSize = new Vec2(scaleX, scaleY);
        score++;
        ((MyGame)game).playerScore = score;
    }

    private void HandleGravityDirection()
    {
        if (isCollidingWithBlock && canChangeGravity)
        {           
            if (Input.GetKeyDown(Key.LEFT))
            {
                canChangeGravity = false;
                acceleration = new Vec2(-_gravity, 0);
                gravityDirection = "Left";
            }
            else if (Input.GetKeyDown(Key.RIGHT))
            {
                isCollidingWithBlock = false;
                canChangeGravity = false;
                acceleration = new Vec2(_gravity, 0);
                gravityDirection = "Right";
            }
            else if (Input.GetKeyDown(Key.UP))
            {
                isCollidingWithBlock = false;
                canChangeGravity = false;
                acceleration = new Vec2(0, -_gravity);
                gravityDirection = "Up";
            }
            else if (Input.GetKeyDown(Key.DOWN))
            {
                isCollidingWithBlock = false;
                canChangeGravity = false;
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
            if (other is Collectable)
            {
                ConsumeBlob();
                (other as Collectable).CollectBlob();
            }
            if (other is ButtonObject && !(other as ButtonObject).isPushing)
            {
                ButtonObject button = other as ButtonObject;
                button.isPushing = true;
                if (button.wallPair != null)
                {
                    wallAudioSource = movingWallSound.Play();
                }

                if (button.platformPair != null)
                {
                    button.platformPair.shouldMove = true;
                }               
            }
            if (other is Finish)
            {
                MyGame mainGame = (MyGame)game;
                mainGame.currentLevelIndex++;
                if (mainGame.currentLevelIndex >= 3)
                {
                    mainGame.StartMenu("Win Screen");
                }
                else
                {
                    levelFinishAudioSource = levelFinishSound.Play();
                    mainGame.StartLevel(mainGame.currentLevelIndex);
                }
            }
            if (other is FireParticle && fireCD <= 0)
            {
                Vec2 reducedSize = new Vec2(scaleX - 0.2f, scaleY - 0.2f);
                normalSize = reducedSize;
                SetScaleXY(reducedSize.x, reducedSize.y);
                ObjectDeathEffect slimeEffect = new ObjectDeathEffect(position, velocity);
                game.LateAddChild(slimeEffect);
                burnAudioSource = burnSound.Play();
                if (normalSize.x <= minimumSize.x)
                {
                    HandleDying();
                }
                
                fireCD = fireCDDuration;
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
        if ((other is Collectable) ||
                (other is ObjectDeathEffect) ||
                (other is FireEmitter) ||
                (other is TeleportingTile) ||
                (other is FireParticle) || other is null)
        {
            return;
        }

        if (audioSource == null || !audioSource.IsPlaying)
        {
            if (shouldPlayImpactSound)
            {
                shouldPlayImpactSound = false;
                audioSource = impactSound.Play();
            }
        }

        // A GXPEngine method for finding all kinds of useful info about collisions (=overlaps):
        Collision colInfo = collider.GetCollisionInfo(other.collider);
        if (colInfo == null)
        {
            return;
        }

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
            -(1 + bounciness) * pointVelocity.Dot(normal) /
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

        

        velocity += (impulse + friction) * inverseMass;
        angularVelocity += r1perp.Dot(normal) * impulseMagnitude * inverseMomentOfInertia;

        // Dampen linear and angular velocities upon collision
        float linearDamping = 0.95f;
        velocity *= linearDamping;
        float angularDamping = 0.25f;
        angularVelocity *= angularDamping;

        if (standingStill)
        {
            canChangeGravity = true;
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
        if (normalSize.x < minimumSize.x && !isDead)
        {
            isDead = true;
            if (!spawnedDeathParticle)
            {
                spawnedDeathParticle = true;
                ObjectDeathEffect deathEffect = new ObjectDeathEffect(position, velocity);
                game.LateAddChild(deathEffect);
            }         
            alpha = 0;
        }
        if (isDead)
        {
            deadTimer -= 0.0175f;
            if (deadTimer <= 0)
            {
                ((MyGame)game).playerScore -= score;
                game.FindObjectOfType<MyGame>().EndGame();
            }
        }
    }
}