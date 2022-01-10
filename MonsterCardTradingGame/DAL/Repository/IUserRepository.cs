using System;
using System.Collections.Generic;

namespace MonsterCardTradingGame.DAL.Repository
{
    public interface IUserRepository
    {
        /*
         *  Create
         */
        void Create(Model.Credentials cred);

        /*
         *  Read
         */
        IEnumerable<Model.User> GetAll();
        Model.User GetByName(string username);
        //Model.User GetById(Guid id);
        Guid GetIdByUsername(string username);

        /*
         *  Update
         */
        public void UpdateDeck(Guid userid, Guid deckid);
    }
}
