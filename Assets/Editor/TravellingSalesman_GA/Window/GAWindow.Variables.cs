using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using KurdifyEngine.GA;
using System;

namespace KurdifyEditor.GA
{
    public partial class GAWindow : EditorWindow
    {
        public enum InspectorType { Graphs, Data }
        public enum SortingType { BestFirst, WorstFirst, Index }
        public enum CrossoverType { None, OrderedSequence}

        static public GAWindow Window
        {
            get
            {
                if (_Window == null)
                {
                    _Window = GetWindow<GAWindow>();
                }
                return _Window;
            }
        }
        public static TravellingSalesman_GA Data
        {
            get
            {
                if (_Data == null && !string.IsNullOrEmpty(CurrentLoadedSimulationFilepath) && System.IO.File.Exists(CurrentLoadedSimulationFilepath))
                {
                    _Data = TravellingSalesman_GA.Load(FileUtil.GetProjectRelativePath(CurrentLoadedSimulationFilepath));
                }
                return _Data;
            }
            set
            {
                if (value == null) CurrentLoadedSimulationFilepath = "";
                _Data = value;
            }
        }
        public Generation<int> CurrentGeneration
        {
            get
            {
                if (Data == null || Data.Generations.Count == 0)
                    return null;


                return Data.Generations[SortedGenerations[CurrentSelected]];
            }
        }
        public int CurrentSelected
        {
            get
            {
                if (Data != null)
                    _CurrentSelected = Mathf.Clamp(_CurrentSelected, 0, Data.Generations.Count - 1);

                return _CurrentSelected;
            }
            set
            {
                _CurrentSelected = (_CurrentSelected >= 0) ? value : 0;

                Refresh();
            }
        }
        public static Color BackgroundColor
        {
            get
            {
                return (EditorGUIUtility.isProSkin) ? _DarkBackground : _LightBackground;
            }
        }
        public static Color ItemSelected
        {
            get
            {
                return (EditorGUIUtility.isProSkin) ? _ItemSelectedDark : _ItemSelectedLight;
            }
        }
        public static Color ItemSelectedNotFocused
        {
            get
            {
                return (EditorGUIUtility.isProSkin) ? _ItemSelectedNotFocusedDark : _ItemSelectedNotFocusedLight;
            }
        }
        public static string CurrentLoadedSimulationFilepath
        {
            get
            {
                return EditorPrefs.GetString("GACurrentData");
            }
            set
            {
                EditorPrefs.SetString("GACurrentData", value);
            }
        }
        public float GridWidth
        {
            get
            {
                return _GridWidth * GridSize;
            }
        }
        public float GridHeight
        {
            get
            {
                return _GridHeight * GridSize;
            }
        }
        public SortingType CurrentGraphSorting
        {
            get
            {
                return _CurrentGraphSorting;
            }
            set
            {
                if (_CurrentGraphSorting != value)
                {
                    _CurrentGraphSorting = value;
                    Refresh();
                }
            }
        }
        public SortingType CurrentGenerationsSorting
        {
            get
            {
                return _CurrentGenerationSorting;
            }
            set
            {
                if (_CurrentGenerationSorting != value)
                {
                    _CurrentGenerationSorting = value;
                    Refresh();
                }
            }
        }
        public List<int> SortedGenerations
        {
            get
            {
                switch (CurrentGenerationsSorting)
                {
                    case SortingType.BestFirst:
                        return BestSortedGenerations;
                    case SortingType.WorstFirst:
                        return WorstSortedGenerations;
                }

                return IndexSortedGenerations;
            }
        }
        public List<int> BestSortedGenerations
        {
            get
            {
                if (_BestSortedGenerations == null || _BestSortedGenerations.Count != Data.Generations.Count)
                {
                    _BestSortedGenerations = new List<int>(System.Linq.Enumerable.Range(0, Data.Generations.Count));
                    _BestSortedGenerations.Clear();
                    int bestGeneration = 0;

                    _BestSortedGenerations.Add(0);
                    for (int index = 1; index < Data.Generations.Count; index++)
                    {
                        if (Data.Generations[index].FitnessSum > Data.Generations[bestGeneration].FitnessSum)
                        {
                            bestGeneration = index;
                            _BestSortedGenerations.Insert(0, index);
                        }
                        else _BestSortedGenerations.Add(index);
                    }
                }
                return _BestSortedGenerations;
            }
        }
        public List<int> WorstSortedGenerations
        {
            get
            {
                if (_WorstSortedGenerations == null || _WorstSortedGenerations.Count != Data.Generations.Count)
                {
                    _WorstSortedGenerations = new List<int>(System.Linq.Enumerable.Range(0, Data.Generations.Count));
                }
                return _WorstSortedGenerations;
            }
        }
        public List<int> IndexSortedGenerations
        {
            get
            {
                if (_IndexSortedGenerations == null || _IndexSortedGenerations.Count != Data.Generations.Count)
                {
                    _IndexSortedGenerations = new List<int>(System.Linq.Enumerable.Range(0, Data.Generations.Count));
                }
                return _IndexSortedGenerations;
            }
        }
        #region Toolbar
        KurdifyEditor.Menu FileMenu
        {
            get
            {
                if (_FileMenu == null)
                {
                    _FileMenu = new KurdifyEditor.Menu();
                    //_FileMenu.BackgroundBox = true;
                    _FileMenu.ButtonStyle = EditorStyles.toolbarButton;
                    _FileMenu.Items = new KurdifyEditor.MenuItem[4];
                    _FileMenu.Items[0] = new KurdifyEditor.MenuItem("New", 0);
                    _FileMenu.Items[1] = new KurdifyEditor.MenuItem("Open", 1);
                    _FileMenu.Items[2] = new KurdifyEditor.MenuItem("Save", 2);
                    _FileMenu.Items[3] = new KurdifyEditor.MenuItem("Exit", 3);

                    _FileMenu.OnItemClicked += FileMenuItemClicked;
                }
                return _FileMenu;
            }
        }

