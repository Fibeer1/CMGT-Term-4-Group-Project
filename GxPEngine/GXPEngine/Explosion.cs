using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Physics;

namespace GXPEngine
{
    class Explosion : AnimationSprite
    {
        //General variables
        private Player player;
        private int _radius;
        private float lifeTime = 0.5f;
        public bool shouldExplode = false;
        
        //Movement variables
        public Vec2 position
        {
            get
            {
                return _position;
            }
        }
        private Vec2 _position;

        //Collision variables
        private Collider coll;
        private ColliderManager engine;

        //Sound variable
        private SoundChannel explosionSound;
               
        public Explosion() : base("explosionSheet.png", 6, 1)
        {          
            SetOrigin(width / 2, height / 2);
            player = game.FindObjectOfType(typeof(Player)) as Player;
            _position = player.position;
            _radius = width;
            x = _position.x;
            y = _position.y;

            coll = new AABB(this, position, _radius * 0.7f, _radius * 0.7f);
            engine = ColliderManager.main;
            engine.AddTriggerCollider(coll);
                    
            explosionSound = new Sound("Explosion.wav", false, true).Play();
        }

        protected override void OnDestroy()
        {
            // Remove the collider when the sprite is destroyed:
            engine.RemoveTriggerCollider(coll);
        }

        private void Update()
        {
            HandleExploding();
        }

        private void HandleExploding()
        {
            if (shouldExplode)
            {
                Animate(0.25f);
                lifeTime -= 0.025f;
                if (lifeTime <= 0)
                {
                    player.shouldExplode = false;
                    LateRemove();
                    LateDestroy();
                }
            }           
        }
    }
}
