using System;
using MPLCore.DI;
using UnityEngine;
using UnityEngine.UI;

namespace MPLCore.UiSystems
{
    public class UiCanvasLayer : MonoBehaviour
    {
        public event Action<UiViewDefinition, UiCanvasLayerDefinition> UiLoaded = delegate { };
        public event Action<UiViewDefinition, UiCanvasLayerDefinition> UiOpened = delegate { };
        public event Action<UiViewDefinition, UiCanvasLayerDefinition> UiClosed = delegate { };
        public event Action OpacityUpdated = delegate { };
        
        private DiContainer diContainer;

        private Canvas canvas;
        private CanvasScaler canvasScaler;
        private CanvasGroup canvasGroup;

        public UiCanvasLayerDefinition UiCanvasLayerDefinition { get; private set; }

        public void UnloadAllUi()
        {
            UiPresenter[] list = GetAllUiPresenters();

            foreach (UiPresenter uiPresenter in list)
            {
                if (uiPresenter ? uiPresenter : false)
                {
                    // uiPresenter.ExecuteStateTrigger(UiPresenter.StateTrigger.Unload);
                }
            }

            // modalUiData.Clear();
            // queuedUiData.Clear();
            // loadedUiPresenter.Clear();
        }
        
        public void Init(UiCanvasLayerDefinition uiCanvasLayerDefinition, DiContainer diContainer, Camera uiCamera)
        {
            UiCanvasLayerDefinition = uiCanvasLayerDefinition;
            this.diContainer = diContainer;

            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.sortingOrder = -uiCanvasLayerDefinition.Ordinal;

            canvas.worldCamera = uiCamera;

            canvas.planeDistance = Math.Max(UiSystemConstants.StartPlaneDistance + uiCanvasLayerDefinition.Ordinal * UiSystemConstants.PlaneDistanceBetweenLayerOrdinals, UiSystemConstants.MinPlaneDistance);

            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.screenMatchMode = UiCanvasLayerDefinition.ScreenMatchMode;
            canvasScaler.matchWidthOrHeight = uiCanvasLayerDefinition.MatchWidthOrHeight;
            canvasScaler.referenceResolution = uiCanvasLayerDefinition.ReferenceResolution;
        }
    
        private UiPresenter[] GetAllUiPresenters(bool includeInactive = true)
        {
            UiPresenter[] result = this ? GetComponentsInChildren<UiPresenter>(includeInactive) : Array.Empty<UiPresenter>();
            return result;
        }
        
        public bool Visible
        {
            get { return canvas.enabled; }
            set { canvas.enabled = value; }
        }
        
        public bool IsOpaque { get; private set; }
    }
}
