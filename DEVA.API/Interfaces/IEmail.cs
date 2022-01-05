using System.Threading.Tasks;
using DEVA.API.DTOs;

namespace DEVA.API.Interfaces
{
    public interface IEmail
    {
        Task Send(string emailAddress, string body, EmailOptionsDTO options);
    }
}