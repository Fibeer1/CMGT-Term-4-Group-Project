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
        private int levelIndex;

        public Level(int index) : base("MapEmpty.png", false, false)
        {
            Map levelData = MapParser.ReadMap("Level " + index + ".tmx");
            SpawnObjects(levelData);
            SpawnTiles(levelData);
            camera = new Camera(0, 0, game.width, game.height);
            camera.SetXY(player.position.x, player.position.y);
            game.AddChild(camera);
            player.camera = camera;
            levelIndex = index;
            SetScaleXY(1.1f, 1.1f);

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
                        string tilesetFile = "DefaultTiles.png";
                        CollisionTile tile = new CollisionTile(tilesetFile, 3, 3);
                        tile.SetFrame(tileNumber - 1);
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
                        CollectableStar star = new CollectableStar(obj.X, obj.Y);
                        AddChild(star);
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
                        break;
                    case "Teleport":
                        TeleportingTile teleportTile = new TeleportingTile(obj.GetIntProperty("PairIndex"), obj.X, obj.Y);
                        if (levelIndex >= 2)
                        {
                            teleportTile.shouldTeleportPlayer = true;
                        }
                        if (levelIndex >= 2)
                        {
                            teleportTile.shouldTeleportPlayer = true;
                        }
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
    }
}
