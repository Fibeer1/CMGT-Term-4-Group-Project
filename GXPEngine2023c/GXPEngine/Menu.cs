using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GXPEngine
{
    class Menu : GameObject
    {
        private string menuType;

        public Menu(string pType) : base()
        {
            menuType = pType;       
            if (menuType == "Game Over")
            {
                StartGameOver();
            }
            else if (menuType == "Main Menu")
            {
                StartMainMenu();
            }
            else if (menuType == "Win Screen")
            {
                StartWinScreen();
            }
        }

        private void StartGameOver()
        {
            Sprite background = new Sprite("GameOverBackground.png", false, false);
            ButtonUI restartButton;
            ButtonUI quitButton;
            restartButton = new ButtonUI("RestartButton.png", "Restart Level", 445, game.height / 2 - 30);
            quitButton = new ButtonUI("QuitButton.png", "Quit Game", 840, game.height / 2 - 30);
            AddChild(background);
            AddChild(restartButton);
            AddChild(quitButton);
        }

        private void StartMainMenu()
        {
            Sprite background = new Sprite("MainMenuBackground.png", false, false);
            ButtonUI startButton = new ButtonUI("PlayButton.png", "Start Game", game.width / 2, 450);
            ButtonUI quitButton = new ButtonUI("QuitButton.png", "Quit Game", game.width / 2, game.height - 185);
            quitButton.SetScaleXY(1, 1);
            AddChild(background);
            AddChild(startButton);
            AddChild(quitButton);
        }
        private void StartWinScreen()
        {
            Sprite background = new Sprite("WinScreenBackground.png", false, false);
            ButtonUI restartButton = new ButtonUI("RestartButton.png", "Restart Game", game.width / 2 - 200, 420);
            ButtonUI quitButton = new ButtonUI("QuitButton.png", "Quit Game", game.width / 2 + 200, 420);
            AddChild(background);
            AddChild(restartButton);
            AddChild(quitButton);
        }

        public void DestroyAll()
        {
            for (int i = 0; i < GetChildCount(); i++)
            {
                GetChildren()[i].LateDestroy();
            }
        }
    }
}
