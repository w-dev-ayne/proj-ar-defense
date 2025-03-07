using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransformSyncManager : MonoBehaviour
{
    [SerializeField] private GameObject testObj;

    private int generateIndex = 0;

    private Vector3[] generatePositions =
    {
        Vector3.forward,
        Vector3.back,
        Vector3.right,
        Vector3.left
    };

    public Transform offset;

    public void InstantiateTestObject()
    {
        if (offset == null)
            return;
        
        GameObject obj = Instantiate(testObj, offset);
        obj.transform.localPosition = generatePositions[generateIndex++];

        if (generateIndex == 4)
        {
            generateIndex = 0;
        }
    }
    
}
