using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Lovatto.MobileInput;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyGenerator : MonoBehaviour
{
    public float generateTerm = 1.5f;

    [SerializeField] 
    private float generateRange = 3.0f;

    private bool isGenerating = false;

    public List<GameObject> enemies = new List<GameObject>();
    public GameObject enemyPrefab;

    private void Start()
    {
        //GenerateEnemies();
    }

    public void GenerateEnemies()
    {
        isGenerating = true;
        StartCoroutine(CoGenerate());
    }

    public void GenerateEnemy()
    {
        GameObject enemy = Instantiate(enemyPrefab, getRandomVector(), Quaternion.identity, this.transform);
        enemies.Add(enemy);
    }
    
    private IEnumerator CoGenerate()
    {
        WaitForSeconds term = new WaitForSeconds(generateTerm);
        
        while (isGenerating)
        {
            GenerateEnemy();
            yield return term;
        }
    }

    private Vector3 getRandomVector()
    {
        float x = Random.Range(-1.0f, 1.0f);
        float z = Random.Range(-1.0f, 1.0f);

        Vector3 randomVector3 = new Vector3(x, 0 , z).normalized;
        Vector3 camVector = Camera.main.transform.position - new Vector3(0, Camera.main.transform.position.y, 0);

        return (randomVector3 * generateRange) + new Vector3(0, RayDetector.Instance.gamePlane.position.y, 0) + camVector;
    }

    public void StopGenerate()
    {
        StopAllCoroutines();

        foreach (Target target in FindObjectsOfType<Target>())
        {
            Destroy(target.gameObject);
        }
    }
}
