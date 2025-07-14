using Core.UiSystems;
using UnityEngine;

namespace Game.Ui
{
    [CreateAssetMenu(menuName = "MPL/Ui/UiCatalog")]
    public class GameUiCatalog : ScriptableObject
    {
        public UiViewDefinition HomeScreen     = new(nameof(HomeScreen));
        public UiViewDefinition GameScreen     = new(nameof(GameScreen));
    }
}
