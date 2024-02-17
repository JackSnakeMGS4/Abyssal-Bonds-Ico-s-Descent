using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HelperMethods
{
    #region Math Helpers
    /// <summary>
    /// Returns the Z rotation (in Quaternions) needed to look in a given direction
    /// </summary>
    /// <param name="targetPos"></param>
    /// <param name="origin"></param>
    /// <returns></returns>
    public static Quaternion GetRotationToDir(Vector3 dir)
    {
        float ang = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        return Quaternion.AngleAxis(ang, Vector3.forward);
    }

    /// <summary>
    /// Returns a normalized direction from an origin to the targetted vector's position
    /// </summary>
    /// <param name="targetPos"></param>
    /// <param name="origin"></param>
    /// <returns></returns>
    public static Vector3 GetDirFromOriginNormalized(Vector3 targetPos, Vector3 origin)
    {
        return (targetPos - origin).normalized;
    }
    
    /// <summary>
    /// Returns direction from origin to targetted vector's position without normalizing it
    /// </summary>
    /// <param name="targetPos">The end position from the origin</param>
    /// <param name="origin">The position that should be treated as the origin of this vector</param>
    /// <returns></returns>
    public static Vector3 GetDirFromOrigin(Vector3 targetPos, Vector3 origin)
    {
        return targetPos - origin;
    }

    /// <summary>
    /// Return a random position inside a circle with an inner circle area's removed.
    /// </summary>
    /// <param name="origin"></param>
    /// <param name="innerCircleRadius">Determines the inner circle's area that should be ignored from point generation</param>
    /// <param name="outerCircleRadius">Determines the maximum area covered by the ring's outer circle</param>
    /// <returns></returns>
    public static Vector3 PickRandomPointInAnnulus(Vector3 origin, float innerCircleRadius, float outerCircleRadius)
    {
        var dir = (Vector3) (Random.insideUnitCircle * origin).normalized;
        var randomDist = Random.Range(innerCircleRadius, outerCircleRadius);
        var point = origin + (dir * randomDist);

        return point;
    }

    /// <summary>
    /// Takes a given vector and returns a new vector whose arc to the input vector is x degrees (based on Unit Circle with right being 0 degrees)
    /// </summary>
    /// <param name="angleDegrees">The arc (in degrees) between the input vector and the output vector. 
    /// I.E. 30f will output a vector that is offset by 30 degrees from the input vector (NOTE: )</param>
    /// <param name="dir">The vector denoting an entity's direction and degrees from 0. 
    /// I.E. A vector of (1,0) rests at 360 (or 0) degrees; if the angle offset is 30, the output vector will have a degree of 30.</param>
    /// <returns></returns>
    public static Vector2 VectorFromAngleAndDirection(float angleDegrees, Vector2 dir)
    {
        var angFromXCoord = Mathf.Round(Mathf.Acos(dir.x) * Mathf.Rad2Deg);
        var angFromYCoord = Mathf.Round(Mathf.Asin(dir.y) * Mathf.Rad2Deg);

        // Next line useful for visualizing out how this function works
        Debug.Log($"Degree using Cos X: {angFromXCoord}/using Sin Y: {angFromYCoord}");
        //Debug.DrawLine(visionOrigin.position, visionOrigin.position + new Vector3(Mathf.Cos(angFromXCoord * Mathf.Deg2Rad) * 5f,
        //    Mathf.Sin(angFromYCoord * Mathf.Deg2Rad) * 5f));

        if (dir.x < 0 && dir.y < 0)
        {
            return new Vector2(Mathf.Cos((360 - (angleDegrees + angFromXCoord)) * Mathf.Deg2Rad), Mathf.Sin((360 - (angleDegrees + angFromXCoord)) * Mathf.Deg2Rad));
        }

        if (dir.x > 0)
        {
            return new Vector2(Mathf.Cos((angleDegrees + angFromYCoord) * Mathf.Deg2Rad), Mathf.Sin((angleDegrees + angFromYCoord) * Mathf.Deg2Rad));
        }
        else if (dir.x < 0)
        {
            return new Vector2(Mathf.Cos((angleDegrees + angFromXCoord) * Mathf.Deg2Rad), Mathf.Sin((angleDegrees + angFromXCoord) * Mathf.Deg2Rad));
        }

        return Vector2.zero;
    }
    #endregion
}
