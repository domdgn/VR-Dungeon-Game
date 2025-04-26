using UnityEngine.InputSystem;
using UnityEngine;
using System;

namespace Hands.Scripts
{
    public class SmoothHandAnimation : MonoBehaviour
    {
        [SerializeField] private Animator handAnimator;
        [SerializeField] private InputActionReference triggerActionRef;
        [SerializeField] private InputActionReference gripActionRef;

        private static readonly int triggerAnimation = Animator.StringToHash("Trigger");
        private static readonly int gripAnimation = Animator.StringToHash("Grip");

        private void Update()
        {
            float triggerValue = triggerActionRef.action.ReadValue<float>();
            handAnimator.SetFloat(triggerAnimation, triggerValue);

            float gripValue = gripActionRef.action.ReadValue<float>();
            handAnimator.SetFloat(gripAnimation, gripValue);
        }
    }

}

