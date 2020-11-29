using System;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using VampireVillage.Network;

[CustomEditor(typeof(Client))]
public class ClientEditor : Editor
{
    private Client client;
    private VisualElement rootElement;

    private VisualTreeAsset visualTree;
    private StyleSheet styleSheet;

    public void OnEnable()
    {
        client = (Client)target;
        rootElement = new VisualElement();

        visualTree = Resources.Load<VisualTreeAsset>("UXML/ClientEditor");
        styleSheet = Resources.Load<StyleSheet>("USS/Styles");
        rootElement.styleSheets.Add(styleSheet);
    }

    public override VisualElement CreateInspectorGUI()
    {
        rootElement.Clear();
        visualTree.CloneTree(rootElement);

        var playerId = rootElement.Q<Label>("playerId");
        playerId.text = (client.playerId == null || client.playerId == Guid.Empty) ? "-" : client.playerId.ToString();

        var playerName = rootElement.Q<Label>("playerName");
        playerName.text = client.playerName == "" ? "-" : client.playerName;

        return rootElement;
    }
}
