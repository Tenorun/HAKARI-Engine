using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortraitDebugObject : MonoBehaviour
{
    public GameObject pm;

    SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    public int testInt1 = 0;
    public int testInt2 = 0;
    public bool testTrigger = false;
    void Update()
    {
        if (testTrigger)
        {
            testTrigger = false;
            spriteRenderer.sprite = pm.GetComponent<PortraitManager>().imageArray[testInt1, testInt2];
        }
    }
}
