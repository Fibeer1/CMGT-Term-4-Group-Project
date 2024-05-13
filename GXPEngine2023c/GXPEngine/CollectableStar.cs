using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GXPEngine
{
    class CollectableStar : Sprite
    {
        private Vec2 _position;
        public CollectableStar(float xPos, float yPos) : base("CollectableStar.png")
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

        public void CollectStar()
        {
            alpha = 0;
            ObjectDeathEffect deathEffect = new ObjectDeathEffect(_position, new Vec2(0,0));
            parent.LateAddChild(deathEffect);
            LateRemove();
            LateDestroy();
        }
    }
}
