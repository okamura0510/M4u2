using UnityEngine;
using System;
using System.Collections.Generic;
using System.Reflection;
using Object = UnityEngine.Object;
using static M4u2.M4uBindingFlags;

namespace M4u2
{
    /// <summary>
    /// バインドオブジェクト(ReadObject/WriteObject)
    /// </summary>
    [Serializable]
    public class M4uBindingObject
    {
        [SerializeField, M4uBindingFlagsMaskField] M4uBindingFlags bindingFlags = 
            FlattenHierarchy | Instance | Static | Public | NonPublic | GetField | GetProperty;
        [SerializeField] M4uBindingMember selectMember;
#if UNITY_EDITOR
        [SerializeField] Object @object;
        [SerializeField] string memberPath;
        [SerializeField] string[] memberPaths;
        [SerializeField] List<M4uBindingMember> members = new List<M4uBindingMember>();
#endif

        public BindingFlags BindingFlags
        {
            get
            {
                var value = BindingFlags.Default;
                if(bindingFlags.HasFlag(DeclaredOnly))     value |= BindingFlags.DeclaredOnly;
                if(bindingFlags.HasFlag(FlattenHierarchy)) value |= BindingFlags.FlattenHierarchy;
                if(bindingFlags.HasFlag(Instance))         value |= BindingFlags.Instance;
                if(bindingFlags.HasFlag(Static))           value |= BindingFlags.Static;
                if(bindingFlags.HasFlag(Public))           value |= BindingFlags.Public;
                if(bindingFlags.HasFlag(NonPublic))        value |= BindingFlags.NonPublic;
                return value;
            }
        }
        public bool IsGetField                => bindingFlags.HasFlag(GetField);
        public bool IsGetProperty             => bindingFlags.HasFlag(GetProperty);
        public bool IsOwnGameObjectOnly       => bindingFlags.HasFlag(OwnGameObjectOnly);
        public bool IsNotifierOnly            => bindingFlags.HasFlag(NotifierOnly);
        public bool IsDynamicBinding          => bindingFlags.HasFlag(DynamicBinding);
        public M4uBindingMember SelectMember  { get { return selectMember; } set { selectMember = value; } }
#if UNITY_EDITOR
        public Object Object                  { get { return @object;      } set { @object      = value; } }
        public string MemberPath              { get { return memberPath;   } set { memberPath   = value; } }
        public string[] MemberPaths           { get { return memberPaths;  } set { memberPaths  = value; } }
        public List<M4uBindingMember> Members { get { return members;      } set { members      = value; } }
#endif

        public M4uBindingObject() { }
        public M4uBindingObject(M4uBindingFlags bindingFlags)
        {
            this.bindingFlags = bindingFlags;
        }
    }
}