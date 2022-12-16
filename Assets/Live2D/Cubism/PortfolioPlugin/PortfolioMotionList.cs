/**
 * Copyright(c) Live2D Inc. All rights reserved.
 *
 * Use of this source code is governed by the MIT License.
 */

using UnityEngine;

namespace Live2D.Cubism.PortfolioPlugin
{
    [CreateAssetMenu(menuName = "Live2D Cubism/Portfolio/Motion list")]
    public class PortfolioMotionList : ScriptableObject
    {
        [SerializeField] public PortfolioGroupedMotion[] Motions;
    }
}
