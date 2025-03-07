using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ImageManager : MonoBehaviour
{
    [SerializeField] private ARTrackedImageManager m_TrackedImageManager;
    [SerializeField] private Transform cameraTransform;

    [SerializeField] private TextMeshProUGUI imageInfoTmp;
    [SerializeField] private TextMeshProUGUI cameraInfoTmp;
    
    [SerializeField] private TransformSyncManager transformSyncManager;
    [SerializeField] private GameObject imagePrefabObj;

    private Dictionary<string, TrackedImage> contents = new Dictionary<string, TrackedImage>(); // 인식하여 등록된 ARContent들
    private Dictionary<string, ARTrackedImage> images = new Dictionary<string, ARTrackedImage>();
    private ARTrackedImage currentImage; // 현재 인식 중인 이미지

    void OnEnable() => m_TrackedImageManager.trackedImagesChanged += OnChanged;

    void OnDisable() => m_TrackedImageManager.trackedImagesChanged -= OnChanged;

    void OnChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (var newImage in eventArgs.added)
        {
            if (!images.ContainsKey(newImage.referenceImage.name))
            {
                GameObject tempObj = Instantiate(imagePrefabObj);
                
                images.Add(newImage.referenceImage.name, newImage);
                contents.Add(newImage.referenceImage.name, tempObj.GetComponent<TrackedImage>());

                transformSyncManager.offset = tempObj.transform;
                
                Debug.Log($"{newImage.referenceImage.name} TracedImage is Added");
            }
        }

        foreach (var image in eventArgs.updated)
        {
            if (image.trackingState.Equals(TrackingState.Tracking) && !contents[image.referenceImage.name].isTracking) 
            {
                OnImageIn(image);
            }
            // 이미지 Tracking 끊겼을 때 (화면 밖으로 나갔을 때)
            else if (image.trackingState.Equals(TrackingState.Limited) && contents[image.referenceImage.name].isTracking) 
            {
                OnImageOut(image);
            }
        }
    }
    
    private void OnImageIn(ARTrackedImage image)
    {
        Debug.Log($"{image.referenceImage.name} is in.");
        
        contents[image.referenceImage.name].isTracking = true;
        
        GameObject contentObj = contents[image.referenceImage.name].gameObject;
        
        contentObj.transform.SetParent(image.transform);
        contentObj.transform.localPosition = Vector3.zero;
        //contentObj.transform.localEulerAngles = new Vector3(0, 180,0);

        contentObj.SetActive(true);
    }
    
    /// <summary>
    /// 이미지가 화면 밖으로 나가거나, 인식이 되지 않을 때 호출
    /// </summary>
    /// <param name="image">인식을 놓친 이미지</param>
    private void OnImageOut(ARTrackedImage image) // 이미지 인식 안 될 때
    {
        Debug.Log($"{image.referenceImage.name} is out.");

        GameObject contentObj = contents[image.referenceImage.name].gameObject;
        
        contentObj.transform.SetParent(null);
        
        contents[image.referenceImage.name].isTracking = false;
        
    }

    public void OnClickTestButton()
    {
        GameObject tempObj = Instantiate(imagePrefabObj);
    }
    
    void ListAllImages()
    {
        Debug.Log(
            $"There are {m_TrackedImageManager.trackables.count} images being tracked.");

        foreach (var trackedImage in m_TrackedImageManager.trackables)
        {
            imageInfoTmp.text = $"Image Position : {trackedImage.transform.position}\n" +
                                $"Image Rotation : {trackedImage.transform.eulerAngles}";
            
            cameraInfoTmp.text = $"Camera Position : {cameraTransform.position}\n" +
                                 $"Camera Rotation : {cameraTransform.eulerAngles}";
            
            Debug.Log($"Image: {trackedImage.referenceImage.name} is at " +
                      $"{trackedImage.transform.position}");
        }
    }

    void Update()
    {
        //ListAllImages();
    }
}
