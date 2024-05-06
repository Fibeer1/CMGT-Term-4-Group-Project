using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GXPEngine
{
    class RamParticle : Sprite
    {
        //General variables
        private float lifeTime = 0.5f;
        private Player player;

        //Movement variable
        private Vec2 _position;
        
        public RamParticle() : base("Ship.png")
        {
            SetOrigin(width / 2, height / 2);
            player = game.FindObjectOfType<Player>();
            _position = player.position;
            x = _position.x;
            y = _position.y;
            rotation = player.rotation;
            alpha = 0.25f;
            color = player.color;
        }

        private void Update()
        {
            lifeTime -= 0.025f;
            alpha -= 0.01f;
            if (lifeTime <= 0)
            {
                LateRemove();
                LateDestroy();
            }
        }
    }
}
