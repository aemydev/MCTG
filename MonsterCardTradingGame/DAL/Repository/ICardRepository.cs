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
        /*IEnumerable<Model.Card>*/
        void GetAll();

        List<string> GetAllCardIdsWithoutOwner();
        Model.Card GetById(int id); //DB ID

        void Update(Model.Card card);
        void Delete(Model.Card card);
    }
}
