using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Kickable : MonoBehaviour
{

    static Dictionary<KeyCode, Vector3Int> kickKeys = new Dictionary<KeyCode, Vector3Int>
    {
        {KeyCode.UpArrow, Vector3Int.forward},
        {KeyCode.DownArrow, Vector3Int.back},
        {KeyCode.LeftArrow, Vector3Int.left},
        {KeyCode.RightArrow, Vector3Int.right},
    };
    private new Rigidbody rigidbody;
    public int kickForce = 50;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var keyCode in kickKeys.Keys)
        {
            if (Input.GetKeyDown(keyCode))
            {
                this.rigidbody.AddForce(kickForce * kickKeys[keyCode]);
            }   
        }
        
    }
}
