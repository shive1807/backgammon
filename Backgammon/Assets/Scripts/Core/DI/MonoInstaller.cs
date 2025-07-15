using UnityEngine;

namespace Core.DI
{
    public abstract class MonoInstaller : MonoBehaviour
    {
        public abstract void InstallBindings(DiContainer container);
    }
}