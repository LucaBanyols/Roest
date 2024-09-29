using UnityEngine;

namespace Ursaanimation.CubicFarmAnimals
{
    public class AnimationController : MonoBehaviour
    {
        public Animator animator;
        public string walkForwardAnimation = "walk_forward";
        public string walkBackwardAnimation = "walk_backwards";
        public string runForwardAnimation = "run_forward";
        public string turn90LAnimation = "turn_90_L";
        public string turn90RAnimation = "turn_90_R";
        public string trotAnimation = "trot_forward";
        public string sittostandAnimation = "sit_to_stand";
        public string standtositAnimation = "stand_to_sit";

        void Start()
        {
            animator = GetComponent<Animator>();
        }

        void Update()
        {
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");

            Vector3 movement = new Vector3(horizontalInput, 0f, verticalInput);
            movement.Normalize();

            if (movement != Vector3.zero)
            {
                float moveAngle = Mathf.Atan2(movement.x, movement.z) * Mathf.Rad2Deg;
                moveAngle = (360 + moveAngle) % 360;

                if (moveAngle >= 45f && moveAngle < 135f)
                {
                    animator.Play(walkForwardAnimation);
                }
                else if (moveAngle >= 135f && moveAngle < 225f)
                {
                    animator.Play(walkBackwardAnimation);
                }
                else if (moveAngle >= 225f && moveAngle < 315f)
                {
                    animator.Play(runForwardAnimation);
                }
                else
                {
                    animator.Play(turn90LAnimation);
                }
            }
            else
            {
                animator.Play(standtositAnimation);
            }
        }
    }
}
