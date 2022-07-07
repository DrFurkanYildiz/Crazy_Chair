using System.Collections.Generic;
using UnityEngine;

public class StoreController : MonoBehaviour
{
    public static StoreController Instance;
    private bool _isStoreActive;
    public bool IsStoreActive
    {
        get { return _isStoreActive; }
        set { _isStoreActive = value; }
    }

    private int _chairIndex = 0;
    public int ChairIndex { get { return _chairIndex; } }

    List<int> ownedChairIndex = new List<int>() { 0 };

    void Awake() => Instance = this;
    void Update() => StoreControl();

    public void Setup()
    {
        UISystem.Instance.StoreInButton.OnClick.AddListener(StoreActive);
        UISystem.Instance.StoreChairSelectButton.OnClick.AddListener(StoreSelectedChair);
        UISystem.Instance.StoreChairBuyButton.OnClick.AddListener(StoreBuyChair);

        UISystem.Instance.storeRightButton.OnClick.AddListener(StoreNextChair);
        UISystem.Instance.storeLeftButton.OnClick.AddListener(StorePrevionsChair);
    }
    private void StoreControl()
    {
        if (IsStoreActive)
        {
            if (InputSystem.Instance.SwipeLeft)
                StoreNextChair();
            else if (InputSystem.Instance.SwipeRight)
                StorePrevionsChair();


            IndexCalculate();
            ChairManager.Instance.ChairModelsUpdate(_chairIndex);
            StoreUIControl();
        }

        if (IsPurchasableChair())
            UISystem.Instance.PurchasableAllertObject.SetActive(true);
        else
            UISystem.Instance.PurchasableAllertObject.SetActive(false);
    }
    private void StoreUIControl()
    {
        if (ownedChairIndex.Contains(_chairIndex))
            UISystem.Instance.StoreSelectChair();
        else
            UISystem.Instance.StoreBuyChair(GameData.Instance.GetChairCost(_chairIndex));
    }
    private void StoreNextChair()
    {
        _chairIndex++;
    }
    private void StorePrevionsChair()
    {
        _chairIndex--;
    }
    private void IndexCalculate()
    {
        GameObject go = ChairManager.Instance.ChairModelsPrefab;
        int childCount = go.transform.childCount - 1;

        if (_chairIndex > childCount)
            _chairIndex = 0;
        if (_chairIndex < 0)
            _chairIndex = childCount;
    }
    
    private void StoreActive()
    {
        IsStoreActive = true;
        SoundManager.Instance.ClickSound();
        UISystem.Instance.UIStoreActive();

        CameraManager.Instance.SwitchStoreCamera();


        HumanHandler guest = ChairManager.Instance.GetChair().GetGuest();
        HumanSpawner.Instance.GuestDespawn(guest, guest.GetFirstGuest());


        HumanSpawner.Instance.GuestSpawn();

        ChairManager.Instance.RemoveChair();
        ChairManager.Instance.ChairModelsCreate(_chairIndex);

        ChairManager.Instance.StopSparkParticle();
    }
    private void StoreSelectedChair()
    {
        IsStoreActive = false;
        UISystem.Instance.UIStoreInactive();

        CameraManager.Instance.SwitchGameCamera();

        ChairManager.Instance.RefreshChair(_chairIndex);
        ChairManager.Instance.ChairModelsRemove();

        GameData.Instance.StaminaUpdate();
        SaveManager.Instance.OnSave();
    }
    private void StoreBuyChair()
    {
        if (IsCanChairBuy(out int buyIndex))
        {
            GameData.Instance.CoinTake(GameData.Instance.GetChairCost(_chairIndex));
            ownedChairIndex.Add(buyIndex);
            SaveManager.Instance.OnSave();
            HapticsManager.Instance.CreateHaptic(.8f, .8f);
            //Debug.Log(buyIndex + " Nolu Sandalye Alındı!");
        }
    }

    private bool IsCanChairBuy(out int buyIndex)
    {
        for (int i = 0; i < ownedChairIndex.Count; i++)
        {
            if (ownedChairIndex[i] != _chairIndex)
                if (GameData.Instance.TargetCoin >= GameData.Instance.GetChairCost(_chairIndex))
                {
                    buyIndex = _chairIndex;
                    return true;
                }
        }

        buyIndex = 0;
        return false;
    }
    public bool IsCanChairBuy()
    {
        for (int i = 0; i < ownedChairIndex.Count; i++)
        {
            if (ownedChairIndex[i] != _chairIndex)
                if (GameData.Instance.TargetCoin >= GameData.Instance.GetChairCost(_chairIndex))
                    return true;
        }
        return false;
    }

    private bool IsPurchasableChair()
    {
        for (int i = 0; i < 8; i++)
        {
            if (!ownedChairIndex.Contains(i))
                if (GameData.Instance.TargetCoin >= GameData.Instance.GetChairCost(i))
                    return true;
        }
        return false;
    }


    public void SetSaveObject(SaveObject saveObject)
    {
        _isStoreActive = saveObject._isStoreActive;
        _chairIndex = saveObject._chairIndex;
        ownedChairIndex = saveObject.ownedChairIndex;
    }
    public SaveObject GetSaveObject()
    {
        return new SaveObject
        {
            _isStoreActive = _isStoreActive,
            _chairIndex = _chairIndex,
            ownedChairIndex = ownedChairIndex
        };
    }

    [System.Serializable]
    public class SaveObject
    {
        public bool _isStoreActive;
        public int _chairIndex = 0;

        public List<int> ownedChairIndex = new List<int>() { 0 };
    }
}
