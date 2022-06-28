VzoVfx
======

![gif](https://user-images.githubusercontent.com/343936/153742955-95cc9846-adc5-4d6d-a777-68a175155c06.gif)

**VzoVfx** is a Unity VFX Graph extensions for the [VZO] plugin that allows
accurate synchronization between DAW and VFX.

[VZO]: https://github.com/keijiro/vzo

Components
----------

### VZO Trigger

**VZO Trigger** invokes given UnityEvent entries with received note data
(`int pitch, float velocity`).

### VZO Trigger to VFX

VZO Trigger can't invokes VFX events directly. You can use **VZO Trigger to
VFX** to relay these events to VFX. It invokes VFX events on VZO note events
with note attributes. See the `VzoTrigger.unity` sample for details.

### VZO Buffer

**VZO Buffer** receives VZO note/CC events and stores the current status into a
Graphics Buffer. It's handy to create non-trigger dependent effects. See the
`VzoBuffer.unity` sample for details.
