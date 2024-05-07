using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GXPEngine
{
    public class PlayerData
    {
        private float score = 0f;

        const int lifeCount = 3;
        private int currentLifeCount = 3;

        const float maxStamina = 1000;

        //higher value means slower stamina reduction
        const float staminaReduceRate = 25;
        private float staminaRightNow = 0f;

        const float movementSpeed = 7.5f;
        const float heightJump = 17.5f;
        const float gravityStrength = .6f;
        const float spriteScale = 0.5f;

        const float biteCDTime = 0.5f;
        const float hornCDTime = 3;
        const float hornFiringRange = 400;
        const float hornProjectileSpeed = 15;
        const float hornStaminaNum = 25;

        const string runningSound = "Gallop.WAV";
        const string bitingSound = "Bite.WAV";
        const string shootingSound = "Horn_Shoot.WAV";
        const string hurtSound = "Horse_Hurt.WAV";
        const string deathSound = "Horse_Death.MP3";
        const string hornHitSound = "Horn_Hit.WAV";

        public float playerScore
        {
            get
            {
                return score;
            }
            set
            {
                score = value;
            }
        }

        public int lives
        {
            get
            {
                return lifeCount;
            }
        }

        public int currentLives
        {
            get
            {
                return currentLifeCount;
            }
            set
            {
                currentLifeCount = value;
            }
        }

        public float stamina
        {
            get
            {
                return maxStamina;
            }
        }

        public float staminaRate
        {
            get
            {
                return staminaReduceRate;
            }
        }

        public float currentStamina
        {
            get
            {
                return staminaRightNow;
            }
            set
            {
                staminaRightNow = value;
            }
        }

        public float speed
        {
            get
            {
                return movementSpeed;
            }
        }

        public float jumpHeight
        {
            get
            {
                return heightJump;
            }
        }

        public float gravity
        {
            get
            {
                return gravityStrength;
            }
        }
        public float scale
        {
            get
            {
                return spriteScale;
            }
        }

        public float biteCD
        {
            get
            {
                return biteCDTime;
            }
        }

        public float hornCD
        {
            get
            {
                return hornCDTime;
            }
        }

        public float hornRadius
        {
            get
            {
                return hornFiringRange;
            }
        }

        public float hornSpeed
        {
            get
            {
                return hornProjectileSpeed;
            }
        }

        public float hornStaminaDrain
        {
            get
            {
                return hornStaminaNum;
            }
        }

        public string runSound
        {
            get
            {
                return runningSound;
            }
        }

        public string biteSound
        {
            get
            {
                return bitingSound;
            }
        }

        public string shootSound
        {
            get
            {
                return shootingSound;
            }
        }

        public string damageSound
        {
            get
            {
                return hurtSound;
            }
        }

        public string dieSound
        {
            get
            {
                return deathSound;
            }
        }

        public string projectileHitSound
        {
            get
            {
                return hornHitSound;
            }
        }

        public PlayerData()
        {
            Reset();
        }

        void Reset()
        {
            staminaRightNow = maxStamina;
        }
    }
}
