using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PHL.Common.Utility;
public class Actor_Shooting : ActorBehaviour
{
    private Actor_PlayerInput _input;
    private Actor_Camera _camera;
    private Actor_Animation _animation;
    private Actor_CharacterController _characterController;
    private Canvas _canvas;
    [SerializeField] private Transform _fpscam;
    [SerializeField] private BoxCollider _scanZone;
    [SerializeField] private Graphic _reticleGraphic;
    [SerializeField] private GameObject _muzzleFlare;
    private RectTransform _canvasTransform;
    [SerializeField] private RectTransform _cursorTransform;
    [SerializeField] private float _aimSpeed;
    [SerializeField] private float _damage;

    private float minX;
    private float maxX;
    private float minY;
    private float maxY;
    private float rotX;
    private float rotY;
    private int ammo = 6;

    public bool isReload = false;
    public LayerMask enemyLayer;
    public override void AssignActorReferences(Actor newActor)
    {
        base.AssignActorReferences(newActor);
        _input = GetBehaviour<Actor_PlayerInput>();
        _camera = GetBehaviour<Actor_Camera>();
        _animation = GetBehaviour<Actor_Animation>();
        _characterController = GetBehaviour<Actor_CharacterController>();
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
        MoveReticule();

    }
    public void Fire()
    {
        if (ammo != 0 && !isReload)
        {
            StartCoroutine(MuzzleFlareRoutine());
            ammo -= 1;
            RaycastHit hit;
            Vector3 castDir = _cursorTransform.position - _camera.FirstPersonCam.transform.position;
            if (Physics.Raycast(_camera.FirstPersonCam.transform.position, castDir, out hit, Mathf.Infinity, enemyLayer))
            {
                Debug.DrawRay(_camera.FirstPersonCam.transform.position, castDir * hit.distance, Color.yellow);
                HashSet<HitboxRoot> rootsHit = new HashSet<HitboxRoot>();
                Actor enemy = hit.transform.gameObject.GetComponentInParent<Actor>();
                if (enemy.GetBehaviour<Actor_Enemy>().isShootable)
                {
                    HitboxCollider hitbox = hit.transform.GetComponent<HitboxCollider>();
                    DamageType damageType = DamageType.Base;
                    if (hitbox != null)
                    {
                        if (!rootsHit.Contains(hitbox.root))
                        {
                            rootsHit.Add(hitbox.root);
                            if(hitbox.gameObject.tag == "Weakpoint")
                            {
                                damageType = DamageType.Weakpoint;
                            }
                            hitbox.Hit(new DamageInfo()
                            {
                                amount = _damage,
                                damageTeam = DamageTeam.Any,
                                source = actor,
                                damageType = damageType
                            });
                        }
                    }
                }
            }
            else
            {
                Debug.DrawRay(_camera.FirstPersonCam.transform.position, castDir * 1000, Color.white);
                Debug.Log("Did not Hit");
            }
        }
        if (ammo == 0 && !isReload)
        {
            Reload();
        }
    }
    private void MoveReticule()
    {
        if (!isReload)
        {
            if (_input.isAiming)
            {
                _reticleGraphic.CrossFadeAlpha(1, 0.1f, true);
                rotX += _input.smoothInputAim.x * _aimSpeed;
                rotY += _input.smoothInputAim.y * _aimSpeed;
                rotX = Mathf.Clamp(rotX, -45, 45);
                rotY = Mathf.Clamp(rotY, -45, 45);

                _fpscam.localRotation = Quaternion.Euler(-rotY, rotX, 0f);
            }
            else
            {
                _reticleGraphic.CrossFadeAlpha(0, 0.1f, true);
                rotX = 0;
                rotY = 0;
            }
        }
        /*Vector2 delta = new Vector2(_aimSpeed * _input.smoothInputAim.x * Time.deltaTime, _aimSpeed * _input.smoothInputAim.y * Time.deltaTime);
        Vector2 currentPosition = _cursorTransform.anchoredPosition;
        Vector2 newPosition = currentPosition + delta;
        if (_canvas != null)
        {
            // Clamp to canvas.
            //var pixelRect = _canvas.pixelRect;
            newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
            newPosition.y = Mathf.Clamp(newPosition.y, minY, maxY);
        }
        _cursorTransform.anchoredPosition = newPosition;*/
    }
    public void Scan()
    {
        Collider[] overlaps = _scanZone.OverlapBox(enemyLayer);
        foreach (Collider overlap in overlaps)
        {
            if (overlap.transform.gameObject.tag == "Enemy")
            {
                Actor enemy = overlap.GetComponentInParent<Actor>();
                enemy.GetBehaviour<Actor_Enemy>().MaterialSwap();
                enemy.GetBehaviour<Actor_Enemy>().isShootable = true;
                enemy.gameObject.layer = 10;
                //do stuff with hitboxes maybe?
            }
            if (overlap.transform.gameObject.tag == "Weakpoint")
            {
                Actor enemy = overlap.GetComponentInParent<Actor>();
                enemy.GetBehaviour<Actor_Weakpoint>().RevealWeakpoint();
            }

        }
    }
    public void Reload()
    {
        if (!isReload)
        {
            isReload = true;
            _animation.PlayReload();
            StartCoroutine(ReloadRoutine());
        }
    }

    private IEnumerator ReloadRoutine()
    {
        _reticleGraphic.CrossFadeAlpha(0, 0.01f, true);
        yield return new WaitForSeconds(0.66f);
        isReload = false;
        ammo = 6;
    }
    private IEnumerator MuzzleFlareRoutine()
    {
        _muzzleFlare.SetActive(true);
        yield return new WaitForSeconds(0.05f);
        _muzzleFlare.SetActive(false);
    }
    private void GetMinMaxRect()
    {
        minX = (_canvasTransform.sizeDelta.x - _cursorTransform.sizeDelta.x) * -0.5f;
        maxX = (_canvasTransform.sizeDelta.x - _cursorTransform.sizeDelta.x) * 0.5f;
        minY = (_canvasTransform.sizeDelta.y - _cursorTransform.sizeDelta.y) * -0.5f;
        maxY = (_canvasTransform.sizeDelta.y - _cursorTransform.sizeDelta.y) * 0.5f;
    }
}
