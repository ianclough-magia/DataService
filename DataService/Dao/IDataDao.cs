﻿﻿using System.Collections.Generic;
  using DataService.Model;

  namespace Connector.Dao
{
    public interface IDataDao
    {
        string Save(string formName, string stage, string userId, string jsonData);

        void Save(string formName, string requestId, string stage, string userId, string jsonData);

        Form Load(string formName, string requestId);

        List<Form> GetByStage(string stage);
    }
}