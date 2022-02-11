using UnityEngine;
using System.Collections.Generic;

namespace Vzo.Vfx {

public sealed class VzoTrigger : MonoBehaviour
{
    #region Editable properties

    [System.Serializable]
    sealed class NoteEvent : UnityEngine.Events.UnityEvent<int, float> {}

    [SerializeField] VzoConfig _config = null;
    [SerializeField] int _channel = 0;
    [SerializeField] NoteEvent _noteEvent = null;

    #endregion

    #region Private objects

    Queue<(int index, float level)> _queue = new Queue<(int, float)>();

    string _prefix;

    #endregion

    #region OSC callback

    void OnOscMessage(string address, OscJack.OscDataHandle data)
    {
        // Note that this will be invoked from non-main threads.
        if (!address.StartsWith(_prefix)) return;
        var index = int.Parse(address.Substring(_prefix.Length));
        var level = data.GetElementAsFloat(0);
        lock (_queue) _queue.Enqueue((index, level));
    }

    #endregion

    #region MonoBehaviour implementation

    void Start()
    {
        // OSC address prefix
        _prefix = $"/note/{_channel}/";

        // OSC callback registration
        var server = OscJack.OscMaster.GetSharedServer(_config.UdpPort);
        server.MessageDispatcher.AddCallback(string.Empty, OnOscMessage);
    }

    void OnDestroy()
    {
        // OSC callback unregistration
        var server = OscJack.OscMaster.GetSharedServer(_config.UdpPort);
        server.MessageDispatcher.RemoveCallback(string.Empty, OnOscMessage);
    }

    void Update()
    {
        // Event pump
        while (_queue.Count > 0)
        {
            var (index, level) = (0, 0.0f);
            lock (_queue) { (index, level) = _queue.Dequeue(); }
            _noteEvent.Invoke(index, level);
        }
    }

    #endregion
}

} // namespace Vzo.Vfx
