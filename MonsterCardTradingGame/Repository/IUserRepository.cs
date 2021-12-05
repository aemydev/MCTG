using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterCardTradingGame.DB.Repository
{
    public interface IUserRepository
    {
        bool Create(Model.User user); //void
        IEnumerable<Model.User> GetAllUser();
        Model.User GetByName(string username);
        Model.User GetById(int id); //DB ID
        void UpdateUser(Model.User user);
        void DeleteUser(Model.User user);
    }
}
