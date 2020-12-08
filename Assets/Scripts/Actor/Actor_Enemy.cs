using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor_Enemy : ActorBehaviour
{
    [SerializeField]private GameObject _modelPrefab;
    [SerializeField]private Material _enemyMaterial;
    [SerializeField]private GameObject _bloodExplosionPrefab;
    private SkinnedMeshRenderer[] _enemyModelRenderers;
    public bool isShootable = false;
    public override void AssignActorReferences(Actor newActor)
    {
        base.AssignActorReferences(newActor);

    }
    public override void InitializeBehaviour(Actor newActor)
    {
        base.InitializeBehaviour(newActor);
        _enemyModelRenderers = _modelPrefab.GetComponentsInChildren<SkinnedMeshRenderer>();

    }
    public override void UpdateBehaviour()
    {
        base.UpdateBehaviour();
    }
    public void MaterialSwap()
    {
        foreach(SkinnedMeshRenderer renderer in _enemyModelRenderers)
        {
            renderer.material = _enemyMaterial;
        }
    }
    public void Death()
    {
        Instantiate(_bloodExplosionPrefab, transform.position, Quaternion.identity);
        ParticleSystem ps = _bloodExplosionPrefab.GetComponent<ParticleSystem>();
        var shape = ps.shape;
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.SkinnedMeshRenderer;
        shape.skinnedMeshRenderer = _enemyModelRenderers[0];
        if(!ps.isPlaying)
            ps.Play();
        actor.gameObject.SetActive(false);
    }
}
