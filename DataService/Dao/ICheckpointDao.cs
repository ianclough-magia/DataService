using System.Collections.Generic;
using DataService.Model;

namespace Connector.Dao
{
    public interface ICheckpointDao
    {
        Checkpoint GetCheckpointData(string userId, string formName);

        string SetCheckpointData(string userId, string formName, string checkpointData);

        bool DeleteCheckpoint(string userId, string formName);
        IEnumerable<Checkpoint> ListCheckpoints(string userId);
    }
}