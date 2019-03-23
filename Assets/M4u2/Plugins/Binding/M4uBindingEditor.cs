#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.Reflection;
using Object = UnityEngine.Object;

namespace M4u2
{
    /// <summary>
    /// M4uBindingのエディタ拡張
    /// </summary>
    [CustomEditor(typeof(M4uBinding))]
    class M4uBindingEditor : Editor
    {
        const string FirstSelectReadMessage   = "Select ReadPath";
        const string FirstSelectWriteMessage  = "Select WritePath";
        const string EmptyDynamicReadMessage  = "Enter ReadPath";
        const string EmptyDynamicWriteMessage = "Enter WritePath";
        const string FoldoutBindingMessage    = "BindingData";
        const string FoldoutBindingSaveKey    = nameof(M4uBindingEditor) + "." + nameof(isFoldoutBinding);
        const float ObjectFieldWidthRate      = 0.29f;
        const float MemberPopupWidthRate      = 1 - ObjectFieldWidthRate;
        const float Object2MemberSpace        = 5;

        static bool isFoldoutBinding;
        M4uBinding binding;
        SerializedProperty initializeOnEditorProperty;
        SerializedProperty initializeOnRuntimeProperty;
        SerializedProperty readObjectProperty;
        SerializedProperty writeObjectProperty;

        M4uBindingObject ReadObject  => binding.ReadObject;
        M4uBindingObject WriteObject => binding.WriteObject;
        M4uBindingMember ReadMember  => ReadObject.SelectMember;
        M4uBindingMember WriteMember => WriteObject.SelectMember;

        void OnEnable()
        {
            isFoldoutBinding            = (PlayerPrefs.GetInt(FoldoutBindingSaveKey, 0) == 1);
            binding                     = (M4uBinding)target;
            initializeOnEditorProperty  = serializedObject.FindProperty("initializeOnEditor");
            initializeOnRuntimeProperty = serializedObject.FindProperty("initializeOnRuntime");
            readObjectProperty          = serializedObject.FindProperty("readObject");
            writeObjectProperty         = serializedObject.FindProperty("writeObject");
            RefreshBinding();
        }
        
        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();

            if(ReadObject.Members.Count == 0 || WriteObject.Members.Count == 0)
            {
                RefreshBinding();
            }

            GUILayout.Space(5);
            DrawBindingObject(ReadObject);
            GUILayout.Space(2);
            DrawBindingObject(WriteObject);

            var prev         = isFoldoutBinding;
            isFoldoutBinding = EditorGUILayout.Foldout(isFoldoutBinding, FoldoutBindingMessage);
            if(isFoldoutBinding != prev)
            {
                PlayerPrefs.SetInt(FoldoutBindingSaveKey, (isFoldoutBinding ? 1 : 0));
                PlayerPrefs.Save();
            }
            if(isFoldoutBinding)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(initializeOnEditorProperty);
                EditorGUILayout.PropertyField(initializeOnRuntimeProperty);
                EditorGUILayout.PropertyField(readObjectProperty,  true);
                EditorGUILayout.PropertyField(writeObjectProperty, true);
                EditorGUI.indentLevel--;
            }

