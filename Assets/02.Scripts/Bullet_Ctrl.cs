using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEngine.Pool;

public class Bullet_Ctrl : MonoBehaviour
{
    public float damage = 20f;

    //�Ѿ� �߻� �ӵ�
    public float speed = 3000.0f;

    private Rigidbody Rigid;

    private void Awake()
    {
        Rigid = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        Rigid.velocity = Vector3.zero;
        Rigid.angularVelocity = Vector3.zero;
        Invoke("ReturnToPool", 5.0f); // ���� �ð� �� Ǯ�� ����
    }
    private void OnDisable()
    {
        CancelInvoke(); // ��Ȱ��ȭ �� Invoke ���
    }

    void Start()
    {
        speed = 3000.0f;

        //Destroy(gameObject, 4.0f);  //4�ʵ� ����
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider coll)
    {
        if (coll.gameObject.layer == LayerMask.NameToLayer("Wall"))
        ReturnToPool();
    }

    void ReturnToPool()
    {
        BulletPool_Mgr.Inst.ReturnToPool(this.gameObject);
    }

    public void Init(Vector3 dir, float damage)
    {
        this.damage = damage;

        Rigidbody rb = GetComponent<Rigidbody>();
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.AddForce(dir * speed);

        StartCoroutine(AutoReturn());
    }

    IEnumerator AutoReturn()
    {
        yield return new WaitForSeconds(4f);
        BulletPool_Mgr.Inst.ReturnToPool(this.gameObject);
    }
}
