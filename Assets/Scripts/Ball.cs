using System.Collections;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Ball : MonoBehaviourPunCallbacks, IPunObservable
{
    public float waitTime = 10.0f;
    public float maxSpeed = 15.0f;
    public float accelerationRate = 0.5f;
    public float throwForceMultiplier = 5.0f;
    public float interpolationBackTime = 0.1f;

    private Coroutine myCoroutine = null;
    private Rigidbody ballRigidbody = null;
    private Transform ballTransform = null;
    private Vector3 ballOriginPosition = Vector3.zero;
    private Quaternion ballOriginRotation = Quaternion.identity;
    private bool shouldMove = false;
    private bool isOnBox = false;
    private bool isGrabbed = false;

    private Vector3 networkPosition;
    private Quaternion networkRotation;
    private Vector3 networkVelocity;
    private Vector3 networkAngularVelocity;

    void Start()
    {
        ballRigidbody = GetComponent<Rigidbody>();
        ballTransform = transform;
        ballOriginPosition = ballTransform.position;
        ballOriginRotation = ballTransform.rotation;
        ResetBall();
    }

    void Update()
    {
        if (!photonView.IsMine)
        {
            ballTransform.position = Vector3.Lerp(ballTransform.position, networkPosition, Time.deltaTime / interpolationBackTime);
            ballTransform.rotation = Quaternion.Slerp(ballTransform.rotation, networkRotation, Time.deltaTime / interpolationBackTime);
            return;
        }

        if (shouldMove && !isGrabbed)
        {
            if (isOnBox)
            {
                ballRigidbody.velocity = Vector3.zero;
                ballRigidbody.angularVelocity = Vector3.zero;
            }
            else
            {
                if (ballRigidbody.velocity.magnitude < maxSpeed)
                {
                    ballRigidbody.velocity += ballTransform.forward * accelerationRate * Time.deltaTime;
                }
            }
        }
    }

    private void ResetBall()
    {
        ballRigidbody.velocity = Vector3.zero;
        ballRigidbody.angularVelocity = Vector3.zero;
        ballRigidbody.interpolation = RigidbodyInterpolation.Interpolate;
        shouldMove = false;
        isGrabbed = false;
    }

    private IEnumerator ReturnObject()
    {
        yield return new WaitForSeconds(waitTime);
        ResetBall();
        ballTransform.position = ballOriginPosition;
        ballTransform.rotation = ballOriginRotation;
        myCoroutine = null;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!photonView.IsMine)
            return;

        if (collision.gameObject.CompareTag("Plane") || collision.gameObject.CompareTag("Duck") || collision.gameObject.CompareTag("Pin"))
        {
            if (myCoroutine == null)
            {
                myCoroutine = StartCoroutine(ReturnObject());
            }
        }

        if (collision.gameObject.CompareTag("Box"))
        {
            isOnBox = true;
        }
        else
        {
            isOnBox = false;
        }
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(ballTransform.position);
            stream.SendNext(ballTransform.rotation);
            stream.SendNext(ballRigidbody.velocity);
            stream.SendNext(ballRigidbody.angularVelocity);
            stream.SendNext(isGrabbed);
        }
        else
        {
            networkPosition = (Vector3)stream.ReceiveNext();
            networkRotation = (Quaternion)stream.ReceiveNext();
            networkVelocity = (Vector3)stream.ReceiveNext();
            networkAngularVelocity = (Vector3)stream.ReceiveNext();
            isGrabbed = (bool)stream.ReceiveNext();

            if (!isGrabbed)
            {
                ballRigidbody.velocity = networkVelocity;
                ballRigidbody.angularVelocity = networkAngularVelocity;
            }
        }
    }

    public void GrabBall(Player newOwner)
    {
        photonView.RequestOwnership();
        isGrabbed = true;
        shouldMove = false;
        ballRigidbody.isKinematic = true;
        photonView.RPC("SetGrabbedRPC", RpcTarget.All, true, newOwner.ActorNumber);
    }

    public void ThrowBall(Vector3 force)
    {
        isGrabbed = false;
        shouldMove = true;
        ballRigidbody.isKinematic = false;
        ballRigidbody.velocity = Vector3.zero;
        ballRigidbody.angularVelocity = Vector3.zero;
        ballRigidbody.AddForce(force * throwForceMultiplier, ForceMode.Impulse);
        photonView.RPC("SetGrabbedRPC", RpcTarget.All, false, PhotonNetwork.LocalPlayer.ActorNumber);
    }

    [PunRPC]
    private void SetGrabbedRPC(bool grabbed, int ownerActorNumber)
    {
        isGrabbed = grabbed;
        if (grabbed)
        {
            photonView.TransferOwnership(ownerActorNumber);
        }

        if (!photonView.IsMine)
        {
            ballRigidbody.isKinematic = true;
        }
        else
        {
            ballRigidbody.isKinematic = grabbed;
        }
    }
}