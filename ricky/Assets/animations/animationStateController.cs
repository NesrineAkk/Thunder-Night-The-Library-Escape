using UnityEngine;

public class animationStateController : MonoBehaviour
{
    Animator animator;
    int isWalkingHash;
    int isRunningHash;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        Debug.Log(animator);
        isWalkingHash = Animator.StringToHash("isWalking");
        isRunningHash = Animator.StringToHash("isRunning");
    }

    // Update is called once per frame
    void Update()
    {
        bool isRunning = animator.GetBool(isRunningHash);
        bool isWalking = animator.GetBool(isWalkingHash);
        bool forwardPressed = Input.GetKey("w");
        bool runPressed = Input.GetKey("left shift");

        if(!isWalking && forwardPressed)
        {
            animator.SetBool(isWalkingHash, true);
        }

        if(isWalking && !forwardPressed)
        {
            animator.SetBool(isWalkingHash, false);
        }

        if(!isRunning && (runPressed && forwardPressed))
        {
            animator.SetBool(isRunningHash, true);
        }

        if(isRunning && (!runPressed || !forwardPressed))
        {
            animator.SetBool(isRunningHash, false);
        }
    }
}
