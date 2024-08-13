using System;
using System.Reflection;
using UnityEngine;

public static class GameObjectExtension
{
    public static void Clone(this GameObject from, GameObject to, Type[] types = null, BindingFlags flags = ~BindingFlags.Default) {
        if (to == null) {
           return; 
        }
        
        foreach (Type type in types) {
            if (from.GetComponent(type) == null) {
                continue;
            }
            
            if (to.GetComponent(type) == null) {
                to.AddComponent(type);
            }
            
            var originalComponent = from.GetComponent(type);
            var originalType = originalComponent.GetType();
            var copyComponent = to.GetComponent(type);

            FieldInfo[] fieldsInfo = originalType.GetFields(flags);
            try {
                foreach (var info in fieldsInfo) {
                    info.SetValue(copyComponent, info.GetValue(originalComponent), flags, null, null);
                }

                PropertyInfo[] propertiesInfo = originalType.GetProperties(flags);

                foreach (var info in propertiesInfo) {
                    info.SetValue(copyComponent, info.GetValue(originalComponent), flags, null, null, null);
                }
            }
            catch
            {
            continue;
            }
        }
    }
}