            serializedObject.ApplyModifiedProperties();
            if(EditorGUI.EndChangeCheck()) RefreshBinding();
        }

        void DrawBindingObject(M4uBindingObject bo)
        {
            EditorGUILayout.BeginHorizontal();

            var baseRect = EditorGUILayout.GetControlRect();
            if(bo.IsDynamicBinding)
            {
                var memberPath = EditorGUI.TextField(baseRect, bo.SelectMember.MemberPath);
                if(!Application.isPlaying)
                {
                    if(string.IsNullOrEmpty(memberPath)     ||
                       memberPath == FirstSelectReadMessage ||
                       memberPath == FirstSelectWriteMessage)
                    {
                        bo.SelectMember.MemberPath = bo.MemberPaths[0];
                    }
                    else
                    {
                        bo.SelectMember.MemberPath = memberPath;
                    }
                }
            }
            else
            {
                var rect    = baseRect;
                rect.width *= ObjectFieldWidthRate;

                if(bo.IsOwnGameObjectOnly) EditorGUI.BeginDisabledGroup(true);
                {
                    GUI.Box(rect, "");
                    bo.Object = EditorGUI.ObjectField(rect, bo.Object, typeof(Object), true);
                }
                if(bo.IsOwnGameObjectOnly) EditorGUI.EndDisabledGroup();

                rect.x    += rect.width + Object2MemberSpace;
                rect.width = baseRect.width * MemberPopupWidthRate - Object2MemberSpace;

                var memeberIdx = Array.IndexOf(bo.MemberPaths, bo.MemberPath);
                memeberIdx     = Mathf.Max(memeberIdx, 0);
                memeberIdx     = EditorGUI.Popup(rect, memeberIdx, bo.MemberPaths);
                if(!Application.isPlaying)
                {
                    bo.MemberPath   = bo.MemberPaths[memeberIdx];
                    bo.SelectMember = bo.Members.Find(m => m.MemberPath == bo.MemberPath);
                }
            }

            EditorGUILayout.EndHorizontal();
        }
        
        void RefreshBinding()
        {
            if(Application.isPlaying) return;

            RefreshBindingObject(ReadObject,  true);
            RefreshBindingObject(WriteObject, false);

            if(binding.InitializeOnEditor && !ReadObject.IsDynamicBinding && !WriteObject.IsDynamicBinding)
            {
                ReadMember.BindObject  = ReadMember.Object;
                WriteMember.BindObject = WriteMember.Object;
                binding.InitializeMember(true);
            }
        }

        void RefreshBindingObject(M4uBindingObject bo, bool isRead)
        {
            if(bo.IsOwnGameObjectOnly) bo.Object = binding.gameObject;

            bo.Members.Clear();
            if(isRead)
            {
                var mes = bo.IsDynamicBinding ? EmptyDynamicReadMessage  : FirstSelectReadMessage;
                bo.Members.Add(new M4uBindingMember(mes));
            }
            else
            {
                var mes = bo.IsDynamicBinding ? EmptyDynamicWriteMessage : FirstSelectWriteMessage;
                bo.Members.Add(new M4uBindingMember(mes));
            }

            if(bo.Object is GameObject)
            {
                var components = ((GameObject)bo.Object).GetComponents<Component>();
                foreach(var component in components)
                {
                    LoadMembers(bo, component);
                }
            }
            else if(bo.Object != null)
            {
                LoadMembers(bo, bo.Object);
            }

            bo.MemberPaths = bo.Members.Select(m => m.MemberPath).ToArray();
        }

        void LoadMembers(M4uBindingObject bo, Object @object)
        {
            if(bo.IsNotifierOnly && !(@object is IM4uNotifier)) return;
            if(bo.IsDynamicBinding) return;

            var objectType  = @object.GetType();
            var objectName  = objectType.Name;
            var memberInfos = objectType.GetMembers(bo.BindingFlags);
            foreach(var memberInfo in memberInfos)
            {
                var memberType = M4uMemberType.None;
                if(memberInfo.MemberType == MemberTypes.Field)
                {
                    if(!bo.IsGetField) continue;
                    memberType = M4uMemberType.Field;
                }
                else if(memberInfo.MemberType == MemberTypes.Property)
                {
                    if(!bo.IsGetProperty) continue;
                    memberType = M4uMemberType.Property;
                }
                else
                {
                    continue;
                }

                var memberName = memberInfo.Name;
                var memberPath = $"{objectName}/{memberName}";
                var member     = new M4uBindingMember(memberPath, memberType, @object, memberName);
                bo.Members.Add(member);
            }
        }
    }
}
#endif