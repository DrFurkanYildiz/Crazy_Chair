using UnityEngine;
using System;
public class ChairMovement : MonoBehaviour
{
    private Chair _chair;
    public Action OnTabToTurn;
    
    private float elapsedTime;
    [SerializeField] private float turnNumber;
    private float desiredDuration = 4f;
    private void Start()
    {
        _chair = GetComponent<Chair>();
        OnTabToTurn += TabWithChairTurn;
    }
    private void Update()
    {
        if (ChairManager.Instance.IsReadyToThrow && !StoreController.Instance.IsStoreActive)
            Movement();
    }

    public void RotationReset()
    {
        _chair.ChairTopTransform.eulerAngles = Vector3.zero;

        ChairManager.Instance.TurnAmount = 0f;
        turnNumber = 0;
    }

    private void Movement()
    {
        elapsedTime += Time.deltaTime;
        float slowdownSpeed = elapsedTime / desiredDuration;
        turnNumber = Mathf.MoveTowards(turnNumber, 0f, Mathf.SmoothStep(0, 1, slowdownSpeed));

        float turnSpeed = GameData.Instance.BaseChairSpeed * turnNumber;
        ChairManager.Instance.TurnAmount = turnSpeed;


        _chair.ChairTopTransform.Rotate(transform.up * turnSpeed * Time.deltaTime);
    }
    private void TabWithChairTurn()
    {
        if (ChairManager.Instance.IsReadyToThrow)
            if (!Helpers.IsOverUI())
            {
                turnNumber++;
                elapsedTime = 0f;
            }
    }
}
