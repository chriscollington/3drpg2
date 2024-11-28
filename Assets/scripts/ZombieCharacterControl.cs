using UnityEngine;

public class ZombieCharacterControl : MonoBehaviour
{
    private enum ControlMode
    {
        Tank,
        Direct
    }

    [SerializeField] private float m_moveSpeed = 2f;
    [SerializeField] private float m_turnSpeed = 200f;
    [SerializeField] private Animator m_animator = null;
    [SerializeField] private Rigidbody m_rigidBody = null;
    [SerializeField] private ControlMode m_controlMode = ControlMode.Tank;
    [SerializeField] private Transform m_playerTransform = null;
    [SerializeField] private float m_attackDistance = 1.5f;  // The distance within which the zombie will attack

    // Attack cooldown to prevent the zombie from attacking repeatedly in quick succession
    [SerializeField] private float m_attackCooldown = 2f;
    private float m_timeSinceLastAttack = 0f;

    private float m_currentV = 0;
    private Vector3 m_currentDirection = Vector3.zero;

    private void Awake()
    {
        if (!m_animator) { m_animator = gameObject.GetComponent<Animator>(); }
        if (!m_rigidBody) { m_rigidBody = gameObject.GetComponent<Rigidbody>(); }
    }

    private void FixedUpdate()
    {
        // Calculate the distance to the player
        float distanceToPlayer = Vector3.Distance(transform.position, m_playerTransform.position);

        // If the zombie is too far to attack, move towards the player
        if (distanceToPlayer > m_attackDistance)
        {
            // Move the zombie towards the player based on the chosen control mode
            switch (m_controlMode)
            {
                case ControlMode.Direct:
                    DirectUpdate();
                    break;

                case ControlMode.Tank:
                    TankUpdate();
                    break;

                default:
                    Debug.LogError("Unsupported state");
                    break;
            }
        }
        else
        {
            // If the zombie is close enough, perform an attack (if cooldown is over)
            PerformAttack(distanceToPlayer);
        }
    }

    private void TankUpdate()
    {
        // Calculate direction to the player
        Vector3 directionToPlayer = (m_playerTransform.position - transform.position).normalized;
        m_currentV = directionToPlayer.magnitude;

        // Move the zombie toward the player
        transform.position += directionToPlayer * m_moveSpeed * Time.deltaTime;

        // Rotate toward the player
        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, m_turnSpeed * Time.deltaTime);

        // Set walking animation speed
        m_animator.SetFloat("MoveSpeed", m_currentV);
    }

    private void DirectUpdate()
    {
        // Calculate direction to the player
        Vector3 directionToPlayer = (m_playerTransform.position - transform.position).normalized;
        m_currentV = directionToPlayer.magnitude;

        // Move the zombie towards the player
        transform.position += directionToPlayer * m_moveSpeed * Time.deltaTime;

        // Rotate toward the player
        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, m_turnSpeed * Time.deltaTime);

        // Set walking animation speed
        m_animator.SetFloat("MoveSpeed", m_currentV);
    }

    private void PerformAttack(float distanceToPlayer)
    {
        // Only attack if the cooldown has passed
        if (m_timeSinceLastAttack >= m_attackCooldown)
        {
            // Stop walking (set MoveSpeed to 0) and trigger attack animation
            m_animator.SetFloat("MoveSpeed", 0f);
            m_animator.SetTrigger("Attack");

            // Reset the cooldown timer
            m_timeSinceLastAttack = 0f;
        }
        else
        {
            // Increment the time since the last attack
            m_timeSinceLastAttack += Time.deltaTime;
        }
    }
}