using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Tools
{
    [System.Serializable]
    public class SmartGridLayoutGroup : LayoutGroup
    {
        [Header("Grid Settings")]
        [SerializeField] private Vector2 m_CellSize = new Vector2(100, 100);
        [SerializeField] private Vector2 m_Spacing = new Vector2(10, 10);
        [SerializeField] private int m_MaxItemsPerRow = 0; // 0 = автоматически
    
        [Header("Alignment")]
        [SerializeField] private bool m_CenterRows = true;
        [SerializeField] private bool m_FitCellsToContainer = false;
    
        [Header("Auto Sizing")]
        [SerializeField] private bool m_AutoCalculateCellSize = false;
        [SerializeField] private float m_PreferredAspectRatio = 1f;

        public Vector2 cellSize
        {
            get { return m_CellSize; }
            set { SetProperty(ref m_CellSize, value); }
        }

        public Vector2 spacing
        {
            get { return m_Spacing; }
            set { SetProperty(ref m_Spacing, value); }
        }

        public int maxItemsPerRow
        {
            get { return m_MaxItemsPerRow; }
            set { SetProperty(ref m_MaxItemsPerRow, Mathf.Max(0, value)); }
        }

        public bool centerRows
        {
            get { return m_CenterRows; }
            set { SetProperty(ref m_CenterRows, value); }
        }

        public bool fitCellsToContainer
        {
            get { return m_FitCellsToContainer; }
            set { SetProperty(ref m_FitCellsToContainer, value); }
        }

        public bool autoCalculateCellSize
        {
            get { return m_AutoCalculateCellSize; }
            set { SetProperty(ref m_AutoCalculateCellSize, value); }
        }

        public override void CalculateLayoutInputHorizontal()
        {
            base.CalculateLayoutInputHorizontal();
        
            int childCount = 0;
            for (int i = 0; i < rectTransform.childCount; i++)
            {
                RectTransform child = rectTransform.GetChild(i) as RectTransform;
                if (child == null || !child.gameObject.activeInHierarchy)
                    continue;
                childCount++;
            }

            if (childCount == 0)
            {
                SetLayoutInputForAxis(0, 0, 0, 0);
                return;
            }

            if (m_AutoCalculateCellSize)
            {
                CalculateAutoCellSize();
            }

            int itemsPerRow = CalculateItemsPerRow();
            int rowCount = Mathf.CeilToInt((float)childCount / itemsPerRow);
        
            float totalWidth = itemsPerRow * m_CellSize.x + (itemsPerRow - 1) * m_Spacing.x;
            float totalHeight = rowCount * m_CellSize.y + (rowCount - 1) * m_Spacing.y;
        
            SetLayoutInputForAxis(totalWidth, totalWidth, totalWidth, 0);
        }

        public override void CalculateLayoutInputVertical()
        {
            int childCount = 0;
            for (int i = 0; i < rectTransform.childCount; i++)
            {
                RectTransform child = rectTransform.GetChild(i) as RectTransform;
                if (child == null || !child.gameObject.activeInHierarchy)
                    continue;
                childCount++;
            }

            if (childCount == 0)
            {
                SetLayoutInputForAxis(0, 0, 0, 1);
                return;
            }

            int itemsPerRow = CalculateItemsPerRow();
            int rowCount = Mathf.CeilToInt((float)childCount / itemsPerRow);
        
            float totalHeight = rowCount * m_CellSize.y + Mathf.Max(0, rowCount - 1) * m_Spacing.y;
        
            SetLayoutInputForAxis(totalHeight, totalHeight, totalHeight, 1);
        }

        public override void SetLayoutHorizontal()
        {
            SetCellsAlongAxis(0);
        }

        public override void SetLayoutVertical()
        {
            SetCellsAlongAxis(1);
        }

        private void SetCellsAlongAxis(int axis)
        {
            List<RectTransform> activeChildren = new List<RectTransform>();
        
            for (int i = 0; i < rectTransform.childCount; i++)
            {
                RectTransform child = rectTransform.GetChild(i) as RectTransform;
                if (child != null && child.gameObject.activeInHierarchy)
                    activeChildren.Add(child);
            }

            if (activeChildren.Count == 0)
                return;

            int itemsPerRow = CalculateItemsPerRow();
            int rowCount = Mathf.CeilToInt((float)activeChildren.Count / itemsPerRow);

            Vector2 actualCellSize = m_CellSize;
        
            if (m_FitCellsToContainer)
            {
                float availableWidth = rectTransform.rect.width - padding.horizontal;
                float cellWidth = (availableWidth - (itemsPerRow - 1) * m_Spacing.x) / itemsPerRow;
                actualCellSize.x = Mathf.Max(cellWidth, 0);
            }

            for (int i = 0; i < activeChildren.Count; i++)
            {
                int row = i / itemsPerRow;
                int col = i % itemsPerRow;
            
                int itemsInCurrentRow = Mathf.Min(itemsPerRow, activeChildren.Count - row * itemsPerRow);
            
                Vector2 position = CalculateChildPosition(row, col, itemsInCurrentRow, actualCellSize);
            
                if (axis == 0)
                {
                    SetChildAlongAxis(activeChildren[i], 0, position.x, actualCellSize.x);
                }
                else
                {
                    SetChildAlongAxis(activeChildren[i], 1, position.y, actualCellSize.y);
                }
            }
        }

        private Vector2 CalculateChildPosition(int row, int col, int itemsInRow, Vector2 cellSize)
        {
            Vector2 position = Vector2.zero;
        
            if (m_CenterRows)
            {
                float rowWidth = itemsInRow * cellSize.x + (itemsInRow - 1) * m_Spacing.x;
                float containerWidth = rectTransform.rect.width - padding.horizontal;
                float startX = (containerWidth - rowWidth) * 0.5f;
                position.x = startX + col * (cellSize.x + m_Spacing.x);
            }
            else
            {
                position.x = col * (cellSize.x + m_Spacing.x);
            }
        
            position.y = row * (cellSize.y + m_Spacing.y);
        
            position += GetAlignmentOffset();
        
            return position;
        }

        private Vector2 GetAlignmentOffset()
        {
            Vector2 offset = Vector2.zero;
            Vector2 containerSize = rectTransform.rect.size;
            containerSize.x -= padding.horizontal;
            containerSize.y -= padding.vertical;
        
            int childCount = 0;
            for (int i = 0; i < rectTransform.childCount; i++)
            {
                RectTransform child = rectTransform.GetChild(i) as RectTransform;
                if (child != null && child.gameObject.activeInHierarchy)
                    childCount++;
            }
        
            if (childCount == 0) return offset;
        
            int itemsPerRow = CalculateItemsPerRow();
            int rowCount = Mathf.CeilToInt((float)childCount / itemsPerRow);
        
            Vector2 gridSize = new Vector2(
                itemsPerRow * m_CellSize.x + (itemsPerRow - 1) * m_Spacing.x,
                rowCount * m_CellSize.y + (rowCount - 1) * m_Spacing.y
            );
        
            switch (childAlignment)
            {
                case TextAnchor.UpperRight:
                case TextAnchor.MiddleRight:
                case TextAnchor.LowerRight:
                    offset.x = containerSize.x - gridSize.x;
                    break;
                case TextAnchor.UpperCenter:
                case TextAnchor.MiddleCenter:
                case TextAnchor.LowerCenter:
                    if (!m_CenterRows) 
                        offset.x = (containerSize.x - gridSize.x) * 0.5f;
                    break;
            }
        
            switch (childAlignment)
            {
                case TextAnchor.MiddleLeft:
                case TextAnchor.MiddleCenter:
                case TextAnchor.MiddleRight:
                    offset.y = (containerSize.y - gridSize.y) * 0.5f;
                    break;
                case TextAnchor.LowerLeft:
                case TextAnchor.LowerCenter:
                case TextAnchor.LowerRight:
                    offset.y = containerSize.y - gridSize.y;
                    break;
            }
        
            return offset;
        }

        private int CalculateItemsPerRow()
        {
            if (m_MaxItemsPerRow > 0)
                return m_MaxItemsPerRow;
        
            float availableWidth = rectTransform.rect.width - padding.horizontal;
            if (availableWidth <= 0)
                return 1;
        
            int itemsPerRow = Mathf.FloorToInt((availableWidth + m_Spacing.x) / (m_CellSize.x + m_Spacing.x));
            return Mathf.Max(1, itemsPerRow);
        }

        private void CalculateAutoCellSize()
        {
            float availableWidth = rectTransform.rect.width - padding.horizontal;
            float availableHeight = rectTransform.rect.height - padding.vertical;
        
            if (availableWidth <= 0 || availableHeight <= 0)
                return;
        
            int childCount = 0;
            for (int i = 0; i < rectTransform.childCount; i++)
            {
                RectTransform child = rectTransform.GetChild(i) as RectTransform;
                if (child != null && child.gameObject.activeInHierarchy)
                    childCount++;
            }
        
            if (childCount == 0)
                return;
        
            int bestItemsPerRow = 1;
            float bestRatio = float.MaxValue;
        
            for (int itemsPerRow = 1; itemsPerRow <= childCount; itemsPerRow++)
            {
                int rowCount = Mathf.CeilToInt((float)childCount / itemsPerRow);
            
                float cellWidth = (availableWidth - (itemsPerRow - 1) * m_Spacing.x) / itemsPerRow;
                float cellHeight = (availableHeight - (rowCount - 1) * m_Spacing.y) / rowCount;
            
                if (cellWidth <= 0 || cellHeight <= 0)
                    continue;
            
                float ratio = Mathf.Abs((cellWidth / cellHeight) - m_PreferredAspectRatio);
            
                if (ratio < bestRatio)
                {
                    bestRatio = ratio;
                    bestItemsPerRow = itemsPerRow;
                }
            }
        
            int finalRowCount = Mathf.CeilToInt((float)childCount / bestItemsPerRow);
            m_CellSize.x = (availableWidth - (bestItemsPerRow - 1) * m_Spacing.x) / bestItemsPerRow;
            m_CellSize.y = (availableHeight - (finalRowCount - 1) * m_Spacing.y) / finalRowCount;
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            m_CellSize.x = Mathf.Max(m_CellSize.x, 1f);
            m_CellSize.y = Mathf.Max(m_CellSize.y, 1f);
        }
#endif
    }
}