using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GXPEngine
{
    class ButtonPlatform : Sprite
    {
        public Vec2 position;
        public int buttonPairIndex;
        public ButtonPlatform(int pairIndex, float xPos, float yPos, float pRotation) : base("MovingPlatform.png")
        {
            buttonPairIndex = pairIndex;
            position = new Vec2(xPos, yPos);
            rotation = pRotation;
        }
        private void Update()
        {
            UpdateScreenPosition();
        }
        private void UpdateScreenPosition()
        {
            x = position.x;
            y = position.y;
        }
    }
}
