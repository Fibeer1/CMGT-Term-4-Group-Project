using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GXPEngine
{
    class Button : EasyDraw
    {
        string text;
        public Button(string pText, float pX, float pY) : base(150, 50)
        {
            SetXY(pX, pY);
            text = pText;
            Clear(System.Drawing.Color.FromArgb(50, 50, 50));
            TextAlign(CenterMode.Center, CenterMode.Center);
            Text(text);
        }

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (HitTestPoint(Input.mouseX, Input.mouseY))
                {
                    if (text == "Start Game")
                    {
                        MyGame myGame = game.FindObjectOfType(typeof(MyGame)) as MyGame;
                        Menu menu = parent as Menu;
                        menu.DestroyAll();
                        myGame.StartGame();
                    }
                    else if (text == "Restart")
                    {
                        Menu menu = parent as Menu;
                        menu.DestroyAll();
                        game.FindObjectOfType<MyGame>().StartGame();
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
