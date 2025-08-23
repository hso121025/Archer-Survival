using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sound_Mgr : MonoBehaviour
{
    public AudioClip MonsterDieSound;
    public GameObject Audio;

    public static Sound_Mgr Inst;

    void Awake()
    {
        if (Inst == null)
        {
            Inst = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator MonsterSound()
    {
        GameObject MonDieSound = Instantiate(Audio);
        MonDieSound.GetComponent<AudioSource>().clip = MonsterDieSound;
        MonDieSound.GetComponent<AudioSource>().Play();
        
        yield return new WaitForSeconds(MonsterDieSound.length);
    }
}
