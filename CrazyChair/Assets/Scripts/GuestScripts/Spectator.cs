using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spectator : MonoBehaviour
{
    public List<GameObject> humanPrefabList = new List<GameObject>();
    [HideInInspector] public Animator animator;
    public bool IsDancingSpectator;
    public int AnimIndex;
    private void Awake()
    {
        animator = GetComponent<Animator>();
        RandomType();
    }
    private void Start()
    {
        if (IsDancingSpectator)
            animator.SetBool("IsDancing", true);
        else
            animator.SetBool("IsDancing", false);

        animator.SetInteger("RandomAnim", AnimIndex);
    }
    private void RandomType()
    {
        humanPrefabList[Random.Range(0, humanPrefabList.Count)].SetActive(true);
    }
}
