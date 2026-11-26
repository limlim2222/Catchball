using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pin : MonoBehaviour
{
    private PinManager pinManager;
    private Rigidbody pinRigidbody;
    private Transform pinTransform;
    private Vector3 pinOriginPosition;
    private Quaternion pinOriginRotation;
    private bool isResetting = false;
    private bool isFallen = false;

    void Start()
    {
        pinManager = FindObjectOfType<PinManager>();
        if (pinManager != null)
        {
            pinManager.RegisterPin(this);
        }
        else
        {
            Debug.LogError("PinManager not found in the scene.");
        }

        pinRigidbody = GetComponent<Rigidbody>();
        pinTransform = GetComponent<Transform>();

        pinOriginPosition = pinTransform.position;
        pinOriginRotation = pinTransform.rotation;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isResetting) return;

        if (collision.gameObject.CompareTag("Interactable") || collision.gameObject.CompareTag("Pin"))
        {
            if (!IsStandingUp())
            {
                isFallen = true;
                pinManager.PinFell();
            }
        }
    }

    public void ResetPin()
    {
        if (pinRigidbody != null)
        {
            pinRigidbody.velocity = Vector3.zero;
            pinRigidbody.angularVelocity = Vector3.zero;
        }
        else
        {
            Debug.LogWarning("Rigidbody component is missing on the Pin game object.");
        }

        pinTransform.position = pinOriginPosition;
        pinTransform.rotation = pinOriginRotation;

        isResetting = false;
        isFallen = false;
    }

    private bool IsStandingUp()
    {
        return transform.up.y > 0.5f;
    }

    public void StartReset()
    {
        isResetting = true;
    }

    public bool IsFallen()
    {
        return isFallen;
    }
}
