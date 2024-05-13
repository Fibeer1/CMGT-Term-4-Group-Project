﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GXPEngine
{
    class FireEmitter : Sprite
    {
        private Vec2 position;
        private float fireEmitCD;
        private float fireEmitCDDuration = 3;
        bool shouldEmitFire = true;

        public FireEmitter(float xPos, float yPos, float pRotation) : base("Empty.png")
        {
            SetOrigin(width / 2, height / 2);
            position = new Vec2(xPos, yPos);
            rotation = pRotation;
            fireEmitCD = fireEmitCDDuration;
            UpdateScreenPosition();
        }

        private void UpdateScreenPosition()
        {
            x = position.x;
            y = position.y;
        }

        private void Update()
        {
            if (shouldEmitFire)
            {
                if (fireEmitCD < 0)
                {
                    EmitFire();
                }
                fireEmitCD -= 0.0175f;
            }            
        }

        private void EmitFire()
        {
            FireParticle fireParticle = new FireParticle(position, rotation);
            parent.LateAddChild(fireParticle);
            fireEmitCD = fireEmitCDDuration;
        }
    }
}
