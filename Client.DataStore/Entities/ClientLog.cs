namespace Client.DataStore
{
    public class ClientLog
    {
        public int Id { get; set; }
        
        public string Client { get; set; }

        public string TimeStamp { get; set; }

        public int AccessCounts { get; set; }

        public DateTime? BlockEnds { get; set; }

        public DateTime CreatedTime { get; set; }
    }
}