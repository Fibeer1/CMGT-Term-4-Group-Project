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

    private Sound healthPickupSound = new Sound("HealthPickup.wav", false, false);
    private Sound damageTakenSound = new Sound("DamageTaken.wav", false, false);
    private Sound powerupPickupSound = new Sound("PowerupPickup.wav", false, false);
    private Sound deathSound = new Sound("DeathEffect.wav", false, false);
    private Sound bulletShootSound = new Sound("BulletShoot.wav", false, false);
    private Sound boostSound = new Sound("BoostSound.wav", false, false);
    private Sound superSpeedImpact = new Sound("SuperspeedImpact.wav", false, false);

    //Movement variables           
    public Vec2 position
    {
        get
        {
            return _position;
        }
    }
    private Vec2 velocity;
    private int _radius;
    private Vec2 _position;
    private float _speed;
    private float _acceleration = 0.25f;
    private float turnSpeed = 5;
    private float heldSpeed = 0;
    private float maxSpeedNormal = 5;
    private float maxSpeedHeld = 30;

    //Collision variables
    private Collider coll;
    private ColliderManager engine;

    //Ram variables   
    private float maxRamSpeed = 10;
    public float ramCooldown = 0; //default value is 0.25
    private float ramCooldownBuff = 0;
    private bool isHoldingMomentum = false;

    //Explosion variables
    public float explosionCooldown = 0; //default value is 0.5
    private float explosionCooldownBuff = 0;
    public bool shouldExplode = false;
    private bool ramExplosion = false; //Becomes true when the player has the Claymores powerup

    //Gun variables
    private Gun gun;
    private float gunCooldown;

    //Powerups
    private string explosionPowerup;
    public int explosionPowerupIndex; //1 is Large, 2 is Burst, 3 is Shrapnel Release
    private string ramPowerup;
    public int ramPowerupIndex; //1 is Boost, 2 is Claymores, 3 is Rifles
    private string bulletPowerup;

    //Large variable
    private int largeExplosionSize = 2;

    //Burst variables
    private int explosionCount = 3;
    private float burstExplosionTimer = 0.1f;
    private bool duringBurstAnimation = false;

    //Bullet variables
    public float bulletLifetimeBuff = 0;
    public int bulletBouncinessBuff = 0;
    public int bulletPiercingBuff = 0;

    //Color Indicator variables
    private float[] colorIndicationRGB = new float[3];
    private float colorIndicatorTimer = 0.1f;       
    private bool showColorIndicator;

    //The player receives points from killing enemies
    //After each second wave after the first the player can get a powerup. Possible powerups are:
    //stat boost (mainly cooldown), explosion, ram or bullets if the conditions are met
    //At 30 seconds the wave ends, the powerups appear on the map, enemies stop spawning and the player gets
    //teleported to the spawn position for 15 seconds
    //If the player has endgame powerups the only boxes that can spawn are stat boxes

    //Types of powerups:
    //None
    //Ram:
        //Boost, Ram charges faster (wave 1)
        //Claymores, Explosion when ramming (wave 3)
        //Rifles, 3 bullets fire in a cone in front of the player after it releases built up speed (wave 5)
    //Explosion:
        //Large, Larger explosion (wave 1)
        //Burst, Burst of explosions (short time interval between each one to cover more space,
            //these explosions have shorter lifetime and are smaller) (wave 3)
        //Shrapnel release, 8 bullets fire in the 8 cardinal direction (wave 5)
    //Gun:
        //Burst, fires 3 bullets in quick succession instead of one, no cooldown (wave 1)
        //Quintuple-shot, fires 5 bullets at the same time in a small cone (wave 3)
        //Exploding bullets, upon collision with an enemy, the bullet emits an explosion with the same powerups as the player (wave 5)
    //Bullets (fired from more powerful variations of the explosion and the ram)
        //Bullet powerups become available after wave 7
        //Bouncy
        //Shotgun, bullets are twice as much but their range is 30% shorter
        //Piercing, bullets pass through an enemy upon collision and kill it, one bullet can kill up to 3 enemies

    private bool superSpeed => _speed >= maxRamSpeed;

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

        gun = new Gun();
        AddChild(gun);
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
            HandleRamMechanics();
            HandleExploding();
            HandleExplosionBurst();
            HandleGunMechanics();
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
            if (col.owner is Powerup)
            {
                PickupPowerup(col.owner);
            }
            else if (col.owner is HealthPickup)
            {
                PickupHealth(col.owner);
            }
            else if (col.owner is Enemy)
            {
                InteractWithEnemy(col.owner);
            }
        }
    }

    private void PickupPowerup(GameObject powerup)
    {
        audioSource = powerupPickupSound.Play();
        Powerup powerUp = powerup as Powerup;
        if (powerUp.type == "Explosion")
        {
            explosionPowerup = powerUp.powerupName;
            explosionPowerupIndex++;
        }
        else if (powerUp.type == "Ram")
        {
            ramPowerup = powerUp.powerupName;
            ramPowerupIndex++;
        }
        else if (powerUp.type == "Bullet")
        {
            bulletPowerup = powerUp.powerupName;
        }
        else if (powerUp.type == "Stat")
        {
            if (powerUp.powerupName == "Explosion CD" && explosionCooldownBuff < 0.5f)
            {
                explosionCooldownBuff += 0.1f;
            }
            if (powerUp.powerupName == "Ram CD" && ramCooldown < 0.1f)
            {
                ramCooldownBuff += 0.05f;
            }
            else if (powerUp.powerupName == "Speed")
            {
                _acceleration += 0.05f;
            }
            else if (powerUp.powerupName == "Max Speed")
            {
                maxSpeedNormal += 1;
            }
            else if (powerUp.powerupName == "Ram Speed")
            {
                maxSpeedHeld += 2;
            }
            else if (powerUp.powerupName == "Max HP")
            {
                maxHealth += 1;
                if (healthPoints < maxHealth)
                {
                    healthPoints += 1;
                }
            }
            else if (powerUp.powerupName == "Bullet Range")
            {
                bulletLifetimeBuff += 0.25f;
            }
            else if (powerUp.powerupName == "Bullet Bounciness")
            {
                bulletBouncinessBuff++;
            }
            else if (powerUp.powerupName == "Bullet Sharpness")
            {
                bulletPiercingBuff++;
            }
            else if (powerUp.powerupName == "Health Taken")
            {
                healthTaken++;
            }
            else if (powerUp.powerupName == "Health Taken")
            {
                healthTaken++;
            }
        }
        WaveManager waveManager = game.FindObjectOfType<WaveManager>();
        waveManager.StartWave();
        powerUp.LateDestroy();
    }

    private void PickupHealth(GameObject healthBox)
    {
        audioSource = healthPickupSound.Play();
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

    private void InteractWithEnemy(GameObject other)
    {
        Enemy enemy = other as Enemy;
        if (superSpeed && enemy.modifier != "Elite")
        {
            score += 1;
            if (ramPowerup == "Claymores")
            {
                shouldExplode = true;
                ramExplosion = true;
            }
            audioSource = superSpeedImpact.Play();
        }
        else
        {
            audioSource = damageTakenSound.Play();
            healthPoints--;
            showColorIndicator = true;
            colorIndicationRGB[0] = 0.25f;
            colorIndicationRGB[1] = 0;
            colorIndicationRGB[2] = 0;
        }
        enemy.Die();
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

    private void HandleRamMechanics()
    {
        //The player stops, stores its current speed, increases it and then releases it, destroying any enemies in its way
        if (_speed >= maxRamSpeed)
        {
            RamParticle particle = new RamParticle();
            game.AddChild(particle);
        }       
        if (Input.GetKey(Key.LEFT_SHIFT) && ramCooldown <= 0)
        {
            isHoldingMomentum = true;
            if (_speed > 0.25f)
            {
                if (ramPowerup == "Boost")
                {
                    heldSpeed = _speed * 10;
                }
                else
                {
                    heldSpeed = _speed * 5;
                }
            }
            if (heldSpeed > maxSpeedHeld)
            {
                heldSpeed = maxSpeedHeld;
            }
            _speed = 0;
            velocity = new Vec2(0,0);
        }
        if (Input.GetKeyUp(Key.LEFT_SHIFT) && heldSpeed > 0)
        {
            audioSource = boostSound.Play();
            if (ramPowerup == "Rifles")
            {
                audioSource = bulletShootSound.Play();
                Bullet bullet1 = new Bullet(position, rotation - 30, bulletPowerup);
                Bullet bullet2 = new Bullet(position, rotation, bulletPowerup);
                Bullet bullet3 = new Bullet(position, rotation + 30, bulletPowerup);
                game.AddChild(bullet1);
                game.AddChild(bullet2);
                game.AddChild(bullet3);
                ramCooldown = 0.5f - ramCooldownBuff;
            }
            else
            {
                ramCooldown = 0.25f - ramCooldownBuff;
            }
            isHoldingMomentum = false;
            _speed = heldSpeed;
            velocity += Vec2.GetUnitVectorDeg(rotation) * _speed;
            heldSpeed = 0;
        }
        if (ramCooldown > 0)
        {
            ramCooldown -= 0.0175f;
        }
    }
    
    private void HandleExploding()
    {
        //The player explodes, killing any nearby enemies and pushing back others
        if (Input.GetKeyDown(Key.SPACE) && explosionCooldown <= 0 && !isHoldingMomentum)
        {
            shouldExplode = true;
        }
        if (shouldExplode)
        {
            shouldExplode = false;
            Explosion explosion = new Explosion();
            explosionCooldown = 0.5f - explosionCooldownBuff;
            if (explosionPowerup == "Large")
            {
                explosion.SetScaleXY(largeExplosionSize, largeExplosionSize);
                explosion.shouldExplode = true;
            }
            else
            {
                explosion.SetScaleXY(1.5f, 1.5f);
                explosion.shouldExplode = true;
            }
            if (explosionPowerup == "Burst")
            {
                explosion.LateDestroy();
                duringBurstAnimation = true;
                explosionCooldown = 1f - explosionCooldownBuff;
            }
            if (explosionPowerup == "Shrapnel Release")
            {
                explosion.shouldExplode = true;
                audioSource = bulletShootSound.Play();
                for (int i = 0; i < 8; i++)
                {
                    Bullet bullet = new Bullet(explosion.position, rotation + 45 * i, bulletPowerup);
                    game.AddChild(bullet);
                }
                explosionCooldown = 0.75f - explosionCooldownBuff;
            }
            if (explosion != null)
            {
                game.AddChild(explosion);                
            }
            if (ramExplosion)
            {
                explosionCooldown = 0;
                ramExplosion = false;
            }            
        }
        if (explosionCooldown > 0)
        {
            explosionCooldown -= 0.0175f;
        }
    }

    private void HandleExplosionBurst()
    {
        if (duringBurstAnimation)
        {
            burstExplosionTimer -= 0.01f;
            if (burstExplosionTimer <= 0 && explosionCount > 0)
            {
                Explosion explosion = new Explosion();
                explosion.SetScaleXY(1.5f, 1.5f);
                explosion.shouldExplode = true;
                game.AddChild(explosion);
                explosionCount--;
                burstExplosionTimer = 0.05f;
                if (explosionCount <= 0)
                {
                    duringBurstAnimation = false;
                    explosionCount = 3;
                }
            }
        }
    }

    private void HandleGunMechanics()
    {
        if (Input.GetMouseButtonDown(0) && gunCooldown <= 0)
        {
            //Shoot the player's gun
            audioSource = bulletShootSound.Play();
            //get the spawn position (the * 25 is meant to make the bullet spawn at the tip of the barrel)
            Vec2 localForwardVec = Vec2.GetUnitVectorDeg(gun.rotation) * 25;
            float playerRotationRadians = Vec2.Deg2Rad(rotation);
            localForwardVec.SetAngleRadians(playerRotationRadians);
            Vec2 spawnPosition = position + localForwardVec;
            float bulletRotation = gun.rotation + rotation;
            game.AddChild(new Bullet(spawnPosition, bulletRotation, bulletPowerup));
            gunCooldown = 0.2f;
        }
        if (gunCooldown > 0)
        {
            gunCooldown -= 0.0175f;
        }
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
                audioSource = deathSound.Play();
                spawnedDeathParticle = true;
                ObjectDeathEffect deathEffect = new ObjectDeathEffect(position, velocity);
                game.LateAddChild(deathEffect);
            }         
            alpha = 0;
            gun.alpha = 0;
            deadTimer -= 0.0175f;
            if (deadTimer <= 0)
            {
                game.FindObjectOfType<MyGame>().EndGame();
            }
        }
    }
}
