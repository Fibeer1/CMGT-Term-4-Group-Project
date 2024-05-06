using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GXPEngine
{
    class Gun : Sprite
    {
        //General variable
        private Player player;

        //Movement variable
        private Vec2 mousePos;

        //Auto-aim variables
        private bool autoAimOn = false;
        private Enemy autoAimTarget;
        private float autoAimDetectionRadius = 500f;
        
        public Gun() : base("Gun.png", false)
        {
            SetOrigin(0, height / 2);           
        }

        private void Update()
        {
            if (player == null)
            {
                player = parent as Player;
            }
            HandleAimModeSwitch();
            HandleTransform();
        }

        private void HandleAimModeSwitch()
        {
            if (Input.GetKeyDown(Key.F))
            {
                autoAimOn = !autoAimOn;
            }
        }

        private void HandleTransform()
        {
            if (autoAimOn)
            {
                autoAimTarget = GetNearestEnemy();
                if (autoAimTarget != null)
                {
                    //make the gun rotate towards the nearest enemy
                    Vec2 diff = autoAimTarget.position - player.position;
                    float rotAngle = diff.GetAngleRadians() * 360 / Vec2.Deg2Rad(360);
                    rotation = rotAngle - player.rotation;
                }
            }
            else
            {
                //make the gun rotate towards the mouse
                mousePos.x = Input.mouseX;
                mousePos.y = Input.mouseY;
                Vec2 diff = mousePos - player.position;
                float rotAngle = diff.GetAngleRadians() * 360 / Vec2.Deg2Rad(360);
                rotation = rotAngle - player.rotation;
            }
        }

        private Enemy GetNearestEnemy()
        {
            Enemy[] enemies = game.FindObjectOfType<EntityManager>().FindObjectsOfType<Enemy>();
            float distance = autoAimDetectionRadius;
            Enemy closestEnemy = null;
            foreach (Enemy enemy in enemies)
            {
                Vec2 delta = enemy.position - player.position;
                if (delta.Length() < distance)
                {
                    //cycle through all living enemies to get the nearest one
                    closestEnemy = enemy;
                    distance = delta.Length();
                }
            }
            return closestEnemy;
        }
    }
}
