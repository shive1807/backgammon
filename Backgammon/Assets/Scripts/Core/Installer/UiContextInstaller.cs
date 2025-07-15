using Core.DI;
using Core.UiSystems;
using UnityEngine;

namespace Core.Installer
{
    public class UiContextInstaller : MonoInstaller
    {
        public override void InstallBindings(DiContainer container)
        {
            container.Bind<UiRoot>().AsSingle().NonLazy();
            
        }
    }
}
