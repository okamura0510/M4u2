using UnityEngine;

namespace M4u2
{
    /// <summary>
    /// シンプルバリデーション
    /// </summary>
    [AddComponentMenu(nameof(M4u2) + "/" + nameof(M4uSimpleValidation), 100)]
    public class M4uSimpleValidation : M4uConvertBool
    {
        public override bool Decorate(object notifier, string memberName, ref object memberValue)
        {
            var temp = memberValue;

            base.Decorate(notifier, memberName, ref memberValue);

            var isSuccess = (bool)memberValue;
            memberValue = temp;

            return isSuccess;
        }
    }
}