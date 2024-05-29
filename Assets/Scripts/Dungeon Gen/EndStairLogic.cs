using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndStairLogic : MonoBehaviour
{
    Controller controller;
    private float Speed = 1f;
    [Header("Rotation Configs")]
    [SerializeField] private float RotationAmount = 25f;
    [SerializeField] private float ForwardDirection = 0;

    private Vector3 StartRotation;
    private Vector3 Forward;

    private Coroutine AnimationCoroutine;
    bool IsRunning = false;

    private void Awake()
    {
        FindController();

        StartRotation = transform.rotation.eulerAngles;
        Forward = transform.forward;
    }

    private void FindController()
    {
        if (controller == null)
        {
            GameObject gameController = GameObject.FindGameObjectWithTag("GameController");
            if (gameController != null)
                controller = gameController.GetComponent<Controller>();
        }
    }

    public void LoadNextLevel(Vector3 UserPosition)
    {
        if (!IsRunning)
        {
            IsRunning = true;
            float dot = Vector3.Dot(Forward, (UserPosition - transform.position).normalized);
            AnimationCoroutine = StartCoroutine(DoRotationOpen(dot));
        }
    }

    private IEnumerator DoRotationOpen(float ForwardAmount)
    {
        Quaternion startRotation = transform.rotation;
        Quaternion endRotation;

        if (ForwardAmount >= ForwardDirection)
        {
            endRotation = Quaternion.Euler(new Vector3(0, StartRotation.y - RotationAmount, 0));
        }
        else
        {
            endRotation = Quaternion.Euler(new Vector3(0, StartRotation.y + RotationAmount, 0));
        }

        float time = 0;
        while (time < 1)
        {
            transform.rotation = Quaternion.Slerp(startRotation, endRotation, time);
            yield return null;
            time += Time.deltaTime * Speed;
        }

        startRotation = transform.rotation;
        endRotation = Quaternion.Euler(StartRotation);

        time = 0;
        while (time < 1)
        {
            transform.rotation = Quaternion.Slerp(startRotation, endRotation, time);
            yield return null;
            time += Time.deltaTime * Speed;
        }
        EventAfterRotation();
    }

    void EventAfterRotation()
    {
        Debug.Log("LoadingNextLevel.");
        controller.LoadNextLevel();
    }
}

