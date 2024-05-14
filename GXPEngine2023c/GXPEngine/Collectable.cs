using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GXPEngine
{
    class Collectable : AnimationSprite
    {
        private Vec2 _position;
        public Collectable(float xPos, float yPos) : base("Collectable.png", 6, 1)
        {
            SetOrigin(width / 2, height / 2);
            _position = new Vec2(xPos, yPos);           
            UpdateScreenPosition();
        }

        private void UpdateScreenPosition()
        {
            x = _position.x;
            y = _position.y;
        }

        private void Update()
        {
            Animate(0.125f);
        }

        public void CollectBlob()
        {
            alpha = 0;
            ObjectDeathEffect deathEffect = new ObjectDeathEffect(_position, new Vec2(0,0));
            parent.LateAddChild(deathEffect);
            LateRemove();
            LateDestroy();
        }
    }
}
