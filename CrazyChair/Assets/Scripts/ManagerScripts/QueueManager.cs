using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QueueManager : MonoBehaviour
{
    public static QueueManager Instance;
    private WaitingQueue waitingQueue;
    [SerializeField] private int _queueCount;
    [SerializeField] private float _queueSpeed;

    void Awake() => Instance = this;
    void Update() => TrySendGuestToChair();

    public void Setup()
    {
        List<Vector3> waitingQueuePositionList = new List<Vector3>();

        Vector3 firstPosition = new Vector3(-30, 0, 0);
        float positionSize = 10f;
        for (int i = 0; i < _queueCount; i++)
        {
            waitingQueuePositionList.Add(firstPosition + new Vector3(-1, 0, 0) * positionSize * i);
        }
        waitingQueue = new WaitingQueue(waitingQueuePositionList);
        QueueAddGuest();

        FunctionPeriodic.Create(() => { QueueAddGuest(); ClearQueueSpaces(); }, _queueSpeed);
        
    }
    private void QueueAddGuest()
    {
        if (waitingQueue.CanAddGuest())
        {
            HumanHandler guest = HumanSpawner.Instance.GetIdleGuest();
            waitingQueue.AddGuest(guest);
        }
    }
    public int GetQueueCount()
    {
        return _queueCount;
    }
    
    private void TrySendGuestToChair()
    {
        Chair emptyChair = ChairManager.Instance.GetEmptyChair();
        if (emptyChair != null)
        {
            HumanHandler guest = waitingQueue.GetFirstInQueue();
            if (guest != null)
            {
                emptyChair.SetGuest(guest);
                guest.MoveTo(emptyChair.GetPosition(), () => {
                    guest.PlayAnimationSitDown(() =>
                    {
                        if (ChairManager.Instance.IsGuestFlying || ChairManager.Instance.GetChair().IsEmpty())
                            TrySendGuestToChair();
                    });
                });
            }
        }
    }

    private void ClearQueueSpaces()
    {
        for (int i = 0; i < waitingQueue.GetGuestList().Count; i++)
        {
            if (waitingQueue.GetGuestList()[i] == null)
            {
                waitingQueue.GetGuestList().RemoveAt(i);
            }
        }
    }
}
