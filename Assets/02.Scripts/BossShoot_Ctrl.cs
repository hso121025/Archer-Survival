using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BossShootCtrl : MonoBehaviour
{
    public GameObject BossBullet;   //�Ѿ� ������
    public Transform BossFirePos;   //�Ѿ� �߻� ��ǥ

    public Transform Bullets;

    PlayerCtrl player;
    BossCtrl boss;

    public int bulletCount = 36;
    public float interval = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        player = GameMgr.Inst.Player.GetComponent<PlayerCtrl>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public IEnumerator CreateBullet()
    {
        Vector3 targetPos = new Vector3(player.transform.position.x, BossFirePos.position.y, player.transform.position.z);
        Vector3 Dir = (targetPos - BossFirePos.position).normalized;
        Quaternion rot = Quaternion.LookRotation(Dir);

        GameObject bullet = Instantiate(BossBullet, BossFirePos.transform.position, rot, Bullets);

        bullet.GetComponent<BossBullet_Ctrl>().Init(Dir);

        yield return new WaitForSeconds(0.1f);

    }

    public IEnumerator Create360Bullets(int repeatCount = 3)
    {
        Debug.Log("[BossShoot] 360�� �Ѿ� �߻� ����");

        int bulletCount = 24;
        float interval = 1f;

        for (int i = 0; i < repeatCount; i++)
        {
            float angleStep = 360f / bulletCount;

            for (int j = 0; j < bulletCount; j++)
            {
                float angle = j * angleStep;
                Vector3 dir = Quaternion.Euler(0, angle, 0) * Vector3.forward;

                Quaternion rot = Quaternion.LookRotation(dir);
                Vector3 spawnPos = transform.position + Vector3.up * 5f; // ���� ��ġ ����, �ణ ����

                GameObject bullet = Instantiate(BossBullet,spawnPos, rot);

                BossBullet_Ctrl ctrl = bullet.GetComponent<BossBullet_Ctrl>();
                ctrl.SetSpeed(700f);
                ctrl.Init(dir);  // �ӵ� ���� ����
            }

            yield return new WaitForSeconds(interval);
        }
    }
}

