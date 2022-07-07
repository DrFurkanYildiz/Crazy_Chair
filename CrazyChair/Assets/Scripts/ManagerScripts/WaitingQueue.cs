using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitingQueue
{
    public event EventHandler OnGuestAdded;
    public event EventHandler OnGuestArrivedAtFrontOfQueue;

    private const float POSITION_SIZE = 10f;


    private List<HumanHandler> guestList;
    private List<Vector3> positionList;
    private Vector3 entrancePosition;


    public WaitingQueue(List<Vector3> positionList)
    {
        this.positionList = positionList;

        CalculateEntrancePosition();

        guestList = new List<HumanHandler>();
    }
    public List<HumanHandler> GetGuestList()
    {
        return guestList;
    }

    private void CalculateEntrancePosition()
    {
        if (positionList.Count <= 1)
        {
            entrancePosition = positionList[positionList.Count - 1];
        }
        else
        {
            Vector3 dir = positionList[positionList.Count - 1] - positionList[positionList.Count - 2];
            entrancePosition = positionList[positionList.Count - 1] + dir;
        }
    }

    public void AddPosition(Vector3 position)
    {
        positionList.Add(position);
        CalculateEntrancePosition();
    }


    public void AddPosition_Down()
    {
        AddPosition(positionList[positionList.Count - 1] + new Vector3(0, -1) * POSITION_SIZE);
    }

    public void AddPosition_Up()
    {
        AddPosition(positionList[positionList.Count - 1] + new Vector3(0, +1) * POSITION_SIZE);
    }

    public void AddPosition_Left()
    {
        AddPosition(positionList[positionList.Count - 1] + new Vector3(-1, 0) * POSITION_SIZE);
    }

    public void AddPosition_Right()
    {
        AddPosition(positionList[positionList.Count - 1] + new Vector3(+1, 0) * POSITION_SIZE);
    }



    public bool CanAddGuest()
    {
        return guestList.Count < positionList.Count;
    }

    public void AddGuest(HumanHandler guest)
    {
        guestList.Add(guest);
        guest.MoveTo(entrancePosition, () => {
            guest.MoveTo(positionList[guestList.IndexOf(guest)], () => { GuestArrivedAtQueuePosition(guest); });
        });
        if (OnGuestAdded != null) OnGuestAdded(this, EventArgs.Empty);
    }

    public HumanHandler GetFirstInQueue()
    {
        if (guestList.Count == 0)
        {
            return null;
        }
        else
        {
            HumanHandler guest = guestList[0];
            guestList.RemoveAt(0);
            RelocateAllGuests();
            return guest;
        }
    }

    private void RelocateAllGuests()
    {
        for (int i = 0; i < guestList.Count; i++)
        {
            HumanHandler guest = guestList[i];
            guest.MoveTo(positionList[i], () => { GuestArrivedAtQueuePosition(guest); });
        }
    }

    private void GuestArrivedAtQueuePosition(HumanHandler guest)
    {
        if (guest == guestList[0])
        {
            if (OnGuestArrivedAtFrontOfQueue != null) OnGuestArrivedAtFrontOfQueue(this, EventArgs.Empty);
        }
    }
}
