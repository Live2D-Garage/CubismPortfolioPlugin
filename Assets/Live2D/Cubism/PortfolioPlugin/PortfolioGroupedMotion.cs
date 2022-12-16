/**
 * Copyright(c) Live2D Inc. All rights reserved.
 *
 * Use of this source code is governed by the MIT License.
 */

using System;
using UnityEngine;


namespace Live2D.Cubism.PortfolioPlugin
{
    [Serializable]
    public class PortfolioGroupedMotion
    {
        /// <summary>
        /// モーショングループ名
        /// </summary>
        [SerializeField] public string MotionGroupName;

        /// <summary>
        /// モーショングループ内のモーション一覧
        /// </summary>
        [SerializeField] public AnimationClip[] Motions;
    }
}
