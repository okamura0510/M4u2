using UnityEngine;
using System.Runtime.CompilerServices;

namespace M4u2
{
    /// <summary>
    /// IM4uNotifierを継承したバインドクラス。本クラスを継承してSetMemberメソッドを呼ぶことで通知を行える。
    /// 本クラスじゃなくてもIM4uNotifierを継承すればバインドクラスは自由に作成可能。
    /// </summary>
    public abstract class M4uBindableBase : MonoBehaviour, IM4uNotifier
    {
        /// <summary>
        /// メンバーが変更されたことを通知
        /// </summary>
        public event M4uMemberChangedEvent MemberChanged;

        /// <summary>
        /// メンバー設定
        /// </summary>
        /// <param name="member">メンバー</param>
        /// <param name="value">値</param>
        /// <param name="memberName">メンバー名(CallerMemberNameで自動設定される)</param>
        protected void SetMember<T>(ref T member, T value, [CallerMemberName] string memberName = null)
        {
            if(Equals(member, value)) return;

            member = value;
            MemberChanged?.Invoke(this, memberName, value);
        }
    }
}