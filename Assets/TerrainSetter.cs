using UnityEngine;
using UnityEngine.U2D;

public class TerrainSetter : MonoBehaviour
{
    public SpriteShapeController shape;
    public float width = 2000;
    public float height = 20;


    [ContextMenu("Generate Terrain")]
    public void Generate() 
    {
        shape.spline.Clear();

        shape.spline.InsertPointAt(0, new Vector3(-width/2f, -20, 0));
        shape.spline.InsertPointAt(1, new Vector3(-width/2f, 0.686584f, 0));
        shape.spline.InsertPointAt(2, new Vector3(width/2f, 0.686584f, 0));
        shape.spline.InsertPointAt(3, new Vector3(width/2f, -20, 0));

        Debug.Log("bpin");
    }
}
