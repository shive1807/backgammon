using System;
using UnityEngine;

namespace Core.UiSystems
{
    public class UiCanvasLayer : MonoBehaviour
    {
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
    
        private UiPresenter[] GetAllUiPresenters(bool includeInactive = true)
        {
            UiPresenter[] result = this ? GetComponentsInChildren<UiPresenter>(includeInactive) : Array.Empty<UiPresenter>();
            return result;
        }
    }
}
