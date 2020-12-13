using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUD_Element : MonoBehaviour
{
    [SerializeField] protected GameObject _parentObject;

    protected HUD _parentHud;
    protected Actor _currentActor
    {
        get
        {
            if(_parentHud == null)
            {
                return null;
            }

            return _parentHud.currentActor;
        }
    }

    public virtual void Initialize(HUD newParentHud)
    {
        if (_parentHud == null)
        {
            _parentHud = newParentHud;
        }
    }

    protected virtual void Update()
    {
        if (_parentObject != null)
        {
            if (CanDisplay())
            {
                if (!_parentObject.activeSelf)
                {
                    _parentObject.SetActive(true);
                }
            }
            else
            {
                if (_parentObject.activeSelf)
                {
                    _parentObject.SetActive(false);
                }
            }
        }
    }

    public virtual void AssignActorBehaviours()
    {

    }

    public virtual bool CanDisplay()
    {
        return true;
    }
}
