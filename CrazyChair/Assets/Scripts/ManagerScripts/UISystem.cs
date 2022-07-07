using UnityEngine;
using TMPro;
using Lean.Gui;
using System;

public class UISystem : MonoBehaviour
{
    public static UISystem Instance;

    [SerializeField] GameObject staminaBar;
    [SerializeField] TextMeshProUGUI metreGoster;
    [SerializeField] TextMeshProUGUI CoinGoster;
    [SerializeField] GameObject mesafeGostergeObjesi;
    public GameObject respawnButton;

    [SerializeField] TextMeshProUGUI staminaLevelText;
    [SerializeField] TextMeshProUGUI staminaGerekliMiktarText;

    [SerializeField] TextMeshProUGUI chairSpeedLevelText;
    [SerializeField] TextMeshProUGUI chairSpeedGerekliMiktarText;

    [SerializeField] TextMeshProUGUI earningsLevelText;
    [SerializeField] TextMeshProUGUI earningsGerekliMiktarText;

    public LeanButton StoreInButton;
    public LeanButton StoreChairSelectButton;
    public LeanButton StoreChairBuyButton;

    public LeanButton storeLeftButton;
    public LeanButton storeRightButton;

    public GameObject LevelPanelObject;
    public LeanButton ChairStaminaLevelButton;
    public LeanButton ChairSpeedLevelButton;
    public LeanButton EarningsLevelButton;


    public TextMeshProUGUI ChairCostText;

    public GameObject TapObject;
    public GameObject SwipeUpObject;
    public enum TutorialShowState
    {
        Tap,
        Shot,
        Inactive
    }

    [SerializeField] private TutorialShowState tutorialShowState;
    public bool IsTutorialShowDone;

    public GameObject PurchasableAllertObject;

    private void Awake()
    {
        Instance = this;
    }
    public void Setup()
    {
        ChairStaminaLevelButton.OnClick.AddListener(GameData.Instance.StaminaLevelUp);
        ChairSpeedLevelButton.OnClick.AddListener(GameData.Instance.ChairSpeedLevelUp);
        EarningsLevelButton.OnClick.AddListener(GameData.Instance.CoinEarningsLevelUp);
    }
    void Update()
    {
        LevelRespawnStoreButtonInteractableUpdate();
        LevelPanelPrint();
        CoinAndDistanceValuePrint();
        TapToStartShow();
    }

    public void UIStoreActive()
    {
        LevelPanelObject.SetActive(false);

        StoreInButton.gameObject.SetActive(false);
        staminaBar.SetActive(false);
        mesafeGostergeObjesi.gameObject.SetActive(false);

        ChairCostText.gameObject.SetActive(true);

        storeRightButton.gameObject.SetActive(true);
        storeLeftButton.gameObject.SetActive(true);

    }
    public void UIStoreInactive()
    {
        LevelPanelObject.SetActive(true);

        StoreInButton.gameObject.SetActive(true);
        staminaBar.SetActive(true);
        mesafeGostergeObjesi.gameObject.SetActive(true);

        StoreChairSelectButton.gameObject.SetActive(false);
        StoreChairBuyButton.gameObject.SetActive(false);
        ChairCostText.gameObject.SetActive(false);

        storeRightButton.gameObject.SetActive(false);
        storeLeftButton.gameObject.SetActive(false);
    }
    public void StoreSelectChair()
    {
        StoreChairSelectButton.gameObject.SetActive(true);
        StoreChairBuyButton.gameObject.SetActive(false);
        ChairCostText.gameObject.SetActive(false);
    }
    public void StoreBuyChair(int chairCost)
    {
        StoreChairBuyButton.gameObject.SetActive(true);
        StoreChairSelectButton.gameObject.SetActive(false);
        ChairCostText.gameObject.SetActive(true);
        ChairCostText.text = ValuePrint(chairCost);
    }
    public void FollowCameraActive()
    {
        if (!StoreController.Instance.IsStoreActive)
            mesafeGostergeObjesi.gameObject.SetActive(true);
    }
    public void FollowCameraInctive()
    {
        mesafeGostergeObjesi.gameObject.SetActive(false);
    }

