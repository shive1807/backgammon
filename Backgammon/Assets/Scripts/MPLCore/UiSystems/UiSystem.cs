using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MPLCore.DI;
using UnityEngine;

namespace MPLCore.UiSystems
{
    public interface IUiSystem
    {
        event Action<UiViewDefinition, UiCanvasLayerDefinition> UiLoaded;
        event Action<UiViewDefinition, UiCanvasLayerDefinition> UiOpened;
        event Action<UiViewDefinition, UiCanvasLayerDefinition> UiClosed;
        event Action<UiViewDefinition, UiCanvasLayerDefinition> UiUnloaded;
        IEnumerator PreloadAsync(UiViewDefinition uiViewDefinition, UiCanvasLayerDefinition uiCanvasLayerDefinition);
        IEnumerator OpenAsync(UiViewDefinition uiViewDefinition, UiCanvasLayerDefinition uiCanvasLayerDefinition, IUiData uiData = null);
        IEnumerator OpenMultipleAsync(UiViewDefinition[] uiDefinitions, UiCanvasLayerDefinition uiCanvasLayerDefinition);

        void Preload(UiViewDefinition uiViewDefinition, UiCanvasLayerDefinition uiCanvasLayerDefinition);

        void Open(UiViewDefinition uiViewDefinition, UiCanvasLayerDefinition uiCanvasLayerDefinition, IUiData uiData = null);

        void OpenMultipleCloseAllOthers(UiViewDefinition[] uiDefinitionList, UiCanvasLayerDefinition uiLayerDefinition, IUiData[] uiDataList = null);

        void Close(UiViewDefinition uiViewDefinition, UiCanvasLayerDefinition uiCanvasLayerDefinition);

        void CloseAllByLayer(UiCanvasLayerDefinition uiCanvasLayerDefinition);

        void CloseAllByLayerExcept(UiCanvasLayerDefinition uiLayerDefinition, UiViewDefinition[] uiNotToClose);

        void CloseAllExceptLayers(UiCanvasLayerDefinition[] layersNotToClose);

        void CloseAll();

        void Unload(UiViewDefinition uiViewDefinition, UiCanvasLayerDefinition uiCanvasLayerDefinition);

        void UnloadAllByLayer(UiCanvasLayerDefinition uiCanvasLayerDefinition);

        void UnloadAll();

        void SetLayerVisibility(UiCanvasLayerDefinition uiCanvasLayerDefinition, bool visible);

        bool BackButtonPressed();
    }
    
    public static class UiSystemConstants
    {
        public const int PlaneBuffer = 10;
        public const float PlaneDistanceBetweenLayerOrdinals = 10;
        public const float MinPlaneDistance = 0.1f;
        public const float StartPlaneDistance = 5;
    }
    
    public class UiSystem : IUiSystem, IDisposable
    {
        [Inject] private readonly UiRoot uiRoot;

        [Inject]
        private readonly DiContainer diContainer;

        public void Dispose()
        {
            // TODO release managed resources here
        }

        public event Action<UiViewDefinition, UiCanvasLayerDefinition> UiLoaded;
        public event Action<UiViewDefinition, UiCanvasLayerDefinition> UiOpened;
        public event Action<UiViewDefinition, UiCanvasLayerDefinition> UiClosed;
        public event Action<UiViewDefinition, UiCanvasLayerDefinition> UiUnloaded;
        
        private readonly Dictionary<UiCanvasLayerDefinition, UiCanvasLayer> canvasLayerMap = new Dictionary<UiCanvasLayerDefinition, UiCanvasLayer>();
        public IEnumerator PreloadAsync(UiViewDefinition uiViewDefinition, UiCanvasLayerDefinition uiCanvasLayerDefinition)
        {
            throw new NotImplementedException();
        }

        public IEnumerator OpenAsync(UiViewDefinition uiViewDefinition, UiCanvasLayerDefinition uiCanvasLayerDefinition,
            IUiData uiData = null)
        {
            throw new NotImplementedException();
        }

        public IEnumerator OpenMultipleAsync(UiViewDefinition[] uiDefinitions, UiCanvasLayerDefinition uiCanvasLayerDefinition)
        {
            throw new NotImplementedException();
        }

        public void Preload(UiViewDefinition uiViewDefinition, UiCanvasLayerDefinition uiCanvasLayerDefinition)
        {
            throw new NotImplementedException();
        }

        public void Open(UiViewDefinition uiViewDefinition, UiCanvasLayerDefinition uiCanvasLayerDefinition, IUiData uiData = null)
        {
            throw new NotImplementedException();
        }

        public void OpenMultipleCloseAllOthers(UiViewDefinition[] uiDefinitionList, UiCanvasLayerDefinition uiLayerDefinition,
            IUiData[] uiDataList = null)
        {
            throw new NotImplementedException();
        }

        public void Close(UiViewDefinition uiViewDefinition, UiCanvasLayerDefinition uiCanvasLayerDefinition)
        {
            throw new NotImplementedException();
        }

        public void CloseAllByLayer(UiCanvasLayerDefinition uiCanvasLayerDefinition)
        {
            throw new NotImplementedException();
        }

