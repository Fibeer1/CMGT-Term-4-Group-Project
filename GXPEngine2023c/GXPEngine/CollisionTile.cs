using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GXPEngine
{
    class CollisionTile : AnimationSprite
    {
        public string type; //Can be Death
        public CollisionTile(string filename, int cols, int rows) : base (filename, cols, rows)
        {

        }
    }
}
