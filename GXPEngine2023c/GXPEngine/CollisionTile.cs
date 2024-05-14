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
        public CollisionTile(string filename, int cols, int rows) : base (filename, cols, rows)
        {

        }
    }
}
