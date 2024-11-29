using UnityEngine;

public class RandomStartAnimation : MonoBehaviour
{
    private Animator animator;
    private float offset;

    void Awake()
    {
        animator = GetComponent<Animator>();
        offset = Random.Range(0f, 1f);
        animator.SetFloat("RandomStart", offset);
        animator.speed = 1 + (1 + offset);
    }
}
