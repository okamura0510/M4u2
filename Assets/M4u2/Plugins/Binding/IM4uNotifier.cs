namespace M4u2
{
    /// <summary>
    /// メンバーが変更されたことをM4uBindingに通知
    /// </summary>
    public interface IM4uNotifier
    {
        /// <summary>
        /// メンバーが変更されたことを通知
        /// </summary>
        event M4uMemberChangedEvent MemberChanged;
    }

    /// <summary>
    /// メンバーが変更されたことを通知
    /// </summary>
    /// <param name="notifier">通知者</param>
    /// <param name="memberName">メンバー名</param>
    /// <param name="memberValue">メンバー値</param>
    public delegate void M4uMemberChangedEvent(object notifier, string memberName, object memberValue);
}