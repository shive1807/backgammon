using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Core.UiSystems
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
    
    
    
    public class UiSystem : IUiSystem, IDisposable
    {
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }

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
    }
}
