using UnityEngine;
using System.Collections;
namespace Hiker.GUI
{
    public class UIFollowTarget : MonoBehaviour
    {
        public Transform Target;
        public Vector3 DeltaOffset3D;
        public bool SnapInViewport;
        public int Width, Height;
        private float MinScale { get; set; }
        private float MaxScale { get; set; }
        // Logics.

        RectTransform rectTrans;
        RectTransform parRectTrans;

        public void SetTarget(Transform target, float min_scale, float max_scale, Vector3 delta)
        {
            this.Target = target;
            this.MinScale = min_scale;
            this.MaxScale = max_scale;
            this.DeltaOffset3D = delta;
            this.UpdatePosition();
            rectTrans = GetComponent<RectTransform>();
            parRectTrans = transform.parent.GetComponentInParent<RectTransform>();
        }

        void OnEnable()
        {
            this.UpdatePosition();
            if (Target == null)
            {
#if DEBUG
                Debug.Log("Target is NULL" + gameObject.name);
#endif
            }
        }
        void LateUpdate()
        {
            this.UpdatePosition();
        }

        private void UpdatePosition()
        {
            if (this.Target == null)
            {
                return;
            }
            if (rectTrans == null)
            {
                rectTrans = GetComponent<RectTransform>();
            }
            if (parRectTrans == null)
            {
                parRectTrans = transform.parent.GetComponentInParent<RectTransform>();
            }

            //if (GUIManager.Instance == null)
            //{
            //    return;
            //}
            //if (this.transform == null)
            //{
            //    return;
            //}

            //if (this.GameCamera == null)
            //{
            //    this.GameCamera = Camera.main;
            //}

            //if(this.uiCam == null)
            //{
            //    this.uiCam = UICamera.currentCamera;
            //}

            Vector3 screenPos = Camera.main.WorldToScreenPoint(Target.position + this.DeltaOffset3D);
            //screenPos.z = 0;
            Vector2 localPos;
            var rootCanvas = GetComponentInParent<Canvas>().rootCanvas;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(parRectTrans,
                screenPos,
                rootCanvas.renderMode == RenderMode.ScreenSpaceCamera ? rootCanvas.worldCamera : null,
                out localPos);
            rectTrans.localPosition = localPos;

            //var pos0 = this.GameCamera.WorldToViewportPoint(this.Target.position + this.DeltaOffset3D);
            //var pos1 = this.uiCam.ViewportToWorldPoint(pos0);
            //pos1.z = 0;
            //this.transform.position = pos1;

            //if (this.SnapInViewport)
            //{
            //    var local_pos = this.transform.localPosition;
            //    var max_x = GUIManager.Instance.ViewportWidth / 2 - this.Width / 2 - 30;
            //    var max_y = GUIManager.Instance.ViewportHeight / 2 - this.Height / 2 - 10;

            //    if (local_pos.x > max_x) local_pos.x = max_x;
            //    if (local_pos.x < -max_x) local_pos.x = -max_x;
            //    if (local_pos.y > max_y) local_pos.y = max_y;
            //    if (local_pos.y < -max_y) local_pos.y = -max_y;

            //    this.transform.localPosition = local_pos;
            //}

            //var step_scale = (this.MaxScale - this.MinScale) / (CameraController.Instance.CAMERA_MAX_SIZE - CameraController.Instance.CAMERA_MIN_SIZE);
            //var current_scale = this.MaxScale - step_scale * (Camera.main.orthographicSize - CameraController.Instance.CAMERA_MIN_SIZE);
            //if (current_scale > this.MaxScale) current_scale = this.MaxScale;
            //if (current_scale < this.MinScale) current_scale = this.MinScale;
            ////  current_scale = Mathf.Round(current_scale * 10) / 10f;

            //this.transform.localScale = Vector3.one * current_scale;
        }
    }
}