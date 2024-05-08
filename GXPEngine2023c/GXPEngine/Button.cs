using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GXPEngine
{
    class Button : EasyDraw
    {
        string text;
        MyGame mainGame;

        public Button(string pText, float pX, float pY) : base(150, 50)
        {
            SetXY(pX, pY);
            text = pText;
            Clear(System.Drawing.Color.FromArgb(50, 50, 50));
            TextAlign(CenterMode.Center, CenterMode.Center);
            Text(text);
            mainGame = game.FindObjectOfType<MyGame>();
        }

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (HitTestPoint(Input.mouseX, Input.mouseY))
                {
                    if (text == "Start Game")
                    {
                        Menu menu = parent as Menu;
                        menu.DestroyAll();
                        //mainGame.StartLevel(0);
                        mainGame.TestLevel();
                    }
                    else if (text == "Restart")
                    {
                        Menu menu = parent as Menu;
                        menu.DestroyAll();
                        mainGame.StartLevel(0);
                    }
                    else if (text == "Quit Game")
                    {
                        Environment.Exit(0);
                    }
                    //Screens: Main menu, game over
                }
            }
        }
    }
}
