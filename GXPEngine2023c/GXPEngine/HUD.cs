using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace GXPEngine
{
    class HUD : EasyDraw
    {
        //General variables
        Player player;
        WaveManager waveManager;
        Font textFont = new Font(FontFamily.GenericSansSerif, 15);

        //Cooldown variables
        float explosionTime = 0;
        float explosionCD = 0;
        float ramTime = 0;
        float ramCD = 0;

        public HUD() : base(1280, 720, false)
        {
            player = game.FindObjectOfType<Player>();
            waveManager = game.FindObjectOfType<WaveManager>();
        }

        private void Update()
        {
            graphics.Clear(Color.Empty);
            //Health
            graphics.DrawString("Health: " + player.healthPoints, textFont, Brushes.White, 10, 10);
            //Score
            graphics.DrawString("Score: " + player.score, textFont, Brushes.White, 10, 35);
            //Explode CD
            HandleExplosionCD();
            //Ram CD
            HandleRamCD();
            //Wave Counter
            graphics.DrawString("Wave " + waveManager.currentWave, textFont, Brushes.White, 600, 10);
            //Wave Timer
            graphics.DrawString("Time: " + (int)waveManager.timer, textFont, Brushes.White, 600, 40);
            //Codex
            if (waveManager.currentState == "Break" && waveManager.currentWave < 10)
            {
                graphics.DrawString("Enemy types:", textFont, Brushes.White, 10, 275);
                graphics.DrawString("White - Normal", textFont, Brushes.White, 10, 300);
                graphics.DrawString("Yellow - Small and quick", textFont, Brushes.Yellow, 10, 325);
                graphics.DrawString("Purple - Changes direction randomly", textFont, Brushes.Purple, 10, 350);
                graphics.DrawString("Turquoise - Homes in towards me", textFont, Brushes.Turquoise, 10, 375);
                graphics.DrawString("Green - Slowly rotates towards me", textFont, Brushes.LimeGreen, 10, 400);
                graphics.DrawString("Orange - Homes in towards me and deals damage when rammed", textFont, Brushes.Orange, 10, 425);
                graphics.DrawString("Blue - Slowly homes in towards me and is pushed back from explosions", textFont, Brushes.Blue, 10, 450);
            }
        }

        private void HandleExplosionCD()
        {
            if (player.explosionCooldown > 0)
            {               
                if (explosionCD <= 0)
                {
                    explosionCD = player.explosionCooldown;                    
                }
                float angle = 0;
                if (angle < 360)
                {
                    angle = explosionTime / explosionCD * 360;
                    explosionTime += 0.0175f;
                }
                graphics.DrawString("Explode CD: ", textFont, Brushes.White, 10, 60);
                graphics.FillPie(new SolidBrush(Color.White), 130, 65, 15, 15, 0, angle);
            }
            else
            {
                explosionTime = 0;
                explosionCD = 0;
            }
        }

        private void HandleRamCD()
        {
            if (player.ramCooldown > 0)
            {
                if (ramCD <= 0)
                {
                    ramCD = player.ramCooldown;
                }
                float angle = 0;
                if (angle < 360)
                {
                    angle = ramTime / ramCD * 360;
                    ramTime += 0.0175f;
                }               
                graphics.DrawString("Ram CD: ", textFont, Brushes.White, 10, 85);
                graphics.FillPie(new SolidBrush(Color.White), 100, 90, 15, 15, 0, angle);
            }
            else
            {
                ramTime = 0;
                ramCD = 0;
            }
        }
    }
}
