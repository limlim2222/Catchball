using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class PinManager : MonoBehaviour
{
    public float waitTime = 7.0f;
    public SteamVR_Action_Boolean gripGrabAction;
    private SteamVR_Behaviour_Pose myHand = null;
    private List<Pin> pins = new List<Pin>();
    private Coroutine resetCoroutine = null;
    private int fallenPinCount = 0;

    private void Start()
    {
        myHand = GetComponent<SteamVR_Behaviour_Pose>();

        Pin[] foundPins = FindObjectsOfType<Pin>();
        foreach (Pin pin in foundPins)
        {
            RegisterPin(pin);
        }
    }

    private void Update()
    {
        if (myHand == null)
            return;

        if (gripGrabAction.GetStateDown(myHand.inputSource))
        {
            ResetAllPins(); // 버튼을 누를 때마다 리셋
        }
    }

    public void RegisterPin(Pin pin)
    {
        pins.Add(pin);
    }

    public void PinFell()
    {
        fallenPinCount++;

        if (resetCoroutine == null && fallenPinCount == pins.Count)
        {
            resetCoroutine = StartCoroutine(ResetPins());
        }
    }

    public void ResetAllPins()
    {
        if (resetCoroutine == null)
        {
            resetCoroutine = StartCoroutine(ResetPins());
        }
    }

    private IEnumerator ResetPins()
    {
        foreach (var pin in pins)
        {
            pin.StartReset();
        }

        yield return new WaitForSeconds(waitTime);

        foreach (var pin in pins)
        {
            pin.ResetPin();
        }

        resetCoroutine = null;
        fallenPinCount = 0; // 리셋 후 볼링핀 수 초기화
    }
}
