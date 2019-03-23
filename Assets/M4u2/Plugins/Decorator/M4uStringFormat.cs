using UnityEngine;

namespace M4u2
{
    /// <summary>
    /// バインド結果をフォーマットする
    /// </summary>
    [AddComponentMenu(nameof(M4u2) + "/" + nameof(M4uStringFormat), 100)]
    public class M4uStringFormat : M4uDecorator
    {
        [SerializeField] string format = "{0}";

        public override bool Decorate(object notifier, string memberName, ref object memberValue)
        {
            memberValue = string.Format(format, memberValue);
            return true;
        }
    }
}