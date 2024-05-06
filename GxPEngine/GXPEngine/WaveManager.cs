using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GXPEngine
{
    class WaveManager : GameObject
    {
        //General variables
        private EntityManager entityManager;
        private Player player;

        //Wave variables
        public int currentWave;
        public float timer;
        private float earlyGameWaveTime = 30; //default value is 30
        private float lateGameWaveTime = 20; //default value is 20
        private float shopTime = 15; //default value is 15
        public string currentState; //Can be Wave or Break

        //Sound variables
        private SoundChannel audioSource;
        private Sound waveStartSound = new Sound("WaveStart.wav", false, false);
                
        public WaveManager() : base()
        {
            timer = earlyGameWaveTime;
            currentWave = 1;
            currentState = "Wave";
            player = game.FindObjectOfType<Player>();
            entityManager = game.FindObjectOfType<EntityManager>();
        }

        private void Update()
        {
            timer -= 0.0175f;
            if (currentState == "Wave")
            {
                if (timer < 0)
                {
                    StartBreak();
                }               
            }
            else if (currentState == "Break" && timer < 0)
            {
                StartWave();
            }
        }

        public void StartWave()
        {
            audioSource = waveStartSound.Play();
            currentState = "Wave";
            currentWave++;
            if (currentWave <= 10)
            {
                timer = earlyGameWaveTime;
            }
            else
            {
                timer = lateGameWaveTime;
            }
            entityManager.DestroyPowerups();
            if (entityManager.enemySpawnTime > 0.05f)
            {
                entityManager.enemySpawnTime -= 0.05f;
            }
            entityManager.healthSpawnTime = 5;
            entityManager.shouldSpawnEnemies = true;
        }

        private void StartBreak()
        {
            currentState = "Break";
            entityManager.healthSpawnInterval = 5;
            entityManager.healthSpawnTime = 5;
            entityManager.shouldSpawnEnemies = false;
            player.ResetPosition();
            entityManager.DestroyAllEnemies();
            timer = shopTime;
            string explosionPowerupName = "Normal";
            string ramPowerupName = "Normal";
            //Explosive
            if (player.explosionPowerupIndex == 0)
            {
                explosionPowerupName = "Large";
            }
            else if (player.explosionPowerupIndex == 1)
            {
                explosionPowerupName = "Burst";
            }
            else if (player.explosionPowerupIndex == 2)
            {
                explosionPowerupName = "Shrapnel Release";
            }
            else
            {
                explosionPowerupName = "Maxed";
            }
            if (explosionPowerupName != "Maxed")
            {
                Powerup explosionPowerup = new Powerup("Explosion", explosionPowerupName, new Vec2(game.width - 175, 200));
                entityManager.AddChild(explosionPowerup);
            }
            else
            {
                Powerup statPowerupExplosionReplacement = new Powerup("Stat", "Boost", new Vec2(game.width - 175, 200));
                entityManager.AddChild(statPowerupExplosionReplacement);
            }
            //Ram
            if (player.ramPowerupIndex == 0)
            {
                ramPowerupName = "Boost";
            }
            else if (player.ramPowerupIndex == 1)
            {
                ramPowerupName = "Claymores";
            }
            else if (player.ramPowerupIndex == 2)
            {
                ramPowerupName = "Rifles";
            }
            else
            {
                ramPowerupName = "Maxed";
            }
            if (ramPowerupName != "Maxed")
            {
                Powerup ramPowerup = new Powerup("Ram", ramPowerupName, new Vec2(game.width - 175, 550));
                entityManager.AddChild(ramPowerup);
            }
            else
            {
                Powerup statPowerupRamReplacement = new Powerup("Stat", "Boost", new Vec2(game.width - 175, 550));
                entityManager.AddChild(statPowerupRamReplacement);
            }
            //Stat
            Powerup statPowerup = new Powerup("Stat", "Boost", new Vec2(175, 200));
            entityManager.AddChild(statPowerup);
            if (currentWave >= 7)
            {
                int bulletRNG = Utils.Random(0, 4);
                string bulletPowerupName = "Normal";
                if (bulletRNG == 0)
                {
                    bulletPowerupName = "Bouncy";
                }
                else if (bulletRNG == 1)
                {
                    bulletPowerupName = "Shotgun";
                }
                else if (bulletRNG == 2)
                {
                    bulletPowerupName = "Homing";
                }
                else if (bulletRNG == 3)
                {
                    bulletPowerupName = "Piercing";
                }
                Powerup bulletPowerup = new Powerup("Bullet", bulletPowerupName, new Vec2(175, 550));
                entityManager.AddChild(bulletPowerup);
            }
        }
    }
}
