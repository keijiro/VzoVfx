using UnityEngine;
using UnityEngine.VFX;
using System.Collections.Generic;

namespace VzoVfx {

public sealed class VzoVfxTriggerWithPitch : MonoBehaviour
{
    [SerializeField] VzoConfig _config = null;
    [SerializeField] int _channel = 0;
    [SerializeField] VisualEffect _vfx = null;
    [SerializeField] string _noteOnEvent = "NoteOn";
    [SerializeField] string _noteOffEvent = null;
    [SerializeField] string _pitchProperty = "Pitch";
    [SerializeField] string _velocityProperty = "Velocity";

    struct NoteEvent
    {
        public int Pitch;
        public float Velocity;
    }

    (int noteOn, int noteOff, int pitch, int velocity) _IDs;

    string _prefix;
    VFXEventAttribute _attrib;
    Queue<NoteEvent> _queue = new Queue<NoteEvent>();

    static int NameToID(string name)
      => string.IsNullOrEmpty(name) ? 0 : Shader.PropertyToID(name);

    void OnOscMessage(string address, OscJack.OscDataHandle data)
    {
        if (!address.StartsWith(_prefix)) return;
        var velocity = data.GetElementAsFloat(0);
        var pitch = int.Parse(address.Substring(_prefix.Length));
        _queue.Enqueue(new NoteEvent { Pitch = pitch, Velocity = velocity });
    }

    void Start()
    {
        _IDs.noteOn   = NameToID(_noteOnEvent);
        _IDs.noteOff  = NameToID(_noteOffEvent);
        _IDs.pitch    = NameToID(_pitchProperty);
        _IDs.velocity = NameToID(_velocityProperty);

        _prefix = $"/note/{_channel}/";
        _attrib = _vfx.CreateVFXEventAttribute();

        var server = OscJack.OscMaster.GetSharedServer(_config.UdpPort);
        server.MessageDispatcher.AddCallback(string.Empty, OnOscMessage);
    }

    void Update()
    {
        while (_queue.Count > 0)
        {
            var ev = _queue.Dequeue();
            var noteOn = ev.Velocity > 0;
            if ( noteOn && _IDs.noteOn  == 0) continue;
            if (!noteOn && _IDs.noteOff == 0) continue;
            _attrib.SetInt(_IDs.pitch, ev.Pitch);
            _attrib.SetFloat(_IDs.velocity, ev.Velocity);
            _vfx.SendEvent(noteOn ? _IDs.noteOn : _IDs.noteOff, _attrib);
        }
    }

    void OnDestroy()
    {
        var server = OscJack.OscMaster.GetSharedServer(_config.UdpPort);
        server.MessageDispatcher.RemoveCallback(string.Empty, OnOscMessage);
    }
}

} // namespace VzoVfx
