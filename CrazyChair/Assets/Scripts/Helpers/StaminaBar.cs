using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class StaminaBar : MonoBehaviour {

    public static StaminaBar Instance;

    [SerializeField] private AnimationCurve curve;
    [SerializeField] private float targetStamina;
    [SerializeField] private float stamina;


    private float barMaskWidth;
    private RectTransform barMaskRectTransform;
    private RectTransform edgeRectTransform;
    private RawImage barRawImage;

    public Gradient gradient;
    private void Awake() {
        Instance = this;

        barMaskRectTransform = transform.Find("barMask").GetComponent<RectTransform>();
        barRawImage = transform.Find("barMask").Find("bar").GetComponent<RawImage>();
        edgeRectTransform = transform.Find("edge").GetComponent<RectTransform>();

        barMaskWidth = barMaskRectTransform.sizeDelta.x;
    }
    private void Update() {
        ColorChanger();
        StaminaUIBarUpdate();
    }

    public void StaminaUpdate()
    {
        if (targetStamina > GameData.Instance.BaseStamina)
            targetStamina = GameData.Instance.BaseStamina;

        DOTween.To(() => stamina, x => stamina = x, targetStamina, .5f);
    }
    private void StaminaUIBarUpdate()
    {
        Rect uvRect = barRawImage.uvRect;
        uvRect.x += .2f * Time.deltaTime;
        barRawImage.uvRect = uvRect;

        Vector2 barMaskSizeDelta = barMaskRectTransform.sizeDelta;
        barMaskSizeDelta.x = GetStaminaNormalized() * barMaskWidth;
        barMaskRectTransform.sizeDelta = barMaskSizeDelta;

        edgeRectTransform.anchoredPosition = new Vector2(GetStaminaNormalized() * barMaskWidth, 0);


        edgeRectTransform.gameObject.SetActive(stamina > .2f && GetStaminaNormalized() < 1);
    }
    public void SetTargetStamina(float stamina)
    {
        targetStamina = stamina;
        this.stamina = stamina;
    }
    public float GetStaminaNormalized()
    {
        return stamina / GameData.Instance.BaseStamina;
    }
    public void Damage(float damagePoint)
    {

        if (targetStamina > 0)
        {
            targetStamina -= damagePoint;
            StaminaUpdate();
        }
    }
    public void Heal(float healingPoints)
    {
        if (targetStamina < GameData.Instance.BaseStamina)
        {
            targetStamina += healingPoints;
            StaminaUpdate();
        }
    }
    private void ColorChanger()
    {
        barRawImage.color = gradient.Evaluate(stamina / GameData.Instance.BaseStamina);
    }
}