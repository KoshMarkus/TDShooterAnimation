using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] GameObject ragdoll;
    [SerializeField] GameObject animatedModel;
    [SerializeField] GameObject enemy;

    void Start()
    {
        ragdoll.SetActive(false);
        animatedModel.SetActive(true);
    }

    public void Death()
    {
        CopyPose(animatedModel.transform, ragdoll.transform);
        ragdoll.SetActive(true);
        animatedModel.SetActive(false);
        gameObject.tag = "Untagged";
        StartCoroutine(CreateAnother());
    }

    private void CopyPose(Transform sourceTransform, Transform targetTransform)
    {
        for (int i = 0; i < sourceTransform.childCount; i++)
        {
            Transform source = sourceTransform.GetChild(i);
            Transform target = targetTransform.GetChild(i);
            target.position = source.position;
            target.rotation = source.rotation;

            CopyPose(source, target);
        }
    }

    IEnumerator CreateAnother()
    {
        yield return new WaitForSeconds(5.0f);
        GameObject newEnemy = Instantiate(enemy, new Vector3(Random.Range(10.0f, -10.0f), 0, Random.Range(10.0f, -10.0f)), Quaternion.identity);
        newEnemy.tag = "Enemy";
    }
}
