using UnityEngine;

namespace VzoVfx {

public static class Util
{
    public static int NameToID(string name)
      => string.IsNullOrEmpty(name) ? 0 : Shader.PropertyToID(name);
}

} // namespace VzoVfx
