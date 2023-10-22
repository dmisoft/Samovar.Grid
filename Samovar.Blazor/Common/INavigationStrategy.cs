using System.Linq;
using System.Threading.Tasks;

namespace Samovar.Blazor
{
    //public interface INavigationStrategy { 
    //}
    public interface INavigationStrategy//<T> : INavigationStrategy
    {
        Task ProcessDataPrequery<T>(IQueryable<T> data);
        
        Task Activate();
        
        Task Deactivate();
    }
}
