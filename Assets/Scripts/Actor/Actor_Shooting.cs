using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PHL.Common.Utility;
public class Actor_Shooting : ActorBehaviour
{
    private Actor_PlayerInput _input;
    private Actor_Camera _camera;
    private Canvas _canvas;
    [SerializeField] private BoxCollider _scanZone;
    [SerializeField] private Graphic _reticleGraphic;
    private RectTransform _canvasTransform;
    [SerializeField] private RectTransform _cursorTransform;
    [SerializeField] private float _aimSpeed;
    private float minX;
    private float maxX;
    private float minY;
    private float maxY;
    public LayerMask enemyLayer;
    public override void AssignActorReferences(Actor newActor)
    {
        base.AssignActorReferences(newActor);
        _input = GetBehaviour<Actor_PlayerInput>();
        _camera = GetBehaviour<Actor_Camera>();
    }
    public override void InitializeBehaviour(Actor newActor)
    {
        base.InitializeBehaviour(newActor);
        _canvas = _reticleGraphic?.GetComponentInParent<Canvas>();
        _canvasTransform = _canvas.GetComponent<RectTransform>();
        GetMinMaxRect();
    }
    public override void UpdateBehaviour()
    {
        base.UpdateBehaviour();
        if (_input.isAiming)
        {
            MoveReticule();
            _reticleGraphic.CrossFadeAlpha(1, 0.1f, true);
        }
        else
        {
            _reticleGraphic.CrossFadeAlpha(0, 0.1f, true);
        }
    }
    public void Fire()
    {
        RaycastHit hit;
        Vector3 castDir = _cursorTransform.position - _camera.FirstPersonCam.transform.position;
        if (Physics.Raycast(_camera.FirstPersonCam.transform.position, castDir, out hit, Mathf.Infinity, enemyLayer))
        {
            Debug.DrawRay(_camera.FirstPersonCam.transform.position, castDir * hit.distance, Color.yellow);
            Debug.Log("Did Hit " + hit.transform);
            Actor enemy = hit.transform.gameObject.GetComponent<Actor>();
            if (enemy.GetBehaviour<Actor_Enemy>().isShootable)
            {
                enemy.GetBehaviour<Actor_Enemy>().Death();
            }
        }
        else
        {
            Debug.DrawRay(_camera.FirstPersonCam.transform.position, castDir * 1000, Color.white);
            Debug.Log("Did not Hit");
        }
    }
    private void MoveReticule()
    {
        Vector2 delta = new Vector2(_aimSpeed * _input.smoothInputAim.x * Time.deltaTime, _aimSpeed * _input.smoothInputAim.y * Time.deltaTime);
        Vector2 currentPosition = _cursorTransform.anchoredPosition;
        Vector2 newPosition = currentPosition + delta;
        if (_canvas != null)
        {
            // Clamp to canvas.
            //var pixelRect = _canvas.pixelRect;
            newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
            newPosition.y = Mathf.Clamp(newPosition.y, minY, maxY);
        }
        _cursorTransform.anchoredPosition = newPosition;
    }
    public void Scan()
    {
        Collider[] overlaps = _scanZone.OverlapBox(enemyLayer);
        foreach (Collider overlap in overlaps)
        {
            Debug.Log(overlap.gameObject.name);
            Actor enemy = overlap.GetComponent<Actor>();
            enemy.GetBehaviour<Actor_Enemy>().MaterialSwap();
            enemy.GetBehaviour<Actor_Enemy>().isShootable = true;
            //do stuff with hitboxes maybe?
        }
    }
    private void GetMinMaxRect()
    {
        minX = (_canvasTransform.sizeDelta.x - _cursorTransform.sizeDelta.x) * -0.5f;
        maxX = (_canvasTransform.sizeDelta.x - _cursorTransform.sizeDelta.x) * 0.5f;
        minY = (_canvasTransform.sizeDelta.y - _cursorTransform.sizeDelta.y) * -0.5f;
        maxY = (_canvasTransform.sizeDelta.y - _cursorTransform.sizeDelta.y) * 0.5f;
    }
}
