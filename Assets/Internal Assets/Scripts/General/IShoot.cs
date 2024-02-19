using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IShoot
{
    public void Fire(Vector3 targetPos, bool isAiming);
    public void FireCharged(Vector3 targetPos, bool isAiming);
}