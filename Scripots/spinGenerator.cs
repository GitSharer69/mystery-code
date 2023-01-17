
using UnityEngine;

public class spinGenerator : MonoBehaviour
{
    public Vector3 Rotate;
    void Update()
    {
        transform.Rotate(Rotate * Time.deltaTime);
    }
}
