namespace DoorsAccess.Domain.Utils
{
    public class Clock: IClock
    {
        public DateTime UtcNow() => DateTime.UtcNow;
    }
}