        KurdifyEditor.Menu EditMenu
        {
            get
            {
                if (_EditMenu == null)
                {
                    _EditMenu = new KurdifyEditor.Menu();
                    _EditMenu.Width = 120;
                    _EditMenu.ButtonStyle = EditorStyles.toolbarButton;
                    _EditMenu.Items = new KurdifyEditor.MenuItem[7];
                    _EditMenu.Items[0] = new KurdifyEditor.MenuItem("Next Generation", 0);
                    _EditMenu.Items[1] = new KurdifyEditor.MenuItem("Next 5 Generations", 1);
                    _EditMenu.Items[2] = new KurdifyEditor.MenuItem("Next 10 Generations", 2);
                    _EditMenu.Items[3] = new KurdifyEditor.MenuItem("Next 50 Generations", 3);
                    _EditMenu.Items[4] = new KurdifyEditor.MenuItem("Next 100 Generations", 4);
                    _EditMenu.Items[5] = new KurdifyEditor.MenuItem("Next 5000 Generations", 5);
                    _EditMenu.Items[6] = new KurdifyEditor.MenuItem("Clear Generation", 6);

                    _EditMenu.OnItemClicked += EditMenuItemClicked;
                }
                return _EditMenu;
            }
        }

        KurdifyEditor.Menu _FileMenu;
        KurdifyEditor.Menu _EditMenu;
        #endregion

        public float GridSize = 1f;
        private const int _GridWidth = 210;
        private const int _GridHeight = 170;
        internal InspectorType CurrentInspector = InspectorType.Graphs;
        private SortingType _CurrentGraphSorting = SortingType.BestFirst;
        private SortingType _CurrentGenerationSorting = SortingType.Index;
        internal string[] CrossoverOptions = new string[] { "None", "Ordered Sequence" };
        internal string[] GraphSortingOptions = new string[] { "Best To Worst", "Worst To Best" };
        internal string[] GenerationsSortingOptions = new string[] { "Best To Worst", "Worst To Best", "Index" };

        private List<int> _BestSortedGenerations;
        private List<int> _WorstSortedGenerations;
        private List<int> _IndexSortedGenerations;
        private static Color _ItemSelectedDark = new Color(0.2431372549019608f, 0.3725490196078431f, 0.5882352941176471f, 1f);
        private static Color _ItemSelectedNotFocusedDark = new Color(0.2823529411764706f, 0.2823529411764706f, 0.2823529411764706f, 1f);

        private static Color _ItemSelectedLight = new Color(0.2431372549019608f, 0.4901960784313725f, 0.9058823529411765f, 1f);
        private static Color _ItemSelectedNotFocusedLight = new Color(0.5607843137254902f, 0.5607843137254902f, 0.5607843137254902f, 1f);

        private static Color _LightBackground = new Color(0.7607843137254902f, 0.7607843137254902f, 0.7607843137254902f, 1f);
        private static Color _DarkBackground = new Color(0.1647058823529412f, 0.1647058823529412f, 0.1647058823529412f, 1f);

        private static GAWindow _Window;
        private static TravellingSalesman_GA _Data;
        private int _CurrentSelected;

        protected float NextY = 0;
    }
}
