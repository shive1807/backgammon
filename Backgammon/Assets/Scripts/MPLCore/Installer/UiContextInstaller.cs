using MPLCore.DI;
using MPLCore.UiSystems;
using UnityEngine;

namespace MPLCore.Installer
{
    public class UiContextInstaller : MonoInstaller
    {
        public override void InstallBindings(DiContainer container)
        {
            container.Bind<UiRoot>().AsSingle().NonLazy();
            
        }
    }
}
