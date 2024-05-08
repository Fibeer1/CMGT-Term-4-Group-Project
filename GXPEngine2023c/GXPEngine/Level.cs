using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using TiledMapParser;

namespace GXPEngine
{
    class Level : Sprite
    {
        public Player player;
        Camera camera;
        public HUD hud;        
        int lvlNumber;

        public Level(int index) : base("MapEmpty.png", false, false)
        {
            SetOrigin(width / 2, height / 2);
            Map levelData = MapParser.ReadMap("Level " + index + ".tmx");
            SpawnTiles(levelData);
            SpawnObjects(levelData);
            
            List<GameObject> children = GetChildren();
            foreach (GameObject child in children)
            {
                child.x -= width / 2;
                child.y -= height / 2;
            }

            player.SetPosition(new Vec2(player.position.x - width / 2, player.position.y - height / 2));
            camera = new Camera(0, 0, game.width, game.height);
            camera.SetScaleXY(1.5f, 1.5f);
            game.AddChild(camera);
            player.camera = camera;
            lvlNumber = index;
            LevelRotator rotator = new LevelRotator();
            rotator.level = this;
            rotator.player = player;
            camera.AddChild(rotator);
            

            //HUD gets added last
            hud = new HUD();
            camera.AddChild(hud);
            hud.SetXY(camera.x - game.width / 2, camera.y - game.height / 2);           
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
                        CollisionTile tile = new CollisionTile(tilesetFile, 9, 4, tileNumber - 1);
                        tile.x = col * tile.width;
                        tile.y = row * tile.height;
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
                        player.SetPosition(new Vec2(obj.X, obj.Y));
                        game.AddChild(player);
                        player.SetSpawnPoint();
                        break;
                    case "Finish":
                        Finish finish = new Finish();
                        finish.SetXY(obj.X, obj.Y);
                        AddChild(finish);
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

            foreach (TiledObject obj in objectGroup.Objects)
            {
                //switch (obj.Name)
                //{
                //    case "EnemyTrigger":
                //        Enemy enemy = new Enemy();
                //        enemy.SetXY(obj.X, obj.Y);
                //        AddChild(enemy);
                //        enemy.Start();
                //        break;
                //}
            }
        }
    }
}
