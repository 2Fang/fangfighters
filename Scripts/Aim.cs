using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aim : MonoBehaviour
{

    Rigidbody2D aimBar;

    [SerializeField] string RS_h = "RS_h";
    [SerializeField] string RS_v = "RS_v";
    [SerializeField] string RS_B = "RS_B";

    float x;
    float y;


    // Start is called before the first frame update
    void Start()
    {
        aimBar = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        y = Input.GetAxis(RS_v);
        x = Input.GetAxis(RS_h);
        aimBar.rotation = Mathf.Rad2Deg * Mathf.Atan(y / x);
        if (x < 0)
            aimBar.rotation -= 180;
        if (x == 0f && y < 0f)
            aimBar.rotation = 270;
    }

    public bool CheckButton(string button)
    {
        return button == RS_B;
    }

}
