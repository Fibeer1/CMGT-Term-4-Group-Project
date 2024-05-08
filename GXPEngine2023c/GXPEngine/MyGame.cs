using System;                                   
using GXPEngine;                                
using System.Drawing;                           
using System.Collections.Generic;
using System.Linq;

public class MyGame : Game 
{
	Player player;
	HUD hud;
	SoundChannel audioSource;
	Level level;
	LineSegment[] mapBorders = new LineSegment[8];
	public Sprite[] staticBlocks;

	public MyGame() : base(1280, 720, false, false)     // Create a window that's 1280x720 and NOT fullscreen
	{
		targetFps = 60;
		Menu menu = new Menu("Main Menu");
		AddChild(menu);
		HandleUnitTests();
	}
	public void TestLevel()
    {
		SpawnBlocks();
		player = new Player();
		player.SetPosition(new Vec2(Utils.Random(0, width), 0));
		game.AddChild(player);
		player.SetSpawnPoint();
	}

	void SpawnBlocks(int number = 50, int seed = 0, float staticBlockScaleX = 1, float staticBlockScaleY = 1)
	{
		List<Sprite> blocks = new List<Sprite>();

		Random rnd = new Random(seed); // fixed seed
		for (int i = 0; i < number; i++)
		{
			// Create a sprite with random position, rotation and scale:
			Sprite staticBlock = new Sprite("HealthPickup.png");
			staticBlock.SetOrigin(staticBlock.width / 2, staticBlock.height / 2);
			staticBlock.rotation = rnd.Next(0, 180);
			staticBlock.x = rnd.Next(0, width);
			staticBlock.y = rnd.Next(0, height);
			staticBlock.scaleX = ((float)rnd.NextDouble() * 2 + 0.5f) * staticBlockScaleX;
			staticBlock.scaleY = ((float)rnd.NextDouble() * 2 + 0.5f) * staticBlockScaleY;
			AddChild(staticBlock);
			blocks.Add(staticBlock);
		}

		Sprite leftWall = new Sprite("HealthPickup.png");
		leftWall.scaleY = 2 * height / leftWall.height;
		AddChild(leftWall);
		blocks.Add(leftWall);

		Sprite rightWall = new Sprite("HealthPickup.png");
		rightWall.scaleY = 2 * height / rightWall.height;
		rightWall.x = width - rightWall.width;
		AddChild(rightWall);
		blocks.Add(rightWall);

		staticBlocks = blocks.ToArray();
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
		AddChild(level);
	}

	private void DestroyChildren()
	{
		List<GameObject> children = GetChildren();
		foreach (GameObject child in children)
		{
			child.LateDestroy();
		}
	}

	public void EndGame()
    {
		audioSource.Stop();
		Menu menu = new Menu("Game Over");
		for (int i = 0; i < game.GetChildCount(); i++)
		{
			GameObject child = game.GetChildren()[i];
			child.LateRemove();
			child.LateDestroy();
		}		
		AddChild(menu);
	}

	//private void AddMapBorders()
 //   {
	//	mapBorders[0] = new NLineSegment(new Vec2(25, 25), new Vec2(game.width / 2, 3), 0xff00ff00, 3);
	//	mapBorders[1] = new NLineSegment(new Vec2(game.width / 2, 3), new Vec2(game.width - 25, 25), 0xff00ff00, 3);
	//	mapBorders[2] = new NLineSegment(new Vec2(game.width - 25, 25), new Vec2(game.width - 3, game.height / 2), 0xff00ff00, 3);
	//	mapBorders[3] = new NLineSegment(new Vec2(game.width - 3, game.height / 2), new Vec2(game.width - 25, game.height - 25), 0xff00ff00, 3);
	//	mapBorders[4] = new NLineSegment(new Vec2(game.width - 25, game.height - 25), new Vec2(game.width / 2, game.height - 3), 0xff00ff00, 3);
	//	mapBorders[5] = new NLineSegment(new Vec2(game.width / 2, game.height - 3), new Vec2(25, game.height - 25), 0xff00ff00, 3);
	//	mapBorders[6] = new NLineSegment(new Vec2(25, game.height - 25), new Vec2(3, game.height / 2), 0xff00ff00, 3);
	//	mapBorders[7] = new NLineSegment(new Vec2(3, game.height / 2), new Vec2(25, 25), 0xff00ff00, 3);
	//	AddChild(mapBorders[0]);
	//	AddChild(mapBorders[1]);
	//	AddChild(mapBorders[2]);
	//	AddChild(mapBorders[3]);
	//	AddChild(mapBorders[4]);
	//	AddChild(mapBorders[5]);
	//	AddChild(mapBorders[6]);
	//	AddChild(mapBorders[7]);
	//}

