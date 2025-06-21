using UnityEngine;
using UnityEngine.XR;
using System.Collections.Generic;

/// <summary>
/// Disables XR headset tracking so that transform.position and rotation can be controlled by script.
/// </summary>
public class XRTrackingDisabler : MonoBehaviour
{
    void Start()
    {
        // Disable XR positional tracking (keep device mode, not floor)
        List<XRInputSubsystem> xrSystems = new List<XRInputSubsystem>();
        SubsystemManager.GetInstances(xrSystems);

        foreach (var xr in xrSystems)
        {
            if (xr.TrySetTrackingOriginMode(TrackingOriginModeFlags.Device))
            {
                Debug.Log("[XRTrackingDisabler] Set tracking origin to Device.");
            }

            if (xr.TryRecenter())
            {
                Debug.Log("[XRTrackingDisabler] XR headset recentered.");
            }
        }
    }
}