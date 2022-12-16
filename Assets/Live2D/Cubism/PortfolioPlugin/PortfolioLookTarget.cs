/**
 * Copyright(c) Live2D Inc. All rights reserved.
 *
 * Use of this source code is governed by the MIT License.
 */

using Live2D.Cubism.Framework.LookAt;
using UnityEngine;

namespace Live2D.Cubism.PortfolioPlugin
{
    /// <summary>
    /// 視線追従させる座標
    /// </summary>
    public class PortfolioLookTarget : MonoBehaviour, ICubismLookTarget
    {
        /// <summary>
        /// 視線追従用の座標を取得
        /// </summary>
        /// <returns>ドラッグ中のカーソルの座標（ドラッグしていない場合はPrefabの位置）</returns>
        public Vector3 GetPosition()
        {
            if (!Input.GetMouseButton(0))
            {
                return gameObject.transform.position;
            }

            var targetPosition = Input.mousePosition;

            targetPosition = (Camera.main.ScreenToViewportPoint(targetPosition) * 2) - Vector3.one;

            return targetPosition;
        }

        /// <summary>
        /// 視線追従する期間
        /// </summary>
        /// <returns>視線追従する期間（このコンポーネントでは常に追従させる）</returns>
        public bool IsActive()
        {
            return true;
        }
    }
}
