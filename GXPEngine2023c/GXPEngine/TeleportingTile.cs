using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GXPEngine
{
    class TeleportingTile : Sprite
    {
        public TeleportingTile pairTile;
        private int pairTileIndex;
        public bool isThisTheFirstTile = false;
        public Vec2 position;

        public TeleportingTile(int pairIndex, float xPos, float yPos, float pRotation) : base("TeleportTile.png")
        {
            SetOrigin(width / 2, height / 2);
            position = new Vec2(xPos, yPos);
            rotation = pRotation;
            pairTileIndex = pairIndex;
            UpdateScreenPosition();
        }

        private void UpdateScreenPosition()
        {
            x = position.x;
            y = position.y;
        }

        public void CheckTeleportPair()
        {
            if (!isThisTheFirstTile)
            {
                return;
            }
            List<GameObject> parentChildren = parent.GetChildren();
            foreach (GameObject gameObject in parentChildren)
            {
                if (gameObject is TeleportingTile && gameObject != this)
                {
                    if ((gameObject as TeleportingTile).pairTileIndex == pairTileIndex)
                    {
                        pairTile = gameObject as TeleportingTile;
                        pairTile.pairTile = this;
                    }
                }
            }
        }
    }
}
