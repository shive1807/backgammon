using System;
using System.Collections.Generic;
using System.Reflection;
//Di container core.
namespace Core.DI
{
    // ReSharper disable once InconsistentNaming
    public class DiContainer : IDisposable
    {
        private readonly Dictionary<Type, object>                    _singletonInstances = new();
        private readonly Dictionary<Type, Type>                      _typeBindings       = new();
        private readonly Dictionary<Type, object>                    _constantBindings   = new();
        private readonly Dictionary<Type, Func<DiContainer, object>> _factoryBindings    = new();
        private readonly Dictionary<Type, BindingScope>              _bindingScopes      = new();
        private readonly HashSet<Type>                               _currentlyResolving = new();

        public enum BindingScope
        {
            Transient,      // New instance every time
            Singleton,      // Single instance per container
            Cached          // Cached but can be disposed
        }

        // Bind interface to implementation
        public DiBinder<T> Bind<T>()
        {
            return new DiBinder<T>(this);
        }

        // Bind to a constant instance
        public void BindInstance<T>(T instance)
        {
            _constantBindings[typeof(T)] = instance;
            _bindingScopes[typeof(T)] = BindingScope.Singleton;
        }

        // Bind to a factory method
        public void BindFactory<T>(Func<DiContainer, T> factory)
        {
            _factoryBindings[typeof(T)] = container => factory(container);
            _bindingScopes[typeof(T)] = BindingScope.Transient;
        }

        // Internal binding registration
        internal void RegisterBinding<T>(Type implementationType, BindingScope scope)
        {
            _typeBindings[typeof(T)] = implementationType;
            _bindingScopes[typeof(T)] = scope;
        }

        internal void RegisterFactory<T>(Func<DiContainer, object> factory, BindingScope scope)
        {
            _factoryBindings[typeof(T)] = factory;
            _bindingScopes[typeof(T)] = scope;
        }

        // Resolve dependencies
        public T Resolve<T>()
        {
            return (T)Resolve(typeof(T));
        }

        public object Resolve(Type type)
        {
            // Check for circular dependencies
            if (_currentlyResolving.Contains(type))
            {
                throw new InvalidOperationException($"Circular dependency detected for {type.Name}");
            }

            try
            {
                _currentlyResolving.Add(type);
                return ResolveInternal(type);
            }
            finally
            {
                _currentlyResolving.Remove(type);
            }
        }

        private object ResolveInternal(Type type)
        {
            // 1. Check for constant bindings
            if (_constantBindings.TryGetValue(type, out object constantInstance))
            {
                return constantInstance;
            }

            // 2. Check for singleton instances
            if (_bindingScopes.TryGetValue(type, out BindingScope scope) && 
                scope == BindingScope.Singleton && 
                _singletonInstances.TryGetValue(type, out object singletonInstance))
            {
                return singletonInstance;
            }

            // 3. Check for factory bindings
            if (_factoryBindings.TryGetValue(type, out Func<DiContainer, object> factory))
            {
                object instance = factory(this);
                if (scope == BindingScope.Singleton)
                {
                    _singletonInstances[type] = instance;
                }
                return instance;
            }

            // 4. Check for type bindings
            if (_typeBindings.TryGetValue(type, out Type implementationType))
            {
                object instance = CreateInstance(implementationType);
                if (scope == BindingScope.Singleton)
                {
                    _singletonInstances[type] = instance;
                }
                return instance;
            }

            // 5. Try to create concrete type directly
            if (!type.IsAbstract && !type.IsInterface)
            {
                return CreateInstance(type);
            }

            throw new InvalidOperationException($"No binding found for {type.Name}");
        }

        private object CreateInstance(Type type)
        {
            // Find the best constructor (with most parameters we can resolve)
            ConstructorInfo[] constructors = type.GetConstructors();
            ConstructorInfo bestConstructor = null;
            int maxResolvableParams = -1;

            foreach (ConstructorInfo constructor in constructors)
            {
                ParameterInfo[] parameters = constructor.GetParameters();
                int resolvableParams = 0;

                foreach (ParameterInfo param in parameters)
                {
                    if (CanResolve(param.ParameterType))
                        resolvableParams++;
                    else
                        break;
                }

                if (resolvableParams == parameters.Length && resolvableParams > maxResolvableParams)
                {
                    bestConstructor = constructor;
                    maxResolvableParams = resolvableParams;
                }
            }

            if (bestConstructor == null)
            {
                throw new InvalidOperationException($"No suitable constructor found for {type.Name}");
            }

            // Resolve constructor parameters
            ParameterInfo[] constructorParams = bestConstructor.GetParameters();
            object[] args = new object[constructorParams.Length];

            for (int i = 0; i < constructorParams.Length; i++)
            {
                args[i] = Resolve(constructorParams[i].ParameterType);
            }

            return Activator.CreateInstance(type, args);
        }

        private bool CanResolve(Type type)
        {
            return _constantBindings.ContainsKey(type) ||
                   _typeBindings.ContainsKey(type) ||
                   _factoryBindings.ContainsKey(type) ||
                   (!type.IsAbstract && !type.IsInterface);
        }

        public void Dispose()
        {
            foreach (object instance in _singletonInstances.Values)
            {
                if (instance is IDisposable disposable)
                    disposable.Dispose();
            }

            _singletonInstances.Clear();
            _typeBindings.Clear();
            _constantBindings.Clear();
            _factoryBindings.Clear();
            _bindingScopes.Clear();
        }
    }
}
