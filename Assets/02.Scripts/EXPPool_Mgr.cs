using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EXPPool_Mgr : MonoBehaviour
{
    public static EXPPool_Mgr Inst;

    [SerializeField] private GameObject EXPPrefab;
    [SerializeField] private int InitPoolSize = 1;

    private List<GameObject> ExpPool = new List<GameObject>();

    public Transform EXPs;

    private void Awake()
    {
        Inst = this;

        for (int i = 0; i < InitPoolSize; i++)
        {
            GameObject exp = Instantiate(EXPPrefab, EXPs);
            exp.SetActive(false);
            ExpPool.Add(exp);
        }
    }

    public GameObject GetEXP(Vector3 position)
    {
        foreach (GameObject exp in ExpPool)
        {
            if (!exp.activeInHierarchy)
            {
                exp.transform.position = position;
                exp.SetActive(true);
                return exp;
            }
        }

        // 없으면 새로 만들어서 추가
        GameObject newExp = Instantiate(EXPPrefab, EXPs);
        newExp.transform.position = position;
        ExpPool.Add(newExp);
        return newExp;
    }

    public void ReturnToPool(GameObject exp)
    {
        Item_Ctrl itemCtrl = exp.GetComponent<Item_Ctrl>();
        if (itemCtrl != null)
            itemCtrl.ResetItem();

        exp.SetActive(false);
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
