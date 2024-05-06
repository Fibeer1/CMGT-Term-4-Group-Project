using System;
using GXPEngine;	// For Mathf

public struct Vec2 
{
	public float x;
	public float y;

	public Vec2 (float pX = 0, float pY = 0) 
	{
		x = pX;
		y = pY;
	}
	public Vec2(GXPEngine.Core.Vector2 vec)
	{
		x = vec.x;
		y = vec.y;
	}

	public override string ToString () 
	{
		return String.Format ("({0},{1})", x, y);
	}

	public void SetXY(float pX, float pY) 
	{
		x = pX;
		y = pY;
	}

	public float Length()
	{
		return Mathf.Sqrt(x * x + y * y);
	}
	public void Normalize()
	{
		if (x != 0 || y != 0)
		{
			float length = Length();
			x /= length;
			y /= length;
		}
	}
	public Vec2 Normalized()
	{
		return new Vec2(x / Length(), y / Length());
	}

	public float Distance(Vec2 b)
    {
		Vec2 distanceVec = b - this;
		float distance = distanceVec.Length();
		return distance;
    }

	public static Vec2 Lerp(Vec2 a, Vec2 b, float t)
	{
		return a + (b - a) * t;
	}
	public static Vec2 Inertia(Vec2 a, Vec2 b, float aPercent, float bPercent)
	{
		//a is the previous velocity
		//b is the current velocity
		//aPercent is the percentage of the previous velocity
		//bPercent is the percentage of the current velocity
		return a * aPercent + b * bPercent;
	}

	public static float Deg2Rad(float degrees)
	{
		return Mathf.PI / 180 * degrees;
	}

	public static float Rad2Deg(float radians)
	{
		return 180 / Mathf.PI * radians;
	}

	public static Vec2 GetUnitVectorDeg(float angle)
	{
		angle = Deg2Rad(angle);
		Vec2 unitVector = new Vec2(Mathf.Cos(angle), Mathf.Sin(angle));
		return unitVector;
	}

	public static Vec2 GetUnitVectorRad(float angle)
	{
		Vec2 unitVector = new Vec2(Mathf.Cos(angle), Mathf.Sin(angle));
		return unitVector;
	}

	public static Vec2 RandomUnitVector()
	{
		Vec2 unitVector = new Vec2(Mathf.Cos(Utils.Random(0, 361)), Mathf.Sin(Utils.Random(0, 361)));
		return unitVector;
	}

	public float GetAngleDegrees()
	{
		float angle = Mathf.Atan2(y, x);
		angle = Rad2Deg(angle);
		return angle;
	}

	public float GetAngleRadians()
	{
		float angle = Mathf.Atan2(y, x);
		return angle;
	}

	public void SetAngleDegrees(float angle)
	{
		angle = Deg2Rad(angle);
		Vec2 currentVecX = new Vec2(x, 0);
		Vec2 currentVecY = new Vec2(0, y);
		float angleCos = Mathf.Cos(angle);
		float angleSin = Mathf.Sin(angle);
		currentVecX.x = x * angleCos;
		currentVecX.y = x * angleSin;
		currentVecY.x = -y * angleSin;
		currentVecY.y = y * angleCos;
		x = currentVecX.x + currentVecY.x;
		y = currentVecX.y + currentVecY.y;
	}

	public void SetAngleRadians(float angle)
	{
		Vec2 currentVecX = new Vec2(x, 0);
		Vec2 currentVecY = new Vec2(0, y);
		float angleCos = Mathf.Cos(angle);
		float angleSin = Mathf.Sin(angle);
		currentVecX.x = x * angleCos;
		currentVecX.y = x * angleSin;
		currentVecY.x = -y * angleSin;
		currentVecY.y = y * angleCos;
		x = currentVecX.x + currentVecY.x;
		y = currentVecX.y + currentVecY.y;
	}

	public void RotateDegrees(float angle)
	{
		Vec2 currentVecX = new Vec2(x, 0);
		Vec2 currentVecY = new Vec2(0, y);
		float angleCos = Mathf.Cos(angle);
		float angleSin = Mathf.Sin(angle);
		currentVecX.x = x * angleCos;
		currentVecX.y = x * angleSin;
		currentVecY.x = -y * angleSin;
		currentVecY.y = y * angleCos;
		x += (currentVecX.x + currentVecY.x);
		y += (currentVecX.y + currentVecY.y);
	}

	public void RotateRadians(float angle)
	{
		angle = Rad2Deg(angle);
		Vec2 currentVecX = new Vec2(x, 0);
		Vec2 currentVecY = new Vec2(0, y);
		float angleCos = Mathf.Cos(angle);
		float angleSin = Mathf.Sin(angle);
		currentVecX.x = x * angleCos;
		currentVecX.y = x * angleSin;
		currentVecY.x = -y * angleSin;
		currentVecY.y = y * angleCos;
		x += (currentVecX.x + currentVecY.x);
		y += (currentVecX.y + currentVecY.y);
	}

	public void RotateAroundDegrees(Vec2 point, float angle)
	{
		Vec2 diffVec = new Vec2(x - point.x, y - point.y);
		float angleCos = Mathf.Cos(angle);
		float angleSin = Mathf.Sin(angle);
		float newX = diffVec.x * angleCos - diffVec.y * angleSin;
		float newY = diffVec.y * angleCos + diffVec.x * angleSin;
		Vec2 newVec = new Vec2(newX, newY);
		newVec += point;
		x = newVec.x;
		y = newVec.y;
	}

	public void RotateAroundRadians(Vec2 point, float angle)
	{
		angle = Rad2Deg(angle);
		Vec2 diffVec = new Vec2(x - point.x, y - point.y);
		float angleCos = Mathf.Cos(angle);
		float angleSin = Mathf.Sin(angle);
		float newX = diffVec.x * angleCos - diffVec.y * angleSin;
		float newY = diffVec.y * angleCos + diffVec.x * angleSin;
		Vec2 newVec = new Vec2(newX, newY);
		newVec += point;
		x = newVec.x;
		y = newVec.y;
	}

	public Vec2 Normal()
	{
		Vec2 normal = new Vec2(-y, x);
		normal.Normalize();
		return normal;
	}
	public float Dot(Vec2 b)
	{
		float dotProduct = x * b.x + y * b.y;
		return dotProduct;
	}
	public float ScalarProjection(Vec2 b)
	{
		float projection = Dot(b.Normalized());
		return projection;
	}

	public Vec2 VectorProjection(Vec2 b)
	{
		float projection = ScalarProjection(b);
		Vec2 projectionVec = projection * b.Normalized();
		return projectionVec;
	}

	public void Reflect(Vec2 surfaceNormal, float pBounciness = 1)
	{
		Vec2 inVec = this;
		Vec2 outVec = inVec - (1 + pBounciness) * inVec.Dot(surfaceNormal) * surfaceNormal;
		this = outVec;
	}

	public static Vec2 operator +(Vec2 left, Vec2 right) {
		return new Vec2 (left.x + right.x, left.y + right.y);
	}

	public static Vec2 operator -(Vec2 left, Vec2 right) {
		return new Vec2 (left.x - right.x, left.y - right.y);
	}

	public static Vec2 operator *(Vec2 v, float scalar) {
		return new Vec2 (v.x * scalar, v.y * scalar);
	}

	public static Vec2 operator *(float scalar, Vec2 v) {
		return new Vec2 (v.x * scalar, v.y * scalar);
	}

	public static Vec2 operator /(Vec2 v, float scalar) {
		return new Vec2 (v.x / scalar, v.y / scalar);
	}
}

