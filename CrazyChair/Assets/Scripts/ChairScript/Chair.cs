using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chair : MonoBehaviour
{
    [HideInInspector] public Transform ChairTopTransform;
    private HumanHandler _guestThrown;
    private float damagePoint = 5f;
    private ChairMovement chairMovement;
    private void Awake()
    {
        ChairTopTransform = transform.GetChild(0);
        chairMovement = GetComponent<ChairMovement>();
    }
    public bool IsEmpty()
    {
        return _guestThrown == null;
    }

    public void SetGuest(HumanHandler guest)
    {
        this._guestThrown = guest;
    }
    public HumanHandler GetGuest()
    {
        return _guestThrown;
    }

    public void ClearGuest()
    {
        _guestThrown = null;
    }

    public Vector3 GetPosition()
    {
        return ChairTopTransform.position + Vector3.forward * 5f;
    }
    public ChairMovement GetChairMovement()
    {
        return chairMovement;
    }
    public bool IsBrokenChair()
    {
        return GameData.Instance.Stamina <= 0f;
    }

    public void OnceReturned()
    {
        TakeChairDamage(damagePoint);
        StaminaBar.Instance.Damage(damagePoint);

        GameData.Instance.CoinAdd(GameData.Instance.BaseCoinEarnings);

        CoinCollectManager.Instance.CoinCollectAnimation(.1f);
        HapticsManager.Instance.CreateHaptic(0.6f, 0.6f);

        ChairManager.Instance.StartCoinParticle();
        SoundManager.Instance.EffectOneShot(SoundManager.Instance.turningEffect);
        SoundManager.Instance.EffectOneShot(SoundManager.Instance.coinTurnEffect);
    }


    public void TakeChairDamage(float damagePoint)
    {
        if (GameData.Instance.Stamina >= damagePoint)
            GameData.Instance.Stamina -= damagePoint;
        else
            GameData.Instance.Stamina = 0f;
    }
}
