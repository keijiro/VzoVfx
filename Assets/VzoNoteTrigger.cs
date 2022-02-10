using UnityEngine;
using UnityEngine.VFX;
using System.Collections.Generic;

namespace VzoVfx {

public sealed class VzoNoteTrigger : MonoBehaviour
{
    #region Editable properties

    [SerializeField] VzoConfig _config = null;
    [SerializeField] int _channel = 0;
    [Space]
    [SerializeField] VisualEffect _vfx = null;
    [SerializeField] string _noteOnEvent = "NoteOn";
    [SerializeField] string _noteOffEvent = null;
    [SerializeField] string _pitchProperty = "Pitch";
    [SerializeField] string _velocityProperty = "Velocity";

    #endregion

    #region Private objects

    (int noteOn, int noteOff, int pitch, int velocity) _IDs;

    Queue<(int pitch, float velocity)> _queue = new Queue<(int, float)>();

    string _prefix;
    VFXEventAttribute _attrib;

    #endregion

    #region OSC callback

    void OnOscMessage(string address, OscJack.OscDataHandle data)
    {
        // Note that this will be invoked from non-main threads.
        if (!address.StartsWith(_prefix)) return;
        var velocity = data.GetElementAsFloat(0);
        var pitch = int.Parse(address.Substring(_prefix.Length));
        lock (_queue) _queue.Enqueue((pitch, velocity));
    }

    #endregion

    #region MonoBehaviour implementation

    void Start()
    {
        // VFX identifiers
        _IDs.noteOn   = Util.NameToID(_noteOnEvent);
        _IDs.noteOff  = Util.NameToID(_noteOffEvent);
        _IDs.pitch    = Util.NameToID(_pitchProperty);
        _IDs.velocity = Util.NameToID(_velocityProperty);

        // Initialization
        _prefix = $"/note/{_channel}/";
        _attrib = _vfx.CreateVFXEventAttribute();

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
            var (pitch, velocity) = (0, 0.0f);
            lock (_queue) { (pitch, velocity) = _queue.Dequeue(); }

            var noteOn = velocity > 0;
            if ( noteOn && _IDs.noteOn  == 0) continue;
            if (!noteOn && _IDs.noteOff == 0) continue;

            _attrib.SetInt(_IDs.pitch, pitch);
            _attrib.SetFloat(_IDs.velocity, velocity);

            _vfx.SendEvent(noteOn ? _IDs.noteOn : _IDs.noteOff, _attrib);
        }
    }

    #endregion
}

} // namespace VzoVfx

