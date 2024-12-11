using UnityEngine;
using FishNet.Object;

public class Ownership : NetworkBehaviour
{
    public GameObject playerCam;

    private void Start()
    {
        if (!Owner.IsLocalClient)
            return;

        playerCam.SetActive(true);
    }
}
