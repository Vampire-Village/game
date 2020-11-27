using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using VampireVillage.Network;
using Mirror;

[CustomEditor(typeof(RoomManager))]
public class RoomManagerEditor : Editor
{
    private RoomManager manager;
    private VisualElement rootElement;

    private VisualTreeAsset visualTree;
    private VisualTreeAsset serverVisualTree;
    private VisualTreeAsset roomVisualTree;
    private StyleSheet styleSheet;
    private StyleSheet roomManagerStyleSheet;

    private Label emptyRoomListText;
    private ListView roomList;
    private Button refreshButton;

    public void OnEnable()
    {
        manager = (RoomManager)target;
        rootElement = new VisualElement();

        visualTree = Resources.Load<VisualTreeAsset>("UXML/RoomManagerEditor");
        serverVisualTree = Resources.Load<VisualTreeAsset>("UXML/RoomManagerServer");
        roomVisualTree = Resources.Load<VisualTreeAsset>("UXML/RoomManagerRoom");
        styleSheet = Resources.Load<StyleSheet>("USS/Styles");
        roomManagerStyleSheet = Resources.Load<StyleSheet>("USS/RoomManagerStyles");
        rootElement.styleSheets.Add(styleSheet);
        rootElement.styleSheets.Add(roomManagerStyleSheet);
    }

    public override VisualElement CreateInspectorGUI()
    {
        rootElement.Clear();
        visualTree.CloneTree(rootElement);

        if (manager.mode == NetworkManagerMode.ServerOnly)
            rootElement.Add(CreateServerGUI());

        return rootElement;
    }

    private VisualElement CreateServerGUI()
    {
        // Add the server-only view.
        VisualElement serverRootElement = new VisualElement();
        serverVisualTree.CloneTree(serverRootElement);

        // Set list view settings.
        emptyRoomListText = serverRootElement.Q<Label>("emptyRoomListText");
        roomList = serverRootElement.Q<ListView>("roomList");
        roomList.itemHeight = 50;
        roomList.makeItem = MakeRoomListItem;
        roomList.bindItem = BindRoomListItem;

        // Get rooms.
        RefreshRooms();

        // Bind refresh button.
        refreshButton = serverRootElement.Q<Button>("refreshButton");
        refreshButton.clicked += RefreshRooms;

        return serverRootElement;
    }

    private VisualElement MakeRoomListItem()
    {
        var roomRootElement = new VisualElement();
        roomVisualTree.CloneTree(roomRootElement);
        return roomRootElement;
    }

    private void BindRoomListItem(VisualElement roomRootElement, int index)
    {
        var room = roomList.itemsSource[index] as Room;
        var roomCode = roomRootElement.Q<Label>("roomCode");
        var roomPlayers = roomRootElement.Q<Label>("roomPlayers");
        roomCode.text = room.code;
        roomPlayers.text = room.players.Count.ToString();
    }

    private void RefreshRooms()
    {
        var rooms = new List<Room>(manager.rooms.Values);
        if (rooms.Count == 0)
        {
            roomList.style.display = DisplayStyle.None;
            emptyRoomListText.style.display = DisplayStyle.Flex;
        }
        else
        {
            roomList.itemsSource = rooms;
            roomList.style.display = DisplayStyle.Flex;
            emptyRoomListText.style.display = DisplayStyle.None;
            roomList.style.height = rooms.Count == 1 ? roomList.itemHeight : roomList.itemHeight * 2;
        }
    }

    private void OnDisable()
    {
        if (refreshButton != null)
            refreshButton.clicked -= RefreshRooms;
    }
}
