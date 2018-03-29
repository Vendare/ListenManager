using System.Security;

namespace ListenManager.Interfaces
{
    public interface IHavePassword
    { 
        SecureString Passwort { get; }
    }
}
