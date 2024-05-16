using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GXPEngine
{
    class ButtonUI : Sprite
    {
        string text;
        MyGame mainGame;
        Sound buttonClickClip = new Sound("UIButtonClick.wav");

        public ButtonUI(string fileText, string pText, float pX, float pY) : base(fileText)
        {
            SetOrigin(width / 2, height / 2);
            SetXY(pX, pY);
            text = pText;
            SetScaleXY(scaleX * 1.5f, scaleY * 1.5f);
            mainGame = game.FindObjectOfType<MyGame>();
        }

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (HitTestPoint(Input.mouseX, Input.mouseY))
                {
                    ((MyGame)game).buttonUIAudioSource = buttonClickClip.Play();
                    if (text == "Start Game" || text == "Restart Level")
                    {
                        Menu menu = parent as Menu;
                        menu.DestroyAll();
                        mainGame.StartLevel(mainGame.currentLevelIndex);
                    }
                    if (text == "Restart Game")
                    {
                        Menu menu = parent as Menu;
                        menu.DestroyAll();
                        mainGame.currentLevelIndex = 0;
                        mainGame.StartLevel(mainGame.currentLevelIndex);
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
