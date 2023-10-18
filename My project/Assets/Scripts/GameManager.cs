using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public obj lastobj;
    public GameObject objPrefab;
    public Transform objGroup;
    public GameObject effectPrefab;
    public Transform effectGroup;
    public int maxlevel;
    public bool isOver;
    public List<obj> objPool;
    public List<ParticleSystem> effectPool;

    [Range(1,30)]
    public int poolSize;
    public int poolCurser;


    public Text scoreText;
    public Text maxScore;
    public Text endScore;
    public GameObject playbox;
    public int score;
    public bool ischain;

    public GameObject endGroup;
    public GameObject startGroup;

    

    void Awake(){
        Application.targetFrameRate = 60;
        objPool = new List<obj>();
        effectPool = new List<ParticleSystem>();
        for (int i = 0;i < poolSize; i++){
            Makeobj();
        }
        if(PlayerPrefs.HasKey("MaxScore")){
            PlayerPrefs.SetInt("MaxScore",0);
        }
        
         
        maxScore.text = PlayerPrefs.GetInt("MaxScore").ToString();
    }
    public void GameStart()
    {
        playbox.SetActive(true);
        scoreText.gameObject.SetActive(true);
        maxScore.gameObject.SetActive(true);
        startGroup.SetActive(false);

        Nextobj();
        isOver = false;
    }
    void Nextobj(){
        if(isOver){
            return;
        }

        lastobj = Getobj();
        lastobj.level = Random.Range(0,maxlevel);
        //lastobj.level = maxlevel++;
        lastobj.gameObject.SetActive(true);
        StartCoroutine("WaitNext");

    }
    obj Makeobj(){
        
        //파티클 생성
        GameObject instanteff = Instantiate(effectPrefab,effectGroup);
        instanteff.name = "Effect" + effectPool.Count;
        ParticleSystem instanteffect = instanteff.GetComponent<ParticleSystem>();
        effectPool.Add(instanteffect);

        //오브젝트 생성
        //instant 변수로 프리펩을 가져올거다
        GameObject instant = Instantiate(objPrefab,objGroup);
        instant.name = "obj" + objPool.Count;
        //가져온 변수와 연결된 컴포넌트중에 obj를 instantobj에 저장할거다
        obj instantobj = instant.GetComponent<obj>();
        instantobj.manager = this;
        instantobj.effect = instanteffect;
        objPool.Add(instantobj);


 
        return instantobj;
    }
    obj Getobj(){
        
        for (int i = 0; i < objPool.Count;i++){
            poolCurser = (poolCurser +1) % objPool.Count;
            if(!objPool[poolCurser].gameObject.activeSelf){
                return objPool[poolCurser];
            }
        }
        return Makeobj();
    }
    IEnumerator WaitNext()
    {
        while(lastobj != null){
             yield return null;
        }
        yield return new WaitForSeconds(0.1f);
        Nextobj();
    }
    public void TouchDown(){
        if(isOver){
            return;
        }

        if(lastobj == null)
            return;
        lastobj.Drag();

    }
      public void TouchUp(){
        if(isOver){
            return;
        }
        if(lastobj == null)
            return;
        lastobj.Drop();
        ischain = false;
        lastobj = null;
    }
    public void GameOver(){
        if(isOver){
            return;
        }


        isOver = true;
        StartCoroutine(GameOverRoutine());
        
    }

    IEnumerator GameOverRoutine(){
        Debug.Log("게임 오버");
        yield return new WaitForSeconds(1f);
        int maxScore = Mathf.Max(score,PlayerPrefs.GetInt("MaxScore"));
        PlayerPrefs.SetInt("MaxScore",maxScore);
        endScore.text = "점수" + score.ToString();

        endGroup.SetActive(true);
        Debug.Log(maxScore);
    }
    public void Reset()
    {
        SceneManager.LoadScene("Main");
    }

void Update(){
    if (Input.GetButtonDown("Cancel")){
        Application.Quit();
        

    }
}
    void LateUpdate(){
        scoreText.text = score.ToString();
    }
}
