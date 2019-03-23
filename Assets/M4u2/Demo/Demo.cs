using UnityEngine;

namespace M4u2
{
    public class Demo : M4uBindableBase
    {
        public const string Ability = "Good average hitter!";

        public enum Result { SwingMiss, Hit }

        [SerializeField] string message = "Thank you Ichiro.";
        [SerializeField] int hit = 262;
        [SerializeField] Result res;

        public int Hit    { get { return hit; } set { SetMember(ref hit, value); } }
        public Result Res { get { return res; } set { SetMember(ref res, value); } }

        public void OnUpdate()
        {
            Hit = Random.Range(0, 263);
            Res = (Res == Result.SwingMiss) ? Result.Hit : Result.SwingMiss;
        }
    }
}