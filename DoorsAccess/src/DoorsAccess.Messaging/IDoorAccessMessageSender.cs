using System.Threading.Tasks;
using DoorsAccess.Messaging.Messages;

namespace DoorsAccess.Messaging
{
    public interface IDoorAccessMessageSender
    {
        Task SendAsync(DoorAccessEvent message);
        Task SendAsync(DoorAccessCommand message);
    }
}
