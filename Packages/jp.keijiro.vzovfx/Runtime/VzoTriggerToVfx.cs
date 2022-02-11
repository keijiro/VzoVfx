using UnityEngine;
using UnityEngine.VFX;

namespace Vzo.Vfx {

public sealed class VzoTriggerToVfx : MonoBehaviour
{
    #region Editable properties

    [SerializeField] VisualEffect _vfx = null;
    [SerializeField] string _noteOnEvent = "NoteOn";
    [SerializeField] string _noteOffEvent = "NoteOff";
    [SerializeField] string _pitchProperty = "Pitch";
    [SerializeField] string _velocityProperty = "Velocity";

    #endregion

    #region Private objects

    (int noteOn, int noteOff, int pitch, int velocity) _IDs;
    VFXEventAttribute _attrib;

    #endregion

    #region Public method

    public void Invoke(int pitch, float velocity)
    {
        var noteOn = velocity > 0;

        if ( noteOn && _IDs.noteOn  == 0) return;
        if (!noteOn && _IDs.noteOff == 0) return;

        _attrib.SetInt(_IDs.pitch, pitch);
        _attrib.SetFloat(_IDs.velocity, velocity);

        _vfx.SendEvent(noteOn ? _IDs.noteOn : _IDs.noteOff, _attrib);
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
        _attrib = _vfx.CreateVFXEventAttribute();
    }

    #endregion
}

} // namespace Vzo.Vfx
