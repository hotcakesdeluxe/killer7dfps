using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PHL.Common.Utility;

[System.Serializable]
public class ConversationConnection
{
    public string id;
    public ConversationAction connectedConversation;
}

public class ConversationActionEvent : SecureEvent<ConversationAction> { }

public class ConversationAction : MonoBehaviour
{
    [HideInInspector] [SerializeField] private Vector2 _uiPosition;
    [HideInInspector] [SerializeField] protected List<ConversationConnection> _connections;

    protected IEnumerator _currentActionRoutine;

    public bool actionRunning
    {
        get
        {
            return _currentActionRoutine != null;
        }
    }

    public Conversation parentConversation
    {
        get
        {
            if(transform.parent != null)
            {
                return transform.parent.GetComponent<Conversation>();
            }

            return null;
        }
    }

    public List<ConversationConnection> connections => _connections;

    public virtual void Initialize()
    {
        
    }

    public virtual void StartAction()
    {
        //Debug.Log("ConAct" + gameObject.name);

        StopAction();

        _currentActionRoutine = ActionRoutine();
        StartCoroutine(_currentActionRoutine);
    }

    public virtual void StopAction()
    {
        if(_currentActionRoutine != null)
        {
            StopCoroutine(_currentActionRoutine);
            _currentActionRoutine = null;
        }
    }

    protected virtual IEnumerator ActionRoutine()
    {
        //So it stops complaining about being a coroutine
        if (false)
        {
            yield return 0f;
        }

        StopAction();
    }

    public virtual ConversationAction GetNextAction()
    {
        return _connections[0].connectedConversation;
    }
    
    public virtual int GetConnectionCount()
    {
        return 1;
    }

    protected virtual void OnValidate()
    {
        if(_connections == null)
        {
            _connections = new List<ConversationConnection>();
        }

        if (_connections.Count > GetConnectionCount())
        {
            _connections = _connections.GetRange(0, GetConnectionCount());
        }
        
        while(_connections.Count < GetConnectionCount())
        {
            _connections.Add(new ConversationConnection()
            {
                id = "",
                connectedConversation = null
            });
        }
    }

    public virtual Color GetNodeColor()
    {
        return Color.white;
    }

    public virtual Color GetTitleColor()
    {
        return Color.black;
    }

    private void OnDestroy()
    {
        StopAction();
    }
}
