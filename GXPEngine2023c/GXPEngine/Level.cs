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
        private int levelIndex;
        MyGame mainGame;
        Sprite levelBackground;
        Map currentLevelData;
        public SoundChannel musicChannel;
        public SoundChannel fireChannel;
        private Sound gameMusic = new Sound("GameMusic.mp3", true);
        public Sound fireSound = new Sound("Fire.wav");

        public Level(int index) : base("MapEmpty.png", false, false)
        {
            mainGame = game.FindObjectOfType<MyGame>();
            SetUpBackground();                     
            camera = new Camera(0, 0, game.width, game.height);
            game.AddChild(camera);
            camera.AddChild(levelBackground);            

            currentLevelData = MapParser.ReadMap("Level " + index + ".tmx");
            SpawnObjects();
            SpawnTileLayers();
            camera.SetXY(player.position.x, player.position.y);
            player.camera = camera;
            levelIndex = index;
            if (levelIndex >= 2)
            {
                camera.SetScaleXY(1.5f, 1.5f);
            }                                                
            SetScaleXY(1.1f, 1.1f);
            musicChannel = gameMusic.Play();
        }

        private void SpawnTileLayers()
        {
            if (currentLevelData.Layers == null || currentLevelData.Layers.Length == 0)
            {
                return;
            }

            string currentTilesetFile = "DefaultTiles.png";

            for (int layerIndex = 0; layerIndex < currentLevelData.Layers.Length; layerIndex++)
            {
                Layer currentLayer = currentLevelData.Layers[layerIndex];
                short[,] tileNumbers = currentLayer.GetTileArray();

                for (int row = 0; row < currentLayer.Height; row++)
                {
                    for (int col = 0; col < currentLayer.Width; col++)
                    {
                        int tileNumber = tileNumbers[col, row];
                        if (tileNumber > 0)
                        {
                            int tileIndex = tileNumber;
                            bool hasCollision = tileIndex >= 96;
                            CollisionTile currentTile = new CollisionTile(currentTilesetFile, 14, 10, tileIndex, true, hasCollision);
                            if (hasCollision)
                            {
                                currentTile.alpha = 0.5f;
                            }
                            currentTile.SetFrame(tileIndex);
                            currentTile.x = col * currentTile.width;
                            currentTile.y = row * currentTile.height;
                            AddChild(currentTile);
                        }
                    }
                }
            }            
        }

        private void SpawnObjects()
        {
            if (currentLevelData.ObjectGroups == null || currentLevelData.ObjectGroups.Length == 0)
            {
                return;
            }
            ObjectGroup objectGroup = currentLevelData.ObjectGroups[0];
            if (objectGroup.Objects == null || objectGroup.Objects.Length == 0)
            {
                return;
            }
            foreach (TiledObject obj in objectGroup.Objects)
            {
                bool pairIndexedTeleportTile = false;
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
                    case "Collectable":
                        Collectable collectable = new Collectable(obj.X, obj.Y);
                        AddChild(collectable);
                        break;
                    case "MovableWall":
                        ButtonWall wall = new ButtonWall(obj.GetIntProperty("PairIndex"), obj.X, obj.Y, obj.Rotation);
                        AddChild(wall);
                        break;
                    case "WallButton":
                        ButtonObject button = new ButtonObject("Wall", obj.GetIntProperty("PairIndex"), obj.X, obj.Y, obj.Rotation);
                        AddChild(button);
                        break;
                    case "MimicBox":
                        MimicBox mimicBox = new MimicBox(obj.X, obj.Y);
                        mimicBox.player = player;
                        game.AddChild(mimicBox);
                        break;
                    case "FirePit":
                        FireEmitter fireEmitter = new FireEmitter(obj.X, obj.Y, obj.Rotation);
                        AddChild(fireEmitter);
                        fireChannel = new Sound("BoxImpact.wav").Play();
                        break;
                    case "Teleport":
                        TeleportingTile teleportTile = new TeleportingTile(obj.GetIntProperty("PairIndex"), obj.X, obj.Y, obj.Rotation);
                        if (pairIndexedTeleportTile)
                        {
                            pairIndexedTeleportTile = false;
                        }
                        else
                        {
                            teleportTile.isThisTheFirstTile = true;
                            pairIndexedTeleportTile = true;
                        }
                        AddChild(teleportTile);
                        break;
                    case "Platform":
                        ButtonPlatform platform = new ButtonPlatform(obj.GetIntProperty("PairIndex"), obj.X, obj.Y, obj.Rotation);
                        AddChild(platform);
                        break;
                    case "PlatformButton":
                        ButtonObject platformButton = new ButtonObject("Platform", obj.GetIntProperty("PairIndex"), obj.X, obj.Y, obj.Rotation);
                        AddChild(platformButton);
                        break;
                    default:
                        break;
                }
            }
            foreach (GameObject obj in GetChildren())
            {
                if (obj is ButtonObject)
                {
                    (obj as ButtonObject).CheckWallPair();
                }
                if (obj is TeleportingTile)
                {
                    (obj as TeleportingTile).CheckTeleportPair();
                }
            }
        }

        private void SetUpBackground()
        {
            string backgroundImage = "";
            if (mainGame.currentLevelIndex == 0)
            {
                backgroundImage = "Level0Background.png";
            }
            else if (mainGame.currentLevelIndex == 1)
            {
                backgroundImage = "Level1Background.png";
            }
            else
            {
                backgroundImage = "Level2Background.png";
            }
            Sprite background = new Sprite(backgroundImage, false, false);
            background.x -= game.width / 2;
            background.y -= game.height / 2;
            levelBackground = background;
        }
    }
}
