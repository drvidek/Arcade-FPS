using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skybox : MonoBehaviour
{
    [SerializeField] private float _scale = 1;
    // Start is called before the first frame update
    void Start()
    {
        var mat = GetComponent<MeshRenderer>().material;
        mat.mainTextureOffset += new Vector2(Random.Range(-5f, 5f), 0);
        mat.mainTextureScale = Vector2.one * _scale;
    }

}
