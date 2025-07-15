using UnityEngine;
using UnityEngine.UI;

namespace MPLCore.UiSystems
{
    public static class UiCanvasList
    {
        private static readonly Vector2 ReferenceResolution = new Vector2(1920, 1080);
        private static readonly float MatchWidthOrHeight = 1;
        private static readonly CanvasScaler.ScreenMatchMode ScreenMatchMode = CanvasScaler.ScreenMatchMode.Expand;

        public static readonly UiCanvasLayerDefinition Screens                   = NewLayer(nameof(Screens), 13);
        public static readonly UiCanvasLayerDefinition GameplayPopups            = NewLayer(nameof(GameplayPopups), 12);
        public static readonly UiCanvasLayerDefinition GameplayMessageBanners    = NewLayer(nameof(GameplayMessageBanners), 11);
        public static readonly UiCanvasLayerDefinition InteractiveTutorialPopups = NewLayer(nameof(InteractiveTutorialPopups), 10);
        public static readonly UiCanvasLayerDefinition Popups                    = NewLayer(nameof(Popups), 9);
        public static readonly UiCanvasLayerDefinition ForegroundScreens         = NewLayer(nameof(ForegroundScreens), 8);
        public static readonly UiCanvasLayerDefinition Rewards                   = NewLayer(nameof(Rewards), 7);
        public static readonly UiCanvasLayerDefinition Blimps                    = NewLayer(nameof(Blimps), 6);
        public static readonly UiCanvasLayerDefinition Chat                      = NewLayer(nameof(Chat), 5);
        public static readonly UiCanvasLayerDefinition InteractionBlock          = NewLayer(nameof(InteractionBlock), 4);
        public static readonly UiCanvasLayerDefinition Transitions               = NewLayer(nameof(Transitions), 3);
        public static readonly UiCanvasLayerDefinition LoadingScreen             = NewLayer(nameof(LoadingScreen), 2);
        public static readonly UiCanvasLayerDefinition ForegroundPopups          = NewLayer(nameof(ForegroundPopups), 1);

        private static UiCanvasLayerDefinition NewLayer(string name, int ordinal)
        {
            return new UiCanvasLayerDefinition(name, ordinal, ReferenceResolution, MatchWidthOrHeight, ScreenMatchMode);
        }
    }
}