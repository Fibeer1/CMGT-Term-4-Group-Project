using System;
using System.Collections.Generic;
using GXPEngine;
using Physics;

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

    //Sound variables
    private SoundChannel audioSource;

    //private Sound healthPickupSound = new Sound("HealthPickup.wav", false, false);
    //private Sound damageTakenSound = new Sound("DamageTaken.wav", false, false);
    //private Sound powerupPickupSound = new Sound("PowerupPickup.wav", false, false);
    //private Sound deathSound = new Sound("DeathEffect.wav", false, false);
    //private Sound bulletShootSound = new Sound("BulletShoot.wav", false, false);
    //private Sound boostSound = new Sound("BoostSound.wav", false, false);
    //private Sound superSpeedImpact = new Sound("SuperspeedImpact.wav", false, false);

    //Movement variables           
    public Vec2 position
    {
        get
        {
            return _position;
        }
    }
    private Vec2 velocity;
    private Vec2 gravity = new Vec2(0, -2);
    private Vec2 _position;
    private int _radius;    
    private float _speed;
    private float _acceleration = 0.25f;
    private float turnSpeed = 5;
    private float heldSpeed = 0;
    private float maxSpeedNormal = 5;
    private float maxSpeedHeld = 30;

    //Collision variables
    private Collider coll;
    private ColliderManager engine;

    //Color Indicator variables
    private float[] colorIndicationRGB = new float[3];
    private float colorIndicatorTimer = 0.1f;       
    private bool showColorIndicator;

    public Player() : base("Ship.png")
    {
        SetOrigin(width / 2, height / 2);
        ResetPosition();
        maxHealth = healthPoints;
        SetColor(0.75f, 0, 0);
        _radius = width;

        coll = new AABB(this, position, _radius * 0.7f, _radius * 0.7f);
        engine = ColliderManager.main;
        engine.AddTriggerCollider(coll);

        //gun = new Gun();
        //AddChild(gun);
    }

    protected override void OnDestroy()
    {
        // Remove the collider when the sprite is destroyed:
        engine.RemoveTriggerCollider(coll);
    }

    private void Update()
    {
        if (!isDead)
        {
            HandleMovement();
            HandleCollisions();
            HandleColorIndication();
        }
        else
        {
            HandleDying();
        }
    }

    private void HandleMovement()
    {
        if (Input.GetKey(Key.A))
        {
            Turn(-turnSpeed);
        }
        else if (Input.GetKey(Key.D))
        {
            Turn(turnSpeed);
        }
        //gradually increasing/decreasing movement speed
        if (Input.GetKey(Key.W))
        {
            if (_speed < maxSpeedNormal)
            {
                _speed += _acceleration;
            }
        }
        if (Input.GetKey(Key.S))
        {
            if (_speed > -maxSpeedNormal)
            {
                _speed -= _acceleration;
            }
        }
        velocity = Vec2.GetUnitVectorDeg(rotation) * _speed;
        _position += velocity;
        UpdateScreenPosition();
        _speed *= 0.95f;
    }

    private void UpdateScreenPosition()
    {
        x = _position.x;
        y = _position.y;
        coll.position = _position;
    }

    private void HandleCollisions()
    {
        FindEarliestCollisionWithRectangles();
        FindEarliestCollisionWithLineSegments();
    }

    private void FindEarliestCollisionWithRectangles()
    {
        // Check overlapping trigger colliders:
        List<Collider> overlaps = engine.GetOverlaps(coll);

        // Deal with overlaps
        foreach (Collider col in overlaps)
        {
            //if (col.owner is Powerup)
            //{
            //    PickupPowerup(col.owner);
            //}
            //else if (col.owner is HealthPickup)
            //{
            //    PickupHealth(col.owner);
            //}
            //else if (col.owner is Enemy)
            //{
            //    InteractWithEnemy(col.owner);
            //}
        }
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

    private void FindEarliestCollisionWithLineSegments()
    {
        //calculate correct distance from the player's center to the line
        NLineSegment[] lineSegments = game.FindObjectsOfType<NLineSegment>();
        for (int i = 0; i < lineSegments.Length; i++)
        {
            Vec2 diff = lineSegments[i].end - position;
            //calculate the normal of the line
            Vec2 lineNormal = (lineSegments[i].start - lineSegments[i].end).Normal();
            //project the distance onto the normal so that it is exactly between the point of collision and the player's center
            float distance = diff.ScalarProjection(lineNormal);
            //compare distance with player radius
            if (distance < _radius)
            {
                DetectAndResolveLineCollisions();
            }
        }
    }

    private void DetectAndResolveLineCollisions()
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

    public void ResetPosition()
    {
        _position.x = game.width / 2;
        _position.y = game.height / 2;
        _speed = 0;
    }

    private void HandleDying()
    {
        if (isDead)
        {
            engine.RemoveTriggerCollider(coll);
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
