using PHL.Common.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxRoot : MonoBehaviour
{
    [SerializeField] private Actor _parentActor;
    [SerializeField] private DamageTeam _damageTeam;

    public Actor parentActor => _parentActor;
    public DamageTeam damageTeam => _damageTeam;
    public DamageInfoEvent hitEvent { get; private set; } = new DamageInfoEvent();
    
    public void Hit(DamageInfo damageInfo)
    {
        if (CanDamageTeam(damageInfo.damageTeam, _damageTeam))
        {
            hitEvent.Invoke(damageInfo);
        }
    }

    public static bool CanDamageTeam(DamageTeam team1, DamageTeam team2)
    {
        if(team1 == DamageTeam.Any)
        {
            return true;
        }

        if(team1 == team2)
        {
            return false;
        }

        return true;
    }

    public void AssignDamageTeam(DamageTeam newDamageTeam)
    {
        _damageTeam = newDamageTeam;
    }
}
