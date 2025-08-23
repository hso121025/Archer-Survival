using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
public enum ItemType
{
    EXP,
    Coin,
    Bomb,
    Magnet,
    Heart,
    None
}

public class Item_Ctrl : MonoBehaviour
{
    public ItemType itemType = ItemType.None;

    public int MaxEXP = 100;

    private bool MovingToPlayer = false;
    private Transform targetPlayer;

    // Start is called before the first frame update
    void Start()
    {
        targetPlayer = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (MovingToPlayer && targetPlayer != null)
        {
            float speed = 50f;
            transform.position = Vector3.MoveTowards(transform.position, targetPlayer.position, speed * Time.deltaTime);



            if (Vector3.Distance(transform.position, targetPlayer.position) < 0.5f)
            {
                
                // 도착 시 처리
                GetItem(targetPlayer.GetComponent<PlayerCtrl>());
                EXPPool_Mgr.Inst.ReturnToPool(this.gameObject);
            }
        }
    }
    void OnCollisionEnter(Collision coll)
    {
        if (coll.gameObject.tag == "Player")
        {
            PlayerCtrl player = coll.gameObject.GetComponent<PlayerCtrl>();
            if (player != null)
            {
                GetItem(player);
                EXPPool_Mgr.Inst.ReturnToPool(this.gameObject);
            }
        }
    }

    void GetItem(PlayerCtrl player)
    {
        switch (itemType)
        {
            case ItemType.EXP:
                {
                    if (GameMgr.Inst != null)
                    {
                        GameMgr.Inst.GetExp(10);
                    }
                    break;
                }
            case ItemType.Coin:
                {
                    if (GameMgr.Inst != null)
                    {
                        GameMgr.Inst.GetCoin(1);
                    }
                    break;
                }
            case ItemType.Bomb:
                {
                    Collider[] colls = Physics.OverlapSphere(transform.position, 500.0f);

                    foreach (Collider collider in colls)
                    {
                        Monster_Ctrl MonCtrl = collider.GetComponent<Monster_Ctrl>();
                        TurtleShell_Ctrl turtleShell_Ctrl = collider.GetComponent<TurtleShell_Ctrl>();

                        if (MonCtrl != null || turtleShell_Ctrl != null)
                        {
                            MonsterPool_Mgr.Inst.ReturnToPool(collider.gameObject);
                        }
                    }

                    GameMgr.Inst.StartCoroutine(GameMgr.Inst.BombEffect());

                    break;
                }
            case ItemType.Magnet:
                {
                    Item_Ctrl[] items = FindObjectsOfType<Item_Ctrl>();

                    foreach (Item_Ctrl item in items)
                    {
                        if (item.itemType != ItemType.EXP)
                            continue;

                        item.GetComponent<CapsuleCollider>().isTrigger = true;

                        item.StartMoveToPlayer(player.transform);  // 여기서 직접 이동 시작!
                    }
                    break;
                }
            case ItemType.Heart:
                {
                    player.Hp += 50;
                    if (player.Hp > player.initHp)
                        player.Hp = player.initHp;

                    player.imgHpbar.fillAmount = (float)player.Hp / player.initHp;
                    break;
                }
        }

        // EXP 아이템만 풀로 반환
        if (itemType == ItemType.EXP)
        {
            EXPPool_Mgr.Inst.ReturnToPool(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void StartMoveToPlayer(Transform player)
    {
        MovingToPlayer = true;
        targetPlayer = player;
    }

    public void ResetItem()
    {
        MovingToPlayer = false;
        targetPlayer = null;
        GetComponent<Collider>().isTrigger = false;
        // 필요한 초기 상태를 여기서 모두 리셋
    }

}
