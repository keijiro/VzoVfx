using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.VFX.Utility;

namespace Vzo.Vfx {

[AddComponentMenu("VFX/Property Binders/VZO/Buffer Binder")]
[VFXBinder("VZO/Buffer")]
public sealed class VzoBufferBinder : VFXBinderBase
{
    #region VFX Binder Implementation

    public string BufferProperty
      { get => (string)_bufferProperty;
        set => _bufferProperty = value; }

    [VFXPropertyBinding("UnityEngine.GraphicsBuffer"), SerializeField]
    ExposedProperty _bufferProperty = "NoteBuffer";

    public VzoBuffer Target = null;

    public override bool IsValid(VisualEffect component)
      => Target != null &&
         component.HasGraphicsBuffer(_bufferProperty);

    public override void UpdateBinding(VisualEffect component)
    {
        if (Target.GraphicsBuffer == null) return;
        component.SetGraphicsBuffer(_bufferProperty, Target.GraphicsBuffer);
    }

    public override string ToString()
      => $"VZO Buffer : '{_bufferProperty}' -> {Target?.name ?? "(null)"}";

    #endregion
}

} // namespace Vzo.Vfx
