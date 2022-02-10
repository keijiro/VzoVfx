using UnityEngine;
using System.Collections.Generic;

namespace VzoVfx {

public sealed class VzoNoteBuffer : MonoBehaviour
{
    #region Editable properties

    public enum Source { Note, CC }

    [SerializeField] VzoConfig _config = null;
    [SerializeField] Source _source = Source.Note;
    [SerializeField] int _channel = 0;

    #endregion

    #region Public properties

    public GraphicsBuffer NoteBuffer => _buffer;

    #endregion

    #region Private objects

    Queue<(int index, float level)> _queue = new Queue<(int, float)>();

    string _prefix;
    GraphicsBuffer _buffer;
    float[] _temp = new float[128];

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
        var dir = _source == Source.Note ? "note" : "cc";
        _prefix = $"/{dir}/{_channel}/";

        // Graphics buffer allocation
        _buffer = new GraphicsBuffer
          (GraphicsBuffer.Target.Structured, 128, sizeof(float));

        // OSC callback registration
        var server = OscJack.OscMaster.GetSharedServer(_config.UdpPort);
        server.MessageDispatcher.AddCallback(string.Empty, OnOscMessage);
    }

    void OnDestroy()
    {
        // OSC callback unregistration
        var server = OscJack.OscMaster.GetSharedServer(_config.UdpPort);
        server.MessageDispatcher.RemoveCallback(string.Empty, OnOscMessage);

        // Finalization
        _buffer.Dispose();
    }

    void Update()
    {
        // Avoid buffer update when the queue is empty.
        if (_queue.Count == 0) return;

        // Event pump
        lock (_queue)
        {
            while (_queue.Count > 0)
            {
                var (index, level) = _queue.Dequeue();
                _temp[index] = level;
            }
        }

        // CPU -> GPU
        _buffer.SetData(_temp);
    }

    #endregion
}

} // namespace VzoVfx
