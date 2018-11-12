using System.Security.Principal;

namespace ShoesOnContainers.Web.WebMvc.Services
{
    public interface IIdentityService<T>
    {
        T Get(IPrincipal principal);
    }
}