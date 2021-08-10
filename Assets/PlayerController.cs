using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerController : MonoBehaviour
{
    [SerializeField] Transform targetBone;
    [SerializeField] LayerMask aimLayerMask;
    [SerializeField] float speed;
    [SerializeField] Transform aimTarget;
    [SerializeField] float finishDistance;
    [SerializeField] Mesh gunMesh;
    [SerializeField] Mesh swordMesh;
    [SerializeField] GameObject currentWeapon;
    [SerializeField] GameObject pressFText;

    private Animator anim;
    private GameObject enemyToFinish;
    private RigBuilder rigBuilder;
    private bool finishing;


    private void Start()
    {
        anim = gameObject.GetComponent<Animator>();
        rigBuilder = gameObject.GetComponent<RigBuilder>();
        finishing = false;
    }

    public void FixedUpdate()
    {
        if (Input.GetButtonDown("Escape"))
        {
            Application.Quit();
        }

        if (!finishing)
        {
            Movement();
            Aim();
        }

        Finishing();
    }

    private void Movement()
    {
        Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        if (movement.magnitude > 0)
        {
            movement.Normalize();
            movement *= speed * Time.deltaTime;
            transform.Translate(movement, Space.World);
        }

        float velocityX = Vector3.Dot(movement.normalized, transform.forward);
        float velocityZ = Vector3.Dot(movement.normalized, transform.right);

        anim.SetFloat("velocityX", velocityX, 0.1f, Time.deltaTime);
        anim.SetFloat("velocityZ", velocityZ, 0.1f, Time.deltaTime);
    }

    private void Aim()
    {
        rigBuilder.enabled = true;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if(Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, aimLayerMask))
        {
            aimTarget.transform.position = hitInfo.point;

            if (targetBone.localEulerAngles.x > 20 && targetBone.localEulerAngles.x < 340)
            {
                Vector3 direction = hitInfo.point - transform.position;
                direction.y = 0f;
                direction.Normalize();

                Quaternion qTo = Quaternion.LookRotation(direction);

                transform.rotation = Quaternion.Slerp(transform.rotation, qTo, Time.deltaTime * 5f);
            }
        }
    }

    private void Finishing()
    {
        if (Input.GetButtonDown("Finish") && enemyToFinish)
        {
            finishing = true;

            currentWeapon.GetComponent<MeshFilter>().mesh = swordMesh;

            rigBuilder.enabled = false;

            anim.SetTrigger("finishing");

            transform.position = enemyToFinish.transform.position + (transform.position - enemyToFinish.transform.position).normalized*finishDistance;

            Vector3 direction = enemyToFinish.transform.position - transform.position;
            direction.y = 0f;
            direction.Normalize();
            transform.forward = direction;
        }
    }

    private void Finished()
    {
        finishing = false;
        currentWeapon.GetComponent<MeshFilter>().mesh = gunMesh;
    }

    private void TurnRagdoll()
    {
        enemyToFinish.GetComponent<Enemy>().Death();
        enemyToFinish = null;
        pressFText.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            enemyToFinish = other.gameObject;
            pressFText.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            enemyToFinish =  null;
            pressFText.SetActive(false);
        }
    }
}
