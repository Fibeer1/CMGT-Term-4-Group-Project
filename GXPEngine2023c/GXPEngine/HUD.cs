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
        Font textFont = new Font(FontFamily.GenericSansSerif, 15);

        public HUD() : base(1280, 720, false)
        {
            player = game.FindObjectOfType<Player>();
        }

        private void Update()
        {
            graphics.Clear(Color.Empty);
            //Health
            //graphics.DrawString("Health: " + player.healthPoints, textFont, Brushes.White, 10, 10);
            //Score
            //graphics.DrawString("Score: " + player.score, textFont, Brushes.White, 10, 35);
        }
    }
}
