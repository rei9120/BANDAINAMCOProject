using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLineRenderer : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;
	[SerializeField] private Material lineMaterial;
	private Vector3[] arrayPos = { Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero };
	private Vector3 startPos = Vector3.zero;
	private Vector3 nextPos = Vector3.zero;
	private Vector3 beforePos = Vector3.zero;
	private int linePosSize = 5;
	private bool drawLineFlag = false;

    public void Init()
    {
		if (lineRenderer != null)
		{
			lineRenderer.positionCount = linePosSize;
			// マテリアル設定
			if (lineMaterial != null)
			{
				lineRenderer.material = lineMaterial;
			}
			// 色設定
			{
				var colorKeys = new[]
				{
					new GradientColorKey( Color.red, 0 ),
					new GradientColorKey( Color.blue, 1 ),
				};
				var alphaKeys = new[]
				{
					new GradientAlphaKey( 1, 0 ),
					new GradientAlphaKey( 1, 0 ),
				};
				var gradient = new Gradient();
				gradient.SetKeys(colorKeys, alphaKeys);
				lineRenderer.colorGradient = gradient;
			}
			// 線の太さ。
			lineRenderer.startWidth = 0.1f;
			lineRenderer.endWidth = 0.1f;
		}
	}

    // Update is called once per frame
    public void ManagedUpdate(RaycastHit info)
    {
		Debug.Log("startPos = " + startPos);
		if (Input.GetMouseButton(0))
		{
			lineRenderer.positionCount = linePosSize;

			if (startPos == Vector3.zero)
			{
				startPos = info.point;
			}
			nextPos = info.point;
			nextPos.y = startPos.y;
			drawLineFlag = false;
		}
		else
		{
			startPos = Vector3.zero;
			drawLineFlag = true;
		}

		Debug.Log("nextPos = " + nextPos);

		if (drawLineFlag)
		{
			for (int i = 0; i < linePosSize; i++)
			{
				// 追加した頂点の座標を設定
				this.lineRenderer.SetPosition(i, arrayPos[i]);
			}
		}
		else
		{
			for (int i = 0; i < linePosSize; i++)
			{
				var clickPos = Vector3.zero;
				clickPos = nextPos;
				clickPos.y = startPos.y;
                switch (i)
                {
                    case 1:
                        clickPos.z = startPos.y;
                        break;
                    case 2:
                        break;
                    case 3:
                        clickPos.x = startPos.x;
                        break;
                    case 4:
                        clickPos.x = startPos.x;
                        clickPos.z = startPos.y;
                        break;
                    default:
                        clickPos.x = startPos.x;
                        clickPos.z = startPos.y;
                        break;
                }

                // 追加した頂点の座標を設定
                this.lineRenderer.SetPosition(i, clickPos);
				arrayPos[i] = clickPos;
			}
		}
	}

	public Vector3 GetStartLinePos()
	{
		if(arrayPos[0].x < arrayPos[2].x)
        {
			return arrayPos[0];
        }
		else
        {
			return arrayPos[2];
        }
	}

	public Vector3 GetEndLinePos()
    {
		if (arrayPos[0].x < arrayPos[2].x)
		{
			return arrayPos[2];
		}
		else
		{
			return arrayPos[0];
		}
	}

	public bool GetDrawLineFlag()
    {
		return drawLineFlag;
    }
}