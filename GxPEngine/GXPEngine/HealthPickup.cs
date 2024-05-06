using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Physics;

namespace GXPEngine
{
    class HealthPickup : Sprite
    {
        //General variable
        private int _radius;

        //Movement variables
        private Vec2 position
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

        public HealthPickup() : base("HealthPickup.png")
        {
            SetOrigin(width / 2, height / 2);
            _radius = width;
            _position.x = Utils.Random(100, game.width - 100);
            _position.y = Utils.Random(100, game.height - 100);
            x = position.x;
            y = position.y;

            coll = new AABB(this, position, _radius * 0.7f, _radius * 0.7f);
            engine = ColliderManager.main;
            engine.AddTriggerCollider(coll);
        }

        protected override void OnDestroy()
        {
            // Remove the collider when the sprite is destroyed:
            engine.RemoveTriggerCollider(coll);
        }
    }
}
