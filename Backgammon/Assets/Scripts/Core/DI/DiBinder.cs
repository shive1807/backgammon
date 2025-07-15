// Core/DIBinder.cs

using System;
using UnityEngine;

namespace Core.DI
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
    }
}