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
        public Camera camera;

        private Vec2 mousePos;
        private Vec2 initialMousePos;
        private Vec2 cameraPos;
        private bool isRotating;

        public LevelRotator() : base("HealthPickup.png", false, false)
        {
            SetOrigin(width / 2, height / 2);
        }

        private void Update()
        {
            //HandleMouseDragRotation();
            //UpdateScreenPosition();
        }

        private void HandleMouseDragRotation()
        {
            cameraPos = new Vec2(camera.x, camera.y);
            _position = cameraPos;
            Vector2 mousePosInScreen = camera.ScreenPointToGlobal(Input.mouseX, Input.mouseY);
            mousePos = new Vec2(mousePosInScreen.x, mousePosInScreen.y);

            if (Input.GetMouseButton(0) && camera.ScreenPointInWindow(Input.mouseX, Input.mouseY))
            {
                if (!isRotating)
                {
                    isRotating = true;
                    initialMousePos = mousePos;
                }

                Vec2 currentMousePosition = mousePos;

                // Adjust mouse position relative to camera position
                Vec2 adjustedInitialMousePosition = initialMousePos - position;
                Vec2 adjustedCurrentMousePosition = currentMousePosition - position;

                float rotationAngle = adjustedCurrentMousePosition.GetAngleDegrees() -
                    adjustedInitialMousePosition.GetAngleDegrees();

                float newRotation = camera.rotation + rotationAngle;

                camera.rotation = newRotation;

                initialMousePos = currentMousePosition;
            }
            else
            {
                isRotating = false;
            }
        }

        private void UpdateScreenPosition()
        {
            SetXY(position.x, position.y);
        }
    }
}
