﻿﻿namespace Connector.Dao
{
    public interface IDataDao
    {
        void Save(string userId, string formName, string jsonData);

        string Load(string userId, string formName);
    }
}