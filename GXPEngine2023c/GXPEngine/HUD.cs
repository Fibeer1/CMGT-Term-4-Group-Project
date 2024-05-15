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
        public Player player;
        Font textFont = new Font(FontFamily.GenericSansSerif, 15);

        public HUD() : base(1280, 720, false)
        {

        }

        private void Update()
        {
            graphics.Clear(Color.Empty);
            //Score
            graphics.DrawString("Score: " + player.score, textFont, Brushes.White, 10, 35);

        }
    }
}
