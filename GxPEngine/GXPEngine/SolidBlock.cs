using System;
using GXPEngine;
using Physics;


class SolidBlock : AnimationSprite {
	Collider myCollider;
	ColliderManager engine;

	public SolidBlock(Vec2 position, string filename, int cols, int rows) : base(filename, cols, rows) {
		SetFrame(17);
		SetOrigin(width/2, height/2);

		// Create collider, and register it (as solid / collision object):
		myCollider=new AABB(this, position, width/2, height/2);
		engine=ColliderManager.main;
		engine.AddSolidCollider(myCollider);

		x=position.x;
		y=position.y;
	}
	public void MoveColliderWithBlock()
    {
		myCollider.position = new Vec2(x, y);
    }

	protected override void OnDestroy() {
		// Remove the collider when the sprite is destroyed:
		engine.RemoveTriggerCollider(myCollider);
	}
}