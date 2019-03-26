using FoW;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class HeroControl : MonoBehaviour {
    private Rigidbody2D rb;
    public GameObject ob;
    public ParticleSystem deathParticle;
    bool eyeTake=false;
   private float eyeSight=15;
    
    [Range(0.0f, 180.0f)]
   private float eyeAngle=20;
     
    private float speed=250f;
    float h = 0;
    float v = 0;
   public Transform cameraTransform;
   private Animator _animator;
    private FogOfWarUnit _shadow;
    // Use this for initialization
    void Start () {
        rb = this.GetComponent<Rigidbody2D>();
        _animator = this.GetComponent<Animator>();
        _shadow = ob.GetComponent<FogOfWarUnit>();
        ob.transform.position = this.transform.position;
         
    }
	
	// Update is called once per frame
	void Update () {
        TurnMove();
        AnimationPlay();
    }
    void TurnMove() {
        h = Input.GetAxis("Horizontal") * speed * Time.fixedDeltaTime;
        v = Input.GetAxis("Vertical") * speed * Time.fixedDeltaTime;
        if (h > 0.1)
        { //rb.MovePosition(rb.position + Vector2.right * h);
            v = 0;
 
            rb.velocity = (Vector2.right * h);
           ob.transform.rotation = Quaternion.Euler(0, 0, -90);
            //_animator.SetFloat("hor", h);
            if (eyeTake == true)
                _shadow.offset = new Vector2(0.67f, -2.65f);
        }
        if (h < -0.1)
        {
            v = 0;
            //rb.MovePosition(rb.position + Vector2.right * h);
            rb.velocity  = (Vector2.right * h);
            ob.transform.rotation = Quaternion.Euler(0, 0, 90);
            //_animator.SetFloat("hor", h);
            if (eyeTake == true)
                _shadow.offset = new Vector2(-0.6f, -2.7f);
        }
        if (v > 0.1)
        {
            h = 0;
            //rb.MovePosition(rb.position + Vector2.up * v);
            rb.velocity = (Vector2.up * v);
            ob.transform.rotation = Quaternion.Euler(0, 0, 0);
           // _animator.SetFloat("ver", v);
            if (eyeTake == true)
                _shadow.offset = new Vector2(-0.5f, -3.64f);
        }
        if (v < -0.1)
        {
            h = 0;
            //rb.MovePosition(rb.position + Vector2.up * v);
            rb.velocity = (Vector2.up * v);
            ob.transform.rotation = Quaternion.Euler(0, 0, 180);
            //_animator.SetFloat("ver", v);
            if (eyeTake == true)
                _shadow.offset = new Vector2(0.5f, -2.7f);

        }
        if (Mathf.Abs(h) < 0.01 && Mathf.Abs(v) < 0.01)
            rb.velocity = Vector2.zero;
       
    }
    void AnimationPlay() {
        _animator.SetFloat("hor", h);
        _animator.SetFloat("ver", v);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "eye")
        {
            _shadow.circleRadius = eyeSight;
            _shadow.absoluteOffset = false;
            _shadow.angle = eyeAngle;
            //  _shadow.offset.y = -2.45f;
            eyeTake = true;

        }
      
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "enemy")
        {
            GameObject.Instantiate(deathParticle, transform.position, transform.rotation);

            gameObject.GetComponent<SpriteRenderer>().enabled = false;
        }
    }
}
