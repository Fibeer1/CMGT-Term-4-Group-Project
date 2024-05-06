using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Physics;

namespace GXPEngine
{
    public class Enemy : Sprite
    {
        //General variables
        private Player player;
        public bool isDead = false;

        //Modifier variables
        public string modifier;
        private float erraticTimer = 0.5f;
        private bool isModified = false;

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
        private int _radius;
        private float _speed = 5;     

        //Collision variables
        private Collider coll;
        private ColliderManager engine;

        private bool outsideScreen => position.x < -100 || 
            position.x > game.width + 100 ||
            position.y < -100 || 
            position.y > game.height + 100;

        public Enemy() : base("Ship.png")
        {
            player = game.FindObjectOfType<Player>();
            _radius = width;

            coll = new AABB(this, position, _radius * 0.7f, _radius * 0.7f);
            engine = ColliderManager.main;
            engine.AddTriggerCollider(coll);

            HandleModification();

            HandlePositionGeneration();
        }

        protected override void OnDestroy()
        {
            // Remove the collider when the sprite is destroyed:
            engine.RemoveTriggerCollider(coll);
        }

        private void HandleModification()
        {
            //Modifier chance:
            //Wave 1-4 - 30%
            //Wave 5-9 - 50%
            //Wave 10 - 100%
            //An enemy has a specific chance to be modified. Possible modifiers:
            //Fast (speed multiplied by 2, size divided by 1.5f)
            //Erratic (random changes in direction every half-second)
            //Spiral (rotates around the player)
            //Homing (rotates towards the player)
            //Elite (rotates towards the player and is immune to ramming damage)
            WaveManager waveManager = game.FindObjectOfType<WaveManager>();
            int modifierChanceMin = 0;
            int modifierChanceMax = 3;
            if (waveManager.currentWave >= 9 && waveManager.currentWave <= 9)
            {
                modifierChanceMax = 2;
            }
            else if (waveManager.currentWave >= 10)
            {
                modifierChanceMax = 1;
            }
            int modifierChance = Utils.Random(modifierChanceMin, modifierChanceMax); //Default values are 0 and 3
            if (modifierChance == 0)
            {
                isModified = true;
                int modifierRNG = Utils.Random(0, 6); //Default values are 0 and 6
                if (modifierRNG == 0)
                {
                    modifier = "Fast";
                    _speed *= 2;
                    scaleX /= 1.5f;
                    scaleY /= 1.5f;
                    SetColor(1, 1, 0); //Yellow
                }
                else if (modifierRNG == 1)
                {
                    modifier = "Erratic";
                    SetColor(0.75f, 0, 1); //Purple
                }
                else if (modifierRNG == 2)
                {
                    modifier = "Inertia";
                    _speed *= 1.5f;
                    SetColor(0, 1, 0); //Green
                }
                else if (modifierRNG == 3)
                {
                    modifier = "Homing";
                    SetColor(0, 1, 1); //Turquoise
                }
                else if (modifierRNG == 4)
                {
                    modifier = "Elite";
                    SetColor(1, 0.5f, 0); //Orange
                }
                else if (modifierRNG == 5)
                {
                    modifier = "Tank";
                    SetColor(0, 0, 1); //Blue
                }
            }
        }

        private void HandlePositionGeneration()
        {
            int sideRNG = Utils.Random(0, 4); //Default values are 0 and 4
            Vec2 generatedPosition = new Vec2();
            rotation = 90;
            if (sideRNG == 0) //left
            {
                generatedPosition = new Vec2(-width, Utils.Random(100, game.height - 100));
                rotation = Utils.Random(75, 105);
            }
            else if (sideRNG == 1) //right
            {
                generatedPosition = new Vec2(game.width + width, Utils.Random(100, game.height - 100));
                rotation = Utils.Random(-105, -75);
            }
            else if (sideRNG == 2) //top
            {
                generatedPosition = new Vec2(Utils.Random(100, game.width - 100), -width);
                rotation = Utils.Random(175, 195);
            }
            else if (sideRNG == 3) //bottom
            {
                generatedPosition = new Vec2(Utils.Random(100, game.width - 100), game.height + width);
                rotation = Utils.Random(-15, 15);
            }
            if (!isModified)
            {
                //50% chance for a non-modified enemy to rotate towards the player only when it spawns
                int rotateRNG = Utils.Random(0, 2);
                if (rotateRNG == 0)
                {
                    RotateTowardsPlayer();
                }             
            }
            _position = generatedPosition;
            UpdateScreenPosition();
        }

