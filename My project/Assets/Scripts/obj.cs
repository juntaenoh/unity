using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.VisualScripting;
using UnityEditor.Build.Player;
using UnityEngine;

public class obj : MonoBehaviour
{

    public GameManager manager;
    public ParticleSystem effect;
    public bool isDrag;
    Rigidbody2D rigid;
    CircleCollider2D circlecollider;
    public int level;
    public bool isMerge;
    float deadtime;
    Animator ani;
    void Awake(){
        rigid = GetComponent<Rigidbody2D>();
        ani = GetComponent<Animator>();
    }

    void OnEnable(){
        ani.SetInteger("Level",level);
        
    }
    void OnDisable(){
        level = 0;
        isDrag = false;
        isMerge = false;
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.zero;

        rigid.simulated = false;
        rigid.velocity = Vector2.zero;
        rigid.angularVelocity = 0;
        circlecollider.enabled = true;
    }
    void Update()
    {
        if (isDrag){
            Vector3 mousePos =Camera.main.ScreenToWorldPoint(Input.mousePosition);
            float leftBorder = -3.7f + transform.localScale.x/2f;
            float rightBorder = 3.7f -transform.localScale.x/2f;

            if(mousePos.x < leftBorder){
                mousePos.x = leftBorder;
            }
            else if(mousePos.x > rightBorder){
                mousePos.x = rightBorder;
            }

            mousePos.y = 8;
            mousePos.z = 0;
            transform.position = Vector3.Lerp(transform.position,mousePos,0.97f) ;
        }
        
    }
    public void Drag(){
        isDrag = true;

    }
    public void Drop(){

        isDrag = false;
        rigid.simulated = true;
    }
    //2d 충돌 발생하면 자동호출
void OnCollisionStay2D(Collision2D collision){
        //만약 충돌한 오브젝트가 obj라면
        if(collision.gameObject.tag == "obj"){
            //충돌한 오브젝트인 obj의 컴포넌트를 가져와서 other에 저장한다
            obj other = collision.gameObject.GetComponent<obj>();

            if(level == other.level && !isMerge && !other.isMerge && level < 10){
                //위치 가져오기
                float MY_X = transform.position.x;
                float MY_Y = transform.position.y;
                float other_X = other.transform.position.x;
                float other_Y = other.transform.position.y;
                if (MY_Y < other_Y || (MY_Y == other_Y && MY_X > other_X)){
                    levelUp();
       
                    other.Hide(transform.position);

                    
                }

            }
        }

    }
    public void levelUp(){
        checkscore();
        isMerge = true;
        ani.SetInteger("Level",level+1);
        EffectPlay();
        level ++;
        if(manager.maxlevel <= 7){
            manager.maxlevel = Mathf.Max(level,manager.maxlevel);
        }
        isMerge = false;
    }
    public void Hide(Vector3 targetPos){
        isMerge = true;
        rigid.simulated = false;

        isMerge = false;
        gameObject.SetActive(false);
        circlecollider.enabled = false;

    }
    public void checkscore(){
    //연쇄 점수
    if (manager.ischain == true){
        manager.score += (level+1)*2;
    } 
    //그냥 점수
    else if (manager.ischain == false){
        manager.score += level+1;
    }
    manager.ischain = true;
    }
    void OnTriggerExit2D(Collider2D collision){
        if(collision.tag == "Finish"){
            deadtime = 0;
        }

    }
    void OnTriggerStay2D(Collider2D collision){
        if (collision.tag == "Finish"){
            deadtime += Time.deltaTime;
            if(deadtime > 2){
                manager.GameOver();
                
            }
        }
    }
    void EffectPlay(){
        effect.transform.position = transform.position;
        effect.transform.localScale = transform.localScale;
        effect.Play();
    }
}
