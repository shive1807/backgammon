using System;
using UnityEngine.AddressableAssets;

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
}