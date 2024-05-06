using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GXPEngine
{
    class EntityManager : GameObject
    {
		//Enemy spawning
		private float enemySpawnInterval = 0.75f; //Starting value is 0.5f, goes down by 0.015 each wave, minimum value is 0.05
		public float enemySpawnTime = 0.75f;
		public bool shouldSpawnEnemies = true;

		//Health spawning
		public float healthSpawnInterval = 15; //Default value is 15, goes down to 5 during a break
		public float healthSpawnTime = 15;
		
		private void Update()
        {
			HandleEnemySpawning();
			HandleHealthSpawning();
		}

		private void HandleEnemySpawning()
		{
			if (shouldSpawnEnemies)
            {
				enemySpawnInterval -= 0.0175f;
				if (enemySpawnInterval <= 0)
				{
					enemySpawnInterval = enemySpawnTime;
					Enemy enemy = new Enemy();
					AddChild(enemy);
				}
			}			
		}

		private void HandleHealthSpawning()
        {
			healthSpawnInterval -= 0.0175f;
			if (healthSpawnInterval <= 0)
			{
				healthSpawnInterval = healthSpawnTime;
				HealthPickup healthPickup = new HealthPickup();
				AddChild(healthPickup);
			}
		}

		public void DestroyAllEnemies()
		{
			for (int i = 0; i < GetChildCount(); i++)
			{
				Enemy currentChild = GetChildren()[i] as Enemy;
				if (currentChild != null)
                {
					currentChild.Die();
				}				
			}
		}

		public void DestroyPowerups()
        {
			for (int i = 0; i < GetChildCount(); i++)
			{
				Powerup currentChild = GetChildren()[i] as Powerup;
				if (currentChild != null)
                {
					currentChild.LateDestroy();
				}				
			}
        }
	}
}