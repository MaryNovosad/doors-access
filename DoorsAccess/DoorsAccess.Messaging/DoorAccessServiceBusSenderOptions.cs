namespace DoorsAccess.Messaging
{
    public class DoorAccessServiceBusSenderOptions
    {
        public string ConnectionString { get; set; }
        public string DoorsAccessEventsTopic { get; set; }
        public string DoorsAccessCommandsQueue { get; set; }
    }
}