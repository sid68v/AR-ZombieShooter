using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public static EnemyController Instance;
    public float initialDelay = 1f;
    public float turnSpeed = 1f;
    public bool isDead;
    public float attackDistance = 2f;
    public float maxSpeed = 2.5f;
    public int attackDamage = 10;
    public float delayBetweenAttacks = 1f;
    public int deathBonus = 10;
    public int enemyHealth = 100;

    Animator anim;
    AudioSource audioSource;
    AudioClip currentClip;
    GameObject player;
    float time;

    private void Awake()
    {
        if (!Instance)
            Instance = this;
    }

    // Start is called before the first frame update
    void OnEnable()
    {
        anim = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");
        audioSource = GetComponent<AudioSource>();
        isDead = false;
        time = 0;

        StartCoroutine(Move());
    }

    public IEnumerator Move()
    {
        yield return new WaitForSeconds(initialDelay);
        anim.speed = Random.Range(1f,maxSpeed);
        currentClip = GameController.Instance.zombieSounds[Mathf.RoundToInt(Random.Range(0, GameController.Instance.zombieSounds.Length))];
        PlaySound(currentClip, true);

        while (true)
        {
            // turn to face the player
            Vector3 lookDirection = (player.transform.GetChild(0).transform.position - transform.position).normalized;
            lookDirection.y = 0;
            Quaternion requiredRotation = Quaternion.LookRotation(lookDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, requiredRotation, turnSpeed * Time.deltaTime);

            //attack if near
            float currentDistance = Vector3.Distance(transform.position, player.transform.GetChild(0).transform.position);



            if (currentDistance < 100 && currentDistance > attackDistance)
            {
                PlaySound(currentClip, true);
                anim.SetBool("isAttacking", false);
                anim.SetBool("isWalking", true);
            }
            else if (currentDistance <= attackDistance && time > delayBetweenAttacks)
            {
                time = 0;
                PlaySound(GameController.Instance.zombieBiteSound, true);
                anim.SetBool("isAttacking", true);
                anim.SetBool("isWalking", false);
                GameController.Instance.TakeDamage(attackDamage);
            }
            else
                time += Time.deltaTime;

            // if dead...
            if (isDead)
            {
                PlaySound(GameController.Instance.zombieDeathSound, false);
                anim.SetBool("isDead", true);
                //Destroy(gameObject, 4f);
                Invoke("Die", 4f);
                GameController.Instance.AddScore(deathBonus);
                yield return new WaitForSeconds(2f);
                isDead = false;
            }


            yield return new WaitForEndOfFrame();
        }
    }

    private void Die()
    {
        gameObject.SetActive(false);
    }

    public void PlaySound(AudioClip clip, bool loopState)
    {
        if (audioSource.clip.name != clip.name)
        {

            audioSource.pitch = (clip.name == "Zombie Gibberish") ? .3f : 1f;

            audioSource.Stop();
            audioSource.clip = clip;
            audioSource.loop = loopState;
            audioSource.Play();
        }
    }

    public void GetDamage(int damage)
    {
        enemyHealth -= damage;
        if (enemyHealth < 0)
            isDead = true;
    }

   
    // Update is called once per frame
    void Update()
    {

    }
}
