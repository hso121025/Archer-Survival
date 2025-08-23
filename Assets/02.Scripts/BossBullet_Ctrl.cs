using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class BossBullet_Ctrl : MonoBehaviour
{
    //총알의 파괴력
    public int damage = 20;
    //총알 발사 속도
    public float speed = 10.0f;

    private Rigidbody Rigid;

    private void Awake()
    {
        Rigid = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        Rigid.velocity = Vector3.zero;
        Rigid.angularVelocity = Vector3.zero;
    }

    void Start()
    {
        speed = 10f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider coll)
    {
        if (coll.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            return;

        Destroy(this.gameObject);

    }

    public void Init(Vector3 dir)
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.AddForce(dir * speed);
    }

    public void SetSpeed(float s)
    {
        speed = s;
    }
}
