using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BazaarSuperSDK))]
[CanEditMultipleObjects]
public class BazaarSuperSDKEditor : Editor
{
    GUIStyle versionStyle;


    public override void OnInspectorGUI()
    {
        versionStyle = new GUIStyle(GUI.skin.label)
        {
            // alignment = TextAnchor.LowerLeft,
            // margin = new RectOffset(),
            padding = new RectOffset(0, 10,20,0),
            fontSize = 16,
            fontStyle = FontStyle.Bold,
        };


        BazaarSuperSDK sdk = (BazaarSuperSDK)target;

        GUILayout.BeginHorizontal();
        Texture banner = (Texture)AssetDatabase.LoadAssetAtPath("Assets/Bazaar/Logo.png", typeof(Texture));
        GUILayout.Box(banner);
        GUILayout.Label("Bazaar SuperSDK v" + sdk.version, versionStyle);
        GUILayout.EndHorizontal();

        GUILayout.Label("Tapsell", versionStyle);
        GUILayout.BeginHorizontal();
        GUILayout.Label("  Token");
        sdk.tapsellToken = GUILayout.TextField(sdk.tapsellToken, GUILayout.Width(300));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("  Zone ID");
        sdk.tapsellZoneId = GUILayout.TextField(sdk.tapsellZoneId, GUILayout.Width(300));
        GUILayout.EndHorizontal();

        GUILayout.Label("AppMetrica", versionStyle);
        GUILayout.BeginHorizontal();
        GUILayout.Label("  API Key");
        sdk.appMettricaApiKey = GUILayout.TextField(sdk.appMettricaApiKey, GUILayout.Width(300));
        GUILayout.EndHorizontal();
        // EditorGUILayout.LabelField("\n\nBazaar SuperSDK v" + version.stringValue, statesLabel);

        // base.DrawDefaultInspector();
    }
}