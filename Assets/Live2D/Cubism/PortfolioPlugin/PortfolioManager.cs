/**
 * Copyright(c) Live2D Inc. All rights reserved.
 *
 * Use of this source code is governed by the MIT License.
 */

using System.Collections.Generic;
using Live2D.Cubism.Framework.Expression;
using Live2D.Cubism.Framework.Motion;
using UnityEngine;
using UnityEngine.UI;

namespace Live2D.Cubism.PortfolioPlugin
{
    [DisallowMultipleComponent]
    public class PortfolioManager : MonoBehaviour
    {
        #region Varuables
        /// <summary>
        /// 表示するモデルインデックス切り替え用ドロップダウンメニューの参照
        /// </summary>
        [SerializeField] public Dropdown DropdownModelGroup;

        /// <summary>
        /// モデル切り替え用ドロップダウンメニューの参照
        /// </summary>
        [SerializeField] public Dropdown DropdownModel;

        /// <summary>
        /// モーション切り替え用ドロップダウンメニューの参照
        /// </summary>
        [SerializeField] public Dropdown DropdownMotion;

        /// <summary>
        /// 表情切り替え用ドロップダウンメニューの参照
        /// </summary>
        [SerializeField] public Dropdown DropdownExpression;


        /// <summary>
        /// シーン内のモデルのPrefab
        /// </summary>
        private PortfolioController[] _prefabs;

        /// <summary>
        /// 現在表示しているモデル
        /// </summary>
        private int _currentModelGroupIndex = -1;

        /// <summary>
        /// モーションリスト内のモーションが所属しているモーショングループ
        /// </summary>
        private int[] _motionGroupIndices;

        /// <summary>
        /// モーションリスト内のモーションがモーショングループの何番目か
        /// </summary>
        private int[] _motionIndices;

        /// <summary>
        /// フォーカスされているモデルのインデックス
        /// 複数モデル表示時に選択する
        /// </summary>
        private int _focusedModelIndex;

        /// <summary>
        /// フォーカスされているモデルの現在の表情
        /// </summary>
        private int[] _currentExpressionIndices;

        /// <summary>
        /// フォーカスされているモデルの現在のモーション
        /// </summary>
        private int[] _currentMotionIndices;
        

        #endregion


        #region Unity Events Handling

        /// <summary>
        /// Called by Unity.
        /// </summary>
        private void Awake()
        {
            Refresh();
        }

        #endregion


        #region Function

        /// <summary>
        /// 初期化処理
        /// </summary>
        public void Refresh()
        {
            _prefabs = FindObjectsOfType<PortfolioController>();

            if (_prefabs.Length <= 0)
            {
                return;
            }
            _currentExpressionIndices = new int[_prefabs.Length];
            _currentMotionIndices = new int[_prefabs.Length];
            
            if (DropdownModelGroup != null)
            {
                DropdownModelGroup.onValueChanged.RemoveAllListeners();
                DropdownModelGroup.onValueChanged.AddListener(OnChangeModel);

                ResetDropdownModelGroupSettings();

                OnChangeModel(0);
            }

            if (DropdownModel != null)
            {
                DropdownModel.onValueChanged.RemoveAllListeners();
                DropdownModel.onValueChanged.AddListener(OnChangeFocusedModel);

                ResetDropdownModelSettings();
            }

            if (DropdownMotion != null)
            {
                DropdownMotion.onValueChanged.RemoveAllListeners();

                DropdownMotion.onValueChanged.AddListener(OnChangeMotion);

                ResetDropdownMotionSettings();
            }

            if (DropdownExpression != null)
            {
                DropdownExpression.onValueChanged.RemoveAllListeners();

                DropdownExpression.onValueChanged.AddListener(OnChangeExpression);

                ResetDropdownExpressionSettings();
            }
        }

        /// <summary>
        /// 表示モデル切り替え用Dropdownを再設定
        /// </summary>
        private void ResetDropdownModelGroupSettings()
        {
            if (DropdownModelGroup == null)
            {
                return;
            }

            DropdownModelGroup.ClearOptions();

            var groupIndices = new List<string>();
            for (var i = 0; i < _prefabs.Length; i++)
            {
                if (groupIndices.Contains(_prefabs[i].ModelGroupNumber.ToString()))
                {
                    continue;
                }

                groupIndices.Add(_prefabs[i].ModelGroupNumber.ToString());
            }

            groupIndices.Sort();

            DropdownModelGroup.AddOptions(groupIndices);
        }

        /// <summary>
        /// 選択モデル切り替え用Dropdownを再設定
        /// </summary>
        private void ResetDropdownModelSettings()
        {
            if (DropdownModel == null)
            {
                return;
            }

            DropdownModel.ClearOptions();
            var prefabNames = new List<string>();
            for (var i = 0; i < _prefabs.Length; i++)
            {
                if (_prefabs[i].ModelGroupNumber != _currentModelGroupIndex)
                {
                    continue;
                }

                prefabNames.Add(_prefabs[i].gameObject.name);
            }

            DropdownModel.AddOptions(prefabNames);
        }


