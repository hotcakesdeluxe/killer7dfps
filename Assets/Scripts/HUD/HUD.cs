using PHL.Common.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUD : MonoBehaviour
{
    [SerializeField] private Actor _currentActor;
    [SerializeField] private Canvas _canvas;

    private Camera _currentCamera;
    private bool _initialized;
    private HUD_Global _globalHud;
    private List<HUD_Element> _hudElements = new List<HUD_Element>();

    public static Dictionary<Actor, HUD> playerHUDs = new Dictionary<Actor, HUD>();
    public static SecureEvent<HUD> initEvent { get; private set; } = new SecureEvent<HUD>();
    public static GameObject currentPopup;

    public Actor currentActor => _currentActor;
    public Camera currentCamera => _currentCamera;
    public Canvas canvas => _canvas;

    private void Start()
    {
        initEvent.Invoke(this);
        Initialize();

        if (_currentActor != null)
        {
            AssignToActor(_currentActor);
        }
    }

    public T GetElement<T>() where T : HUD_Element
    {
        return (T)(_hudElements.Find(x => x is T));
    }

    public void Initialize()
    {
        if (!_initialized)
        {
            _initialized = true;

            _hudElements = new List<HUD_Element>(GetComponentsInChildren<HUD_Element>());
            _hudElements.AddRange(_globalHud.hudElements);

            foreach (HUD_Element hudElement in _hudElements)
            {
                hudElement.Initialize(this);
            }

            //Quitting.quitToMenuEvent.AddListener(QuitReceived);
        }
    }

    public void AssignToActor(Actor newActor)
    {
        _currentActor = newActor;
        
        foreach (Actor key in playerHUDs.Keys)
        {
            if (playerHUDs[key] == this)
            {
                playerHUDs.Remove(key);
                break;
            }
        }

        playerHUDs.Add(newActor, this);



        if(_currentActor.GetBehaviour<Actor_Camera>() != null)
        {
            AssignToCamera(_currentActor.GetBehaviour<Actor_Camera>().GetComponent<Camera>());
        }



        AssignHUDActorBehaviours();
    }

    private void AssignHUDActorBehaviours()
    {
        if (currentActor != null)
        {
            foreach (HUD_Element hudElement in _hudElements)
            {
                hudElement.AssignActorBehaviours();
            }
        }
    }

    public void AssignToCamera(Camera newCamera)
    {
        _currentCamera = newCamera;
    }


    private void QuitReceived()
    {
        playerHUDs.Clear();
    }

    public void SetGlobalHud(HUD_Global newGlobal)
    {
        _globalHud = newGlobal;
    }

    public void SetActive(bool active, bool global = true)
    {
        if (_canvas != null)
        {
            _canvas.enabled = active;
            if (global)
            {
                _globalHud?.SetActive(active);
            }
        }
    }
}