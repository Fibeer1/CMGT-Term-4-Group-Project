using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Physics;

namespace GXPEngine
{
    class Bullet : Sprite
    {
        //General variables
        private int _radius;
        private float lifeTime = 1.5f;
        private Player player;

        //Movement variables
        public Vec2 position
        {
            get
            {
                return _position;
            }
        }
        private Vec2 _position;
        private Vec2 velocity;
        private Vec2 previousVelocity;
        private float _speed = 10;

        //Modifier variables
        private string modifier = "";
        private float homingDetectionRadius = 150f;
        private Enemy homingTarget;
        private int bounces = 3;
        private int pierces = 1;

        //Collision variables
        private Collider coll;
        private ColliderManager engine;

        //Sound variables
        private SoundChannel audioSource;
        private Sound impactSound = new Sound("BulletImpact.wav", false, false);

        public Bullet(Vec2 pPosition, float pRotation, string pModifier) : base("Bullet.png")
        {
            SetOrigin(width / 2, height / 2);
            player = game.FindObjectOfType<Player>();
            lifeTime = 1.5f + player.bulletLifetimeBuff;  
            
            _position = pPosition;
            rotation = pRotation;
            velocity = Vec2.GetUnitVectorDeg(rotation) * _speed;
            modifier = pModifier;
            _radius = width;

            coll = new AABB(this, position, _radius * 0.7f, _radius * 0.7f);
            engine = ColliderManager.main;
            engine.AddTriggerCollider(coll);

            UpdateScreenPosition();

            SetModifier();            
        }

        private void SetModifier()
        {
            if (modifier == "Bouncy")
            {
                SetColor(1, 1, 0);
                bounces = 3 + player.bulletBouncinessBuff;
                lifeTime = 5 + player.bulletLifetimeBuff;
            }
            if (modifier == "Homing")
            {
                SetColor(0, 1, 1);
            }
            if (modifier == "Piercing")
            {
                SetColor(1, 0, 0);
                pierces = 1 + player.bulletPiercingBuff;
            }
            if (modifier == "Shotgun")
            {
                _speed = 12.5f;
                lifeTime = 0.35f + player.bulletLifetimeBuff;
                for (int i = 0; i < 10; i++)
                {
                    Bullet bullet = new Bullet(position, rotation + Utils.Random(-20f, 20f), "Normal");
                    game.AddChild(bullet);
                    bullet._speed = 12.5f;
                    bullet.lifeTime = 0.35f + player.bulletLifetimeBuff;
                }
            }
        }

        protected override void OnDestroy()
        {
            // Remove the collider when the sprite is destroyed:
            engine.RemoveTriggerCollider(coll);
        }

        private void Update()
        {
            HandleLifeCycle();
            HandleCollisions();
            HandleHoming();            
            HandleMoving();
            UpdateScreenPosition();
        }

        private void HandleLifeCycle()
        {
            lifeTime -= 0.025f;
            if (lifeTime <= 0)
            {
                LateRemove();
                LateDestroy();
            }
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
                if (col.owner is Enemy)
                {
                    HandleEnemyInteraction(col.owner);
                }
            }
        }

        private void HandleEnemyInteraction(GameObject other)
        {
            bool shouldDestroy = false;
            Enemy enemy = other as Enemy;
            audioSource = impactSound.Play();
            player.score += 1;
            enemy.Die();
            if (modifier == "Piercing" && pierces <= 0)
            {
                shouldDestroy = true;
            }
            else if (modifier != "Piercing")
            {
                shouldDestroy = true;
            }
            pierces--;
            if (shouldDestroy)
            {
                engine.RemoveTriggerCollider(coll);
                LateRemove();
                LateDestroy();
            }
        }

        private void FindEarliestCollisionWithLineSegments()
        {           
            NLineSegment[] lineSegments = game.FindObjectsOfType<NLineSegment>();
            for (int i = 0; i < lineSegments.Length; i++)
            {
                //calculate correct distance from the bullet's center to the line
                Vec2 diff = lineSegments[i].end - position;
                //calculate the normal of the line
                Vec2 lineNormal = (lineSegments[i].start - lineSegments[i].end).Normal();
                //project the distance onto the normal so that it is exactly between the point of collision and the bullet's center
                float bulletDistance = diff.ScalarProjection(lineNormal);
                //compare distance with bullet radius
                if (bulletDistance < _radius)
                {
                    DetectAndResolveLineCollisions(lineNormal);
                }
            }
        }

        private void DetectAndResolveLineCollisions(Vec2 lineNormal)
        {
            audioSource = impactSound.Play();
            //use the Reflect method if the bullet has the Bouncy modifier
            if (modifier == "Bouncy")
            {                
                velocity.Reflect(lineNormal);
                if (bounces <= 0)
                {
                    LateRemove();
                    LateDestroy();
                }
                bounces--;
            }
            else
            {
                LateRemove();
                LateDestroy();
            }
        }       

        private void HandleHoming()
        {
            if (modifier == "Homing")
            {
                if (homingTarget == null)
                {
                    foreach (GameObject gameChild in game.FindObjectOfType<EntityManager>().GetChildren())
                    {                
                        
                        if (gameChild is Enemy)
                        {
                            //get the first enemy that the bullet gets close to
                            Enemy enemy = gameChild as Enemy;
                            Vec2 delta = enemy.position - position;
                            //get and compare the distance and the detection radius
                            if (delta.Length() < homingDetectionRadius)
                            {
                                homingTarget = enemy;
                                break;
                            }
                        }                                               
                    }
                }
                else if (!homingTarget.isDead)
                {
                    //if a homing bullet still has a living target, make it move towards that target smoothly
                    Vec2 delta = homingTarget.position - position;
                    Vec2 deltaNormalized = delta.Normalized();
                    deltaNormalized *= _speed;
                    previousVelocity = velocity;
                    velocity = deltaNormalized;
                    //use the Inertia method for smooth movement
                    velocity = Vec2.Inertia(previousVelocity, velocity, 0.75f, 0.25f);
                }
            }           
        }

        private void HandleMoving()
        {
            _position += velocity;
        }

        private void UpdateScreenPosition()
        {            
            x = position.x;
            y = position.y;
            coll.position = position;
        }
    }
}