    private void LevelPanelPrint()
    {
        if (GameData.Instance.StaminaLevel < GameData.Instance.GetMaxLevel)
            staminaGerekliMiktarText.text = ValuePrint(GameData.Instance.GetStaminaIncreasePrice);
        else
            staminaGerekliMiktarText.text = "Max";

        if (GameData.Instance.ChairSpeedLevel < GameData.Instance.GetMaxLevel)
            chairSpeedGerekliMiktarText.text = ValuePrint(GameData.Instance.GetChairSpeedIncreasePrice);
        else
            chairSpeedGerekliMiktarText.text = "Max";

        if (GameData.Instance.CoinEarningsLevel < GameData.Instance.GetMaxLevel)
            earningsGerekliMiktarText.text = ValuePrint(GameData.Instance.GetCoinEarningsIncreasePrice);
        else
            earningsGerekliMiktarText.text = "Max";


        staminaLevelText.text = "Lv." + GameData.Instance.StaminaLevel.ToString();
        chairSpeedLevelText.text = "Lv." + GameData.Instance.ChairSpeedLevel.ToString();
        earningsLevelText.text = "Lv." + GameData.Instance.CoinEarningsLevel.ToString();
        
    }
    private void LevelRespawnStoreButtonInteractableUpdate()
    {
        if (!GameData.Instance.IsStaminaLevelIncrease() || GameData.Instance.IsChairOnDamage || GameData.Instance.StaminaLevel >= GameData.Instance.GetMaxLevel)
            ChairStaminaLevelButton.interactable = false;
        else
            ChairStaminaLevelButton.interactable = true;


        if (!GameData.Instance.IsChairSpeedLevelIncrease() || GameData.Instance.IsChairOnDamage || GameData.Instance.ChairSpeedLevel >= GameData.Instance.GetMaxLevel)
            ChairSpeedLevelButton.interactable = false;
        else
            ChairSpeedLevelButton.interactable = true;


        if (!GameData.Instance.IsTicketPriceLevelIncrease() || GameData.Instance.IsChairOnDamage || GameData.Instance.CoinEarningsLevel >= GameData.Instance.GetMaxLevel)
            EarningsLevelButton.interactable = false;
        else
            EarningsLevelButton.interactable = true;

        if (StoreController.Instance.IsCanChairBuy())
            StoreChairBuyButton.interactable = true;
        else
            StoreChairBuyButton.interactable = false;


        if (GameData.Instance.Stamina == 0)
            respawnButton.SetActive(true);
        else
            respawnButton.SetActive(false);


        if (!ChairManager.Instance.IsReadyToThrow || StoreController.Instance.IsStoreActive
            || GameData.Instance.IsChairOnDamage)
            StoreInButton.interactable = false;
        else
            StoreInButton.interactable = true;
    }
    private void CoinAndDistanceValuePrint()
    {
        CoinGoster.text = ValuePrint(GameData.Instance.Coin);
        metreGoster.text = GameData.Instance.FallDistance.ToString("00.") + "m";
    }
    public string ValuePrint(double value)
    {
        if (value < 1000f)
            return value.ToString("0");
        else if (value >= 1000 && value < 1000000)
            return (value / 1000f).ToString("0.0") + "K";
        else if (value >= 1000000 && value < 1000000000f)
            return (value / 1000000f).ToString("0.0") + "M";
        else
            return (value / 1000000000f).ToString("0.0") + "B";
    }
    private void TapToStartShow()
    {
        if (IsTutorialShowDone) TapToStartStateChange(TutorialShowState.Inactive);
        else
        {
            if (ChairManager.Instance.TurnAmount < 400f && !StoreController.Instance.IsStoreActive
                && !ChairManager.Instance.GetChair().IsBrokenChair())
                TapToStartStateChange(TutorialShowState.Tap);
            else
            {
                if (StoreController.Instance.IsStoreActive) TapToStartStateChange(TutorialShowState.Inactive);
                else
                {
                    if (ChairManager.Instance.GetChair().IsBrokenChair())
                        TapToStartStateChange(TutorialShowState.Inactive);
                    else
                        TapToStartStateChange(TutorialShowState.Shot);
                }
            }
        }

        TapToStartStateUpdate();
    }
    private void TapToStartStateUpdate()
    {
        switch (tutorialShowState)
        {
            case TutorialShowState.Tap:
                TapObject.SetActive(true);
                SwipeUpObject.SetActive(false);
                break;
            case TutorialShowState.Shot:
                TapObject.SetActive(false);
                SwipeUpObject.SetActive(true);
                break;
            case TutorialShowState.Inactive:
                TapObject.SetActive(false);
                SwipeUpObject.SetActive(false);
                break;
        }
    }
    public void TapToStartStateChange(TutorialShowState showState)
    {
        tutorialShowState = showState;
    }
    public void RespawnButtonFunc()
    {
        GameData.Instance.StaminaUpdate();
        HapticsManager.Instance.CreateHaptic(.3f, .3f);
        HumanHandler guest = ChairManager.Instance.GetChair().GetGuest();
        HumanSpawner.Instance.GuestDespawn(guest, guest.GetFirstGuest());
        HumanSpawner.Instance.GuestSpawn();
        ChairManager.Instance.ResetChair();
        ChairManager.Instance.IsGuestFlying = false;
    }

}
