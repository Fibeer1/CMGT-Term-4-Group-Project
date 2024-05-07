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
        }

        private void StartGameOver()
        {
            EasyDraw gameOverText = new EasyDraw(250, 75, false);
            Button restartButton;
            Button quitButton;
            gameOverText.TextSize(25);
            gameOverText.TextAlign(CenterMode.Center, CenterMode.Center);
            gameOverText.SetXY(game.width / 2 - gameOverText.width / 2, 50);
            gameOverText.Text("Game over!");
            restartButton = new Button("Restart", game.width / 2 - 150 / 2, 425);
            quitButton = new Button("Quit Game", game.width / 2 - 150 / 2, 500);
            AddChild(gameOverText);
            AddChild(restartButton);
            AddChild(quitButton);
        }

        private void StartMainMenu()
        {
            EasyDraw title = new EasyDraw(400, 75, false);
            EasyDraw controls = new EasyDraw(360, 480, false);
            Button startButton = new Button("Start Game", game.width / 2 - 150 / 2, 250);
            Button quitButton = new Button("Quit Game", game.width / 2 - 150 / 2, 350);
            title.TextAlign(CenterMode.Center, CenterMode.Center);
            title.SetXY(game.width / 2 - title.width / 2, 50);
            title.TextSize(30);
            title.Text("Interstellar Carnage");
            controls.SetXY(5, 50);
            controls.Text("Controls: \n" +
            "W - forwards \n" +
            "S - backwards \n" +
            "A - turn left \n" +
            "D - turn right \n" +
            "LShift - ram ability \n" +
            "Enter - explode ability \n" +
            "LMB - shoot \n" +
            "F - toggle auto-aim \n");
            AddChild(title);
            AddChild(controls);
            AddChild(startButton);
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
