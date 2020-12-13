using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PHL.Common.Utility;

public class Actor_Enemy : ActorBehaviour
{
    private Actor_Health _health;
    private Actor_EnemyAnimation _enemyAnimation;
    private Actor_Navigation _navigation;
    [SerializeField] private GameObject _modelPrefab;
    [SerializeField] private Material _enemyMaterial;
    [SerializeField] private GameObject _thickBloodExplosionPrefab;
    [SerializeField] private GameObject _thinBloodExplosionPrefab;
    [SerializeField] private GameObject _attackExplosion;
    [SerializeField] private BoxCollider _attackHurtbox;
    [SerializeField]private float _damage;
    private SkinnedMeshRenderer[] _enemyModelRenderers;
    private Actor _player;
    public LayerMask playerLayer;
    public bool isShootable = false;
    public override void AssignActorReferences(Actor newActor)
    {
        base.AssignActorReferences(newActor);
        _health = GetBehaviour<Actor_Health>();
        _enemyAnimation = GetBehaviour<Actor_EnemyAnimation>();
        _navigation = GetBehaviour<Actor_Navigation>();
    }
    public override void InitializeBehaviour(Actor newActor)
    {
        base.InitializeBehaviour(newActor);
        _enemyModelRenderers = _modelPrefab.GetComponentsInChildren<SkinnedMeshRenderer>();
        _health.instaKilledEvent.AddListener(InstaKill);
        _health.deathEvent.AddListener(Death);
        _navigation.attackRangeEvent.AddListener(Attack);
    }
    public override void UpdateBehaviour()
    {
        base.UpdateBehaviour();
    }
    public void MaterialSwap()
    {
        foreach (SkinnedMeshRenderer renderer in _enemyModelRenderers)
        {
            renderer.material = _enemyMaterial;
            renderer.gameObject.layer = 10;
        }
    }
    private void Attack()
    {
        Collider[] overlaps = _attackHurtbox.OverlapBox(playerLayer);
        HashSet<HitboxRoot> rootsHit = new HashSet<HitboxRoot>();
        foreach (Collider overlap in overlaps)
        {
            _player = overlap.GetComponentInParent<Actor>();
            HitboxCollider hitbox = overlap.transform.GetComponent<HitboxCollider>();
            DamageType damageType = DamageType.Base;
            if (hitbox != null)
            {
                if (!rootsHit.Contains(hitbox.root))
                {
                    rootsHit.Add(hitbox.root);
                    if (hitbox.gameObject.tag == "Weakpoint")
                    {
                        damageType = DamageType.Weakpoint;
                    }
                    hitbox.Hit(new DamageInfo()
                    {
                        amount = _damage,
                        damageTeam = DamageTeam.Enemy,
                        source = actor,
                        damageType = damageType
                    });
                }
            }
        }
        StartCoroutine(AttackRoutine());

    }
    private IEnumerator AttackRoutine()
    {
        yield return new WaitForSeconds(1.5f);
        ParticleSystem ps = _attackExplosion.GetComponent<ParticleSystem>();
        if (!ps.isPlaying)
            ps.Play();
        Destroy(actor.gameObject, 0.5f);
    }
    public void InstaKill()
    {
        Instantiate(_thickBloodExplosionPrefab, transform.position, Quaternion.identity);
        ParticleSystem ps = _thickBloodExplosionPrefab.GetComponent<ParticleSystem>();
        var shape = ps.shape;
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.SkinnedMeshRenderer;
        shape.skinnedMeshRenderer = _enemyModelRenderers[0];
        if (!ps.isPlaying)
            ps.Play();
        actor.gameObject.SetActive(false);
    }
    public void Death()
    {
        _enemyAnimation.PlayDead();
        Instantiate(_thinBloodExplosionPrefab, transform.position, Quaternion.identity);
        ParticleSystem ps = _thinBloodExplosionPrefab.GetComponent<ParticleSystem>();
        var shape = ps.shape;
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.SkinnedMeshRenderer;
        shape.skinnedMeshRenderer = _enemyModelRenderers[0];
        if (!ps.isPlaying)
            ps.Play();

        Destroy(actor.gameObject, 2f);
    }

}
