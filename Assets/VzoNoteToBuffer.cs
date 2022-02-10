using UnityEngine;
using System.Collections.Generic;

namespace VzoVfx {

public sealed class VzoNoteToBuffer : MonoBehaviour
{
    [SerializeField] VzoConfig _config = null;
    [SerializeField] int _channel = 0;

    struct NoteEvent
    {
        public int Pitch;
        public float Velocity;
    }

    Queue<NoteEvent> _queue = new Queue<NoteEvent>();

    string _prefix;
    GraphicsBuffer _buffer;
    float[] _temp = new float[128];

    public GraphicsBuffer NoteBuffer => _buffer;

    void OnOscMessage(string address, OscJack.OscDataHandle data)
    {
        if (!address.StartsWith(_prefix)) return;
        var velocity = data.GetElementAsFloat(0);
        var pitch = int.Parse(address.Substring(_prefix.Length));
        _queue.Enqueue(new NoteEvent { Pitch = pitch, Velocity = velocity });
    }

    void Start()
    {
        _prefix = $"/note/{_channel}/";
        _buffer = new GraphicsBuffer
          (GraphicsBuffer.Target.Structured, 128, sizeof(float));

        var server = OscJack.OscMaster.GetSharedServer(_config.UdpPort);
        server.MessageDispatcher.AddCallback(string.Empty, OnOscMessage);
    }

    void OnDestroy()
    {
        var server = OscJack.OscMaster.GetSharedServer(_config.UdpPort);
        server.MessageDispatcher.RemoveCallback(string.Empty, OnOscMessage);

        _buffer.Dispose();
    }

    void Update()
    {
        if (_queue.Count == 0) return;

        while (_queue.Count > 0)
        {
            var ev = _queue.Dequeue();
            _temp[ev.Pitch] = ev.Velocity;
        }

        _buffer.SetData(_temp);
    }
}

} // namespace VzoVfx
