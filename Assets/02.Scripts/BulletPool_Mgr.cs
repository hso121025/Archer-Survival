using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPool_Mgr : MonoBehaviour
{
    public static BulletPool_Mgr Inst;

    [SerializeField] private GameObject BulletPrefab;
    [SerializeField] private int InitPoolSize = 10;

    private List<GameObject> BulletPool = new List<GameObject>();

    public Transform Bullets;

    private void Awake()
    {
        Inst = this;

        for (int i = 0; i < InitPoolSize; i++)
        {
            GameObject bullet = Instantiate(BulletPrefab, Bullets);
            bullet.SetActive(false);
            BulletPool.Add(bullet);
        }
    }

    public GameObject GetBullet(Vector3 position, Quaternion rotation)
    {
        foreach (var bullet in BulletPool)
        {
            if (bullet == null)
                continue;

            if (!bullet.activeSelf)  //  �߿�! activeInHierarchy���� Ȯ����
            {
                bullet.transform.position = position;
                bullet.transform.rotation = rotation;
                bullet.SetActive(true);
                return bullet;
            }
        }

        // �� ��� ���̸� ���� ����
        GameObject newBullet = Instantiate(BulletPrefab, position, rotation, Bullets);
        newBullet.SetActive(true);  //  �� ����� ��
        BulletPool.Add(newBullet);
        return newBullet;
    }

    public void ReturnToPool(GameObject bullet)
    {
        bullet.SetActive(false);
    }


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
