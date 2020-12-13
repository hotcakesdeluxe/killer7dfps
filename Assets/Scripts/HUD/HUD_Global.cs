using System.Collections.Generic;
using UnityEngine;

public class HUD_Global : MonoBehaviour
{
    public List<HUD_Element> hudElements => new List<HUD_Element>(GetComponentsInChildren<HUD_Element>());
    [SerializeField] private Canvas _canvas;

    private void Awake()
    {
        HUD.initEvent.AddListener(AssignGlobalHud);
    }

    private void AssignGlobalHud(HUD hud)
    {
        hud.SetGlobalHud(this);
    }

    public void SetActive(bool active)
    {
        _canvas.enabled = active;
    }
}
