using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using VampireVillage.Network;
using Mirror;

[CustomEditor(typeof(LobbyManager))]
public class LobbyManagerEditor : Editor
{
    private LobbyManager manager;
    private VisualElement rootElement;

    private VisualTreeAsset visualTree;
    private VisualTreeAsset onlineVisualTree;
    private StyleSheet styleSheet;

    public void OnEnable()
    {
        manager = (LobbyManager)target;
        rootElement = new VisualElement();

        visualTree = Resources.Load<VisualTreeAsset>("UXML/LobbyManagerEditor");
        onlineVisualTree = Resources.Load<VisualTreeAsset>("UXML/LobbyManagerOnline");
        styleSheet = Resources.Load<StyleSheet>("USS/Styles");
        rootElement.styleSheets.Add(styleSheet);
    }

    public override VisualElement CreateInspectorGUI()
    {
        rootElement.Clear();
        visualTree.CloneTree(rootElement);

        if (manager.mode != NetworkManagerMode.Offline)
            rootElement.Add(CreateOnlineGUI());

        return rootElement;
    }

    private VisualElement CreateOnlineGUI()
    {
        var onlineRootElement = new VisualElement();
        onlineVisualTree.CloneTree(onlineRootElement);

        var roomCode = onlineRootElement.Q<Label>("roomCode");
        roomCode.text = manager.room?.code;

        var playerTotal = onlineRootElement.Q<Label>("playerTotal");
        playerTotal.text = manager.players.Count.ToString();

        return onlineRootElement;
    }
}
