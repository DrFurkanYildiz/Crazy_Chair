using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CoinCollectManager : MonoBehaviour
{
    public static CoinCollectManager Instance;

    [SerializeField] GameObject coinPrefab;
    [SerializeField] RectTransform target;
    int maxCoin = 50;
    public int GetMaxCoin { get { return maxCoin; } }
    Queue<GameObject> coinsQueue = new Queue<GameObject>();

    [SerializeField] [Range(.5f, .9f)] float minAnimDuration;
    [SerializeField] [Range(.9f, 2f)] float maxAnimDuration;

    [SerializeField] Vector3 targetPosition;
    [SerializeField] Ease easeType;
    private void Awake()
    {
        Instance = this;
        targetPosition = Helpers.GetWorldPositionOfCanvasElement(target) + new Vector2(17f, 0f);

        PrepareCoin();
    }
    void PrepareCoin()
    {
        GameObject coin;
        for (int i = 0; i < maxCoin; i++)
        {
            coin = Instantiate(coinPrefab);
            coin.transform.parent = transform;
            coin.SetActive(false);
            coinsQueue.Enqueue(coin);
        }

    }
    void Animate(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            if(coinsQueue.Count > 0)
            {
                GameObject coin = coinsQueue.Dequeue();
                coin.SetActive(true);

                coin.transform.position = new Vector3(-38f, 22f, -26f);
                float duration = Random.Range(minAnimDuration, maxAnimDuration);
                coin.transform.DOMove(targetPosition, duration)
                    .SetEase(easeType)
                    .OnComplete(() =>
                    {
                        CoinCollectAnimation(.1f);
                        coin.SetActive(false);
                        coinsQueue.Enqueue(coin);
                    });
            }
        }
    }
    public void AddCoins(int amount)
    {
        Animate(amount);
    }
    public void CoinCollectAnimation(float duration)
    {
        target.transform.localScale = Vector3.one;
        target.transform.DOScale(new Vector3(1.2f, 1.2f, 1f), duration)
        .SetEase(easeType)
        .OnComplete(() =>
        {
            target.transform.localScale = Vector3.one;
        });
    }
}
