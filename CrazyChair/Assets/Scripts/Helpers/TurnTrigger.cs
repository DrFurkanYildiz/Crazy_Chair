using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnTrigger : MonoBehaviour
{
    private Chair _chair;
    private void Start()
    {
        _chair = transform.parent.transform.parent.GetComponent<Chair>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "TurnTrigger")
            _chair.OnceReturned();
    }
}
