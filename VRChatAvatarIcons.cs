// Assets/Editor/VRChatAvatarIcons.cs
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

[InitializeOnLoad]
public static class VRChatAvatarIcons
{
    // Dictionary that maps component types to their corresponding icons
    static readonly Dictionary<System.Type, GUIContent> componentIcons = new()
    {
        { typeof(Animator), EditorGUIUtility.IconContent("Animator Icon") },
        { typeof(VRC.SDK3.Avatars.Components.VRCAvatarDescriptor), EditorGUIUtility.IconContent("d_Prefab Icon") },
        { typeof(SkinnedMeshRenderer), EditorGUIUtility.IconContent("SkinnedMeshRenderer Icon") },
        { typeof(AudioSource), EditorGUIUtility.IconContent("AudioSource Icon") },
        { typeof(VRC.SDK3.Dynamics.PhysBone.Components.VRCPhysBone), EditorGUIUtility.IconContent("Cloth Icon") },
        { typeof(VRC.SDK3.Dynamics.PhysBone.Components.VRCPhysBoneCollider), EditorGUIUtility.IconContent("SphereCollider Icon") },
        { typeof(UnityEngine.Collider), EditorGUIUtility.IconContent("BoxCollider Icon") },
        { typeof(UnityEngine.Canvas), EditorGUIUtility.IconContent("Canvas Icon") }
    };

    // Icon size and spacing between icons
    static readonly float iconSize = 20f;
    static readonly float padding = 2f;

    // Register the callback when the script loads
    static VRChatAvatarIcons()
    {
        EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyGUI;
    }

    // Called for each GameObject row in the Hierarchy window
    static void OnHierarchyGUI(int instanceID, Rect selectionRect)
    {
        GameObject obj = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
        if (obj == null) return;

        // Calculate how many icons can fit based on available width
        float availableWidth = selectionRect.width;
        int maxIcons = Mathf.FloorToInt((availableWidth - 60f) / (iconSize + padding));
        if (maxIcons <= 0) return;

        // Define starting position for drawing icons (from right to left)
        Rect iconRect = new(selectionRect.xMax - iconSize - padding, selectionRect.y + (selectionRect.height - iconSize) / 2, iconSize, iconSize);

        // Collect all matching component types from the predefined dictionary
        HashSet<System.Type> foundTypes = new();
        foreach (var pair in componentIcons)
        {
            if (obj.GetComponentInChildren(pair.Key, true))
            {
                foundTypes.Add(pair.Key);
            }
        }

        // Gather unique Modular Avatar component types with icons
        HashSet<System.Type> modularAvatarTypesShown = new();
        List<(Texture icon, string label)> modularIcons = new();
        foreach (var comp in obj.GetComponentsInChildren<Component>(true))
        {
            var type = comp.GetType();
            if (type.Namespace != null && type.Namespace.Contains("nadena.dev.modular_avatar") && !modularAvatarTypesShown.Contains(type))
            {
                modularAvatarTypesShown.Add(type);
                var icon = EditorGUIUtility.ObjectContent(comp, type).image;
                if (icon == null) continue;
                modularIcons.Add((icon, type.Name));
            }
        }

        // Combine both standard and modular avatar icons into one list
        List<(Texture icon, string label)> allIcons = new();
        foreach (var type in foundTypes)
        {
            allIcons.Add((componentIcons[type].image, type.Name));
        }
        allIcons.AddRange(modularIcons);

        // Draw as many icons as will fit
        int count = Mathf.Min(allIcons.Count, maxIcons);
        for (int i = 0; i < count; i++)
        {
            var (icon, label) = allIcons[i];
            GUI.Label(iconRect, new GUIContent(icon, label));
            iconRect.x -= iconSize + padding;
        }

        // If there are more icons than can be shown, show a "+N" label
        int remaining = allIcons.Count - count;
        if (remaining > 0)
        {
            var text = $"+{remaining}";
            GUIStyle style = new(EditorStyles.label)
            {
                fontSize = 10,
                normal = { textColor = Color.gray }
            };
            var size = style.CalcSize(new GUIContent(text));
            Rect textRect = new(iconRect.x - size.x - padding, iconRect.y + 3, size.x, size.y);
            GUI.Label(textRect, text, style);
        }
    }
}
