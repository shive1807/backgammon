using UnityEngine;

namespace MPLCore.DI
{
    public abstract class MonoInstaller : MonoBehaviour
    {
        public abstract void InstallBindings(DiContainer container);
    }
}