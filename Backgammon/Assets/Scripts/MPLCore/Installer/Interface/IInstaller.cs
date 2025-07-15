namespace MPLCore.Installer.Interface
{
    public interface IInstaller 
    {
        void InstallBindings();
        
        bool isEnabled { get; }
    }
}