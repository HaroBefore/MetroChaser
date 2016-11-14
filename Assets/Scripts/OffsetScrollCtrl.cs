using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OffsetScrollCtrl : MonoBehaviour {

    private new Renderer renderer;
    public static float speed = 0f;
    public static float alpha = 0f;
    float offset;

	// Use this for initialization
	void Start () {
        renderer = GetComponent<Renderer>();
	}

    // Update is called once per frame
    void Update()
    {
        if(speed >= 0.05f)
        {
            offset += speed * Time.deltaTime;
            renderer.material.mainTextureOffset = new Vector2(offset, 0);
           
        }
    }
}
