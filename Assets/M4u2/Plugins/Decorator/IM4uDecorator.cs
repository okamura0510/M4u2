namespace M4u2
{
    /// <summary>
    /// バインド結果を装飾する
    /// </summary>
    public interface IM4uDecorator
    {
        /// <summary>
        /// バインド結果を装飾する
        /// </summary>
        /// <param name="notifier">通知者</param>
        /// <param name="memberName">メンバー名</param>
        /// <param name="memberValue">メンバー値</param>
        /// <returns>バインド可能か？falseの場合、バインドを無視する(Validation)</returns>
        bool Decorate(object notifier, string memberName, ref object memberValue);
    }
}