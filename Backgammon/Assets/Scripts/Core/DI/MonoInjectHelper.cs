using System;
using System.Reflection;
using Core.Context;
using UnityEngine;

namespace Core.DI
{
    public static class MonoInjectHelper
    {
        public static void InjectIntoGameObject(GameObject target, DiContainer container = null)
        {
            container ??= ProjectContext.Container;
            
            MonoBehaviour[] components = target.GetComponentsInChildren<MonoBehaviour>();
            foreach (MonoBehaviour component in components)
            {
                InjectIntoObject(component, container);
            }
        }

        public static void InjectIntoObject(object target, DiContainer container = null)
        {
            container ??= ProjectContext.Container;
            
            Type targetType = target.GetType();
            
            // Inject into fields
            FieldInfo[] fields = targetType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (FieldInfo field in fields)
            {
                InjectAttribute injectAttr = field.GetCustomAttribute<InjectAttribute>();
                if (injectAttr != null)
                {
                    try
                    {
                        object value = container.Resolve(field.FieldType);
                        field.SetValue(target, value);
                    }
                    catch (System.Exception e)
                    {
                        if (!injectAttr.Optional)
                        {
                            Debug.LogError($"Failed to inject {field.Name} in {targetType.Name}: {e.Message}");
                        }
                    }
                }
            }

            // Inject into properties
            PropertyInfo[] properties = targetType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (PropertyInfo property in properties)
            {
                InjectAttribute injectAttr = property.GetCustomAttribute<InjectAttribute>();
                if (injectAttr != null && property.CanWrite)
                {
                    try
                    {
                        object value = container.Resolve(property.PropertyType);
                        property.SetValue(target, value);
                    }
                    catch (System.Exception e)
                    {
                        if (!injectAttr.Optional)
                        {
                            Debug.LogError($"Failed to inject {property.Name} in {targetType.Name}: {e.Message}");
                        }
                    }
                }
            }
        }
    }
}