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
        [SerializeField] private float speed = 0.4f;

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

            placementIndicator.material.SetFloat("_Progress", v);
        }

        public void Show(Vector3 position)
        {
            parent.localPosition = GameManager.Instance.TestPos(position, canvas);

            if (isShowing) 
            {
                return;
            }

            Tween.Scale(progressBarParent, 1f, speed);
            Tween.Scale(backgroundParent, 1f, speed);

            isShowing = true;
        }

        public void Hide()
        {
            if (!isShowing)
            {
                return;
            }

            Tween.Scale(progressBarParent, 0f, speed);
            Tween.Scale(backgroundParent, 0f, speed);

            isShowing = false;
        }

        public void UpdateStatus(Status status)
        {
            if (currentStatus == status) 
            {
                return;
            }

            if (status == Status.Invalid)
            {
                Tween.Custom(placementIndicator.material.GetFloat("_BlockedStatus"), 1f, duration: 0.2f, onValueChange: newVal => placementIndicator.material.SetFloat("_BlockedStatus", newVal));

            }
            else 
            {
                Tween.Custom(placementIndicator.material.GetFloat("_BlockedStatus"), 0f, duration: 0.2f, onValueChange: newVal => placementIndicator.material.SetFloat("_BlockedStatus", newVal));

            }

            if (status == Status.Valid) 
            {

            }
            else if (status == Status.Invalid)
            {

            }
            else if (status == Status.Complete)
            {

            }

            currentStatus = status;


        }
    }
}
