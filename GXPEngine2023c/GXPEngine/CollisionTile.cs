using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GXPEngine
{
    class CollisionTile : AnimationSprite
    {
        public CollisionTile(string filename, int cols, int rows, int tileFrame) : base (filename, cols, rows, tileFrame, false, false)
        {

        }
    }
}
