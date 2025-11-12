using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ChainShoot : MonoBehaviour
{
    [SerializeField] float refreshRate = 0.01f;
    [SerializeField][Range(1, 10)] int maximunEnemiesInChain = 3;
    [SerializeField] float delayBetweenEachChain = 0.3f;
    [SerializeField] Transform playerFirePoint;
    [SerializeField] EnemyDetector playerEnemyDetector;
    [SerializeField] GameObject LineRandererPrefab;

    bool shooting;
    bool shot;
    float counter = 1;
    GameObject currentClosestEnemy;
    List<GameObject> spawnedLineRenderers = new List<GameObject>();
    List<GameObject> enemiesInChain = new List<GameObject>();
    List<GameObject> activeEffect = new List<GameObject>();

    void StopShooting()
    {
        shooting = false;
        shot = false;
        counter = 1;

        for (int i = 0; i < spawnedLineRenderers.Count; i++)
        {
            Destroy(spawnedLineRenderers[i]);
        }

        spawnedLineRenderers.Clear();
        enemiesInChain.Clear();

        for (int i = 0; i < activeEffect.Count; i++)
        {
            Destroy(activeEffect[i]);
        }

        activeEffect.Clear();

    }

    IEnumerator UpdateLineRenderer(GameObject LineR, Transform startPos, Transform endPos, bool getClosestEnemyToPlayer = false)
    {
        if(shooting && shot && LineR != null)
        {
            LineR.GetComponent<LineRendererController>().SetPosition(startPos, endPos);
            yield return new WaitForSeconds(refreshRate);

            if(currentClosestEnemy != playerEnemyDetector.GetClosestEnemy())
            {
                StopShooting();
                //StartShooting();

            }
        }
        else
        {
            StartCoroutine(UpdateLineRenderer(LineR, startPos, endPos));
        }
    }

    void NewLineRenderer(Transform startPos, Transform endPos, bool getClosestEnemyToPlayer = false)
    {
        GameObject LineR = Instantiate(LineRandererPrefab);
        spawnedLineRenderers.Add(LineR);
        StartCoroutine(UpdateLineRenderer(LineR, startPos, endPos, getClosestEnemyToPlayer));

    }

    IEnumerator ChainReaction(GameObject closestEnemey)
    {
        yield return new WaitForSeconds(delayBetweenEachChain);
        if(counter == maximunEnemiesInChain)
        {
            yield return null;
        }
        else
        {
            if(shooting)
            {
                counter++;
                enemiesInChain.Add(closestEnemey);

                if(!enemiesInChain.Contains(closestEnemey.GetComponent<EnemyDetector>().GetClosestEnemy()))
                {
                    NewLineRenderer(closestEnemey.transform, closestEnemey.GetComponent<EnemyDetector>().GetClosestEnemy().transform);
                        StartCoroutine(ChainReaction(closestEnemey.GetComponent<EnemyDetector>().GetClosestEnemy()));
                }
            }
        }
    }

}
