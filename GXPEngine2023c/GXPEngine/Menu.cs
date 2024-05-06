using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GXPEngine
{
    class Menu : GameObject
    {
        string type;

        Sound gameOverHell = new Sound("GameOverHellSound.mp3");
        SoundChannel menuChannel;
        public Menu(string pType) : base()
        {
            type = pType;
            Start();
        }
        void Start()
        {
            if (type == "Main Menu")
            {
                Sprite background = new Sprite("MainMenuBackground.png", false, false);
                Sprite title = new Sprite("title.png", false, false);
                Button startButton = new Button("Press the nose to start", "Start Game", game.width / 2 - 400 / 2, 350);
                title.SetXY(game.width / 2 - title.width / 2, 50);
                AddChild(background);
                AddChild(title);
                AddChild(startButton);
            }
            else if (type == "Game Over")
            {
                MyGame mainGame = (MyGame)game;
                string restartButtonText = "Press the nose to restart";
                if (mainGame.completedLevelIndices.Count < 2)
                {
                    Sprite background = new Sprite("GameoverCandy.png", false, false);
                    AddChild(background);
                }
                else
                {
                    menuChannel = gameOverHell.Play();
                    Sprite background = new Sprite("GameoverHell.png", false, false);
                    AddChild(background);
                    restartButtonText = "";
                }
                EasyDraw score = new EasyDraw(300, 50, false);
                Button restartButton = new Button(restartButtonText, "Restart Game", game.width / 2 - 400 / 2, 600);
                score.TextFont("Concert One", 15);
                score.TextAlign(CenterMode.Center, CenterMode.Center);
                score.SetXY(game.width / 2 - score.width / 2, 175);
                score.Text("Score: " + mainGame.playerData.playerScore);
                AddChild(score);
                AddChild(restartButton);
            }
            else if (type == "Win Screen")
            {
                Sprite background = new Sprite("WinScreen.png", false, false);                
                EasyDraw score = new EasyDraw(300, 50, false);
                Button restartButton = new Button("", "Restart Game", game.width / 2 - 400 / 2, 425);
                score.TextFont("Concert One", 15);
                score.TextAlign(CenterMode.Center, CenterMode.Center);
                score.SetXY(game.width / 2 - score.width / 2, 175);
                score.Text("Score: " + ((MyGame)game).playerData.playerScore);
                AddChild(background);
                AddChild(score);
                AddChild(restartButton);
            }
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
