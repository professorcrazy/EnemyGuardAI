using System.Collections;
using UnityEngine;

public class GuardCombat : MonoBehaviour, IDamage
{
    public Transform attackPoint;
    public GameObject batPrefab; // Prefab for the swinging object
    public GameObject bulletPrefab; // Prefab for the ranged attack bullet
    public Transform firePoint; // Point where the bullet is instantiated

    public float meleeAttackRange = 1.5f;
    public float rangedAttackRange = 10f;
    public float attackCooldown = 2f;
    public int maxHealth = 100;

    private int currentHealth;
    private bool isAttacking = false;
    private int meleeDamage;
    private int rangedDamage;
    private GameObject meleeWeapon;
    [SerializeField] private LayerMask enemyLayer;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // Implement death logic, such as playing an animation or spawning particles
        Destroy(gameObject);
    }

    public void Attack()
    {
        if (Vector3.Distance(transform.position, attackPoint.position) < meleeAttackRange && !isAttacking)
        {
            StartCoroutine(MeleeAttack());
        }
        else if (Vector3.Distance(transform.position, attackPoint.position) < rangedAttackRange && !isAttacking)
        {
            StartCoroutine(RangedAttack());
        }
    }

    IEnumerator MeleeAttack() //rewrite
    {
        isAttacking = true;

        // Activate the melee weapon
        meleeWeapon.SetActive(true);

        // Perform attack logic and apply damage to enemies in range
        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, meleeAttackRange, enemyLayer);
        foreach (Collider enemy in hitEnemies)
        {
            IDamage enemyDamageable = enemy.GetComponent<IDamage>();
            if (enemyDamageable != null)
            {
                enemyDamageable.TakeDamage(meleeDamage);
            }
        }

        // Deactivate the melee weapon after a delay
        yield return new WaitForSeconds(0.5f);
        meleeWeapon.SetActive(false);

        yield return new WaitForSeconds(attackCooldown);

        isAttacking = false;
    }

    IEnumerator RangedAttack() //rewrite
    {
    
        isAttacking = true;
        yield return new WaitForSeconds(attackCooldown);

        // Instantiate the bullet at the fire point
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        // Perform attack logic and apply damage to the target (assumed to implement IDamage)
        IDamage target = bullet.GetComponent<IDamage>();
        if (target != null)
        {
            target.TakeDamage(rangedDamage);
        }

        Destroy(bullet, 2f); // Destroy the bullet after a delay
        isAttacking = false;
    }

}
