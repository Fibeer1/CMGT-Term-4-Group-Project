using System;                                   
using GXPEngine;                                
using System.Drawing;
using System.Collections.Generic;
using System.Linq;

class MyGame : Game {

	public int currentLevelIndex;
	public List<int> completedLevelIndices = new List<int>();
	
	public PlayerData playerData;
	public EnemyData enemyData;
	public HUDData hudData;

	public MyGame() : base(1366, 768, false, false)
	{
		playerData = new PlayerData();
		enemyData = new EnemyData();
		hudData = new HUDData();

		targetFps = 60;
		StartMenu("Main Menu");
		currentLevelIndex = Utils.Random(0, 5);
	}
	public void StartMenu(string menuType)
    {
		DestroyChildren();
		Menu menu = new Menu(menuType);
		AddChild(menu);
	}
	public void StartLevel(int levelIndex)
    {
		DestroyChildren();
		Level level = new Level(levelIndex);
		currentLevelIndex = levelIndex;
		AddChild(level);
	}

	private void DestroyChildren()
    {
		List<GameObject> children = GetChildren();
		foreach (GameObject child in children)
		{
			child.LateDestroy();
			if (child is Level)
            {
				Level level = child as Level;
				level.musicChannel.Stop();
            }
		}
	}
	static void Main()
	{
		new MyGame().Start();
	}
}