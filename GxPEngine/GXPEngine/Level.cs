using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using TiledMapParser;

namespace GXPEngine
{
    class Level : GameObject
    {
        public Player player;
        Camera camera;
        public HUD hud;
        Sound cheerfulMusic = new Sound("cheerfulMusic.mp3", true);
        Sound hellishMusic = new Sound("hellishMusic.mp3", true);

        public SoundChannel musicChannel;

        int lvlNumber;

        public Level(int index) : base()
        {
            string levelBackground = "";
            //if (((MyGame)game).completedLevelIndices.Count == 0)
            //{
            //    musicChannel = cheerfulMusic.Play();
            //    levelBackground = "OverworldBackground.png";
            //}
            Sprite background1 = new Sprite(levelBackground, false, false);
            AddChild(background1);            
            Sprite background2 = new Sprite(levelBackground, false, false);
            background2.SetXY(background1.width, background1.y);
            AddChild(background2);

            Map levelData = MapParser.ReadMap("Level " + index + ".tmx");
            SpawnTiles(levelData);
            SpawnObjects(levelData);
            camera = new Camera(0, 0, game.width, game.height);
            AddChild(camera);
            //player.camera = camera;
            camera.SetScaleXY(1.5f, 1.5f);

            lvlNumber = index;

            //HUD gets added last
            hud = new HUD();
            //hud.level = this;
            camera.AddChild(hud);
            hud.SetXY(camera.x - game.width / 2, camera.y - game.height / 2);
            //hud.Start();
        }
        private void SpawnTiles(Map leveldata)
        {
            if (leveldata.Layers == null || leveldata.Layers.Length == 0)
            {
                return;
            }
            Layer tileLayer = leveldata.Layers[0];
            short[,] tileNumbers = tileLayer.GetTileArray();
            for (int row = 0; row < tileLayer.Height; row++)
            {
                for (int col = 0; col < tileLayer.Width; col++)
                {
                    int tileNumber = tileNumbers[col, row];
                    if (tileNumber > 0)
                    {
                        string tilesetFile = "OverworldTileSet.png";
                        SolidBlock tile = new SolidBlock(new Vec2(0, 0),tilesetFile, 9, 4);
                        if (tileNumber >= 7 && tileNumber <= 9 || tileNumber >= 16 && tileNumber <= 18 || tileNumber >= 25 && tileNumber <= 27)
                        {
                            tile.collider.isTrigger = true;
                        }
                        tile.SetFrame(tileNumber - 1);
                        tile.x = col * tile.width;
                        tile.y = row * tile.height;
                        tile.MoveColliderWithBlock();
                        AddChild(tile);
                    }
                }
            }
        }
        private void SpawnObjects(Map leveldata)
        {
            if (leveldata.ObjectGroups == null || leveldata.ObjectGroups.Length == 0)
            {
                return;
            }
            ObjectGroup objectGroup = leveldata.ObjectGroups[0];
            if (objectGroup.Objects == null || objectGroup.Objects.Length == 0)
            {
                return;
            }
            foreach (TiledObject obj in objectGroup.Objects)
            {
                switch (obj.Name)
                {                    
                    case "Player":
                        player = new Player();
                        player.SetXY(obj.X, obj.Y);
                        AddChild(player);
                        //player.SetSpawnPoint();
                        break;
                    case "Enemy":
                        //Enemy enemy = new Enemy();
                        //enemy.SetXY(obj.X, obj.Y);
                        //AddChild(enemy);
                        //enemy.Start();
                        break;
                    case "Finish":
                        //Finish finish = new Finish();
                        //finish.SetXY(obj.X, obj.Y);
                        //AddChild(finish);
                        break;
                    case "Trigger":
                        //EnemyTrigger trigger = new EnemyTrigger();
                        //trigger.SetXY(obj.X, obj.Y);
                        //trigger.SetScaleXY(obj.Width, obj.Height);
                        //AddChild(trigger);
                        break;
                    default:
                        break;
                }
            }           
        }

        public void NewEnemies()
        {
            Map levelData = MapParser.ReadMap("Level " + lvlNumber + ".tmx");

            List<GameObject> children = GetChildren();
            for (int i = 0; i < children.Count; i++)
            {
                //if (children[i] is Enemy)
                //{
                //    children[i].LateDestroy();
                //}
            }
            
            if (levelData.ObjectGroups == null || levelData.ObjectGroups.Length == 0)
            {
                return;
            }
            ObjectGroup objectGroup = levelData.ObjectGroups[0];
            if (objectGroup.Objects == null || objectGroup.Objects.Length == 0)
            {
                return;
            }

            //foreach (TiledObject obj in objectGroup.Objects)
            //{
            //    switch (obj.Name)
            //    {
            //        case "EnemyTrigger":
            //            Enemy enemy = new Enemy();
            //            enemy.SetXY(obj.X, obj.Y);
            //            AddChild(enemy);
            //            enemy.Start();
            //            break;
            //    }
            //}
        }
    }
}
