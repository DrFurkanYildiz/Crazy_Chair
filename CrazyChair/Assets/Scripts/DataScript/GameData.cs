using UnityEngine;
using DG.Tweening;

public class GameData : MonoBehaviour
{
    public static GameData Instance;

    private double coin;
    private double targetCoin;
    public double Coin { get { return coin; }}
    public double TargetCoin { get { return targetCoin; } }

    private float _baseStamina = 100f;
    private float stamina;
    public float Stamina
    {
        get { return stamina; }
        set { stamina = value; }
    }
    public float BaseStamina
    {
        get { return _baseStamina; }
    }

    private float _baseChairSpeed = 10f;
    private float _chairSpeed;
    public float ChairSpeed
    {
        get { return _chairSpeed; }
        set { _chairSpeed = value; }
    }
    public float BaseChairSpeed { get { return _baseChairSpeed; } }




    private int _baseCoinEarnings = 50;
    private int _coinEarnings;
    public int CoinEarnings
    {
        get { return _coinEarnings; }
        set { _coinEarnings = value; }
    }
    public int BaseCoinEarnings { get { return _baseCoinEarnings; } }


    public int StaminaLevel { get { return _staminaLevel; }}
    public int CoinEarningsLevel { get { return _coinEarningsLevel; }}
    public int ChairSpeedLevel { get { return _chairSpeedLevel; }}

    public int GetStaminaIncreasePrice { get { return LevelIncreasePrice(StaminaLevel); } }
    public int GetChairSpeedIncreasePrice { get { return LevelIncreasePrice(ChairSpeedLevel); } }
    public int GetCoinEarningsIncreasePrice { get { return LevelIncreasePrice(CoinEarningsLevel); } }

    private const int MAX_LEVEL = 25;
    public int GetMaxLevel { get { return MAX_LEVEL; } }

    private int _staminaLevel = 1;
    private int _coinEarningsLevel = 1;
    private int _chairSpeedLevel = 1;

    private float _fallDistance;

    public float FallDistance
    {
        get { return _fallDistance; }
        set { _fallDistance = value; }
    }
    public bool IsChairOnDamage { get { return stamina < _baseStamina; } }

    private void Awake()
    {
        Instance = this;
    }
    public void Setup()
    {
        coin = targetCoin;
        stamina = _baseStamina;
        StaminaBar.Instance.SetTargetStamina(_baseStamina);
    }
    public void CoinAdd(int addCoin)
    {
        targetCoin += addCoin;

        DOTween.To(() => coin, x => coin = x, targetCoin, 2);
    }
    public void CoinTake(int takeCoin)
    {
        targetCoin -= takeCoin;

        DOTween.To(() => coin, x => coin = x, targetCoin, 2);
    }
    public void StaminaUpdate()
    {
        Stamina = _baseStamina;
        StaminaBar.Instance.Heal(_baseStamina);
        StaminaBar.Instance.StaminaUpdate();
    }

    public void StaminaLevelUp()
    {
        if (IsStaminaLevelIncrease() && StaminaLevel < MAX_LEVEL)
        {
            StaminaLevelIncrease(); 
            HapticsManager.Instance.CreateHaptic(1f, 1f);
            SaveManager.Instance.OnSave();
        }
    }
    private void StaminaLevelIncrease()
    {
        CoinTake(LevelIncreasePrice(StaminaLevel));
        _baseStamina += StaminaIncreaseAmount();
        StaminaUpdate();
        _staminaLevel++;
    }
    public bool IsStaminaLevelIncrease()
    {
        return targetCoin >= LevelIncreasePrice(StaminaLevel);
    }
    private int StaminaIncreaseAmount()
    {
        return 50;
    }
    
    private int LevelIncreasePrice(int level)
    {
        switch (level)
        {
            case 1: return 500;
            case 2: return 2500;
            case 3: return 4500;
            case 4: return 8000;
            case 5: return 12000;
            case 6: return 17000;
            case 7: return 23000;
            case 8: return 30000;
            case 9: return 38000;
            case 10: return 46800;
            case 11: return 56300;
            case 12: return 66600;
            case 13: return 77800;
            case 14: return 98000;
            case 15: return 120000;
            case 16: return 138300;
            case 17: return 160000;
            case 18: return 185000;
            case 19: return 210000;
            case 20: return 240000;
            case 21: return 268000;
            case 22: return 300000;
            case 23: return 332000;
            case 24: return 365000;
            default: return 0;
        }
    }


