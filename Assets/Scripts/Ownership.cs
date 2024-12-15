using UnityEngine;
using FishNet.Object;

public class Ownership : NetworkBehaviour
{
    public Camera playerCam;
    public GameObject leftEye;
    public GameObject rightEye;

    private void Start()
    {
        if (!Owner.IsLocalClient)
            return;

        leftEye.GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
        rightEye.GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
        playerCam.gameObject.SetActive(true);
    }
}
