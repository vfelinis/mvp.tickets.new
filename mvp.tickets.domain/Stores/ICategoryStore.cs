using mvp.tickets.domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mvp.tickets.domain.Stores
{
    public interface ICategoryStore
    {
        Task<IBaseQueryResponse<IEnumerable<ICategoryModel>>> Query(ICategoryQueryRequest request);
        Task<IBaseCommandResponse<int>> Create(ICategoryCreateCommandRequest request);
        Task<IBaseCommandResponse<bool>> Update(ICategoryUpdateCommandRequest request);
    }
}