	private void HandleUnitTests()
    {
		// Length
		Vec2 lengthVec = new Vec2(4, 3);
		float lengthResult = lengthVec.Length();
		Console.WriteLine("Length ok ?: " + (lengthResult == 5));

		// Normalized
		Vec2 normVec = new Vec2(4, 3);
		Vec2 normalizedVec = normVec.Normalized();
		Console.WriteLine("Normalization ok ?: " + (normalizedVec.x == 0.8f && normalizedVec.y == 0.6f));

		// Distance
		Vec2 distA = new Vec2(6, 4);
		Vec2 distB = new Vec2(3, 0);
		float distance = distA.Distance(distB);
		Console.WriteLine("Distance ok ?: " + (distance == 5));

		// Lerp
		Vec2 lerpVec1 = new Vec2(5, 7);
		Vec2 lerpVec2 = new Vec2(6, 8);
		Vec2 lerpResult = Vec2.Lerp(lerpVec1, lerpVec2, 0.5f);
		Console.WriteLine("Linear Interpolation ok ?: " + (lerpResult.x == 5.5f && lerpResult.y == 7.5f));

		// Inertia
		Vec2 inertiaVec1 = new Vec2(4, 7);
		Vec2 inertiaVec2 = new Vec2(6, 10);
		Vec2 inertiaResult = Vec2.Inertia(inertiaVec1, inertiaVec2, 0.99f, 0.01f);
		Console.WriteLine("Inertia ok ?: " + (inertiaResult.x == 4.02f && inertiaResult.y == 7.03f));

		// Deg2Rad
		float deg2RadDegrees = 45;
		float deg2RadRadians = Vec2.Deg2Rad(deg2RadDegrees);
		Console.WriteLine("Deg2Rad ok ?: " + (deg2RadRadians == 0.785398163397f));

		// Rad2Deg		
		float Rad2DegRadians = 1.23f;
		float Rad2DegDegrees = Vec2.Rad2Deg(Rad2DegRadians);
		Console.WriteLine("Rad2Deg ok ?: " + (Rad2DegDegrees == 70.4738088f));

		// Unit Vector Degrees
		float unitVecDegAngle = 70;
		Vec2 unitVecDegResult = Vec2.GetUnitVectorDeg(unitVecDegAngle);
		Console.WriteLine("Unit Vector from degrees ok ?: " + (unitVecDegResult.x == 0.342020154f && unitVecDegResult.y == 0.9396926f));

		// Unit Vector Radians
		float unitVecRadAngle = 1.25f;
		Vec2 unitVecRadResult = Vec2.GetUnitVectorRad(unitVecRadAngle);
		Console.WriteLine("Unit Vector from radians ok ?: " + (unitVecRadResult.x == 0.315322369f && unitVecRadResult.y == 0.9489846f));

		// Get Angle degrees
		Vec2 angleDegVec = new Vec2(4, 3);
		float angleDeg = angleDegVec.GetAngleDegrees();
		Console.WriteLine("Get Angle Degrees ok ?: " + (angleDeg == 36.86989764584402f));

		// Get Angle radians
		Vec2 angleRadVec = new Vec2(4, 3);
		float angleRad = angleRadVec.GetAngleRadians();
		Console.WriteLine("Get Angle Radians ok ?: " + (angleRad == 0.6435011087932844f));

		// Set Angle degrees
		Vec2 angleDegSetVec = new Vec2(3, 4);
		float degSetAngle = 30;
		angleDegSetVec.SetAngleDegrees(degSetAngle);
		Console.WriteLine("Set Angle Degrees ok ?: " + (angleDegSetVec.x == 0.5980761f && angleDegSetVec.y == 4.964102f));

		// Set Angle radians
		Vec2 angleRadSetVec = new Vec2(3, 4);
		float radSetAngle = 0.5235987756f;
		angleRadSetVec.SetAngleRadians(radSetAngle);
		Console.WriteLine("Set Angle Radians ok ?: " + (angleRadSetVec.x == 0.5980761f && angleRadSetVec.y == 4.964102f));

		// Rotate degrees
		Vec2 rotateDegVec = new Vec2(4, 3);
		float degRotateAngle = 25;
		rotateDegVec.RotateDegrees(degRotateAngle);
		Console.WriteLine("Rotate Degrees ok ?: " + (rotateDegVec.x == 8.361867f && rotateDegVec.y == 5.44420147f));

		// Rotate radians
		Vec2 rotateRadVec = new Vec2(4, 3);
		float radRotateAngle = 1.5f;
		rotateRadVec.RotateRadians(radRotateAngle);
		Console.WriteLine("Rotate Radians ok ?: " + (rotateRadVec.x == 4.96082449f && rotateRadVec.y == -1.90681338f));

		// Rotate around degrees
		Vec2 rotateAroundDegVec = new Vec2(5, 7);
		Vec2 rotateAroundDegPoint = new Vec2(3, 4);
		float rotateAroundDegAngle = 30;
		rotateAroundDegVec.RotateAroundDegrees(rotateAroundDegPoint, rotateAroundDegAngle);
		Console.WriteLine("Rotate Around Degrees ok ?: " + (rotateAroundDegVec.x == 6.272598f && rotateAroundDegVec.y == 2.486691f));

		// Rotate around radians
		Vec2 rotateAroundRadVec = new Vec2(5, 7);
		Vec2 rotateAroundRadPoint = new Vec2(3, 4);
		float rotateAroundRadAngle = 2.5f;
		rotateAroundRadVec.RotateAroundRadians(rotateAroundRadPoint, rotateAroundRadAngle);
		Console.WriteLine("Rotate Around Radians ok ?: " + (rotateAroundRadVec.x == 6.45393658f && rotateAroundRadVec.y == 2.9654355f));

		// Normal
		Vec2 normalVec = new Vec2(3, 9);
		Vec2 normalOfNormalVec = normalVec.Normal();
		Console.WriteLine("Normal ok ?: " + (normalOfNormalVec.x == -0.9486833f && normalOfNormalVec.y == 0.316227764f));

		// Dot Scalar
		Vec2 dotAVec = new Vec2(4, 3);
		Vec2 dotBVec = new Vec2(5, 6);
		float dotProductScalar = dotAVec.Dot(dotBVec);
		Console.WriteLine("Dot Product Scalar ok ?: " + (dotProductScalar == 38));

		// Dot Projection
		Vec2 dotProjectionAVec = new Vec2(4, 3);
		Vec2 dotProjectionBVec = new Vec2(5, 6);
		float dotProductProjection = dotProjectionAVec.ScalarProjection(dotProjectionBVec);
		Console.WriteLine("Dot Product Projection ok ?: " + (dotProductProjection == 4.86540127f));

		// Vector Projection
		Vec2 vectorProjectionAVec = new Vec2(4, 3);
		Vec2 vectorProjectionBVec = new Vec2(5, 6);
		Vec2 vectorProjection = vectorProjectionAVec.VectorProjection(vectorProjectionBVec);
		Console.WriteLine("Vector Projection ok ?: " + (vectorProjection.x == 3.114754f && vectorProjection.y == 3.73770475f));

		// Reflect
		Vec2 reflectVec = new Vec2(5, 2);
		Vec2 surfaceVec = new Vec2(0, 2);
		Vec2 surfaceVecNormal = surfaceVec.Normal();
		float bouncinessScalar = 0.5f;
		reflectVec.Reflect(surfaceVecNormal, bouncinessScalar);
		Console.WriteLine("Reflect ok ?: " + (reflectVec.x ==  -2.5f && reflectVec.y == 2f));

		// + operator
		Vec2 plusVector1 = new Vec2(4, 8);
		Vec2 plusVector2 = new Vec2(3, -4);
		Vec2 plusResult = plusVector1 + plusVector2;
		Console.WriteLine("Addition ok ?: " +
		(plusResult.x == 7 && plusResult.y == 4 &&
		plusVector1.x == 4 && plusVector1.y == 8 &&
		plusVector2.x == 3 && plusVector2.y == -4));

		// - operator
		Vec2 minusVector1 = new Vec2(-3, -5);
		Vec2 minusVector2 = new Vec2(5, 5);
		Vec2 minusResult = minusVector2 - minusVector1;
		Console.WriteLine("Subtraction ok ?: " +
		(minusResult.x == 8 && minusResult.y == 10 &&
		minusVector1.x == -3 && minusVector1.y == -5 &&
		minusVector2.x == 5 && minusVector2.y == 5));

		// * operator right
		Vec2 multVectorRight = new Vec2(-7, 4);
		int multiplierRight = 3;
		Vec2 multResultRight = multVectorRight * multiplierRight;
		Console.WriteLine("Multiplication right ok ?: " +
		(multResultRight.x == -21 && multResultRight.y == 12 && multVectorRight.x == -7 && multVectorRight.y == 4));

		// * operator left
		Vec2 multVectorLeft = new Vec2(10, -3);
		int multiplierLeft = 5;
		Vec2 multResultLeft = multiplierLeft * multVectorLeft;
		Console.WriteLine("Multiplication right ok ?: " +
		(multResultLeft.x == 50 && multResultLeft.y == -15 && multVectorLeft.x == 10 && multVectorLeft.y == -3));

		// / operator
		Vec2 divVector = new Vec2(10, -3);
		int divScalar = 5;
		Vec2 divResult = divVector / divScalar;
		Console.WriteLine("Division ok ?: " +
		(divResult.x == 2 && divResult.y == -0.6f));
	}

	static void Main()
	{
		new MyGame().Start();
	}
}