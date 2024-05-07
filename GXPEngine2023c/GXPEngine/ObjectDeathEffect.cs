using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GXPEngine
{
    class ObjectDeathEffect : Sprite
    {
        private Vec2 _position;
        private float lifeTime = 0.5f;
     
        public ObjectDeathEffect(Vec2 pPosition, Vec2 pVelocity) : base("ObjectDeathEffect.png")
        {
            SetOrigin(width / 2, height / 2);
            _position = pPosition + pVelocity;
            UpdateScreenPosition();
        }

        private void UpdateScreenPosition()
        {
            x = _position.x;
            y = _position.y;
        }

        private void Update()
        {
            lifeTime -= 0.025f;
            alpha -= 0.05f;
            SetScaleXY(scaleX += 0.1f, scaleY += 0.1f);
            if (lifeTime <= 0)
            {
                LateRemove();
                LateDestroy();
            }
        }
    }
}
