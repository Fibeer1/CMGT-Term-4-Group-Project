using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GXPEngine.Core;

namespace GXPEngine
{
    class LevelRotator : Sprite
    {
        public Vec2 position
        {
            get
            {
                return _position;
            }
        }
        private Vec2 _position;

        public Level level;
        public Player player;

        private Vec2 mousePos;
        private Vec2 initialMousePos;
        private Vec2 cameraPos;
        private bool isRotating;
        private float minRotation = -90f;
        private float maxRotation = 120f;

        public LevelRotator() : base("Empty.png", false, false)
        {
            SetOrigin(width / 2, height / 2);            
        }
        private void Update()
        {
            if (player.camera != null)
            {
                cameraPos = new Vec2(player.camera.x, player.camera.y);
            }
            _position = player.position;
            Vector2 mousePosInScreen = player.camera.ScreenPointToGlobal(Input.mouseX, Input.mouseY);
            mousePos = new Vec2(mousePosInScreen.x, mousePosInScreen.y);
            //level.rotation -= 5;
            //Console.WriteLine(level.rotation);

            if (Input.GetMouseButton(0) && player.camera.ScreenPointInWindow(Input.mouseX, Input.mouseY))
            {
                if (!isRotating)
                {
                    isRotating = true;
                    initialMousePos = mousePos;
                }

                Vec2 currentMousePosition = mousePos;

                // Adjust mouse position relative to camera position
                Vec2 adjustedInitialMousePosition = initialMousePos - cameraPos;
                Vec2 adjustedCurrentMousePosition = currentMousePosition - cameraPos;

                float rotationAngle = adjustedCurrentMousePosition.GetAngleDegrees() - adjustedInitialMousePosition.GetAngleDegrees();
                //Console.WriteLine(rotationAngle - level.rotation);
                //if (rotationAngle - level.rotation > 10)
                //{
                //    rotationAngle -= 10;
                //}

                float newRotation = level.rotation + rotationAngle;
                
                level.rotation = Mathf.Clamp(newRotation, minRotation, maxRotation);
                Console.WriteLine(level.rotation);

                initialMousePos = currentMousePosition;
            }
            else
            {
                isRotating = false;
            }
        }
    }
}
