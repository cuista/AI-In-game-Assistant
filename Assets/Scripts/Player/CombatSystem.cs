using UnityEngine;

public class CombatSystem : MonoBehaviour
{
    public float meleeRadius = 2f;

    private bool meleePressed;

    private Animator _animator;

    private AudioSource _audioSource;

    [SerializeField] private AudioClip melee1Sound;
    [SerializeField] private AudioClip melee2Sound;
    [SerializeField] private AudioClip melee3Sound;
    private AudioClip meleeSound;

    // Start is called before the first frame update
    void Start()
    {
        meleePressed = false;

        _animator = GetComponent<Animator>();

        _audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameEvent.isPaused)
        {
            meleePressed = Input.GetButtonDown("Melee") || meleePressed;      
        }
    }

    private void FixedUpdate()
    {
        if (!GameEvent.isPaused)
        {
            //Melee attack
            if(meleePressed)
            {
                Vector3 meleePoint = transform.position + transform.forward * 0.8f;
                Collider[] hitColliders = Physics.OverlapSphere(meleePoint, meleeRadius);
                foreach (var hitCollider in hitColliders)
                {

                    GameObject hitObject = hitCollider.transform.gameObject;
                    IReactiveObject target = hitObject.GetComponent<IReactiveObject>();
                    if (target != null)
                    {
                        target.ReactToHits(1);
                        target.AddHitForce(2, transform.position, meleeRadius);
                    }
                    else if(hitObject.CompareTag("Clone"))
                    {
                        CloneCharacter clone = hitObject.GetComponent<CloneCharacter>();
                        clone.Hurt(1);
                    }
                }
                meleePressed = false; //Reset melee input

                switch ((int) Random.Range(0, 3))
                {
                    case 0:_animator.SetTrigger("Melee1"); meleeSound = melee1Sound; break;
                    case 1: _animator.SetTrigger("Melee2"); meleeSound = melee2Sound; break;
                    case 2: default: _animator.SetTrigger("Melee3"); meleeSound = melee3Sound; break;
                }
                _audioSource.PlayOneShot(meleeSound);
            }
        }
    }
}
