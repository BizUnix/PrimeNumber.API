using AutoMapper;

namespace Client.DataStore
{
    public class ClientService : IClientService
    {
        private readonly ClientDataContext _context;
        private readonly int _rateLimit;

        public ClientService(int rateLimit, ClientDataContext context)
        {
            _context = context;
            _rateLimit = rateLimit;
        }

        public ClientLog FindCurentLog(string clientId, string timeStamp)
        {
            // Find logs for client
            return _context.Clients
                .Where(x => x.Client == clientId && x.TimeStamp == timeStamp)
                .FirstOrDefault();
        }

        public ClientLog FindLastLog(string clientId)
        {
            // Find logs for client
            return _context.Clients
                .Where(x => x.Client == clientId &&
                x.CreatedTime > DateTime.Now.AddHours(-1))
                .OrderByDescending(x => x.CreatedTime)
                .FirstOrDefault();
        }

        public void AddLog(ClientLog log)
        {
            // save log
            _context.Clients.Add(log);
            _context.SaveChanges();
        }

        public void DeleteOldLogs(int pastHours)
        {
            // Find and delete old logs
            var oldLogs = _context.Clients
                .Where(x => x.CreatedTime < DateTime.Now.AddHours(-2));
            _context.Clients.RemoveRange(oldLogs);
            _context.SaveChanges();
        }

        public bool HasRateLimitExceeded(string clientId, out double remainingMinutes)
        {
            var timeStamp = DateTime.Now.ToString("yyyyMMddHHmm");
            var currentLog = this.FindCurentLog(clientId, timeStamp);
            
            remainingMinutes = 0;

            if (currentLog != null && currentLog.BlockEnds == null)
            {
                if (currentLog.AccessCounts >= _rateLimit)
                {
                    currentLog.AccessCounts += 1;
                    currentLog.BlockEnds = DateTime.Now.AddHours(1);
                    _context.SaveChanges();
                    remainingMinutes = 60;
                    return true;
                }
                else
                {
                    currentLog.AccessCounts += 1;
                    _context.SaveChanges();
                    return false;
                }
            }

            var lastLog = this.FindLastLog(clientId);
            if (lastLog?.BlockEnds > DateTime.Now)
            {
                TimeSpan ts = (DateTime)lastLog.BlockEnds - DateTime.Now;
                remainingMinutes = ts.TotalMinutes;
                return true;
            }
            else
            {
                this.AddLog(new ClientLog()
                {
                    Client = clientId,
                    TimeStamp = timeStamp,
                    AccessCounts = 1,
                    BlockEnds = null,
                    CreatedTime = DateTime.Now
                });
            }

            return false;
        }

        ~ClientService()
        {
            this.DeleteOldLogs(1);
        }
    }
}
