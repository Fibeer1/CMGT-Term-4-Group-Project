using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Physics;

namespace GXPEngine
{   
    class Powerup : Sprite
    {
        //General variable
        private int _radius;

        //Movement variables
        private Vec2 position
        {
            get
            {
                return _position;
            }
        }
        private Vec2 _position;

        //Powerup variables
        public string type; //Can be Explosive, Ram, Bullet or Stat
        public string powerupName;
        private string description;
        
        //Collision variables
        private Collider coll;
        private ColliderManager engine;

        //Text variables
        private EasyDraw nameText = new EasyDraw(175, 25, false);
        private EasyDraw descriptionText = new EasyDraw(400, 250, false);

        //if type is Explosive, can be Large, Burst and Shrapnel Release
        //if type is Ram, can be Boost, Claymores, Rifles
        //if type is Bullet, can be Bouncy or Shotgun
        //if type is Stat, can be Explosion CD, Ram CD, Speed, Max Speed, Ram Speed, Max HP, Bullet Range, Health Taken, Bullet Bounciness, Bullet Sharpness

        public Powerup(string pType, string pPowerupName, Vec2 pPosition) : base("PowerupBox.png")
        {
            SetOrigin(width / 2, height / 2);
            type = pType;
            powerupName = pPowerupName;
            _radius = width;
            _position = pPosition;

            coll = new AABB(this, position, _radius * 0.7f, _radius * 0.7f);
            engine = ColliderManager.main;
            engine.AddTriggerCollider(coll);

            x = position.x;
            y = position.y;
            if (type == "Stat")
            {
                HandleStatPowerup();
            }
            else if (type == "Explosion")
            {
                HandleExplosivePowerup();
            }
            else if (type == "Ram")
            {
                HandleRamPowerup();
            }
            else if (type == "Bullet")
            {
                HandleBulletPowerup();
            }
            SetUpText();           
        }

        private void HandleStatPowerup()
        {
            int randomStat = Utils.Random(0, 10);
            if (randomStat == 0)
            {
                powerupName = "Explosion CD";
                description = "Explosion cooldown decreased";
            }
            if (randomStat == 1)
            {
                powerupName = "Ram CD";
                description = "Ram cooldown decreased";
            }
            if (randomStat == 2)
            {
                powerupName = "Speed";
                description = "Increased acceleration";
            }
            if (randomStat == 3)
            {
                powerupName = "Max Speed";
                description = "Increased max speed";
            }
            if (randomStat == 4)
            {
                powerupName = "Max Ram Speed";
                description = "Increased max ram speed";
            }
            if (randomStat == 5)
            {
                powerupName = "Max HP";
                description = "Increased max health";
            }
            if (randomStat == 6)
            {
                powerupName = "Health Taken";
                description = "Increased health from Health Pickups";                
            }
            if (randomStat == 7)
            {
                powerupName = "Bullet Range";
                description = "Increased Bullet Range";
            }
            if (randomStat == 8)
            {
                powerupName = "Bullet Bounciness";
                description = "Bullets can bounce more times";
            }
            if (randomStat == 9)
            {
                powerupName = "Bullet Sharpness";
                description = "Bullets can pierce more times";
            }
        }

        private void HandleExplosivePowerup()
        {
            if (powerupName == "Large")
            {
                description = "Bigger explosion size";
            }
            else if (powerupName == "Burst")
            {
                description = "Explosion count x3\n " +
                    "Longer explosion cooldown\n " +
                    "Overwrites previous explosion powerups";
            }
            else if (powerupName == "Shrapnel Release")
            {
                description = "Upon exploding, release a flurry of bullets all around you\n" +
                    "Overwrites previous explosion powerups";
            }
        }

        private void HandleRamPowerup()
        {
            if (powerupName == "Boost")
            {
                description = "Charge ram speed faster";
            }
            else if (powerupName == "Claymores")
            {
                description = "Emit an explosion when ramming an enemy\n" +
                    "Explosion is affected by the current explosion powerup\n" +
                    "Overwrites previous ram powerups";
            }
            else if (powerupName == "Rifles")
            {
                description = "Fire rifles when releasing built up speed\n" +
                    "Overwrites previous ram powerups";
            }
        }

        private void HandleBulletPowerup()
        {
            if (powerupName == "Bouncy")
            {
                description = "Bullets fired now bounce off the borders of the map";
            }
            else if (powerupName == "Shotgun")
            {
                description = "Bullet count increased by 10 for each fired bullet\n" +
                    "Bullet range decreased\n" +
                    "Bullets have spread\n" +
                    "Overwrites previous bullet powerups\n" +
                    "Does not stack";
            }
            else if (powerupName == "Homing")
            {
                description = "Bullets home towards enemies";
            }
            else if (powerupName == "Piercing")
            {
                description = "Bullets pierce through enemies";
            }
        }

        private void SetUpText()
        {
            nameText.TextAlign(CenterMode.Center, CenterMode.Center);
            nameText.Text(powerupName, nameText.x + nameText.width / 2, nameText.y + nameText.height / 2);
            nameText.SetXY(width - nameText.width / 1.5f, height - nameText.height * 3.25f);
            AddChild(nameText);
            descriptionText.TextSize(10);
            descriptionText.TextAlign(CenterMode.Center, CenterMode.Min);
            descriptionText.Text(description, descriptionText.x + descriptionText.width / 2, descriptionText.y + descriptionText.height / 2);
            descriptionText.SetXY(width - descriptionText.width / 1.7f, height - descriptionText.height / 2f);
            AddChild(descriptionText);
        }

        protected override void OnDestroy()
        {
            // Remove the collider when the sprite is destroyed:
            engine.RemoveTriggerCollider(coll);
        }
    }
}
