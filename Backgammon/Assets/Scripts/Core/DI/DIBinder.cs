// Core/DIBinder.cs

using System;
using UnityEngine;

namespace Core.DI
{
    // ReSharper disable once InconsistentNaming
    public class DIBinder<T>
    {
        private readonly DiContainer _container;
        private DiContainer.BindingScope _scope = DiContainer.BindingScope.Transient;

        internal DIBinder(DiContainer container)
        {
            this._container = container;
        }

        // Basic bindings first
        public DIBinder<T> To<TImpl>() where TImpl : class, T
        {
            _container.RegisterBinding<T>(typeof(TImpl), _scope);
            return this;
        }

        public DIBinder<T> FromInstance(T instance)
        {
            _container.BindInstance(instance);
            return this;
        }

        public DIBinder<T> FromMethod(Func<DiContainer, T> factory)
        {
            _container.RegisterFactory<T>(c => factory(c), _scope);
            return this;
        }

        // Test this one first
        public DIBinder<T> TestMethod()
        {
            // Just to test if T is recognized
            var typeName = typeof(T).Name;
            Debug.Log($"Type T is: {typeName}");
            return this;
        }

        public DIBinder<T> AsSingle()
        {
            _scope = DiContainer.BindingScope.Singleton;
            return this;
        }

        public DIBinder<T> AsTransient()
        {
            _scope = DiContainer.BindingScope.Transient;
            return this;
        }
    }
}