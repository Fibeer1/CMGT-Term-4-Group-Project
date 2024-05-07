using GXPEngine.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GXPEngine
{
    class Finish : Sprite
    {


        public Finish() : base("LevelPortal.png", false, true)
        {
            SetOrigin(width / 2, height / 2);
            SetScaleXY(2, 2);
        }
    }
}
