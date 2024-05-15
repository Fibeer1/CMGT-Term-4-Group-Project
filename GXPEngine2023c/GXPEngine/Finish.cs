using GXPEngine.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GXPEngine
{
    class Finish : AnimationSprite
    {
        public Finish() : base("FinishPortal.png", 3, 1)
        {
            SetOrigin(width / 2, height / 2);
        }

        private void Update()
        {
            Animate(0.125f);
        }
    }
}
