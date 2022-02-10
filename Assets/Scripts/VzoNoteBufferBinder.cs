using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.VFX.Utility;

namespace VzoVfx {

[AddComponentMenu("VFX/Property Binders/VZO/Note Buffer Binder")]
[VFXBinder("VZO/Note Buffer")]
public sealed class VzoNoteBufferBinder : VFXBinderBase
{
    #region VFX Binder Implementation

    public string BufferProperty
      { get => (string)_bufferProperty;
        set => _bufferProperty = value; }

    [VFXPropertyBinding("UnityEngine.GraphicsBuffer"), SerializeField]
    ExposedProperty _bufferProperty = "NoteBuffer";

    public VzoNoteBuffer Target = null;

    public override bool IsValid(VisualEffect component)
      => Target != null &&
         component.HasGraphicsBuffer(_bufferProperty);

    public override void UpdateBinding(VisualEffect component)
    {
        if (Target.NoteBuffer == null) return;
        component.SetGraphicsBuffer(_bufferProperty, Target.NoteBuffer);
    }

    public override string ToString()
      => $"Note Buffer : '{_bufferProperty}' -> {Target?.name ?? "(null)"}";

    #endregion
}

} // namespace VzoVfx
