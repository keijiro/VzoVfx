using UnityEngine;

namespace VzoVfx {

[CreateAssetMenu(menuName = "VZO Config")]
public sealed class VzoConfig : ScriptableObject
{
    [SerializeField] int _udpPort = 9000;

    public int UdpPort => _udpPort;
}

} // namespace VzoVfx
