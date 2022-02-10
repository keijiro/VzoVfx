using UnityEngine;

namespace Vzo.Vfx.Sample {

public sealed class AppConfig : MonoBehaviour
{
    [SerializeField] int _targetFrameRate = 60;

    void Start()
      => Application.targetFrameRate = _targetFrameRate;
}

} // namespace Vzo.Vfx.Sample
