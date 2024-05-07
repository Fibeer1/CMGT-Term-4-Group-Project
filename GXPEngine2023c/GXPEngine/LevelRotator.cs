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

        public LevelRotator() : base("Empty.png")
        {
            SetOrigin(width / 2, height / 2);           
        }
        private void Update()
        {
            _position = player.position;
            if (Input.GetMouseButton(0))
            {
                
                Vector2 mousePosInScreen = player.camera.ScreenPointToGlobal(Input.mouseX, Input.mouseY);
                Vec2 mousePos = new Vec2(mousePosInScreen.x, mousePosInScreen.y);
                Vec2 diff = new Vec2(mousePos.x - position.x, mousePos.y - position.y);
                float rotAngle = diff.GetAngleRadians() * 360 / Vec2.Deg2Rad(360);
                level.rotation = rotAngle;
                if (level.parent != this)
                {
                    //level.parent = this;
                    //level.SetOrigin(player.camera.x, player.camera.y);
                }
            }
            else
            {
                //level.parent = game;
            }
        }
    }
}
