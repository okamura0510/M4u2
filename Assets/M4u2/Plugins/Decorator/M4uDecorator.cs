using UnityEngine;

namespace M4u2
{
    /// <summary>
    /// IM4uDecoratorを継承した装飾クラス。本クラスを継承してDecorateメソッドを実装することでバインド結果を装飾可能。
    /// 本クラスじゃなくてもIM4uDecoratorを継承さえすれば装飾クラスは自由に作成可能。
    /// </summary>
    public abstract class M4uDecorator : MonoBehaviour, IM4uDecorator
    {
        /// <summary>
        /// バインド結果を装飾する
        /// </summary>
        /// <param name="notifier">通知者</param>
        /// <param name="memberName">メンバー名</param>
        /// <param name="memberValue">メンバー値</param>
        /// <returns>バインド可能か？false の場合、データバインドを無視する(Validation)</returns>
        public abstract bool Decorate(object notifier, string memberName, ref object memberValue);
    }
}