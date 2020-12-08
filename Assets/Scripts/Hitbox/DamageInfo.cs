﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PHL.Common.Utility;

public enum DamageTeam
{
    Neutral,
    Player,
    Enemy,
    Any
}

public enum DamageType
{
    Base
}

[System.Serializable]
public struct DamageInfo
{
    public Actor source;
    public float amount;
    public float hitStopTime;
    public Vector3 force;
    public DamageTeam damageTeam;
    public DamageType damageType;

}

public class DamageInfoEvent : SecureEvent<DamageInfo>{}

