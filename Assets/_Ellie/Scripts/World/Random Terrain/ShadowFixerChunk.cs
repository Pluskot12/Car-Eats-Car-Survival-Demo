using System;
using UnityEngine;
using UnityEngine.U2D;

namespace CarGame
{
    public class ShadowFixerChunk : MonoBehaviour
    {
        public SpriteShapeController shape;
        public float bottomHeight = -50f;

        public void GenerateBorder(TerrainChunk leftChunk, TerrainChunk rightChunk)
        {
            if (leftChunk == null || rightChunk == null)
            {
                Debug.LogError("Cannot generate border: one or both chunks are null");
                return;
            }

            SpriteShapeController leftShape = leftChunk.GetComponent<SpriteShapeController>();
            SpriteShapeController rightShape = rightChunk.GetComponent<SpriteShapeController>();

            if (leftShape == null || rightShape == null)
            {
                Debug.LogError("Cannot generate border: SpriteShapeController not found on chunks");
                return;
            }

            Spline leftSpline = leftShape.spline;
            Spline rightSpline = rightShape.spline;

            


            // Get the last two "upper" points from the left chunk
            int leftPointCount = leftSpline.GetPointCount();
            Vector3 start = leftChunk.transform.TransformPoint(leftSpline.GetPosition(leftPointCount-1));
            Vector3 leftPoint1 = leftChunk.transform.TransformPoint(leftSpline.GetPosition(leftPointCount - 3));
            Vector3 leftPoint2 = leftChunk.transform.TransformPoint(leftSpline.GetPosition(leftPointCount - 2));

            // Get the first two "upper" points from the right chunk
            Vector3 end = rightChunk.transform.TransformPoint(rightSpline.GetPosition(0));
            Vector3 rightPoint1 = rightChunk.transform.TransformPoint(rightSpline.GetPosition(1));
            Vector3 rightPoint2 = rightChunk.transform.TransformPoint(rightSpline.GetPosition(2));

            // Convert to local space of the border
            start = transform.InverseTransformPoint(start);
            leftPoint1 = transform.InverseTransformPoint(leftPoint1);
            leftPoint2 = transform.InverseTransformPoint(leftPoint2);
            rightPoint1 = transform.InverseTransformPoint(rightPoint1);
            rightPoint2 = transform.InverseTransformPoint(rightPoint2);
            end = transform.InverseTransformPoint(end);


            //shape.spline.InsertPointAt(0, new Vector3(leftTop.x, bottomY, 0));

            // Clear existing spline and create new border
            Spline borderSpline = shape.spline;
            borderSpline.Clear();

            // Add points to create the border shape
            borderSpline.InsertPointAt(0, new Vector3(leftPoint1.x, bottomHeight, 0));
            borderSpline.InsertPointAt(1, leftPoint1);
            borderSpline.InsertPointAt(2, leftPoint2);
            //borderSpline.InsertPointAt(3, rightPoint1);
            borderSpline.InsertPointAt(3, rightPoint2);
            borderSpline.InsertPointAt(4, new Vector3(rightPoint2.x, bottomHeight, 0)); // <---- This one sometimes bugs out?

            borderSpline.SetTangentMode(0, ShapeTangentMode.Linear);

            borderSpline.SetTangentMode(1, ShapeTangentMode.Broken);
            //borderSpline.SetLeftTangent(1, leftSpline.GetLeftTangent(leftPointCount - 3));
            borderSpline.SetLeftTangent(1, Vector3.zero);
            borderSpline.SetRightTangent(1, leftSpline.GetRightTangent(leftPointCount - 3));

            borderSpline.SetTangentMode(2,  ShapeTangentMode.Continuous);
            borderSpline.SetLeftTangent(2, leftSpline.GetLeftTangent(leftPointCount - 2));
            borderSpline.SetRightTangent(2, rightSpline.GetRightTangent(1));

            borderSpline.SetTangentMode(3, ShapeTangentMode.Broken);
            borderSpline.SetLeftTangent(3, rightSpline.GetLeftTangent(2));
            //borderSpline.SetRightTangent(3, rightSpline.GetRightTangent(2));
            borderSpline.SetRightTangent(3, Vector3.zero);

            borderSpline.SetTangentMode(4, ShapeTangentMode.Linear);

            shape.RefreshSpriteShape();
            shape.BakeMesh();
            shape.BakeCollider();
        }
    }
}
