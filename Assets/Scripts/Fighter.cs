using UnityEngine;
using System.Collections;

public class Fighter : MonoBehaviour
{
    public enum PlayerType
    {
        HUMAN, AI
    };

    public static float MAX_HEALTH = 100f;
    public float health = MAX_HEALTH;
    public string fighterName;
    public Fighter opponent;
    public bool enable;

    public PlayerType player;
    public FighterStates currentState = FighterStates.IDLE;

    protected Animator animator;
    private Rigidbody myBody;
    private AudioSource audioPlayer;

    //for AI only
    private float random;
    private float randomSetTime;

    // Use this for initialization
    void Start()
    {
        myBody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        audioPlayer = GetComponent<AudioSource>();
    }

    public void UpdateHumanInput()
    {
        if (Input.GetAxis("Horizontal") > 0.1)
        {
            animator.SetBool("WALK", true);
        }
        else
        {
            animator.SetBool("WALK", false);
        }

        if (Input.GetAxis("Horizontal") < -0.1)
        {
            if (opponent.attacking)
            {
                animator.SetBool("WALKBACK", false);
                animator.SetBool("DEFEND", true);
            }
            else
            {
                animator.SetBool("WALKBACK", true);
                animator.SetBool("DEFEND", false);
            }
        }
        else
        {
            animator.SetBool("WALKBACK", false);
            animator.SetBool("DEFEND", false);
        }

        if (Input.GetAxis("Vertical") < -0.1)
        {
            animator.SetBool("DUCK", true);
        }
        else
        {
            animator.SetBool("DUCK", false);
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            animator.SetTrigger("JUMP");
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            animator.SetTrigger("PUNCH_R");
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            animator.SetTrigger("KICK_R");
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            animator.SetTrigger("HADOKEN");
        }
    }

    public void UpdateAiInput()
    {
        animator.SetBool("defending", defending);
        //animator.SetBool ("invulnerable", invulnerable);
        //animator.SetBool ("enable", enable);

        animator.SetBool("opponent_attacking", opponent.attacking);
        animator.SetFloat("distanceToOpponent", getDistanceToOponennt());

        if (Time.time - randomSetTime > 1)
        {
            random = Random.value;
            randomSetTime = Time.time;
        }
        animator.SetFloat("random", random);
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetFloat("health", healtPercent);

        if (opponent != null)
        {
            animator.SetFloat("opponent_health", opponent.healtPercent);
        }
        else
        {
            animator.SetFloat("opponent_health", 1);
        }

        if (enable)
        {
            if (player == PlayerType.HUMAN)
            {
                UpdateHumanInput();
            }
            else
            {
                UpdateAiInput();
            }

        }

        if (health <= 0 && currentState != FighterStates.DEAD)
        {
            animator.SetTrigger("DEAD");
        }
    }

    private float getDistanceToOponennt()
    {
        return Mathf.Abs(transform.position.x - opponent.transform.position.x);
    }

    public virtual void hurt(float damage)
    {
        if (!invulnerable)
        {
            if (defending)
            {
                damage *= 0.2f;
            }
            if (health >= damage)
            {
                health -= damage;
            }
            else
            {
                health = 0;
            }

            if (health > 0)
            {
                animator.SetTrigger("TAKE_HIT");
            }
        }
    }

    public void playSound(AudioClip sound)
    {
        GameUtils.playSound(sound, audioPlayer);
    }

    public bool invulnerable
    {
        get
        {
            return currentState == FighterStates.TAKE_HIT
                || currentState == FighterStates.TAKE_HIT_DEFEND
                    || currentState == FighterStates.DEAD;
        }
    }

    public bool defending
    {
        get
        {
            return currentState == FighterStates.DEFEND
                || currentState == FighterStates.TAKE_HIT_DEFEND;
        }
    }

    public bool attacking
    {
        get
        {
            return currentState == FighterStates.ATTACK;
        }
    }

    public float healtPercent
    {
        get
        {
            return health / MAX_HEALTH;
        }
    }

    public Rigidbody body
    {
        get
        {
            return this.myBody;
        }
    }
}
 