using UnityEngine;

namespace MPLCore.UiSystems
{
    [RequireComponent(typeof(Camera))]
    public class UiRoot : MonoBehaviour
    {
        public Camera UiCamera { get; private set; }

        private void Awake()
        {
            UiCamera = GetComponent<Camera>();
            UiCamera.orthographic = true;
            UiCamera.clearFlags = CameraClearFlags.Nothing;
            UiCamera.backgroundColor = Color.green;
            UiCamera.cullingMask = 1 << 5;
            UiCamera.nearClipPlane = -UiSystemConstants.PlaneBuffer;
            UiCamera.farClipPlane = UiSystemConstants.PlaneBuffer;
        }
    }
}