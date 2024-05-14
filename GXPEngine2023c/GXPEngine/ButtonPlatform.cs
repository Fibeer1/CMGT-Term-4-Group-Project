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
        public bool shouldMove = false;
        private Vec2 startPos;
        private Vec2 endPos;
        private float moveTimer;
        private float moveTimerDuration = 3;
        private bool startToEnd = true;

        public ButtonPlatform(int pairIndex, float xPos, float yPos, float pRotation) : base("MovingPlatform.png")
        {            
            buttonPairIndex = pairIndex;
            position = new Vec2(xPos, yPos);
            rotation = pRotation;
            startPos = position;
            endPos = position + Vec2.GetUnitVectorDeg(rotation - 90) * height * 3;
            moveTimer = moveTimerDuration;
            shouldMove = false;
        }

        private void Update()
        {
            UpdateScreenPosition();
            HandleMoving();
        }

        private void UpdateScreenPosition()
        {
            x = position.x;
            y = position.y;
        }

        private void HandleMoving()
        {
            if (shouldMove)
            {
                Vec2 desiredPosition = new Vec2(0, 0);
                if (startToEnd)
                {
                    desiredPosition = Vec2.Lerp(position, endPos, 0.05f);
                }
                else
                {
                    desiredPosition = Vec2.Lerp(position, startPos, 0.05f);
                }
                moveTimer -= 0.0175f;
                position = desiredPosition;
                if (moveTimer <= 0)
                {
                    moveTimer = moveTimerDuration;
                    startToEnd = !startToEnd;
                }
            }
        }
    }
}
