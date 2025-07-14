using System;
using System.Numerics;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

namespace Core.UiSystems
{
    public enum UiType
    {
        Modal,
        Stacked,
    }

    public enum FillType
    {
        Transparent,
        Opaque,
    }

    public enum HideType
    {
        Normal,
        WhenNotOnTop,
    }

    [Serializable]
    public class UiViewDefinition
    {
        public AssetReference AssetReference;
        public UiType UiType;
        public FillType FillType;
        public HideType HideType;

        [NonSerialized]
        public string TestContext;

        [NonSerialized]
        public readonly string UiViewName;

        public UiViewDefinition(string uiViewName)
        {
            UiViewName = uiViewName;
        }

        public override string ToString()
        {
            return $"{UiViewName} {AssetReference}";
        }
    }
    
    public class UiCanvasLayerDefinition
    {
        public readonly string Name;
        public readonly int Ordinal;
        public readonly Vector2 ReferenceResolution;
        public readonly float MatchWidthOrHeight;
        public readonly CanvasScaler.ScreenMatchMode ScreenMatchMode;

        public UiCanvasLayerDefinition(string name, int ordinal, Vector2 referenceResolution, float matchWidthOrHeight, CanvasScaler.ScreenMatchMode screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight)
        {
            Name = name;
            Ordinal = ordinal;
            ReferenceResolution = referenceResolution;
            MatchWidthOrHeight = matchWidthOrHeight;
            ScreenMatchMode = screenMatchMode;

            if (Ordinal < 0)
            {
                throw new ArgumentException("Ordinals smaller than zero are not allowed");
            }
        }
    }
}