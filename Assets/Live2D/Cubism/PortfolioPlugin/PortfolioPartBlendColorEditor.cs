/**
 * Copyright(c) Live2D Inc. All rights reserved.
 *
 * Use of this source code is governed by the Live2D Open Software license
 * that can be found at https://www.live2d.com/eula/live2d-open-software-license-agreement_en.html.
 */

using System;
using Live2D.Cubism.Core;
using Live2D.Cubism.Rendering;
using UnityEngine;


namespace Live2D.Cubism.PortfolioPlugin
{
    [ExecuteInEditMode, RequireComponent(typeof(CubismPart))]
    public class PortfolioPartBlendColorEditor : MonoBehaviour
    {
        /// <summary>
        /// モデル
        /// </summary>
        private CubismModel _model;

        /// <summary>
        /// レンダーコントローラー
        /// </summary>
        private CubismRenderController _renderController;

        /// <summary>
        /// レンダラーの配列
        /// </summary>
        private CubismRenderer[] _renderers;

        /// <summary>
        /// パーツの配列
        /// </summary>
        private CubismPart _part;

        /// <summary>
        /// <see cref="ChildDrawableRenderers"/> のバックフィールド
        /// </summary>
        [SerializeField, HideInInspector]
        private CubismRenderer[] _childDrawableRenderers;

        /// <summary>
        /// パーツを親に持つアートメッシュの配列
        /// </summary>
        public CubismRenderer[] ChildDrawableRenderers
        {
            get { return _childDrawableRenderers; }
            set { _childDrawableRenderers = value; }
        }

        /// <summary>
        /// <see cref="OverwriteFlagForPartMultiplyColors"/> のバックフィールド
        /// </summary>
        [SerializeField, HideInInspector]
        private bool _isOverwrittenPartMultiplyColors;

        /// <summary>
        /// モデルの乗算色をSDK側で上書きするか。
        /// </summary>
        public bool OverwriteFlagForPartMultiplyColors
        {
            get { return _isOverwrittenPartMultiplyColors; }
            set { _isOverwrittenPartMultiplyColors = value; }
        }

        /// <summary>
        /// <see cref="OverwriteFlagForPartScreenColors"/> のバックフィールド
        /// </summary>
        [SerializeField, HideInInspector]
        private bool _isOverwrittenPartScreenColors;

        /// <summary>
        /// モデルのスクリーン色をSDK側で上書きするか。
        /// </summary>
        public bool OverwriteFlagForPartScreenColors
        {
            get { return _isOverwrittenPartScreenColors; }
            set { _isOverwrittenPartScreenColors = value; }
        }

        /// <summary>
        /// <see cref="MultiplyColor"/> のバックフィールド
        /// </summary>
        [SerializeField, HideInInspector]
        private Color _multiplyColor = Color.white;

        /// <summary>
        /// 乗算色
        /// </summary>
        public Color MultiplyColor
        {
            get
            {
                return _multiplyColor;
            }
            set
            {
                // 同じ値が与えられた場合、早めに返す
                if (value == _multiplyColor)
                {
                    return;
                }

                // 値を保存
                _multiplyColor = (value != null)
                    ? value
                    : Color.white;
            }
        }

        /// <summary>
        /// <see cref="ScreenColor"/> のバックフィールド
        /// </summary>
        [SerializeField, HideInInspector]
        private Color _screenColor = Color.clear;

        /// <summary>
        /// スクリーン色
        /// </summary>
        public Color ScreenColor
        {
            get
            {
                return _screenColor;
            }
            set
            {
                // 同じ値が与えられた場合、早めに返す
                if (value == _screenColor)
                {
                    return;
                }

                // 値を保存
                _screenColor = (value != null)
                    ? value
                    : Color.black;
            }
        }

        private void Start()
        {
            // 初期化
            _model = GetComponentInParent<CubismModel>();
            _renderController = GetComponentInParent<CubismRenderController>();
            _renderers = _renderController.Renderers;
            _part = GetComponent<CubismPart>();
            var drawables = _model.Drawables;

            // 要素を初期化
            _childDrawableRenderers = Array.Empty<CubismRenderer>();

            for (var j = 0; j < _renderers.Length; j++)
            {
                // このオブジェクトが親パーツであるとき
                if (drawables[j].ParentPartIndex == _part.UnmanagedIndex)
                {
                    // 対応するレンダラーを辞書に登録する
                    Array.Resize(ref _childDrawableRenderers, _childDrawableRenderers.Length + 1);
                    // 末尾からのインデックス演算子で[length - 1]を指すように
                    _childDrawableRenderers[^1] = _renderers[j];
                }
            }
        }
    }
}
