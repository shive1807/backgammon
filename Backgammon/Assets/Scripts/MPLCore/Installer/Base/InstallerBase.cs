using MPLCore.DI;
using MPLCore.Installer.Interface;

namespace MPLCore.Installer.Base
{
    public abstract class InstallerBase : IInstaller
    {
        [Inject]
        private DiContainer _container;

        protected DiContainer Container => _container;

        public virtual bool IsEnabled => true;

        public abstract void InstallBindings();

        public bool isEnabled { get; }
    }
}
