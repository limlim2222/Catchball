using UnityEngine;
using Valve.VR;
using Photon.Pun;

public class VRHand : MonoBehaviourPunCallbacks
{
    public SteamVR_Action_Boolean GripGrab;
    private SteamVR_Behaviour_Pose myHand = null;
    private Transform myTransform = null;
    private Rigidbody myRigidbody = null;
    private Ball currentBall = null;
    private bool isGrabbing = false;

    public float throwForceMultiplier = 7.0f;

    private void Start()
    {
        myHand = GetComponent<SteamVR_Behaviour_Pose>();
        myTransform = GetComponent<Transform>();
        myRigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (!photonView.IsMine)
            return;

        if (myHand == null)
            return;

        if (GripGrab.GetStateDown(myHand.inputSource))
        {
            if (!isGrabbing)
            {
                Pickup();
            }
        }

        if (GripGrab.GetStateUp(myHand.inputSource))
        {
            if (isGrabbing)
            {
                Drop();
            }
        }
    }

    private void Pickup()
    {
        Collider[] colliders = Physics.OverlapSphere(myTransform.position, 0.1f);
        float minDistance = Mathf.Infinity;
        GameObject nearestObject = null;

        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Interactable"))
            {
                float distance = Vector3.Distance(myTransform.position, collider.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestObject = collider.gameObject;
                }
            }
        }

        if (nearestObject != null)
        {
            currentBall = nearestObject.GetComponent<Ball>();
            if (currentBall != null)
            {
                isGrabbing = true;
                currentBall.GrabBall(photonView.Owner);
                currentBall.transform.SetParent(myTransform);
                currentBall.transform.localPosition = Vector3.zero;
            }
        }
    }

    private void Drop()
    {
        if (currentBall != null)
        {
            isGrabbing = false;
            currentBall.transform.SetParent(null);

            Vector3 force = myHand.GetVelocity() * throwForceMultiplier;
            currentBall.ThrowBall(force);

            currentBall = null;
        }
    }
}