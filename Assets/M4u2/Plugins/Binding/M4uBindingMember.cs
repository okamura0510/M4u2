using UnityEngine;
using System;
using System.Reflection;
using Object = UnityEngine.Object;

namespace M4u2
{
    public enum M4uMemberType { None, Field, Property }

    /// <summary>
    /// バインドメンバー(ReadMember/WriteMember)
    /// </summary>
    [Serializable]
    public class M4uBindingMember
    {
        [SerializeField] string memberPath;
        [SerializeField] M4uMemberType memberType;
        [SerializeField] Object @object;
        [SerializeField] string memberName;

        public string MemberPath         { get { return memberPath; } set { memberPath = value; } }
        public M4uMemberType MemberType  { get { return memberType; } set { memberType = value; } }
        public Object Object             { get { return @object;    } set { @object    = value; } }
        public string MemberName         { get { return memberName; } set { memberName = value; } }
        public object BindObject         { get; set; }
        public FieldInfo FieldInfo       { get; set; }
        public PropertyInfo PropertyInfo { get; set; }

        public M4uBindingMember(string memberPath)
        {
            this.memberPath = memberPath;
        }

        public M4uBindingMember(string memberPath, M4uMemberType memberType, Object @object, string memberName)
        {
            this.memberPath = memberPath;
            this.memberType = memberType;
            this.@object    = @object;
            this.memberName = memberName;
        }

        public void SetDynamicObject(M4uMemberType memberType, string memberName, object bindObject)
        {
            this.memberType = memberType;
            this.memberName = memberName;
            BindObject      = bindObject;
        }
    }
}