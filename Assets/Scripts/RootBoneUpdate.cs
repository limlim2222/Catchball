using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootBoneUpdate : MonoBehaviour
{
    public Transform hmdTransform;
    public Transform rootBone;
    public Transform pelvisBone;

    private Vector3 initialRootBonePosition;
    private Vector3 initialPelvisBoneLocalPosition;

    private void Start()
    {
        initialRootBonePosition = rootBone.position;

        initialPelvisBoneLocalPosition = pelvisBone.localPosition;
    }

    private void LateUpdate()
    {
        Vector3 hmdPosition = hmdTransform.position;

        Vector3 newRootBonePosition = new Vector3(hmdPosition.x, initialRootBonePosition.y, hmdPosition.z);

        rootBone.position = newRootBonePosition;

        pelvisBone.localPosition = initialPelvisBoneLocalPosition;
    }
}
