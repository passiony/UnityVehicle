using System;
using UnityEngine;

namespace Vertical
{
    public class ColliderStop : MonoBehaviour
    {
        public HUDManager carHUD;
        public HUDManager childHUD;
        public AudioSource impactAudio;

        private void OnCollisionEnter(Collision other)
        {
            var body = other.gameObject.GetComponentInParent<Rigidbody>();
            if (body && body.CompareTag("Car"))
            {
                gameObject.GetComponent<AutoCar>().StartTrigger();
                carHUD.HideAllHUD();
                impactAudio.Play();
            }

            if (body && body.CompareTag("Child"))
            {
                body.constraints = RigidbodyConstraints.FreezeRotationY;
                body.GetComponent<ChildDartOut>().enabled = false;
                body.AddForce(body.transform.right * 100, ForceMode.Impulse);
                body.GetComponent<Animator>().Play("idle");
                impactAudio.Play();

                childHUD.HideAllHUD();
            }
        }
    }
}