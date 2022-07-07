using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance;

    [SerializeField] Camera followCamera;
    [SerializeField] Camera storeCamera;
    [SerializeField] MeshRenderer tvRenderer;
    [SerializeField] Material[] renderMat;
    [SerializeField] Material[] materials;

    [SerializeField] private HumanHandler _followingGuest;
    private float smoothSpeed = 2.5f;
    [SerializeField] private Vector3 _offSet;
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        materials = tvRenderer.sharedMaterials;
    }
    private void FixedUpdate()
    {
        FollowUpdate();
    }

    private void FollowUpdate()
    {
        if (_followingGuest != null)
        {
            GameData.Instance.FallDistance = _followingGuest.FlightDistance;

            if (!_followingGuest.gameObject.activeSelf)
            {
                TvScreenDefaultMaterial();
                ResetFollowCamera();
                UISystem.Instance.FollowCameraInctive();
            }
            else
            {
                if (Mathf.Abs(_followingGuest.FlyingCost) >= 300f)
                {
                    TvScreenCameraMaterial();
                    FollowGuest();
                    UISystem.Instance.FollowCameraActive();
                }
                else
                {
                    TvScreenDefaultMaterial();
                    ResetFollowCamera();
                    UISystem.Instance.FollowCameraInctive();
                }
            }
        }
        else
        {
            TvScreenDefaultMaterial();
            ResetFollowCamera();
            UISystem.Instance.FollowCameraInctive();
        }
    }
    public void SetFollowingGuest(HumanHandler human)
    {
        _followingGuest = human;
    }
    public HumanHandler GetFollowingGuest()
    {
        return _followingGuest;
    }
    public void ClearFollowingGuest()
    {
        _followingGuest = null;
    }
    private void FollowGuest()
    {
        Vector3 desiredPosition = _followingGuest.transform.position + _offSet;

        Vector3 smoothedPosition = 
            Vector3.Slerp(followCamera.transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        followCamera.transform.position = smoothedPosition;

        followCamera.transform.LookAt(_followingGuest.transform);
        followCamera.transform.eulerAngles -= Vector3.right * 10f;

    }
    public void SetFollowCameraNear(float near)
    {
        followCamera.nearClipPlane = near;
    }
    private void ResetFollowCamera()
    {
        followCamera.transform.position = Vector3.zero;
        followCamera.transform.rotation = Quaternion.Euler(Vector3.zero);
    }
    private void TvScreenDefaultMaterial()
    {
        materials[1] = renderMat[1];
        tvRenderer.sharedMaterials = materials;
    }
    private void TvScreenCameraMaterial()
    {
        materials[1] = renderMat[0];
        tvRenderer.sharedMaterials = materials;
    }

    public void SwitchStoreCamera()
    {
        followCamera.gameObject.SetActive(false);
        storeCamera.gameObject.SetActive(true);
    }
    public void SwitchGameCamera()
    {
        followCamera.gameObject.SetActive(true);
        storeCamera.gameObject.SetActive(false);
    }
}
