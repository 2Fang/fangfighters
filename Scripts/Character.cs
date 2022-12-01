using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Character : MonoBehaviour
{

    Rigidbody2D _rigidBody;
    BoxCollider2D collider;
    SpriteRenderer sprite;

    [SerializeField] bool bot;
    [SerializeField] int temp;
    [SerializeField] int _movementSpeed = 10;
    [SerializeField] float maxHealth = 4200;
    [SerializeField] int attackRange = 5;
    [SerializeField] int attackSpeed = 6;
    float health;
    [SerializeField] float maxAmmo = 3;
    [SerializeField] int maxAttackCooldown = 1000;
    float ammo;
    int attackCooldown = 0;
    int maxHealCooldown = 1000;
    int healCooldown = 0;
    [SerializeField] float reloadSpeed;

    [SerializeField] string LS_h = "LS_h";
    [SerializeField] string LS_v = "LS_v";
    [SerializeField] string LS_B = "LS_B";
    [SerializeField] string RS_h = "RS_h";
    [SerializeField] string RS_v = "RS_v";
    [SerializeField] string RS_B = "RS_B";
    [SerializeField] string LT = "LT";
    [SerializeField] string RT = "RT";
    [SerializeField] string LB = "LB";
    [SerializeField] string RB = "RB";
    [SerializeField] string DPAD_h = "DPAD_h";
    [SerializeField] string DPAD_v = "DPAD_v";
    [SerializeField] string A = "A";
    [SerializeField] string B = "B";
    [SerializeField] string X = "X";
    [SerializeField] string Y = "Y";

    bool LTrig = false;
    bool RTrig = false;

    float desiredRotation = 0f;

    Aim aimbar;
    [SerializeField] GameObject[] projectiles;

    HealthBar healthBar;
    AmmoBar ammoBar;

    bool died = false;
    [SerializeField] Vector2[] spawnPoints;

    int invulnerable = 0;
    Color ogColor;


    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
        ammo = maxAmmo;

        _rigidBody = GetComponentInParent<Rigidbody2D>();
        collider = GetComponentInParent<BoxCollider2D>();
        sprite = GetComponentInParent<SpriteRenderer>();
        ogColor = sprite.color;

        foreach (Aim bar in FindObjectsOfType<Aim>())
        {
            if (bar.CheckButton(RS_B)) {
                aimbar = bar;
            }
        }
        aimbar.gameObject.SetActive(false);
        healthBar = GetComponentInChildren<HealthBar>();
        ammoBar = GetComponentInChildren<AmmoBar>();

        for (int i = 0; i < projectiles.Length; i++)
            projectiles[i].GetComponent<Bullet>().SetDistance(attackRange, attackSpeed);


        Respawn();
        invulnerable = 0;

    }

    // Update is called once per frame
    void Update()
    {
        if (died)
            Respawn();
        if (invulnerable > 0)
            Invulnerable();


        if (attackCooldown > 0)
        {
            attackCooldown -= 1;
        }
        if (health < maxHealth)
        {
            if (healCooldown == 0)
            {
                health += (int)(maxHealth * 0.2);
                healCooldown = 120;
            }
        }
        if (ammo < maxAmmo)
        {
            ammo += reloadSpeed;
            ammoBar.reSize(ammo / maxAmmo);
        }

        if (!bot)
            Move();

    }


    void Move()
    {

        _rigidBody.velocity = new Vector2(Input.GetAxis(LS_h) * _movementSpeed, -Input.GetAxis(LS_v) * _movementSpeed);

        if (Mathf.Abs(_rigidBody.velocity.x) >= 0.5 || Mathf.Abs(_rigidBody.velocity.y) >= 0.5)
        {
            desiredRotation = Mathf.Rad2Deg * Mathf.Atan(_rigidBody.velocity.y / _rigidBody.velocity.x);
            if (_rigidBody.velocity.x < 0)
                desiredRotation += 180;
            _rigidBody.rotation = desiredRotation;
        }

        if (Mathf.Abs(_rigidBody.rotation - desiredRotation) > 1)
        {
            _rigidBody.velocity = Vector2.zero;
            _rigidBody.rotation = desiredRotation;
        }

        if (Input.GetAxis(RT) >= 0.9)
        {
            if (!RTrig) 
            {
                RTrig = true;
                aimbar.gameObject.SetActive(true);
            }
            aimbar.transform.position = _rigidBody.position; 
        } 
        else
        {
            if (RTrig)
            {
                RTrig = false;
                aimbar.gameObject.SetActive(false);
            }
        }

        if (Input.GetButtonDown(RB))
        {
            Attack(new Vector2(Input.GetAxis(RS_h), Input.GetAxis(RS_v)));
        }



    }

    public void Attack(Vector2 angle)
    {
        if (attackCooldown > 0) return;
        if (ammo < 1) return;
        ammo -= 1f;
        attackCooldown = maxAttackCooldown;
        projectiles[RotateAmmo()].GetComponent<Bullet>().Shoot(_rigidBody.position, angle);

    }

    public void GetHit(int damage)
    {
        health -= damage;
        healCooldown = maxHealCooldown;
        if (health <= 0)
        {
            health = 0;
            Die();
        }
        healthBar.reSize(health / maxHealth);
    }


    int RotateAmmo()
    {
        for (int i = 0; i < projectiles.Length; i++)
        {
            if (!projectiles[i].activeInHierarchy)
                return i;
        }
        return 0;

    }

    public Vector2 getPos()
    {
        return _rigidBody.position;
    }

    public float getRange()
    {
        return attackRange;
    }


    void Die()
    {
        died = true;
        gameObject.SetActive(false);

    }

    void Respawn()
    {
        died = false;
        invulnerable = temp;
        health = maxHealth;
        ammo = maxAmmo;
        healthBar.reSize(health / maxHealth);
        _rigidBody.position = spawnPoints[(int)(Random.value * spawnPoints.Length)];
    }

    void Invulnerable()
    {
        invulnerable -= 1;
        collider.enabled = false;
        sprite.color = ogColor - new Color(0, 0, 0, Mathf.Sin(Mathf.Deg2Rad * invulnerable));
        if (invulnerable == 0)
        {
            invulnerable = 0;
            collider.enabled = true;
            sprite.color = ogColor;
        }
    }

    public void setMovement(Vector2 speed)
    {
        _rigidBody.velocity = speed * _movementSpeed;
    }

    public void setRotation()
    {
        if (Mathf.Abs(_rigidBody.velocity.x) >= 0.5 || Mathf.Abs(_rigidBody.velocity.y) >= 0.5)
        {
            desiredRotation = Mathf.Rad2Deg * Mathf.Atan(_rigidBody.velocity.y / _rigidBody.velocity.x);
            if (_rigidBody.velocity.x < 0)
                desiredRotation += 180;
            _rigidBody.rotation = desiredRotation;
        }
    }

}
