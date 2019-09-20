using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BabaState {
    Wander,
    Follow,
    Die,
};

public class Baba : Enemy
{
    GameObject player;
    public BabaState currentState = BabaState.Wander;

    public float range;
    public float speed;
    private bool chooseDirection = false;
    private bool dead = false;
    private Vector3 randomDirection;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState) {
            case (BabaState.Wander):
                Wander();
                break;

            case (BabaState.Follow):
                Follow();
                break;

            case (BabaState.Die):
                //Die();
                break;

        }

        if (IsPlayerInRange(range) && currentState != BabaState.Die) {
            currentState = BabaState.Follow;
        } else if (!IsPlayerInRange(range) && currentState != BabaState.Die) {
            currentState = BabaState.Wander;
        }

    }

    private bool IsPlayerInRange(float range) {
        return Vector2.Distance(transform.position, player.transform.position) <= range;
    }

    private IEnumerator ChooseDirection() {
        chooseDirection = true;
        yield return new WaitForSeconds(Random.Range(2f, 8f));
        randomDirection = new Vector3(0, 0, Random.Range(0, 360));
        Debug.Log(randomDirection);
        Quaternion nextRotation = Quaternion.Euler(randomDirection);
        transform.rotation = Quaternion.Lerp(transform.rotation, nextRotation, Random.Range(0.5f, 2.5f));
        chooseDirection = false;
    }

    void Wander() {
        if (!chooseDirection) {
            StartCoroutine(ChooseDirection());
        }
        transform.position += -transform.right * speed * Time.deltaTime;
        if (IsPlayerInRange(range)) {
            currentState = BabaState.Follow;
        }
    }

    void Follow() {
        transform.position = Vector2.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);
    }

}
