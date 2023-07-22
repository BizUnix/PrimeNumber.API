using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.DataStore
{
    public interface IClientService
    {
        ClientLog FindCurentLog(string clientId, string timeStamp);

        ClientLog FindLastLog(string clientId);

        void AddLog(ClientLog model);

        void DeleteOldLogs(int pastHours);

        bool HasRateLimitExceeded(string token, out double remainingTime);
    }
}
