using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GXPEngine
{
    class ButtonObject : Sprite
    {
        private Vec2 _position;
        private int wallPairIndex;
        private ButtonWall wallPair;
        public string pushDirection; //Can be Up, Right, Left, Down
        private bool shouldPushButton = true;
        public bool isPushing = false;
        private float buttonMoveTimer = 0.5f;
        private float wallMoveTimer = 1f;
        private Vec2 buttonPositionToLerp;
        private Vec2 wallPositionToLerp;
        public ButtonObject(int pairIndex, float xPos, float yPos, float pRotation) : base("Button.png")
        {
            SetOrigin(width / 2, height / 2);
            _position = new Vec2(xPos, yPos);
            wallPairIndex = pairIndex;
            rotation = pRotation;
            buttonPositionToLerp = _position + Vec2.GetUnitVectorDeg(rotation + 90) * 10;
            if (rotation == 180)
            {
                pushDirection = "Up";
            }
            else if (rotation == -90)
            {
                pushDirection = "Right";
            }
            else if (rotation == 90)
            {
                pushDirection = "Left";
            }
            else if (rotation == 0)
            {
                pushDirection = "Down";
            }
            UpdateScreenPosition();
        }

        private void Update()
        {
            HandleButtonPushing();
            UpdateScreenPosition();
        }
        private void UpdateScreenPosition()
        {
            x = _position.x;
            y = _position.y;
        }
        private void HandleButtonPushing()
        {
            if (shouldPushButton && isPushing)
            {                               
                
                if (buttonMoveTimer > 0)
                {
                    _position = Vec2.Lerp(_position, buttonPositionToLerp, 0.125f);
                }
                wallPair.position = Vec2.Lerp(wallPair.position, wallPositionToLerp, 0.125f);

                buttonMoveTimer -= 0.0175f;
                wallMoveTimer -= 0.0175f;

                if (buttonMoveTimer < 0 && wallMoveTimer < 0)
                {
                    shouldPushButton = false;
                }
            }            
        }
        public void CheckWallPair()
        {
            List<GameObject> parentChildren = parent.GetChildren();
            foreach (GameObject gameObject in parentChildren)
            {
                if (gameObject is ButtonWall)
                {
                    if ((gameObject as ButtonWall).wallPairIndex == wallPairIndex)
                    {
                        wallPair = gameObject as ButtonWall;
                        wallPositionToLerp = wallPair.position + Vec2.GetUnitVectorDeg(wallPair.rotation + 90) * 100;
                    }
                }
            }
        }
    }
}
