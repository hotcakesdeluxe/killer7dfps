using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxCollider : MonoBehaviour
{
    [SerializeField] private HitboxRoot _root;
    public HitboxRoot root => _root;

    [SerializeField] private float _damageMultiplier = 1f;

    public void Initialize(HitboxRoot newRoot)
    {
        _root = newRoot;
    }

    public void Hit(DamageInfo damageInfo)
    {
        damageInfo.amount *= _damageMultiplier;
        root.Hit(damageInfo);
    }
}