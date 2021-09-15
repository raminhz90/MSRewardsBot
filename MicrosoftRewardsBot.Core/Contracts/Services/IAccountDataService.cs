using System.Collections.Generic;
using System.Threading.Tasks;

using MSRewardsBOT.Core.Models;

namespace MSRewardsBOT.Core.Contracts.Services
{
    public interface IAccountDataService
    {
        Task<IEnumerable<Account>> GetGridDataAsync();

        Task<IEnumerable<Account>> GetListDetailsDataAsync();

        void SaveDatabase(IEnumerable<Account> accounts);

        void UpdateAccount(Account account);
    }
}
