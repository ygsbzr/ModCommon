using System;
using System.Collections.Generic;
using System.Reflection;
using JetBrains.Annotations;
using MonoMod.Utils;

namespace ModCommon.Util
{
    public static class ReflectionHelper
    {
        private static readonly Dictionary<Type, Dictionary<string, MethodInfo>> METHOD_INFOS =
            new Dictionary<Type, Dictionary<string, MethodInfo>>();

        public static MethodInfo GetMethodInfo(object obj, string name, bool instance = true)
        {
            if (obj == null || string.IsNullOrEmpty(name)) return null;

            Type t = obj.GetType();

            if (!METHOD_INFOS.ContainsKey(t))
            {
                METHOD_INFOS.Add(t, new Dictionary<string, MethodInfo>());
            }

            Dictionary<string, MethodInfo> typeInfos = METHOD_INFOS[t];

            if (!typeInfos.ContainsKey(name))
            {
                typeInfos.Add(name,
                    t.GetMethod(name,
                        BindingFlags.NonPublic | BindingFlags.Public |
                        (instance ? BindingFlags.Instance : BindingFlags.Static)));
            }

            return typeInfos[name];
        }

        public static T InvokeMethod<T>(object obj, string name, bool instance = true, params object[] args)
        {
            return (T) obj.GetMethodInfo(name, instance).GetFastDelegate().Invoke(obj, args);
        }
    }

    
    public static class ReflectionExtensions
    {
        [Obsolete("Use SetAttr<TObject, TField>")]
        public static void SetField<T>(this object obj, string name, T val, bool instance = true) =>
            Modding.ReflectionHelper.SetField(obj, name, val);

        [Obsolete("Use GetAttr<TType, TField>")]
        public static T GetField<T>(this object obj, string name, bool instance = true) =>
            Modding.ReflectionHelper.GetField<T>(obj.GetType(), name);
        
        [PublicAPI]
        public static TField GetField<TObject, TField>(this TObject obj, string name) =>
            Modding.ReflectionHelper.GetField<TObject, TField>(obj, name);
        
        [PublicAPI]
        public static void SetField<TObject, TField>(this TObject obj, string name, TField val) =>
            Modding.ReflectionHelper.SetField(obj, name, val);

        [PublicAPI]
        public static MethodInfo GetMethodInfo(this object obj, string name, bool instance = true) =>
            ReflectionHelper.GetMethodInfo(obj, name, instance);
        
        [PublicAPI]
        public static object InvokeMethod<T>(this object obj, string name, bool instance = true, params object[] args) => 
            ReflectionHelper.InvokeMethod<T>(obj, name, instance, args);
    }
}