        private void Update()
        {
            HandleMechanics();
            HandleCollisions();
        }

        private void HandleMechanics()
        {            
            if (modifier == "Homing" || modifier == "Inertia" || modifier == "Elite" || modifier == "Tank")
            {
                //make most modified enemies rotate towards the player
                RotateTowardsPlayer();
            }      
            if (modifier == "Inertia" || modifier == "Tank")
            {
                //make two of the modified enemies move slowly towards the player,
                    //gaining speed as they move in a specific direction
                    //and losing it when they have to steer
                Vec2 newVelocity = velocity;
                Vec2 delta = player.position - _position;
                Vec2 deltaNormalized = delta.Normalized();
                deltaNormalized *= _speed;
                previousVelocity = newVelocity;
                newVelocity = deltaNormalized;
                //use the Inertia method to achieve this slow movement,
                    //translating 99% of the previous frame's velocity and 1% of the new one to the current one
                velocity = Vec2.Inertia(previousVelocity, newVelocity, 0.99f, 0.01f);
            }
            if (modifier == "Tank")
            {
                _speed += 0.01f;
            }
            if (modifier == "Erratic")
            {
                erraticTimer -= 0.025f;
                if (erraticTimer <= 0)
                {
                    erraticTimer = Utils.Random(2, 5);
                    erraticTimer /= 2;
                    Turn(Utils.Random(-90, 90));
                }
            }
            HandleMovement();
            if (outsideScreen)
            {
                LateRemove();
                LateDestroy();
            }
        }

        private void HandleMovement()
        {
            if (modifier != "Inertia" && modifier != "Tank")
            {
                velocity = Vec2.GetUnitVectorDeg(rotation) * _speed;
            }
            _position += velocity;
            UpdateScreenPosition();
        }

        private void UpdateScreenPosition()
        {
            x = position.x;
            y = position.y;
            coll.position = position;
        }

        private void HandleCollisions()
        {
            // Check overlapping trigger colliders:
            List<Collider> overlaps = engine.GetOverlaps(coll);

            // Deal with overlaps
            foreach (Collider col in overlaps)
            {
                if (col.owner is Explosion)
                {
                    HandleExplosionCollision(col.owner);
                }
            }
        }

        private void HandleExplosionCollision(GameObject other)
        {
            Explosion explosion = other as Explosion;
            if (modifier != "Tank")
            {
                player.score += 1;
                Die();
            }
            else
            {
                _speed = 5;
                //explosions push enemies with the Tank modifier back
                //since they always rotate towards the player they just
                    //get moved in the opposite direction of their rotation,
                    //taking distance and the explosion's size into account
                //the longer the distance between the enemy and the player the less forceful the explosion is
                float distance = _position.Distance(player.position);
                Vec2 direction = Vec2.GetUnitVectorDeg(rotation);
                velocity -= direction *  distance / 15 * (explosion.scale / 1.5f);
            }
        }

        private void RotateTowardsPlayer()
        {
            Vec2 diff = player.position - _position;
            float rotAngle = diff.GetAngleRadians() * 360 / Vec2.Deg2Rad(360);
            rotation = rotAngle;
        }

        public void Die()
        {
            engine.RemoveTriggerCollider(coll);
            isDead = true;
            ObjectDeathEffect deathEffect = new ObjectDeathEffect(position, velocity);
            game.LateAddChild(deathEffect);
            LateRemove();
            LateDestroy();
        }
    }
}
