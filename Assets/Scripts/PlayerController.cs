using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    //initializations for different stuff
    Rigidbody2D rb;
    public Text keysText;
    public static int keys = 0;
    public Projectile projectilePrefab;
    private float lastFire;

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
        rb.velocity = new Vector2(horizontal, vertical);
    }

    public void Shoot() {
        float shootHorizontal = 0;
        float shootVertical = 0;
        if (Input.GetKey("left")) {
            shootHorizontal = -1;
        } else if(Input.GetKey("right")) {
            shootHorizontal = 1;
        } else if(Input.GetKey("up")) {
            shootVertical = 1;
        } else if(Input.GetKey("down")) {
            shootVertical = -1;
        }

        if((shootHorizontal != 0 || shootVertical != 0) && Time.time > lastFire + fireDelay){
            lastFire = Time.time;
            Projectile projectile = Instantiate<Projectile>(projectilePrefab, transform.position, transform.rotation);
            projectile.GetComponent<Rigidbody2D>().velocity = new Vector2(shootHorizontal * shotSpeed, shootVertical * shotSpeed);
            projectile.timer = shotTimer;
        }
    }

    public void Ui() {
        keysText.text = "Keys: " + keys;
    }
}
