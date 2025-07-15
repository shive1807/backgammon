// Core/DIBinder.cs

using System;
using UnityEngine;

namespace MPLCore.DI
{
    public class DiBinder<T>
    {
        private readonly DiContainer _container;
        private DiContainer.BindingScope _scope = DiContainer.BindingScope.Transient;

        internal DiBinder(DiContainer container)
        {
            this._container = container;
        }

        // Basic bindings first
        public DiBinder<T> To<TImpl>() where TImpl : class, T
        {
            _container.RegisterBinding<T>(typeof(TImpl), _scope);
            return this;
        }

        public DiBinder<T> FromInstance(T instance)
        {
            _container.BindInstance(instance);
            return this;
        }

        public DiBinder<T> FromMethod(Func<DiContainer, T> factory)
        {
            _container.RegisterFactory<T>(c => factory(c), _scope);
            return this;
        }

        // Test this one first
        public DiBinder<T> TestMethod()
        {
            // Just to test if T is recognized
            var typeName = typeof(T).Name;
            Debug.Log($"Type T is: {typeName}");
            return this;
        }

        public DiBinder<T> AsSingle()
        {
            _scope = DiContainer.BindingScope.Singleton;
            return this;
        }

        public DiBinder<T> AsTransient()
        {
            _scope = DiContainer.BindingScope.Transient;
            return this;
        }

        public DiBinder<T> NonLazy()
        {
            _container.MarkAsNonLazy<T>();
            return this;
        }

        /// <summary>
        /// Creates a GameObject with the class name, adds the component, and binds it as singleton.
        /// Only works with MonoBehaviour-derived classes.
        /// </summary>
        public DiBinder<T> FromNewGameObject()
        {
            // Check if T is assignable from MonoBehaviour
            if (!typeof(UnityEngine.MonoBehaviour).IsAssignableFrom(typeof(T)))
            {
                throw new System.ArgumentException($"Type {typeof(T).Name} must inherit from MonoBehaviour to use FromNewGameObject()");
            }
            
            // Create GameObject with class name
            string gameObjectName = typeof(T).Name;
            var gameObject = new UnityEngine.GameObject(gameObjectName);
            
            // Add component to GameObject using reflection
            var component = (T)(object)gameObject.AddComponent(typeof(T));
            
            // Mark as DontDestroyOnLoad for persistence
            UnityEngine.Object.DontDestroyOnLoad(gameObject);
            
            // Bind the component instance
            _container.BindInstance(component);
            
            UnityEngine.Debug.Log($"[DIBinder] Created GameObject '{gameObjectName}' with component {typeof(T).Name}");
            
            return this;
        }

        /// <summary>
        /// Creates a GameObject with custom name, adds the component, and binds it as singleton.
        /// Only works with MonoBehaviour-derived classes.
        /// </summary>
        public DiBinder<T> FromNewGameObject(string customName)
        {
            // Check if T is assignable from MonoBehaviour
            if (!typeof(UnityEngine.MonoBehaviour).IsAssignableFrom(typeof(T)))
            {
                throw new System.ArgumentException($"Type {typeof(T).Name} must inherit from MonoBehaviour to use FromNewGameObject()");
            }
            
            // Create GameObject with custom name
            var gameObject = new UnityEngine.GameObject(customName);
            
            // Add component to GameObject using reflection
            var component = (T)(object)gameObject.AddComponent(typeof(T));
            
            // Mark as DontDestroyOnLoad for persistence
            UnityEngine.Object.DontDestroyOnLoad(gameObject);
            
            // Bind the component instance
            _container.BindInstance(component);
            
            UnityEngine.Debug.Log($"[DIBinder] Created GameObject '{customName}' with component {typeof(T).Name}");
            
            return this;
        }

        /// <summary>
        /// Creates a GameObject as child of specified parent, adds the component, and binds it.
        /// Only works with MonoBehaviour-derived classes.
        /// </summary>
        public DiBinder<T> FromNewGameObject(UnityEngine.Transform parent)
        {
            // Check if T is assignable from MonoBehaviour
            if (!typeof(UnityEngine.MonoBehaviour).IsAssignableFrom(typeof(T)))
            {
                throw new System.ArgumentException($"Type {typeof(T).Name} must inherit from MonoBehaviour to use FromNewGameObject()");
            }
            
            // Create GameObject with class name as child of parent
            string gameObjectName = typeof(T).Name;
            var gameObject = new UnityEngine.GameObject(gameObjectName);
            gameObject.transform.SetParent(parent);
            
            // Add component to GameObject using reflection
            var component = (T)(object)gameObject.AddComponent(typeof(T));
            
            // Bind the component instance
            _container.BindInstance(component);
            
            UnityEngine.Debug.Log($"[DIBinder] Created GameObject '{gameObjectName}' under parent '{parent.name}' with component {typeof(T).Name}");
            
            return this;
        }
    }
}