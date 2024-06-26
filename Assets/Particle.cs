using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particle : MonoBehaviour
{
    //消滅するまでの時間
    private float LifeTime;
    //消滅するまでの残り時間
    private float LeftLifeTime;
    //移動量
    private Vector3 velocity;
    //初期Scale
    private Vector3 defaultScale;

    // Start is called before the first frame update
    void Start()
    {
        //
        LifeTime = 0.3f;
        //
        LeftLifeTime = LifeTime;
        //
        defaultScale=transform.localScale;
        //
        float maxVelocity = 5;

        velocity = new Vector3
            (
            Random.Range(-maxVelocity, maxVelocity),
            Random.Range(-maxVelocity, maxVelocity),
            Random.Range(-maxVelocity, maxVelocity)
            );
    }

    // Update is called once per frame
    void Update()
    {
        LeftLifeTime -= Time.deltaTime;
        //
        transform.position += velocity * Time.deltaTime;
        //
        transform.localScale = Vector3.Lerp
            (
            new Vector3(0, 0, 0),
            defaultScale,
            LeftLifeTime / LifeTime
            );
        //
        if (LeftLifeTime <= 0) { Destroy(gameObject); }
    }
}
