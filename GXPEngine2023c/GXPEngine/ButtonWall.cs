using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GXPEngine
{
    class ButtonWall : Sprite
    {
        public Vec2 position;
        public int wallPairIndex;
        public ButtonWall(int pairIndex, float xPos, float yPos) : base("MovableWall3.png")
        {
            wallPairIndex = pairIndex;
            position = new Vec2(xPos, yPos);           
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
