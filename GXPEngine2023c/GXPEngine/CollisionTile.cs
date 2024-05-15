using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GXPEngine
{
    class CollisionTile : AnimationSprite
    {
        public bool shouldCollide = true;
        public CollisionTile(string filename, int cols, int rows, int frame, bool keepInCache, bool shouldHaveCollider) : base (filename, cols, rows, frame, false, shouldHaveCollider)
        {

        }
    }
}
