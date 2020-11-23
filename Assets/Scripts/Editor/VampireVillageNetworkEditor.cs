using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using VampireVillage.Network;

[CustomEditor(typeof(VampireVillageNetwork))]
public class VampireVillageNetworkEditor : Editor
{
    private VampireVillageNetwork network;
    private VisualElement rootElement;

    private VisualTreeAsset visualTree;
    private StyleSheet styleSheet;

    public void OnEnable()
    {
        network = (VampireVillageNetwork)target;
        rootElement = new VisualElement();

        visualTree = Resources.Load<VisualTreeAsset>("UXML/VampireVillageNetworkEditor");
        styleSheet = Resources.Load<StyleSheet>("USS/Styles");
        rootElement.styleSheets.Add(styleSheet);
    }

    public override VisualElement CreateInspectorGUI()
    {
        rootElement.Clear();
        visualTree.CloneTree(rootElement);
        return rootElement;
    }
}
