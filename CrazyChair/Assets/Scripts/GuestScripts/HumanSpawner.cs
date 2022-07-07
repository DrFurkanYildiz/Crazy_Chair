using Lean.Pool;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanSpawner : MonoBehaviour
{
    public static HumanSpawner Instance;

    [SerializeField] private GameObject humanPrefab;
    [SerializeField] private GameObject spectatorPrefab;
    private List<HumanHandler> humanList = new List<HumanHandler>();
    [SerializeField] private Transform[] beanBagTransform = new Transform[2];
    [SerializeField] private Transform dancingTransform;
    private void Awake()
    {
        Instance = this;
    }

    public void Setup()
    {
        for (int i = 0; i < QueueManager.Instance.GetQueueCount() + 1; i++)
            GuestSpawn();

        SpectatorHumanSpawn();
    }
    public void GuestSpawn(bool firstGuest = false)
    {

        GameObject go = LeanPool.Spawn(humanPrefab, transform.position, Quaternion.identity);

        humanList.Add(go.GetComponent<HumanHandler>());

        if (firstGuest)
        {
            go.GetComponent<HumanHandler>().SetFirstGuest();
            LeanPool.Detach(go.GetComponent<HumanHandler>());
            go.GetComponent<HumanHandler>().State = HumanState.Ready;
            go.GetComponent<HumanHandler>().animator.SetBool("Baslangic", true);
            go.transform.SetParent(ChairManager.Instance.GetChair().ChairTopTransform, true);
            go.transform.localScale = Vector3.one;
            go.transform.localPosition = new Vector3(0, 0.1f, 0.1f);

            ChairManager.Instance.GetChair().SetGuest(go.GetComponent<HumanHandler>());

        }
        else
        {
            go.GetComponent<HumanHandler>().State = HumanState.Idle;
            go.GetComponent<HumanHandler>().animator.SetBool("Baslangic", false);
        }
    }
    public void GuestDespawn(HumanHandler human, bool isFirstGuest = false)
    {
        if(human != null)
        {
            if (isFirstGuest)
            {
                int index = humanList.IndexOf(human);
                humanList.RemoveAt(index);
                Destroy(human.gameObject);
            }
            else
                LeanPool.Despawn(human);
        }
    } 

    public HumanHandler GetIdleGuest()
    {
        if (humanList.Count > 0)
        {
            HumanHandler human = humanList[0];
            humanList.RemoveAt(0);
            return human;
        }
        else
            return null;
    }
    private void SpectatorHumanSpawn()
    {
        for (int i = 0; i < beanBagTransform.Length; i++)
        {
            GameObject go = Instantiate(spectatorPrefab, beanBagTransform[i].position, beanBagTransform[i].rotation);
            go.transform.SetParent(beanBagTransform[i]);
            go.transform.localPosition += Vector3.forward * .5f;
            go.GetComponent<Spectator>().AnimIndex = i + 1;
        }
        GameObject dancingHuman = Instantiate(spectatorPrefab, dancingTransform);
        dancingHuman.GetComponent<Spectator>().IsDancingSpectator = true;
    }
    
}