        public void CloseAllByLayerExcept(UiCanvasLayerDefinition uiLayerDefinition, UiViewDefinition[] uiNotToClose)
        {
            throw new NotImplementedException();
        }

        public void CloseAllExceptLayers(UiCanvasLayerDefinition[] layersNotToClose)
        {
            throw new NotImplementedException();
        }

        public void CloseAll()
        {
            throw new NotImplementedException();
        }

        public void Unload(UiViewDefinition uiViewDefinition, UiCanvasLayerDefinition uiCanvasLayerDefinition)
        {
            throw new NotImplementedException();
        }

        public void UnloadAllByLayer(UiCanvasLayerDefinition uiCanvasLayerDefinition)
        {
            throw new NotImplementedException();
        }

        public void SetLayerVisibility(UiCanvasLayerDefinition uiCanvasLayerDefinition, bool visible)
        {
            throw new NotImplementedException();
        }

        public bool BackButtonPressed()
        {
            throw new NotImplementedException();
        }
        
        public void UnloadAll()
        {
            foreach (UiCanvasLayer uiLayer in canvasLayerMap.Values.ToArray())
            {
                uiLayer.UnloadAllUi();
            }
        }
        
        private void UiCanvasLayer(UiCanvasLayerDefinition canvasLayerDefinition)
        {
            if (HasUiCanvasLayer(canvasLayerDefinition))
            {
                return;
            }

            InitUiCamera(canvasLayerDefinition);

            string layerName = $"{canvasLayerDefinition.Name} (Ordinal: {canvasLayerDefinition.Ordinal}) ";
            GameObject layerGameObject = new GameObject(layerName);
            layerGameObject.transform.SetParent(uiRoot.transform);

            UiCanvasLayer uiCanvasLayer = layerGameObject.AddComponent<UiCanvasLayer>();
            uiCanvasLayer.Init(canvasLayerDefinition, diContainer, uiRoot.UiCamera);
            uiCanvasLayer.UiLoaded += OnUiLoaded;
            uiCanvasLayer.UiOpened += OnUiOpened;
            uiCanvasLayer.UiClosed += OnUiClosed;
            uiCanvasLayer.OpacityUpdated += OnOpacityUpdated;
            canvasLayerMap.Add(canvasLayerDefinition, uiCanvasLayer);

            SortUiCanvasLayers();
        }
        
        private bool HasUiCanvasLayer(UiCanvasLayerDefinition canvasLayerDefinition)
        {
            return canvasLayerMap.ContainsKey(canvasLayerDefinition);
        }
        
        private void InitUiCamera(UiCanvasLayerDefinition canvasLayerDefinition)
        {
            float layerPlaneDistance = canvasLayerDefinition.Ordinal * UiSystemConstants.PlaneDistanceBetweenLayerOrdinals + UiSystemConstants.PlaneBuffer;
            uiRoot.UiCamera.nearClipPlane = Mathf.Min(uiRoot.UiCamera.nearClipPlane, -layerPlaneDistance);

            const float minNearClipPlane = 0.01f;
            uiRoot.UiCamera.nearClipPlane = Math.Max(uiRoot.UiCamera.nearClipPlane, minNearClipPlane);

            uiRoot.UiCamera.farClipPlane = Mathf.Max(uiRoot.UiCamera.farClipPlane, layerPlaneDistance);

            uiRoot.UiCamera.transform.SetAsLastSibling();
        }

        private void SortUiCanvasLayers()
        {
            UiCanvasLayer[] layers = uiRoot.GetComponentsInChildren<UiCanvasLayer>();
            layers = layers.OrderBy(x => x.UiCanvasLayerDefinition.Ordinal).ToArray();

            for (int i = 0; i < layers.Length; i++)
            {
                UiCanvasLayer uiCanvasLayer = layers[i];
                uiCanvasLayer.gameObject.transform.SetSiblingIndex(i);
            }
        }
        
        private void OnUiLoaded(UiViewDefinition uiViewDefinition, UiCanvasLayerDefinition uiCanvasLayerDefinition)
        {
            UiLoaded(uiViewDefinition, uiCanvasLayerDefinition);
        }

        private void OnUiOpened(UiViewDefinition uiViewDefinition, UiCanvasLayerDefinition uiCanvasLayerDefinition)
        {
            UiOpened(uiViewDefinition, uiCanvasLayerDefinition);
        }

        private void OnUiClosed(UiViewDefinition uiViewDefinition, UiCanvasLayerDefinition uiCanvasLayerDefinition)
        {
            UiClosed(uiViewDefinition, uiCanvasLayerDefinition);
        }
        
        private void OnOpacityUpdated()
        {
            UiCanvasLayer[] layers = uiRoot.GetComponentsInChildren<UiCanvasLayer>();

            bool isOccluded = false;
            foreach (UiCanvasLayer canvasLayer in layers)
            {
                canvasLayer.Visible = !isOccluded;
                if (canvasLayer.IsOpaque)
                {
                    isOccluded = true;
                }
            }
        }

    }
}
