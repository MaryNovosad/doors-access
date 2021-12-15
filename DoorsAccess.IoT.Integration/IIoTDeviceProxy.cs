namespace DoorsAccess.IoT.Integration
{
    public interface IIoTDeviceProxy
    {
        void OpenDoor(long userId, long doorId);
    }
}