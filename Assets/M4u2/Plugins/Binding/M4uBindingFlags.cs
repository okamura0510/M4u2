using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif 

namespace M4u2
{
    /// <summary>
    /// バインドフラグ(System.Reflection.BindingFlagsをラップしたフラグ)
    /// </summary>
    [Flags]
    public enum M4uBindingFlags
    {
        /// <summary>
        /// <see cref="System.Reflection.BindingFlags.DeclaredOnly"/>
        /// 指定した型の階層のレベルで宣言されたメンバーのみを対象(継承メンバーは除外)
        /// </summary>
        DeclaredOnly      = 1 << 0,
        /// <summary>
        /// <see cref="System.Reflection.BindingFlags.FlattenHierarchy"/>
        /// 階層上位のパブリックおよびプロテクトの静的メンバーを返す(継承クラスのプライベートな静的メンバーは除外)
        /// </summary>
        FlattenHierarchy  = 1 << 1,
        /// <summary>
        /// <see cref="System.Reflection.BindingFlags.Instance"/>
        /// インスタンスメンバーを含める
        /// </summary>
        Instance          = 1 << 2,
        /// <summary>
        /// <see cref="System.Reflection.BindingFlags.Static"/>
        /// 静的メンバーを含める
        /// </summary>
        Static            = 1 << 3,
        /// <summary>
        /// <see cref="System.Reflection.BindingFlags.Public"/>
        /// パブリックメンバーを含める
        /// </summary>
        Public            = 1 << 4,
        /// <summary>
        /// <see cref="System.Reflection.BindingFlags.NonPublic"/>
        /// パブリックメンバー以外のメンバーを含める
        /// </summary>
        NonPublic         = 1 << 5,
        /// <summary>
        /// <see cref="System.Reflection.BindingFlags.GetField"/>
        /// フィールドメンバーを含める
        /// </summary>
        GetField          = 1 << 6,
        /// <summary>
        /// <see cref="System.Reflection.BindingFlags.GetProperty"/>
        /// プロパティメンバーを含める
        /// </summary>
        GetProperty       = 1 << 7,
        /// <summary>
        /// M4u2オリジナルフラグ
        /// 自ゲームオブジェクトのみを対象とする(WriteObjectに入れると便利)
        /// </summary>
        OwnGameObjectOnly = 1 << 8,
        /// <summary>
        /// M4u2オリジナルフラグ
        /// IM4uNotifierを継承したクラスのみを対象とする(ReadObjectに入れると便利)
        /// </summary>
        NotifierOnly      = 1 << 9,
        /// <summary>
        /// M4u2オリジナルフラグ
        /// ダイナミックバインディングを有効にする。パスの入力を文字列で行えるようになる(入れ子のパスに対応可能になる)。
        /// </summary>
        DynamicBinding    = 1 << 10
    }

    public class M4uBindingFlagsMaskFieldAttribute : PropertyAttribute { }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(M4uBindingFlagsMaskFieldAttribute))]
    class M4uBindingFlagsMaskFieldDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            property.intValue = EditorGUI.MaskField(position, label, property.intValue, property.enumNames);
        }
    }
#endif
}