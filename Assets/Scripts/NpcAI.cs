using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcAI : MonoBehaviour
{

    [Tooltip("移动速度")]
    public int moveSpeed;
    [Tooltip("追击速度")]
    public int chaseSpeed;
    private int vertical;
    private int horizontal;
    private Rigidbody2D r2d;
    [Tooltip("到达目的地后，停止时间")]
    public float stayTime;
    [Tooltip("单次移动时间")]
    public float maxMoveTime = 3;
    [Tooltip("墙壁所在的layer")]
    public string layer;
    [Tooltip("玩家的Transform")]
    public Transform playerTransform;
    public Animation colorAnimation;

    [Tooltip("最大持续碰撞时间，防止怪物被卡主")]
    public float maxColliderTime = 3.2f;
    [Tooltip("怪物被卡住后的重生点")]
    public Transform[] reMakePoint;
    [Tooltip("请输入作为视野范围判定的circle collider的序号，由上到下为0,1.....")]
    public int index;
    [Tooltip("视野范围判定器的半径")]
    public float radius;
    public AnimationClip alphaUp;
    public AnimationClip alphaDown;

    [Header("以下为private变量")]
    [SerializeField] private bool isFind = false;
    [SerializeField] private float nowMoveTime = 0;
    [SerializeField] private float currentStayTime;

    [SerializeField] private int random1 = 0;
    [SerializeField] private int random2 = 0;
    [SerializeField] private int random3 = 0;
    private bool stillMove;
    [SerializeField] private float colliderTime = 0;
    [SerializeField] private bool meetBound = false;
    private CircleCollider2D[] circleCollider2Ds = new CircleCollider2D[10];
    [SerializeField] private bool isCollidering = false;
    //纵向随机数临时值
    private int temp1 = 0;
    //横向随机数临时值
    private int temp2 = 0;



    private void Awake()
    {
        r2d = GetComponent<Rigidbody2D>();
        r2d.freezeRotation = true;
        if (playerTransform == null)
        {
            playerTransform = GameObject.Find("Player").transform;
        }
        circleCollider2Ds = transform.GetComponents<CircleCollider2D>();
        r2d.gravityScale = 0;


    }

    private void Start()
    {
        if (circleCollider2Ds[index] != null)
        {
            circleCollider2Ds[index].radius = radius;
            circleCollider2Ds[index].isTrigger = true;
        }
        else
        {
            Debug.LogError("index超出总collider数目");
        }

        // Debug.Log(alphaDown);

        colorAnimation.clip = alphaDown;
    }

    private void Update()
    {
        UpdateTime();
        currentStayTime += Time.deltaTime;
        circleCollider2Ds[index].radius = radius;
        if (meetBound == false)
        {

            if (isFind == false)
            {
                Move();
                if (currentStayTime >= stayTime + maxMoveTime)
                {
                    currentStayTime = 0;
                }
            }
            if (isFind)
            {
                MoveToPlayer(playerTransform.position);
            }
        }
        else
        {
            StartCoroutine(ColliderReSet());
            meetBound = false;
        }

    }



    IEnumerator ColliderReSet()
    {
        colorAnimation.clip = alphaDown;
        colorAnimation.Play();
        yield return new WaitForSeconds(1.1f);
        transform.position = reMakePoint[0].position;
        colorAnimation.clip = alphaUp;
        colorAnimation.Play();
        //Debug.Log("IEnumerator");

    }

    private void MoveToPlayer(Vector3 playerPos)
    {
        stillMove = true;
        Vector3 temp = playerPos - transform.position;
        vertical = chaseSpeed * (temp.y >= 0 ? 1 : -1);
        horizontal = chaseSpeed * (temp.x >= 0 ? 1 : -1);
        r2d.velocity = new Vector2(horizontal * Time.deltaTime, vertical * Time.deltaTime);
    }

    private void Move()
    {

        if (currentStayTime >= maxMoveTime && currentStayTime <= maxMoveTime + stayTime)
        {
            r2d.velocity = Vector2.zero;
            stillMove = false;
            return;
        }
        stillMove = true;
        nowMoveTime += Time.deltaTime;
        if (nowMoveTime >= maxMoveTime)
        {
            nowMoveTime = 0;
            random3 = Random.Range(0, 100);
            if (random3 < 50)
            {
                //传送
                temp1 = 0;
                temp2 = 2;
                RandomPos();
                return;
            }

            random1 = Random.Range(0, 100);
            random2 = Random.Range(0, 100);
            if (random1 <= 33) temp1 = -1;
            else if (random1 <= 66) temp1 = 0;
            else if (random1 <= 100) temp1 = 1;

            if (random2 <= 33) temp2 = -1;
            else if (random2 <= 66) temp2 = 0;
            else if (random2 <= 100) temp2 = 1;
            //Debug.Log("重新随机数");
        }

        vertical = moveSpeed * temp1;
        horizontal = moveSpeed * temp2;

        r2d.velocity = new Vector2(horizontal * Time.deltaTime, vertical * Time.deltaTime);

    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Hero")
        {
            isFind = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Hero")
        {
            isFind = false;
        }
    }



    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer(layer))
        {
            isCollidering = true;
            //Debug.Log("碰撞了");
        }
    }


    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer(layer))
        {
            isCollidering = false;
        }
    }

    private void UpdateTime()
    {
        if (isCollidering && stillMove)
        {
            colliderTime += Time.deltaTime;
            if (colliderTime >= maxColliderTime)
            {
                meetBound = true;
                colliderTime = 0;
            }
        }
        else
        {
            colliderTime = 0;
        }
    }

    private void RandomPos()
    {
        int random;
        random = Random.Range(0, reMakePoint.Length);
        StartCoroutine(RandomReSet(random));
    }

    IEnumerator RandomReSet(int random)
    {
        colorAnimation.clip = alphaDown;
        colorAnimation.Play();
        yield return new WaitForSeconds(2.2f);
        transform.position = reMakePoint[random].position;
        colorAnimation.clip = alphaUp;
        colorAnimation.Play();

    }

    public void ResetPos(Vector3 pos)
    {
        transform.position = pos;
    }
}
