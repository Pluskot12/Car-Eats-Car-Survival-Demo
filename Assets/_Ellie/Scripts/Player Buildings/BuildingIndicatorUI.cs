using PrimeTween;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace CarGame
{
    public class BuildingIndicatorUI : MonoBehaviour
    {
        [Header("Progress Bar")]
        [SerializeField] private Canvas canvas;
        [SerializeField] private RectTransform parent;
        [SerializeField] private RectTransform progressBarParent;
        [SerializeField] private Image progressBarImage;

        [Header("Backgrounds")]
        [SerializeField] private RectTransform backgroundParent;
        [SerializeField] private Image placementValid;
        [SerializeField] private Image placementBlocked;
        [SerializeField] private Image placementCompleted;
        [SerializeField] private Image placementIndicator;

        [Header("Animation")]
        [SerializeField] private float speed = 0.15f;
        [SerializeField] private float barAppearSpeed = 0.3f;
        [SerializeField] private AnimationCurve curve;

        public enum Status 
        {
            Valid,
            Invalid,
            Complete
        }

        private Status currentStatus;

        private bool isShowing;

        private void Start()
        {
            progressBarParent.localScale = Vector3.zero;
            backgroundParent.localScale = Vector3.zero;
        }

        public void UpdateProgress(float v)
        {
            progressBarImage.fillAmount = v;
            v = curve.Evaluate(v);
            placementIndicator.material.SetFloat("_Progress", v);
        }
        
        public void OnClick(bool started) 
        {
            if (started)
            {
                Tween.Scale(progressBarParent, 1, barAppearSpeed);
            }
            else
            {
                Tween.Scale(progressBarParent, 0, barAppearSpeed);
            }
        }

        public void Show(Vector3 position)
        {
            parent.localPosition = GameManager.Instance.TestPos(position, canvas);

            if (isShowing) 
            {
                return;
            }

            // Tween.Scale(progressBarParent, 1f, speed);
            Tween.Scale(backgroundParent, 1f, speed);

            isShowing = true;
        }

        public void Hide(bool isComplete)
        {
            if (!isShowing)
            {
                return;
            }

            if (!isComplete && currentStatus == Status.Complete)
            {
                return;
            }
            

            Tween.Scale(progressBarParent, 0f, speed);
            Tween.Scale(backgroundParent, 0f, speed);

            isShowing = false;
        }
        public bool IsShowing => isShowing;
        public void UpdateStatus(Status status)
        {
            if (currentStatus == status) 
            {
                return;
            }

            currentStatus = status;

            if (status == Status.Invalid)
            {
                Tween.Custom(placementIndicator.material.GetFloat("_BlockedStatus"), 1f, duration: 0.2f, onValueChange: newVal => placementIndicator.material.SetFloat("_BlockedStatus", newVal));
            }
            else 
            {
                Tween.Custom(placementIndicator.material.GetFloat("_BlockedStatus"), 0f, duration: 0.2f, onValueChange: newVal => placementIndicator.material.SetFloat("_BlockedStatus", newVal));
            }

            if (status == Status.Complete)
            {
                Tween.PunchScale(backgroundParent, Vector3.one * 0.175f, 0.275f, 5);
                Tween.PunchScale(progressBarParent, Vector3.one * 0.25f, 0.275f, 5).OnComplete(() => Hide(true));
            }

            

        }
    }
}
