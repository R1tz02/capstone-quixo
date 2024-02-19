using UnityEngine;

public class DragonController : MonoBehaviour
{
    private Animator anim;

    // Use this for initialization
    void Start()
    {
        anim = GetComponent<Animator>();
        // Start the hover animation when the script starts
        anim.SetBool("Hover", true);
    }

    // Update is called once per frame
    void Update()
    {
        // You can remove the Update method entirely if the dragon should hover without any user input
    }
}
