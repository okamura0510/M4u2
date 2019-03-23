#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.IO;

namespace M4u2
{
    /// <summary>
    /// Hierarchyにバインドアイコンを表示する
    /// </summary>
    [InitializeOnLoad]
    public class M4uHierarchyIcon : Editor
    {
        const string IconSaveKey   = nameof(M4uHierarchyIcon) + "." + nameof(isShowing);
        const string IconBaseName  = "m4u_hierarchy_Icon_";
        const int IconCount        = 26;
        const float IconRightSpace = 5;

        static bool isShowing;
        static Texture[] icons;

        static M4uHierarchyIcon()
        {
            isShowing = (PlayerPrefs.GetInt(IconSaveKey, 0) == 1);
            Show(isShowing);
        }

        [MenuItem("Tools/" + nameof(M4u2) + "/Show HierarchyIcon")]
        static void ShowHierarchyIcon() => Show(true);

        [MenuItem("Tools/" + nameof(M4u2) + "/Hide HierarchyIcon")]
        static void HideHierarchyIcon() => Show(false);

        static void Show(bool isShowing)
        {
            if(isShowing != M4uHierarchyIcon.isShowing)
            {
                M4uHierarchyIcon.isShowing = isShowing;
                PlayerPrefs.SetInt(IconSaveKey, (isShowing ? 1 : 0));
                PlayerPrefs.Save();
            }

            if(isShowing)
            {
                EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyGUI;
            }
            else
            {
                EditorApplication.hierarchyWindowItemOnGUI -= OnHierarchyGUI;
            }
        }

        static void OnHierarchyGUI(int instanceID, Rect rect)
        {
            var go = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            if(go == null || go.GetComponent<M4uBinding>() == null)
            {
                return;   
            }

            if(icons == null)
            {
                icons          = new Texture[IconCount];
                var assetPaths = AssetDatabase.GetAllAssetPaths();
                var iconPath   = assetPaths.FirstOrDefault(path => path.Contains(IconBaseName));
                var iconDir    = Path.GetDirectoryName(iconPath);
                for(var i = 0; i < icons.Length; i++)
                {
                    iconPath = $"{iconDir}/{IconBaseName}{i}.png";
                    icons[i] = AssetDatabase.LoadAssetAtPath<Texture>(iconPath);
                }
            }

            var iconIdx  = Mathf.Abs(instanceID % icons.Length);
            var icon     = icons[iconIdx];
            var iconX    = rect.x + rect.width - IconRightSpace;
            var iconY    = rect.y;
            var iconRect = new Rect(iconX, iconY, icon.width, icon.height);
            GUI.DrawTexture(iconRect, icon);
        }
    }
}
#endif