        /// <summary>
        /// モーション再生用Dropdownを再設定
        /// </summary>
        private void ResetDropdownMotionSettings()
        {
            if (_prefabs.Length <= 0 || DropdownMotion == null ||
                _focusedModelIndex < 0)
            {
                return;
            }

            DropdownMotion.ClearOptions();

            var motionList = _prefabs[_focusedModelIndex].MotionList;
            if (motionList == null || motionList.Motions.Length <= 0)
            {
                return;
            }


            var options = new List<string>();
            var motionGroupIndicesList = new List<int>();
            var motionIndicesList = new List<int>();
            for (var i = 0; i < motionList.Motions.Length; i++)
            {
                for (var j = 0; j < motionList.Motions[i].Motions.Length; j++)
                {
                    var motion = motionList.Motions[i].Motions[j];

                    if (options.Contains(motion.name))
                    {
                        continue;
                    }

                    options.Add(motion.name);
                    motionGroupIndicesList.Add(i);
                    motionIndicesList.Add(j);
                }
            }

            _motionGroupIndices = motionGroupIndicesList.ToArray();
            _motionIndices = motionIndicesList.ToArray();

            DropdownMotion.AddOptions(options);
            DropdownMotion.value = _currentMotionIndices[_focusedModelIndex];
        }

        /// <summary>
        /// 表情切り替え用Dropdownを再設定
        /// </summary>
        private void ResetDropdownExpressionSettings()
        {
            if (DropdownExpression == null)
            {
                return;
            }

            DropdownExpression.ClearOptions();
            var expressionList = _prefabs[_focusedModelIndex].GetComponent<CubismExpressionController>().ExpressionsList;
            if (expressionList == null || expressionList.CubismExpressionObjects.Length <= 0)
            {
                return;
            }

            var expressionNames = new List<string>();
            for (var i = 0; i < expressionList.CubismExpressionObjects.Length; i++)
            {
                expressionNames.Add(expressionList.CubismExpressionObjects[i].name);
            }
            DropdownExpression.AddOptions(expressionNames);
            DropdownExpression.value = _currentExpressionIndices[_focusedModelIndex];
        }


        /// <summary>
        /// Dropdownで選択されたモデルを表示
        /// </summary>
        /// <param name="modelGroupIndex">表示するモデルのインデックス</param>
        private void OnChangeModel(int modelGroupIndex)
        {
            if (DropdownModel == null || DropdownModelGroup == null)
            {
                return;
            }

            ChangeDisplayedModel(int.Parse(DropdownModelGroup.options[modelGroupIndex].text));

            ResetDropdownModelSettings();
            ResetDropdownExpressionSettings();
            ResetDropdownMotionSettings();
        }

        /// <summary>
        /// モーションや表情を切り替えるモデルの切り替え
        /// </summary>
        /// <param name="optionIndex"></param>
        public void OnChangeFocusedModel(int optionIndex)
        {
            var modelIndex = -1;
            var modelName = DropdownModel.options[optionIndex].text;
            for (var i = 0; i < _prefabs.Length; i++)
            {
                if (_prefabs[i].gameObject.name != modelName)
                {
                    continue;
                }

                modelIndex = i;
                break;
            }

            if (modelIndex < 0)
            {
                return;
            }

            _focusedModelIndex = modelIndex;

            ResetDropdownExpressionSettings();
            ResetDropdownMotionSettings();
        }

        /// <summary>
        /// Dropdownで選択されたモーションを再生
        /// </summary>
        /// <param name="motionIndex">再生するモーションのインデックス</param>
        private void OnChangeMotion(int motionIndex)
        {
            if (_prefabs == null ||
                _motionGroupIndices == null || _motionIndices == null ||
                _focusedModelIndex < 0 || motionIndex < 0)
            {
                return;
            }

            _prefabs[_focusedModelIndex].PlayMotion(_motionGroupIndices[motionIndex], _motionIndices[motionIndex]);

            _currentMotionIndices[_focusedModelIndex] = motionIndex;
        }

        /// <summary>
        /// 表情切り替え
        /// </summary>
        /// <param name="expressionIndex">切り替える表情のインデックス</param>
        private void OnChangeExpression(int expressionIndex)
        {
            if (DropdownExpression == null)
            {
                return;
            }

            _prefabs[_focusedModelIndex].SetExpression(expressionIndex);
        }



        /// <summary>
        /// 表示モデルを切り替え
        /// </summary>
        /// <param name="modelGroupIndex">表示するモデルグループのインデックス</param>
        private void ChangeDisplayedModel(int modelGroupIndex)
        {
            if (modelGroupIndex < 0 || _currentModelGroupIndex == modelGroupIndex)
            {
                return;
            }

            _currentModelGroupIndex = modelGroupIndex;

            var isFirst = true;
            for (var i = 0; i < _prefabs.Length; i++)
            {
                var isActive = _prefabs[i].ModelGroupNumber == modelGroupIndex;
                _prefabs[i].gameObject.SetActive(isActive);

                if (!isActive)
                {
                    continue;
                }

                _prefabs[i].PlayIdleMotion(CubismMotionPriority.PriorityForce);
                _currentMotionIndices[i] = 0;
                _currentExpressionIndices[i] = 0;

                if (!isFirst)
                {
                    continue;
                }

                isFirst = false;
                // 表示する最初の一体目を選択中のモデルとして記録
                _focusedModelIndex = i;
            }
        }

        #endregion
    }
}
