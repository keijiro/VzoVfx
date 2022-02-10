using UnityEngine;

namespace Vzo.Vfx {

static class Util
{
    public static int NameToID(string name)
      => string.IsNullOrEmpty(name) ? 0 : Shader.PropertyToID(name);
}

} // namespace Vzo.Vfx
