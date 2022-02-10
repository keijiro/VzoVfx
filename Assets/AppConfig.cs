using UnityEngine;

namespace VzoVfx {

public sealed class AppConfig : MonoBehaviour
{
    [SerializeField] int _targetFrameRate = 60;

    void Start()
      => Application.targetFrameRate = _targetFrameRate;
}

} // namespace VzoVfx
