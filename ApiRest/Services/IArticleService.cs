using ApiRest.Dto;
using System.Threading.Tasks;

namespace ApiRest.Services
{
    public interface IArticleService
    {
        Task<UserDto> GetById(int id);
    }
}
