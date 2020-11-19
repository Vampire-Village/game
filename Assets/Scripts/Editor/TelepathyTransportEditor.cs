using UnityEditor;
using Mirror;

/// <summary>
/// This disables the default inspector for Telepathy Transport.
/// We should only change the transport settings through Vampire Village Network script.
/// </summary>
[CustomEditor(typeof(TelepathyTransport))]
public class TelepathyTransportEditor : Editor
{
    public override void OnInspectorGUI() {}
}
