using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OlympicCeremony : MonoBehaviour
{
    [SerializeField] AudioClip olympicAnthem;
    [SerializeField] Nations nations;
    [SerializeField] Leaderboards leaderboards;
    [SerializeField] PlayerEntity[] playerEntities;
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
    Player gold;
    bool moving = false;
    bool playingAnthem = false;
    RectTransform moveMe;

    private void Start()
    {
        positions = new Vector3[3];
        for (int i = 0; i < 3; i++)
        {
            positions[i] = transforms[i].localPosition;
        }
    }

    private void Update()
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
    public void StartCeremony(Player gold, Player silver, Player bronze)
    {
        soundtrack.StopSoundtrack();
        movingTime = 0;
        j = 0;
        this.gold = gold;
        speed = Distance / transferTime;
        print("speed: " + speed);
        Player[] winners = new Player[3] { bronze, silver, gold };
        GetComponent<Canvas>().enabled = true;
        GetComponent<AudioSource>().clip = olympicAnthem;
        GetComponent<AudioSource>().Play();
        for (int i = 0; i < 3; i++)
        {
            print(winners[i].name);
            playerEntities[i].flag.texture = nations.GetNation(winners[i].nationality).flag;
            playerEntities[i].name.text = winners[i].name;
            Invoke("TransferEntity", i * (transferTime + coolDown));
        }     
    }

    private void StartAnthem()
    {
        moving = false;
        playingAnthem = true;
        GetComponent<AudioSource>().Stop();
        GetComponent<AudioSource>().clip = nations.GetNation(gold.nationality).anthem;
        GetComponent<AudioSource>().Play();
        if (!gold.NPC) Invoke("ReplayOlympicAnthem", GetComponent<AudioSource>().clip.length + AnthemOlympicCoolDown);
        else Invoke("ContinueSoundtrack", GetComponent<AudioSource>().clip.length + AnthemOlympicCoolDown);
    }

    private void ReplayOlympicAnthem()
    {
        if (playingAnthem)
        {
            GetComponent<AudioSource>().Stop();
            GetComponent<AudioSource>().clip = olympicAnthem;
            GetComponent<AudioSource>().Play();
            Invoke("ContinueSoundtrack", GetComponent<AudioSource>().clip.length + AnthemOlympicCoolDown);
        }        
    }

    private void ContinueSoundtrack()
    {
        soundtrack.PlaySoundtrack();
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

    private void ConcludeCeremony()
    {
        playingAnthem = false;
        //GetComponent<AudioSource>().Stop();
        for (int i = 0; i < 3; i++)
        {
            transforms[i].localPosition = positions[i];
        }
        GetComponent<Canvas>().enabled = false;
        leaderboards.UpdateCaller(2);
        leaderboards.DisplayPlayerMedalTable();
    }
    [System.Serializable]
    public class PlayerEntity
    {
        [SerializeField] public RawImage flag;
        [SerializeField] public Text name;
    }
}
