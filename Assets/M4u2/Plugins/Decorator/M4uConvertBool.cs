using UnityEngine;
using System;

namespace M4u2
{
    /// <summary>
    /// バインド結果をboolに変換
    /// </summary>
    [AddComponentMenu(nameof(M4u2) + "/" + nameof(M4uConvertBool), 100)]
    public class M4uConvertBool : M4uDecorator
    {
        enum Condition 
        {
            NullOrEmpty, 
            Equal,
            Greater,
            GreaterOrEqual,
            Less,
            LessOrEqual 
        }
    
        [SerializeField] Condition condition;
        [SerializeField] string comparisonValue;
        [SerializeField] bool invertResult;

        public override bool Decorate(object notifier, string memberName, ref object memberValue)
        {
            var value = false;
            switch(condition)
            {
                case Condition.NullOrEmpty:
                    if(memberValue is string)
                    {
                        value = string.IsNullOrEmpty((string)memberValue);
                    }
                    else
                    {
                        value = (memberValue == null);
                    }
                    break;

                case Condition.Equal:
                    if(memberValue is Enum)
                    {
                        value = (memberValue.ToString() == comparisonValue);
                    }
                    else
                    {
                        value = memberValue.Equals(comparisonValue);
                    }
                    break;

                case Condition.Greater:
                case Condition.GreaterOrEqual:
                case Condition.Less:
                case Condition.LessOrEqual:
                    var number1 = 0f;
                    var number2 = 0f;
                    if(float.TryParse(memberValue.ToString(), out number1) && 
                       float.TryParse(comparisonValue,        out number2))
                    {
                        switch(condition)
                        {
                            case Condition.Greater:        value = (number1  > number2); break;
                            case Condition.GreaterOrEqual: value = (number1 >= number2); break;
                            case Condition.Less:           value = (number1  < number2); break;
                            case Condition.LessOrEqual:    value = (number1 <= number2); break;
                        }
                    }
                    else
                    {
                        value = false;
                    }
                    break;
            }

            if(invertResult) value = !value;
            memberValue = value;

            return true;
        }
    }
}