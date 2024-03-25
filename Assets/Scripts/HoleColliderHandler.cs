
using UnityEngine;

public class HoleColliderHandler : MonoBehaviour
{
    public bool evenlyAligned;

    public event System.Action<bool> AlignmentChangedEvent;

    public void TriggerEvent(bool _evenlyAligned)
    {
        evenlyAligned = _evenlyAligned;
        AlignmentChangedEvent?.Invoke(evenlyAligned);
    }
}
