﻿﻿using System.Collections.Generic;
  using DataService.Model;

  namespace Connector.Dao
{
    public interface IDataDao
    {
        Form Save(Form form);
        
        Form Load(string formName, string requestId);

        List<Form> GetByStage(string stage);
    }
}