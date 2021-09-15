using MSRewardsBOT.Core.Contracts.Services;
using MSRewardsBOT.Core.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MSRewardsBOT.Core.Services
{

    public class AccountDataService : IAccountDataService
    {
        public AccountDataService()
        {
        }

        public void UpdateAccount(Account account)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            using (db)
            {
                db.Update(account);
                db.SaveChanges();
            }
        }



        private static IEnumerable<Account> AllAccounts()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            using (db)
            {
                return db.Accounts.ToList();
            }

        }

        // Remove this once your DataGrid pages are displaying real data.
        public async Task<IEnumerable<Account>> GetGridDataAsync()
        {
            await Task.CompletedTask;
            return AllAccounts();
        }

        // Remove this once your ListDetails pages are displaying real data.
        public async Task<IEnumerable<Account>> GetListDetailsDataAsync()
        {
            await Task.CompletedTask;
            return AllAccounts();
        }

        public void SaveDatabase(IEnumerable<Account> accounts)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            using (db)
            {
                foreach (var item in accounts)
                {
                    if (db.Accounts.Any(c => c.ID == item.ID))
                    {
                        db.Accounts.Update(item);
                    }
                    else
                    {
                        db.Accounts.Add(item);
                    }

                }

                db.SaveChanges();
            }
        }


    }
}
