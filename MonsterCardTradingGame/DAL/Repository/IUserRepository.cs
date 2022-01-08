using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterCardTradingGame.DAL.Repository
{
    public interface IUserRepository
    {
        void Create(Model.Credentials cred); 
        IEnumerable<Model.User> GetAll();

        Model.User GetByName(string username);
        Model.User GetById(Guid id);
        Guid GetIdByUsername(string username);
        void Update(Model.User user);
        public void UpdateDeck(Guid userid, Guid deckid);

        void Delete(Model.User user);
    }
}
