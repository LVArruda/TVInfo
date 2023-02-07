using Models;

namespace Infrastructure.Interfaces
{
    public interface ITVMazeService
    {
        public Task<IEnumerable<TVShow>> GetTVshowsPerPage(int page);
    }
}
