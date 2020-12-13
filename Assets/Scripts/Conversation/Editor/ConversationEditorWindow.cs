using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ConversationEditorWindow : EditorWindow
{
    private static GUIStyle _nodeStyle;
    private static Conversation _selectedConversation;
    private static HashSet<ConversationAction> _selectedActions = new HashSet<ConversationAction>();
    private static List<ConversationAction> _allActions = new List<ConversationAction>();
    private static Vector2 _scrollPosition;
    private static bool _dragging;
    private static bool _scrolling;
    private static bool _noodling;
    private static bool _multiSelect;
    private static ConversationAction _noodlingAction;
    private static int _noodlingIndex;

    [MenuItem("Window/PHL/Conversation Editor")]
    private static void OpenWindow()
    {
        Texture icon = AssetDatabase.LoadAssetAtPath<Texture>("Assets/Scripts/Conversation/Editor/Resources/ConversationIcon.png");
        ConversationEditorWindow window = GetWindow<ConversationEditorWindow>();
        window.titleContent = new GUIContent("Conversation", icon);
    }

    private void OnEnable()
    {
        UpdateStyle();
        UpdateUISelection();

        _dragging = false;
        _scrolling = false;
        _noodling = false;
        _multiSelect = false;
        _scrollPosition = Vector2.zero;
    }

    //Build default node style
    private void UpdateStyle()
    {
        _nodeStyle = new GUIStyle();

        _nodeStyle.normal.background = Resources.Load<Texture2D>("Node");
        _nodeStyle.border = new RectOffset(5, 5, 5, 5);
        _nodeStyle.padding = new RectOffset(10, 5, 5, 5);
        _nodeStyle.wordWrap = true;
        _nodeStyle.alignment = TextAnchor.MiddleCenter;
    }

    private void OnSelectionChange()
    {
        UpdateUISelection();
    }

    //Populate selected convos and actions with only elements from the same convo
    private void UpdateUISelection()
    {
        Conversation oldConversation = _selectedConversation;

        if (Selection.objects != null)
        {
            bool selectionValid = true;
            _selectedConversation = null;
            _selectedActions.Clear();

            foreach (Object obj in Selection.objects)
            {
                if (obj is GameObject)
                {
                    if (((GameObject)obj).GetComponent<Conversation>() != null)
                    {
                        if (_selectedConversation == null)
                        {
                            _selectedConversation = ((GameObject)obj).GetComponent<Conversation>();
                        }
                        else
                        {
                            Conversation newConversation = ((GameObject)obj).GetComponent<Conversation>();
                            if (newConversation != _selectedConversation)
                            {
                                selectionValid = false;
                                break;
                            }
                        }
                    }
                    else if (((GameObject)obj).GetComponent<ConversationAction>() != null)
                    {
                        if (_selectedConversation == null)
                        {
                            _selectedConversation = (((GameObject)obj).GetComponent<ConversationAction>()).parentConversation;
                            _selectedActions.Add(((GameObject)obj).GetComponent<ConversationAction>());
                        }
                        else
                        {
                            Conversation newConversation = (((GameObject)obj).GetComponent<ConversationAction>()).parentConversation;
                            if (newConversation == _selectedConversation)
                            {
                                _selectedActions.Add(((GameObject)obj).GetComponent<ConversationAction>());
                            }
                            else
                            {
                                selectionValid = false;
                                break;
                            }
                        }
                    }
                    else
                    {
                        selectionValid = false;
                        break;
                    }
                }
                else
                {
                    selectionValid = false;
                    break;
                }
            }

            if(_selectedConversation != null)
            {
                if(AssetDatabase.Contains(_selectedConversation.gameObject))
                {
                    selectionValid = false;
                }
            }

            if(!selectionValid)
            {
                _selectedConversation = null;
                _selectedActions.Clear();
            }
        }

        if (_selectedConversation != null && oldConversation != _selectedConversation)
        {
            Vector2 max = new Vector2(-1000, -1000);
            Vector2 min = new Vector2(1000, 1000);

            if (_allActions != null)
            {
                foreach (ConversationAction conversationAction in _allActions)
                {
                    if (conversationAction != null)
                    {
                        SerializedObject nodeSO = new SerializedObject(conversationAction);
                        SerializedProperty positionProperty = nodeSO.FindProperty("_uiPosition");

                        Vector2 center = positionProperty.vector2Value;
                        center.x += 80;
                        center.y += 25;

                        if (center.x > max.x)
                        {
                            max.x = center.x;
                        }

                        if (center.y > max.y)
                        {
                            max.y = center.y;
                        }

                        if (center.x < min.x)
                        {
                            min.x = center.x;
                        }

                        if (center.y < min.y)
                        {
                            min.y = center.y;
                        }
                    }
                }
            }

            _scrollPosition = -(max + min) / 2f;
            _scrollPosition.x += position.width / 2f;
            _scrollPosition.y += position.height / 2f;
        }

        Repaint();
    }

    private void UpdateUnitySelection()
    {
        if(_selectedActions.Count > 0)
        {
            Object[] newSelection = new Object[_selectedActions.Count];

            int index = 0;
            foreach(ConversationAction conversationAction in _selectedActions)
            {
                newSelection[index] = conversationAction.gameObject;
                index++;
            }

            Selection.objects = newSelection;
        }
        else if(_selectedConversation != null)
        {
            Selection.objects = new Object[] { _selectedConversation.gameObject };
        }
    }

    private void OnGUI()
    {
        DrawGrid(20, 0.2f, Color.gray);
        DrawGrid(100, 0.4f, Color.gray);

        GUI.color = Color.white;

        if(_selectedConversation == null)
        {
            return;
        }

        _allActions = _selectedConversation.actions;

        if (_selectedConversation != null)
        {
            foreach (ConversationAction conversationAction in _allActions)
            {
                DrawNode(conversationAction);
            }
        }

        Event currentEvent = Event.current;

        if (currentEvent.shift)
        {
            _multiSelect = true;
        }
        else
        {
            _multiSelect = false;
        }

        //Mouse events
        if (currentEvent.type == EventType.MouseDown && currentEvent.button == 0)
        {
            bool onNodeHole = false;
            bool onNode = false;

            foreach (ConversationAction conversationAction in _allActions)
            {
                List<Rect> nodeHoleRects = GetConnectionPointsRects(conversationAction);

                for(int i = 0; i < nodeHoleRects.Count; i++)
                {
                    if(nodeHoleRects[i].Contains(Event.current.mousePosition))
                    {
                        onNodeHole = true;
                        _noodling = true;
                        _noodlingAction = conversationAction;
                        _noodlingIndex = i;
                    }
                }

                //If on nodehole...
            }

            if (!onNodeHole)
            {
                foreach (ConversationAction conversationAction in _allActions)
                {
                    Rect nodeRect = GetNodeRect(conversationAction);

                    if (nodeRect.Contains(Event.current.mousePosition))
                    {
                        onNode = true;
                        _dragging = true;

                        if (_multiSelect)
                        {
                            _selectedActions.Add(conversationAction);
                        }
                        else
                        {
                            if(!_selectedActions.Contains(conversationAction))
                            {
                                _selectedActions.Clear();
                                _selectedActions.Add(conversationAction);
                            }
                        }

                        UpdateUnitySelection();
                    }
                }
            }

            if(!onNodeHole && !onNode)
            {
                _selectedActions.Clear();
                UpdateUnitySelection();
                _scrolling = true;
                _dragging = false;
                _noodling = false;
                _noodlingAction = null;
                _noodlingIndex = 0;
            }

            Repaint();
        }
        else if(currentEvent.type == EventType.MouseUp && currentEvent.button == 0)
        {
            if (_noodling)
            {
                foreach (ConversationAction conversationAction in _allActions)
                {
                    if (GetNodeRect(conversationAction).Contains(Event.current.mousePosition))
                    {
                        if(conversationAction != _noodlingAction)
                        {
                            SerializedObject nodeSO = new SerializedObject(_noodlingAction);
                            SerializedProperty connectionProperty = nodeSO.FindProperty(string.Format("_connections.Array.data[{0}]", _noodlingIndex));
                            SerializedProperty actionProperty = connectionProperty.FindPropertyRelative("connectedConversation");
                            actionProperty.objectReferenceValue = conversationAction;
                            nodeSO.ApplyModifiedProperties();

                            break;
                        }
                    }
                }
            }

            _dragging = false;
            _noodling = false;
            _scrolling = false;
            _noodlingAction = null;
            _noodlingIndex = 0;

            Repaint();
        }
        else if (currentEvent.type == EventType.MouseDown && currentEvent.button == 1)
        {
            _dragging = false;
            _noodling = false;
            _scrolling = false;
            _noodlingAction = null;
            _noodlingIndex = 0;

            Repaint();
        }
        else if (currentEvent.type == EventType.MouseUp && currentEvent.button == 1)
        {
            bool nodeSelected = false;

            foreach (ConversationAction conversationAction in _allActions)
            {
                if (GetNodeRect(conversationAction).Contains(Event.current.mousePosition))
                {
                    nodeSelected = true;

                    GenericMenu genericMenu = new GenericMenu();
                    genericMenu.AddItem(new GUIContent("Delete!"), false, () => Delete(conversationAction));
                    genericMenu.ShowAsContext();

                    break;
                }
            }

            if(!nodeSelected)
            {
                GenericMenu genericMenu = new GenericMenu();
                genericMenu.AddItem(new GUIContent("Add Speak"), false, () => AddNode<ConAct_Speak>(currentEvent.mousePosition));
                genericMenu.AddItem(new GUIContent("Add Choice"), false, () => AddNode<ConAct_Choice>(currentEvent.mousePosition));
                //genericMenu.AddItem(new GUIContent("Add Mission State"), false, () => AddNode<ConAct_MissionState>(currentEvent.mousePosition));
                //genericMenu.AddItem(new GUIContent("Add Mission Step"), false, () => AddNode<ConAct_MissionStep>(currentEvent.mousePosition));
                //genericMenu.AddItem(new GUIContent("Add Current Mission"), false, () => AddNode<ConAct_CurrentMission>(currentEvent.mousePosition));
                //genericMenu.AddItem(new GUIContent("Add GameEvent"), false, () => AddNode<ConAct_GameEvent>(currentEvent.mousePosition));
                //genericMenu.AddItem(new GUIContent("Add Multiple GameEvents"), false, () => AddNode<ConAct_MultipleGameEvents>(currentEvent.mousePosition));
                //genericMenu.AddItem(new GUIContent("Add CustomValue Assignment"), false, () => AddNode<ConAct_CustomValueAssignment>(currentEvent.mousePosition));
               // genericMenu.AddItem(new GUIContent("Add CustomValue Conditional"), false, () => AddNode<ConAct_CustomValueConditional>(currentEvent.mousePosition));
                //genericMenu.AddItem(new GUIContent("Add Sound"), false, () => AddNode<ConAct_Sound>(currentEvent.mousePosition));
                //genericMenu.AddItem(new GUIContent("Add Wait"), false, () => AddNode<ConAct_Wait>(currentEvent.mousePosition));
                //genericMenu.AddItem(new GUIContent("Add Animation"), false, () => AddNode<ConAct_Animation>(currentEvent.mousePosition));
                //genericMenu.AddItem(new GUIContent("Add Look At"), false, () => AddNode<ConAct_LookAt>(currentEvent.mousePosition));
                //genericMenu.AddItem(new GUIContent("Add Random Choice"), false, () => AddNode<ConAct_RandomChoice>(currentEvent.mousePosition));
                //genericMenu.AddItem(new GUIContent("Add Tutorial Prompt"), false, () => AddNode<ConAct_TutorialPrompt>(currentEvent.mousePosition));
                genericMenu.ShowAsContext();
            }

            Repaint();
        }
        else if(currentEvent.type == EventType.MouseDrag && currentEvent.button == 0)
        {
            if (_scrolling)
            {
                _scrollPosition += new Vector2Int(Mathf.RoundToInt(currentEvent.delta.x), Mathf.RoundToInt(currentEvent.delta.y));
            }
            else if (_dragging)
            {
                foreach (ConversationAction conversationAction in _selectedActions)
                {
                    SerializedObject nodeSO = new SerializedObject(conversationAction);
                    SerializedProperty positionProperty = nodeSO.FindProperty("_uiPosition");
                    positionProperty.vector2Value += new Vector2(currentEvent.delta.x, currentEvent.delta.y);
                    nodeSO.ApplyModifiedProperties();
                }
            }

            Repaint();
        }

        //Noodling
        if(_noodling)
        {
            if (_noodlingAction == null)
            {
                _noodling = false;
            }
            else
            {
                List<Rect> connectionPointRects = GetConnectionPointsRects(_noodlingAction);

                Vector2 offset = new Vector2(25, 0);
                Rect startRect = connectionPointRects[_noodlingIndex];
                startRect.x = startRect.max.x;
                startRect.width = 0;

                Handles.DrawBezier
                (
                    startRect.center,
                    Event.current.mousePosition,
                    startRect.center + offset,
                    Event.current.mousePosition - offset,
                    new Color(0, 0, 0, 0.333f),
                    null,
                    3f
                );
            }

            Repaint();
        }

        if (GUI.changed)
        {
            Repaint();
        }
    }

    private void AddNode<T>(Vector2 spawnPosition) where T : ConversationAction
    {
        if (_selectedConversation != null)
        {
            GameObject newObject = new GameObject(typeof(T).Name, new System.Type[] { typeof(T) });
            newObject.transform.parent = _selectedConversation.transform;

            SerializedObject nodeSO = new SerializedObject(newObject.GetComponent<T>());
            SerializedProperty positionProperty = nodeSO.FindProperty("_uiPosition");
            positionProperty.vector2Value = spawnPosition - _scrollPosition;
            nodeSO.ApplyModifiedProperties();

            Selection.objects = new Object[] { newObject };
            Repaint();
        }
    }

    private void Delete(ConversationAction action)
    {
        if (_selectedActions.Contains(action))
        {
            Selection.objects = new Object[] { action.parentConversation.gameObject };
        }

        DestroyImmediate(action.gameObject);
        UpdateUISelection();
        Repaint();

        if (_selectedConversation != null)
        {
            EditorUtility.SetDirty(_selectedConversation);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private void DrawGrid(float gridSpacing, float gridOpacity, Color gridColor)
    {
        int widthDivs = Mathf.CeilToInt(position.width / gridSpacing);
        int heightDivs = Mathf.CeilToInt(position.height / gridSpacing) + 1;

        Handles.BeginGUI();
        Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridOpacity);

        Vector3 newOffset = new Vector3(_scrollPosition.x % gridSpacing, _scrollPosition.y % gridSpacing, 0);

        for (int i = 0; i < widthDivs; i++)
        {
            Handles.DrawLine(new Vector3(gridSpacing * i, -gridSpacing, 0) + newOffset,
                             new Vector3(gridSpacing * i, position.height + 150, 0f) + newOffset);
        }

        for (int i = 0; i < heightDivs; i++)
        {
            Handles.DrawLine(new Vector3(-gridSpacing, gridSpacing * i, 0) + newOffset,
                             new Vector3(position.width + gridSpacing, gridSpacing * i, 0f) + newOffset);
        }

        Handles.color = Color.white;
        Handles.EndGUI();
    }

    private Rect GetNodeRect(ConversationAction conversationAction)
    {
        SerializedObject nodeSO = new SerializedObject(conversationAction);
        SerializedProperty positionProperty = nodeSO.FindProperty("_uiPosition");

        int height = 40;

        if (conversationAction.GetConnectionCount() > 1)
        {
            height = conversationAction.GetConnectionCount() * 25;
        }

        return new Rect(positionProperty.vector2Value.x + _scrollPosition.x, positionProperty.vector2Value.y + _scrollPosition.y, 160, height);
    }

    private List<Rect> GetConnectionPointsRects(ConversationAction conversationAction)
    {
        List<Rect> rects = new List<Rect>();

        if (conversationAction.GetConnectionCount() > 0)
        {
            SerializedObject nodeSO = new SerializedObject(conversationAction);

            for (int i = 0; i < conversationAction.GetConnectionCount(); i++)
            {
                Rect rect = GetNodeRect(conversationAction);
                rect.x = rect.max.x;
                rect.y += i * 25;
                rect.y += 5;

                if(conversationAction.GetConnectionCount() == 1)
                {
                    rect.y += 7;
                }

                rect.size = new Vector2(15, 15);
                rects.Add(rect);
            }
        }

        return rects;
    }

    private void DrawNode(ConversationAction conversationAction)
    {
        if (conversationAction.GetConnectionCount() > 0)
        {
            SerializedObject nodeSO = new SerializedObject(conversationAction);
            List<Rect> connectionPointRects = GetConnectionPointsRects(conversationAction);

            for (int i = 0; i < conversationAction.GetConnectionCount(); i++)
            {
                SerializedProperty connectionProperty = nodeSO.FindProperty(string.Format("_connections.Array.data[{0}]", i));
                SerializedProperty idProperty = connectionProperty.FindPropertyRelative("id");
                SerializedProperty actionProperty = connectionProperty.FindPropertyRelative("connectedConversation");

                Rect connectionPointRect = connectionPointRects[i];

                connectionPointRect.x -= 5;
                connectionPointRect.width += 5;

                GUI.color = new Color(0.6f, 0.6f, 0.6f);
                GUI.Box(connectionPointRect, "", _nodeStyle);
                
                if(actionProperty.objectReferenceValue != null && _allActions.Contains((ConversationAction)(actionProperty.objectReferenceValue)))
                {
                    ConversationAction otherAction = (ConversationAction)(actionProperty.objectReferenceValue);
                    Rect startRect = connectionPointRects[i];
                    Rect otherRect = GetNodeRect(otherAction);

                    startRect.width = 0;
                    startRect.x += 15;
                    otherRect.width = 0;

                    Vector2 offset = new Vector2(Vector2.Distance(startRect.center, otherRect.center) / 2f,
                                                 (otherRect.center.y - startRect.center.y) / 2f);

                    Color bezierColor = new Color(0, 0, 0, 0.333f);
                    float bezierWidth = 3;

                    if(_selectedActions.Contains(conversationAction))
                    {
                        bezierColor = new Color(1, 1, 0, 1f);
                        bezierWidth = 5;
                    }
                    else if(_selectedActions.Contains(otherAction))
                    {
                        bezierColor = new Color(1, 1, 0, 1f);
                        bezierWidth = 5;
                    }

                    Handles.DrawBezier
                    (
                        startRect.center,
                        otherRect.center,
                        startRect.center + offset,
                        otherRect.center - offset,
                        bezierColor,
                        null,
                        bezierWidth
                    );

                    Rect breakNoodleRect = new Rect();
                    breakNoodleRect.size = new Vector2(10, 10);
                    breakNoodleRect.center = (startRect.center + otherRect.center) / 2f;
                    GUI.color = new Color(1, 0, 0, 0.666f);

                    if (GUI.Button(breakNoodleRect, "", _nodeStyle))
                    {
                        if (Event.current.button == 0)
                        {
                            actionProperty.objectReferenceValue = null;
                            nodeSO.ApplyModifiedProperties();
                        }
                    }
                }

                GUI.color = conversationAction.GetTitleColor();
                Rect labelRect = connectionPointRects[i];
                labelRect.x += 20;
                labelRect.width = 200;
                GUI.Label(labelRect, idProperty.stringValue);
            }
        }

        if (_selectedActions.Contains(conversationAction))
        {
            Color newColor = conversationAction.GetNodeColor();
            newColor.r -= 0.25f;
            newColor.g -= 0.25f;
            newColor.b -= 0.25f;
            GUI.color = newColor;
        }
        else
        {
            GUI.color = conversationAction.GetNodeColor();
        }

        GUI.Box(GetNodeRect(conversationAction), conversationAction.name, _nodeStyle);
    }
}
