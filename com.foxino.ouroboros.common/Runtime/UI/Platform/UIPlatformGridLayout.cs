using UnityEngine;
using UnityEngine.UI;

namespace Ouroboros.Common.UI.Platform
{
    [RequireComponent(typeof(GridLayoutGroup))]
    public class UIPlatformGridLayout : UIPlatformBase
    {
        [SerializeField] private UIPlatformValueVector2 cellSize;
        [SerializeField] private UIPlatformValueVector2 spacing;
        [SerializeField] private UIPlatformValueInt rowCount;
        [SerializeField] private UIPlatformValueBool platformHasValue;

        public override void SetMobile()
        {
            SetGrid(UIPlatformType.Mobile);
        }

        public override void SetStandalone()
        {
            SetGrid(UIPlatformType.Standalone);
        }

        public override void SaveMobile()
        {
            SaveGrid(UIPlatformType.Mobile);
        }

        public override void SaveStandalone()
        {
            SaveGrid(UIPlatformType.Standalone);
        }

        private void SaveGrid(UIPlatformType platformType)
        {
            var grid = GetComponent<GridLayoutGroup>(); 
            cellSize.SetValue(platformType, grid.cellSize);
            rowCount.SetValue(platformType, grid.constraintCount);
            spacing.SetValue(platformType, grid.spacing);
            platformHasValue.SetValue(platformType, true);
        }

        public void SetGrid(UIPlatformType platformType)
        {
            if (!platformHasValue.GetValue(platformType)) return;

            var grid = GetComponent<GridLayoutGroup>(); 
            grid.cellSize = cellSize.GetValue(platformType);
            grid.constraintCount = rowCount.GetValue(platformType);
            grid.spacing = spacing.GetValue(platformType);
            grid.enabled = false;
            grid.enabled = true;
        }
    }
}