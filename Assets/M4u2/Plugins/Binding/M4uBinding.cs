using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace M4u2
{
    /// <summary>
    /// データバインドを担うビューモデル(MVVMの根幹クラス)
    /// </summary>
    [AddComponentMenu("M4u2/" + nameof(M4uBinding), 0)]
    public class M4uBinding : MonoBehaviour
    {
        [SerializeField] bool initializeOnEditor;
        [SerializeField] bool initializeOnRuntime = true;
        [SerializeField] M4uBindingObject readObject;
        [SerializeField] M4uBindingObject writeObject;
        List<IM4uDecorator> decorators = new List<IM4uDecorator>();
        bool isMemberInitialized;

        public bool InitializeOnEditor      => initializeOnEditor;
        public bool InitializeOnRuntime     => initializeOnRuntime;
        public M4uBindingObject ReadObject  => readObject;
        public M4uBindingObject WriteObject => writeObject;
        public M4uBindingMember ReadMember  => ReadObject.SelectMember;
        public M4uBindingMember WriteMember => WriteObject.SelectMember;

        void Awake()
        {
            if(ReadObject.IsDynamicBinding)
            {
                InitializeDynamicObject(ReadObject, ReadMember);
                InitializeDynamicMember(ReadObject, ReadMember);
            }
            else
            {
                ReadMember.BindObject = ReadMember.Object;
            }

            if(WriteObject.IsDynamicBinding)
            {
                InitializeDynamicObject(WriteObject, WriteMember);
            }
            else
            {
                WriteMember.BindObject = WriteMember.Object;
            }

            var notifier = ReadMember.BindObject as IM4uNotifier;
            if(notifier != null)
            {
                notifier.MemberChanged += OnMemberChanged;   
            }

            if(initializeOnRuntime)
            {
                InitializeMember(true);
            }
            else if(notifier != null && !WriteObject.IsDynamicBinding)
            {
                InitializeMember(false);
            }
        }

        void InitializeDynamicObject(M4uBindingObject bo, M4uBindingMember bm)
        {
            var memberNames   = bm.MemberPath.Replace('.', '/').Split('/');
            var componentName = memberNames[0];
            var transform     = this.transform;
            var @object       = default(object);
            do
            {
                var component = transform.GetComponent(componentName);
                if(component != null && (!bo.IsNotifierOnly || component is IM4uNotifier))
                {
                    @object = component;
                    break;
                }
                transform = transform.parent;
            }
            while(transform != null && !bo.IsOwnGameObjectOnly);

            bm.BindObject = @object;
            bm.MemberName = memberNames.Last();
        }

        void InitializeDynamicMember(M4uBindingObject bo, M4uBindingMember bm)
        {
            if(bm.BindObject == null) return;

            var memberNames = bm.MemberPath.Replace('.', '/').Split('/');
            var @object     = bm.BindObject;
            for(var i = 1; i < memberNames.Length; i++)
            {
                var isLast     = (i == memberNames.Length - 1);
                var memberName = memberNames[i];
                var objectType = @object.GetType();

                if(isLast)
                {
                    var memberInfos = objectType.GetMember(memberName, bo.BindingFlags);
                    if(memberInfos != null && memberInfos.Length >= 1)
                    {
                        var memberType = memberInfos[0].MemberType;
                        if(memberType == MemberTypes.Field)
                        {
                            bm.SetDynamicObject(M4uMemberType.Field, memberName, @object);
                        }
                        else if(memberType == MemberTypes.Property)
                        {
                            bm.SetDynamicObject(M4uMemberType.Property, memberName, @object);
                        }
                    }
                }
                else
                {
                    var nowObject = default(object);
                    if(bo.IsGetProperty)
                    {
                        var propertyInfo = objectType.GetProperty(memberName, bo.BindingFlags);
                        nowObject        = propertyInfo?.GetValue(@object);
                    }

                    if(bo.IsGetField && nowObject == null)
                    {
                        var fieldInfo = objectType.GetField(memberName, bo.BindingFlags);
                        nowObject     = fieldInfo?.GetValue(@object);
                    }

                    if(nowObject == null) break;
                    @object = nowObject;
                }
            }
        }

        public void InitializeMember(bool isInitializeValue)
        {
            isMemberInitialized = true;

            InitializeDecorator();
            if(WriteObject.IsDynamicBinding) InitializeDynamicMember(WriteObject, WriteMember);

            if(WriteMember.BindObject != null)
            {
                if(WriteMember.MemberType == M4uMemberType.Field)
                {
                    WriteMember.FieldInfo = WriteMember.BindObject.GetType().GetField(
                        WriteMember.MemberName, WriteObject.BindingFlags);
                }
                else if(WriteMember.MemberType == M4uMemberType.Property)
                {
                    WriteMember.PropertyInfo = WriteMember.BindObject.GetType().GetProperty(
                        WriteMember.MemberName, WriteObject.BindingFlags);
                }
            }

            if(ReadMember.BindObject != null && isInitializeValue)
            {
                var value = default(object);
                if(ReadMember.MemberType == M4uMemberType.Field)
                {
                    var fieldInfo = ReadMember.BindObject.GetType().GetField(
                        ReadMember.MemberName, ReadObject.BindingFlags);
                    
                    value = fieldInfo?.GetValue(ReadMember.BindObject);
                }
                else if(ReadMember.MemberType == M4uMemberType.Property)
                {
                    var propertyInfo = ReadMember.BindObject.GetType().GetProperty(
                        ReadMember.MemberName, ReadObject.BindingFlags);
                    
                    value = propertyInfo?.GetValue(ReadMember.BindObject);
                }

                if(value != null)
                {
                    OnMemberChanged(ReadMember.BindObject, ReadMember.MemberName, value);   
                }
            }
        }

        void InitializeDecorator()
        {
            decorators.Clear();
            var binding = default(M4uBinding);
            foreach(var component in GetComponents<Component>())
            {
                if(component is M4uBinding)
                {
                    binding = (M4uBinding)component;
                }
                else if(component is IM4uDecorator && binding == this)
                {
                    decorators.Add((IM4uDecorator)component);
                }
            }
        }

        void OnMemberChanged(object notifier, string memberName, object memberValue)
        {
            if(!isMemberInitialized) InitializeMember(false);

            if(notifier.Equals(ReadMember.BindObject) && memberName == ReadMember.MemberName)
            {
                foreach(var decorator in decorators)
                {
                    if(!decorator.Decorate(notifier, memberName, ref memberValue))
                    {
                        return;
                    }
                }

                if(WriteMember.FieldInfo != null)
                {
                    WriteMember.FieldInfo.SetValue(WriteMember.BindObject, memberValue);
                }
                else if(WriteMember.PropertyInfo != null)
                {
                    WriteMember.PropertyInfo.SetValue(WriteMember.BindObject, memberValue);
                }
            }
        }
    }
}