using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterCardTradingGame.DAL.Repository
{
    interface ICardRepository
    {
        void Create(Model.Card card);
        IEnumerable<Model.Card> GetAllUser();
        Model.Card GetByName(string name);
        Model.Card GetById(int id); //DB ID
        void UpdateUser(Model.Card card);
        void DeleteUser(Model.Card card);
    }
}
