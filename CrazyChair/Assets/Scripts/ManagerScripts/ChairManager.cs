using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChairManager : MonoBehaviour
{
    public static ChairManager Instance;
    [SerializeField] private Transform _chairSpawnTransform;

    public List<GameObject> chairPrefabsList = new List<GameObject>();
    public GameObject ChairModelsPrefab;

    private Chair _chair;

    [SerializeField] private float _turnAmount;
    
    private bool _isGuestFlying;
    private bool _isReadyToThrow;
    
    [SerializeField] private int _flightDistance;

    private GameObject _chairModels;
    public float TurnAmount
    {
        get { return _turnAmount; }
        set { _turnAmount = value; }
    }

    public bool IsReadyToThrow
    {
        get { return _isReadyToThrow; }
        set { _isReadyToThrow = value; }
    }
    public bool IsGuestFlying
    {
        get { return _isGuestFlying; }
        set { _isGuestFlying = value; }
    }

    public int FlightDistance
    {
        get { return _flightDistance; }
        set { _flightDistance = value; }
    }


    [SerializeField] private ParticleSystem CoinParticle;
    [SerializeField] private ParticleSystem SparkParticle;

    private void Awake()
    {
        Instance = this;
    }
    public void Setup()
    {
        ChairCreate(StoreController.Instance.ChairIndex);
        HumanSpawner.Instance.GuestSpawn(true);
    }

    private void Update()
    {
        if (TurnAmount > 50 && !GetChair().IsEmpty() && IsReadyToThrow)
            if (InputSystem.Instance.SwipeUp)
                ShootGuest();


        if (TurnAmount >= 200f && !StoreController.Instance.IsStoreActive)
            StartSparkParticle();
        else if (TurnAmount < 200f || StoreController.Instance.IsStoreActive)
            StopSparkParticle();
    }

    private void ChairCreate(int chairIndex)
    {
        GameObject go = Instantiate(chairPrefabsList[chairIndex], transform.position, Quaternion.identity);
        _chair = go.GetComponent<Chair>();
        go.transform.SetParent(_chairSpawnTransform, false);
    }
    public void ChairModelsCreate(int index)
    {
        GameObject go = Instantiate(ChairModelsPrefab, transform.position, Quaternion.identity);
        go.transform.SetParent(_chairSpawnTransform, false);

        for (int i = 0; i < go.transform.childCount; i++)
        {
            go.transform.GetChild(i).gameObject.SetActive(false);
        }

        go.transform.GetChild(index).gameObject.SetActive(true);
        _chairModels = go;
    }
    public void ChairModelsUpdate(int index)
    {
        for (int i = 0; i < _chairModels.transform.childCount; i++)
        {
            _chairModels.transform.GetChild(i).gameObject.SetActive(false);
        }

        _chairModels.transform.GetChild(index).gameObject.SetActive(true);
    }
    public void ChairModelsRemove()
    {
        Destroy(_chairModels);
    }
    public GameObject GetChairModelsObject()
    {
        return _chairModels;
    }
    public void RemoveChair()
    {
        Destroy(_chair.gameObject);
        _chair = null;
    }
    public Chair GetChair()
    {
        if (_chair != null) return _chair;
        else return null;
    }
    public Chair GetEmptyChair()
    {
        if (_chair != null)
            if (_chair.IsEmpty())
                return _chair;

        return null;
    }

    public void ShootGuest()
    {
        if (IsReadyToThrow)
        {

            _chair.GetGuest().transform.SetParent(transform.root);
            _chair.GetGuest().FlyingCost = TurnAmount;

            CameraManager.Instance.ClearFollowingGuest();
            CameraManager.Instance.SetFollowingGuest(_chair.GetGuest());

            _chair.GetGuest().GuestMoveToThrow(GetThrowPosition());
            _chair.GetGuest().SetFallPosition(CrashTargets()[1]);

            ResetChair();
            HumanSpawner.Instance.GuestSpawn();

            GameData.Instance.StaminaUpdate();

            StopSparkParticle();
            UISystem.Instance.IsTutorialShowDone = true;

        }
    }
    public void ResetChair()
    {
        _chair.ClearGuest();
        IsGuestFlying = true;
        IsReadyToThrow = false;
    }
    public void RefreshChair(int chairIndex)
    {
        if (_chair == null)
        {
            ChairCreate(chairIndex);
            HumanSpawner.Instance.GuestSpawn(true);
            IsReadyToThrow = false;
        }

    }
    private Vector3 GetThrowPosition()
    {
        return CrashTargets()[0];
    }

    private Vector3[] CrashTargets()
    {
        Vector3[] targets = new Vector3[2];

        targets[0] = TargetPosition() * (float)(3f / 4f) + new Vector3(0f, Mathf.Abs(TurnAmount / 40f), 0f);
        targets[1] = TargetPosition();

        return targets;
    }
    private Vector3 TargetPosition()
    {
        float angle = GetChair().ChairTopTransform.localEulerAngles.y * (Mathf.PI / 180f);

        float x = Mathf.Abs(TurnAmount / 10f) * Mathf.Sin(angle);
        float z = Mathf.Abs(TurnAmount / 10f) * Mathf.Cos(angle);
        return new Vector3(x, 0f, z);

    }


    public void CalculateShotGain(float gain)
    {
        FlightDistance = Mathf.RoundToInt(gain);
        GameData.Instance.CoinAdd(Mathf.RoundToInt(gain * 10 * Random.Range(1.2f, 1.5f)));
    }



    public void StartSparkParticle()
    {
        SparkParticle.Play();
    }
    public void StopSparkParticle()
    {
        SparkParticle.Stop();
    }
    public void StartCoinParticle()
    {
        CoinParticle.Play();
    }
}
