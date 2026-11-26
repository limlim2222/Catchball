using System.Collections;
using UnityEngine;
using Photon.Pun;

public class DebugBall : MonoBehaviour
{
    public float waitTime = 10.0f;
    public float throwForce = 10.0f;
    public float maxSpeed = 20.0f;

    private Rigidbody ballRigidbody;
    private Vector3 originPosition;
    private Quaternion originRotation;
    private Coroutine returnCoroutine;

    [SerializeField] private PhotonView photonView;

    void Start()
    {
        ballRigidbody = GetComponent<Rigidbody>();
        originPosition = transform.position;
        originRotation = transform.rotation;
        photonView = GetComponent<PhotonView>();

        ResetBall();
    }

    void Update()
    {
        if (photonView.IsMine && ballRigidbody.velocity.magnitude > maxSpeed)
        {
            ballRigidbody.velocity = ballRigidbody.velocity.normalized * maxSpeed;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!photonView.IsMine) return;

        if (returnCoroutine == null && !collision.gameObject.CompareTag("Box"))
        {
            returnCoroutine = StartCoroutine(ReturnBall());
        }
    }

    IEnumerator ReturnBall()
    {
        yield return new WaitForSeconds(waitTime);

        photonView.RPC("ResetBall", RpcTarget.All);
    }

    [PunRPC]
    private void ResetBall()
    {
        ballRigidbody.velocity = Vector3.zero;
        ballRigidbody.angularVelocity = Vector3.zero;
        transform.position = originPosition;
        transform.rotation = originRotation;
        ballRigidbody.isKinematic = true;

        if (returnCoroutine != null)
        {
            StopCoroutine(returnCoroutine);
            returnCoroutine = null;
        }
    }

    public void Pickup(Transform handTransform)
    {
        if (photonView.IsMine)
        {
            photonView.RPC("PickupBall", RpcTarget.All, handTransform.position, handTransform.rotation);
        }
    }

    [PunRPC]
    private void PickupBall(Vector3 handPosition, Quaternion handRotation)
    {
        transform.position = handPosition;
        transform.rotation = handRotation;
        ballRigidbody.isKinematic = true;
    }

    public void Drop(Vector3 throwDirection)
    {
        if (photonView.IsMine)
        {
            photonView.RPC("DropBall", RpcTarget.All, throwDirection);
        }
    }

    [PunRPC]
    private void DropBall(Vector3 throwDirection)
    {
        ballRigidbody.isKinematic = false;
        ballRigidbody.velocity = Vector3.zero;
        ballRigidbody.angularVelocity = Vector3.zero;
        ballRigidbody.AddForce(throwDirection * throwForce, ForceMode.VelocityChange);
    }
}