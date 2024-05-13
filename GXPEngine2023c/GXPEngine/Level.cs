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
            //camera.SetScaleXY(1.5f, 1.5f);
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
                        string tilesetFile = "NewTutorialTileset.png";
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
            int wallPairIndex = 0;
            bool pairIndexedWall = false;
            int teleportTilePairIndex = 0;
            bool pairIndexedTeleportTile = false;
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
                    case "Collectable":
                        CollectableStar star = new CollectableStar(obj.X, obj.Y);
                        AddChild(star);
                        break;
                    case "MovableWall":
                        ButtonWall wall = new ButtonWall(wallPairIndex, obj.X, obj.Y);
                        if (!pairIndexedWall)
                        {
                            pairIndexedWall = true;
                        }
                        AddChild(wall);
                        break;
                    case "Button":
                        ButtonObject button = new ButtonObject(wallPairIndex, obj.X, obj.Y, obj.Rotation);
                        if (pairIndexedWall)
                        {
                            wallPairIndex++;
                            pairIndexedWall = false;
                        }
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
                        TeleportingTile teleportTile = new TeleportingTile(teleportTilePairIndex, obj.X, obj.Y);
                        if (levelIndex >= 2)
                        {
                            teleportTile.shouldTeleportPlayer = true;
                        }
                        if (pairIndexedTeleportTile)
                        {
                            teleportTilePairIndex++;
                            pairIndexedTeleportTile = false;
                        }
                        else
                        {
                            teleportTile.isThisTheFirstTile = true;
                            pairIndexedTeleportTile = true;
                        }                        
                        AddChild(teleportTile);
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
