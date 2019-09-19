using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    //initializations for different stuff
    Rigidbody2D rb;
    Animator animator;
    //public Text keysText;
    public int keys = 0;
    public Projectile projectilePrefab;
    private float lastFire;
    private bool lookShoot;

    //actual ingame stats
    //public float health;
    //public float damage;
    public float fireDelay;
    public float shotTimer;
    public float shotSpeed;
    public float speed;
    //public float knockback;

    // Start is called before the first frame update
    void Start(){
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update(){
        Shoot();
        Move();
        Ui();
    }

    public void Move() {
        float horizontal = Input.GetAxis("Horizontal") * speed;
        float vertical = Input.GetAxis("Vertical") * speed;

        if(Input.GetKey("a") == true && Input.GetKey("d") == true) {
            horizontal = 0;
        }
        if (Input.GetKey("w") == true && Input.GetKey("s") == true)
        {
            vertical = 0;
        }
        if (lookShoot == false) {
            if (Input.GetKey("a")) {
                animator.SetInteger("LookDirection", 3);
            }
            else if (Input.GetKey("d")) {
                animator.SetInteger("LookDirection", 1);
            }
            else if (Input.GetKey("w")) {
                animator.SetInteger("LookDirection", 0);
            }
            else {
                animator.SetInteger("LookDirection", 2);
            }
        }
        rb.velocity = new Vector2(horizontal, vertical);
    }

    public void Shoot() {
        float shootHorizontal = 0;
        float shootVertical = 0;
        if (Input.GetKey("left") && Time.time > lastFire + fireDelay) {
            shootHorizontal = -1;
            animator.SetInteger("LookDirection", 3);
            lookShoot = true;
        } else if(Input.GetKey("right") && Time.time > lastFire + fireDelay) {
            shootHorizontal = 1;
            animator.SetInteger("LookDirection", 1);
            lookShoot = true;
        } else if(Input.GetKey("up") && Time.time > lastFire + fireDelay) {
            shootVertical = 1;
            animator.SetInteger("LookDirection", 0);
            lookShoot = true;
        } else if(Input.GetKey("down") && Time.time > lastFire + fireDelay) {
            shootVertical = -1;
            animator.SetInteger("LookDirection", 2);
            lookShoot = true;
        } else if(Time.time > lastFire + fireDelay){
            lookShoot = false;
        }

        if(shootHorizontal != 0 || shootVertical != 0){
            lastFire = Time.time;
            Projectile projectile = Instantiate<Projectile>(projectilePrefab, transform.position, transform.rotation);
            projectile.GetComponent<Rigidbody2D>().velocity = new Vector2(shootHorizontal * shotSpeed, shootVertical * shotSpeed);
            projectile.timer = shotTimer;
        }
    }

    public void Ui() {
        //keysText.text = "Keys: " + keys;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if(collision.tag == "Pickup") {
            Pickup pickup = collision.gameObject.GetComponent<Pickup>();
            pickup.OnPickup(this);
        }
    }
    
}
