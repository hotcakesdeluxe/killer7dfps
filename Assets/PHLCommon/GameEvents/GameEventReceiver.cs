using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PHL.Common.GameEvents;
using PHL.Common.Utility;

public class GameEventReceiver : MonoBehaviour
{
    [SerializeField] private GameEventData _gameEvent;
    [SerializeField] private bool _debugLog;

    protected virtual void Awake()
    {
        _gameEvent.AddListener(ReceiveEvent);
    }

    protected virtual void ReceiveEvent(GameEventInfo info)
    {
        if (_debugLog)
        {
            Logster.Log("GameEvents", string.Format("Event {0} triggered of type {1}", _gameEvent.name.ToUpper(), this.GetType().FullName.ToUpper()));
        }
    }
}