    public void ChairSpeedLevelUp()
    {
        if (IsChairSpeedLevelIncrease() && ChairSpeedLevel < MAX_LEVEL)
        {
            ChairSpeedLevelIncrease();
            HapticsManager.Instance.CreateHaptic(1f, 1f);
            SaveManager.Instance.OnSave();
        }
    }
    private void ChairSpeedLevelIncrease()
    {
        CoinTake(LevelIncreasePrice(ChairSpeedLevel));
        _baseChairSpeed += ChairSpeedIncreaseAmount();
        _chairSpeedLevel++;
    }
    public bool IsChairSpeedLevelIncrease()
    {
        return targetCoin >= LevelIncreasePrice(ChairSpeedLevel);
    }
    private int ChairSpeedIncreaseAmount()
    {
        if (ChairSpeedLevel < 13)
        {
            if (ChairSpeedLevel == 2)
                return 2;
            else
                return 3;
        }
        else
            return 5;
    }



    public void CoinEarningsLevelUp()
    {
        if (IsTicketPriceLevelIncrease() && CoinEarningsLevel < MAX_LEVEL)
        {
            CoinEarningsLevelIncrease();
            HapticsManager.Instance.CreateHaptic(1f, 1f);
            SaveManager.Instance.OnSave();
        }
    }
    private void CoinEarningsLevelIncrease()
    {
        CoinTake(LevelIncreasePrice(CoinEarningsLevel));
        _baseCoinEarnings += CoinEarningsIncreaseAmount();
        _coinEarningsLevel++;
    }
    public bool IsTicketPriceLevelIncrease()
    {
        return targetCoin >= LevelIncreasePrice(CoinEarningsLevel);
    }
    private int CoinEarningsIncreaseAmount()
    {
        return 50;
    }

    public int GetChairCost(int chairIndex)
    {
        switch (chairIndex)
        {
            case 1:
                return 10000;
            case 2:
                return 30000;
            case 3:
                return 60000;
            case 4:
                return 120000;
            case 5:
                return 240000;
            case 6:
                return 500000;
            case 7:
                return 1000000;
            default:
                return 0;
        }
    }

    public void SetSaveObject(SaveObject saveObject)
    {
        coin = saveObject.coin;
        targetCoin = saveObject.targetCoin;
        stamina = saveObject.stamina;
        _baseStamina = saveObject._baseStamina;
        _baseChairSpeed = saveObject._baseChairSpeed;
        _chairSpeed = saveObject._chairSpeed;
        _baseCoinEarnings = saveObject._baseCoinEarnings;
        _coinEarnings = saveObject._coinEarnings;
        _staminaLevel = saveObject.staminaLevel;
        _coinEarningsLevel = saveObject.coinEarningsLevel;
        _chairSpeedLevel = saveObject.chairSpeedLevel;
    }
    public SaveObject GetSaveObject()
    {
        return new SaveObject
        {
            coin = coin,
            targetCoin = targetCoin,
            stamina = stamina,
            _baseStamina = _baseStamina,
            _baseChairSpeed = _baseChairSpeed,
            _chairSpeed = _chairSpeed,
            _baseCoinEarnings = _baseCoinEarnings,
            _coinEarnings = _coinEarnings,
            staminaLevel = _staminaLevel,
            coinEarningsLevel = _coinEarningsLevel,
            chairSpeedLevel = _chairSpeedLevel
        };
    }

    [System.Serializable]
    public class SaveObject
    {
        public double coin;

        public double targetCoin;

        public float stamina;
        public float _baseStamina;

        public float _baseChairSpeed;
        public float _chairSpeed;

        public int _baseCoinEarnings;
        public int _coinEarnings;

        public int staminaLevel;
        public int coinEarningsLevel;
        public int chairSpeedLevel;
    }
}