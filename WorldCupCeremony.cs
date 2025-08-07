using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldCupCeremony : MonoBehaviour
{
    [SerializeField] AudioClip worldCupAnthem;
    [SerializeField] AudioClip fullAnthem;
    //[SerializeField] Nations nations;
    [SerializeField] Leaderboards leaderboards;
    [SerializeField] NationEntity[] nationEntities;
    [SerializeField] RectTransform[] transforms;
    [SerializeField] float transferTime;
    [SerializeField] float coolDown;
    [SerializeField] float ExtraAnthemDelay = 20f;
    [SerializeField] float AnthemOlympicCoolDown = 5f;
    [SerializeField] const float Distance = 527.5f;
    [SerializeField] Vector3[] positions;
    [SerializeField] Soundtrack soundtrack;
    int j = 0;
    float movingTime = 0;
    float speed;
    Nation first;
    bool moving = false;
    bool playingAnthem = false;
    RectTransform moveMe;


    public void StartCeremony(Nation first, Nation second, Nation third)
    {
        soundtrack.StopSoundtrack();
        movingTime = 0;
        j = 0;
        this.first = first;
        speed = Distance / transferTime;
        print("speed: " + speed);
        Nation[] winners = new Nation[3] { third, second, first };
        GetComponent<Canvas>().enabled = true;
        GetComponent<AudioSource>().clip = worldCupAnthem;
        GetComponent<AudioSource>().Play();
        for (int i = 0; i < 3; i++)
        {
            print(winners[i].name);
            nationEntities[i].flag.texture = winners[i].flag;
            nationEntities[i].name.text = winners[i].name;
            Invoke("TransferEntity", i * (transferTime + coolDown));
        }
    }
    void Start()
    {
        positions = new Vector3[3];
        for (int i = 0; i < 3; i++)
        {
            positions[i] = transforms[i].localPosition;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (moving && movingTime < transferTime)
        {
            //moveMe.LookAt(moveMe.position + new Vector3(moveMe.position.x, moveMe.position.y + 1, moveMe.position.z));
            //moveMe.localPosition = new Vector3(0, 0, 0);
            //print("Distance = " + (speed * Time.deltaTime).ToString());
            moveMe.localPosition = new Vector3(moveMe.localPosition.x, moveMe.localPosition.y + speed * Time.deltaTime, moveMe.localPosition.z);
            //moveMe.transform.Translate(Vector3.up * speed * Time.deltaTime);
            //print("Transforming " + moveMe.name + " to " + moveMe.localPosition);
            movingTime += Time.deltaTime;
        }
        if (playingAnthem && Input.GetMouseButtonDown(0))
        {
            ConcludeCeremony();
        }
    }

    private void TransferEntity()
    {
        moving = true;
        movingTime = 0;
        moveMe = transforms[j];
        //transforms[j].Translate(new Vector3(0, 1, 0));
        j++;
        //if (j == 3)
        //{
        //    GetComponent<AudioSource>().clip = nations.GetNation(gold.nationality).anthem;
        //    GetComponent<AudioSource>().Play();
        //}
        if (j == 3)
        {
            Invoke("StartAnthem", transferTime + coolDown + ExtraAnthemDelay);
        }
    }

    private void StartAnthem()
    {
        moving = false;
        playingAnthem = true;
        GetComponent<AudioSource>().Stop();
        GetComponent<AudioSource>().clip = first.anthem;
        GetComponent<AudioSource>().Play();
        if (first.name == "Israel") Invoke("PlayFullAnthem", GetComponent<AudioSource>().clip.length + AnthemOlympicCoolDown);
        else Invoke("ContinueSoundtrack", GetComponent<AudioSource>().clip.length + AnthemOlympicCoolDown);
    }

    private void PlayFullAnthem()
    {
        if (playingAnthem)
        {
            GetComponent<AudioSource>().Stop();
            GetComponent<AudioSource>().clip = fullAnthem;
            GetComponent<AudioSource>().Play();
            Invoke("ContinueSoundtrack", GetComponent<AudioSource>().clip.length + AnthemOlympicCoolDown);
        }
    }

    private void ContinueSoundtrack()
    {
        soundtrack.PlaySoundtrack();
    }

    private void ConcludeCeremony()
    {
        playingAnthem = false;
        //GetComponent<AudioSource>().Stop();
        for (int i = 0; i < 3; i++)
        {
            transforms[i].localPosition = positions[i];
        }
        GetComponent<Canvas>().enabled = false;
        leaderboards.UpdateCaller(4);
        leaderboards.DisplayNationMedalTable(true);
    }

    [System.Serializable]
    public class NationEntity
    {
        [SerializeField] public RawImage flag;
        [SerializeField] public Text name;
    }
}
