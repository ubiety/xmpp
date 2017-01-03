using Ubiety.Common;
using Ubiety.Core.SM;
using Ubiety.Registries;

namespace Ubiety.States
{
    /// <summary>
    ///
    /// </summary>
    public class StreamManagementState : IState
    {
        public void Execute(Tag data = null)
        {
            if (data == null)
            {
                var enable = TagRegistry.GetTag<Enable>("enable", Namespaces.StreamManagementV3);
                ProtocolState.Socket.Write(enable);
            }
            else
            {
                ProtocolState.State = new RunningState();
            }
        }
    }
}