using DataService.Model;

namespace Connector.Dao
{
    public interface ICheckpointDao
    {
        Checkpoint GetCheckpointData(string userId, string formName);

        string SetCheckpointData(string userId, string formName, string checkpointData);
    }
}