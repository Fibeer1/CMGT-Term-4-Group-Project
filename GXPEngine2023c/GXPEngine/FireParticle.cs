using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GXPEngine
{
    class FireParticle : AnimationSprite
    {
        private Vec2 _position;
        private float lifeTime = 0.5f;

        public FireParticle(Vec2 pPosition, float pRotation) : base("FireAnimation.png", 9, 1)
        {
            SetOrigin(width / 2, height / 2);
            _position = pPosition;
            rotation = pRotation;
            SetScaleXY(1.25f, 1.5f);
            UpdateScreenPosition();
        }

        private void UpdateScreenPosition()
        {
            x = _position.x;
            y = _position.y;
        }

        private void Update()
        {
            Animate(0.3f);
            lifeTime -= 0.0175f;
            if (lifeTime <= 0)
            {
                LateRemove();
                LateDestroy();
            }
        }
    }
}
