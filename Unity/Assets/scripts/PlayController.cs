using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayController : MonoBehaviour
{
    public Rigidbody2D rb;
    public Animator anim;
    public LayerMask ground;
    public Collider2D coll;
    public AudioSource jumpAudio, hurtAudio, cherryAudio;
    public float speed = 10;
    public float Jumpforce;
    public int Cherry = 0;
    public Text cherrynum;
    public int HP = 3;
    public Text HP_num;
    private bool isHurt;

    // Start is called before the first frame update
    void Start()
    {
        PlayerPrefs.DeleteAll();
        rb = GetComponent<Rigidbody2D>();
        //transform.position = GameManger.Instance.lastPosition;

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!isHurt)
        {
            Movement();
        }
        SwichAnim();
    }

    private void Update()
    {
        Jump();
        cherrynum.text = Cherry.ToString();
        HP_num.text = HP.ToString();
    }

    void Movement()
    {
        float honrizontalmove = Input.GetAxis("Horizontal");
        float facedircetion = Input.GetAxisRaw("Horizontal");

        //Character movement
        if (honrizontalmove != 0)
        {
            rb.velocity = new Vector2(honrizontalmove * speed * Time.fixedDeltaTime, rb.velocity.y);
            anim.SetFloat("running", Mathf.Abs(facedircetion));
        }

        if (facedircetion != 0)
        {
            transform.localScale = new Vector3(facedircetion, 1, 1);
        }
    }

    void SwichAnim()
    {
        anim.SetBool("idle", false);

        if (rb.velocity.y < 0.1f && !coll.IsTouchingLayers(ground))
        {
            anim.SetBool("falling", true);
        }
        if (anim.GetBool("jumping"))
        {
            if (rb.velocity.y < 0)
            {
                anim.SetBool("jumping", false);
                anim.SetBool("falling", true);
            }
        }
        else if (isHurt)
        {
            anim.SetBool("hurting", true);
            anim.SetFloat("running", 0);
            if (Mathf.Abs(rb.velocity.x) < 0.1f)
            {
                anim.SetBool("hurting", false);
                anim.SetBool("idle", true);
                isHurt = false;
            }
        }
        else if (coll.IsTouchingLayers(ground))
        {
            anim.SetBool("falling", false);
            anim.SetBool("idle", true);
        }
    }

    //Jump
    void Jump()
    {
        if (Input.GetButtonDown("Jump") && coll.IsTouchingLayers(ground))
        {
            rb.velocity = new Vector2(rb.velocity.x, Jumpforce * Time.fixedDeltaTime);
            jumpAudio.Play();
            anim.SetBool("jumping", true);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //collect 
        if (collision.tag == "Collection")
        {
            cherryAudio.Play();
            collision.GetComponent<Animator>().Play("Item-feedback-Animation");
        }

        //Load
        if (collision.tag == "Deadline")
        {
            hurtAudio.Play();
           // GetComponent<AudioSource>().enabled = false;
            HP -= 1;
            if (HP != 0)
            {
                Load();
            }
            else if(HP == 0)
            {
                Gameover();
            }

        }

        //save
        if (collision.tag == "Save")
        {
            Save();
        }
    }

    //kill monster
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            if (anim.GetBool("falling"))
            {
                enemy.JumpOn();
                rb.velocity = new Vector2(rb.velocity.x, Jumpforce * Time.fixedDeltaTime);
                anim.SetBool("jumping", true);
            }
            //hurt
            else if (transform.position.x < collision.gameObject.transform.position.x)
            {
                HP -= 1;
                if (HP == 0)
                {
                    Gameover();
                }
                rb.velocity = new Vector2(-5, rb.velocity.y);
                hurtAudio.Play();
                isHurt = true;
            }
            else if (transform.position.x > collision.gameObject.transform.position.x)
            {
                HP -= 1;
                if (HP == 0)
                {
                    Gameover();
                }
                rb.velocity = new Vector2(5, rb.velocity.y);
                hurtAudio.Play();
                isHurt = true;
            }
        }
    }
    void Gameover()
    {
         SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 2);

    }

    public void CherryCount()
    {
        Cherry++;
    }

    void Save()
    {
        PlayerPrefs.SetFloat("PlayerX", transform.position.x);
        PlayerPrefs.SetFloat("PlayerY", transform.position.y);
        PlayerPrefs.SetFloat("PlayerZ", transform.position.z);

        PlayerPrefs.Save();
    }

    void Load()
    {
        float NewX = PlayerPrefs.GetFloat("PlayerX");
        float NewY = PlayerPrefs.GetFloat("PlayerY");
        float NewZ = PlayerPrefs.GetFloat("PlayerZ");

        Vector3 newPosition = new Vector3(NewX,NewY,NewZ);
        transform.position = newPosition;
    }
}
