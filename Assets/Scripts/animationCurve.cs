using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class animationCurve : MonoBehaviour
{
    public AnimationCurve curve;
    public float speed;
    public float handle;
    public List<float> biomass;
    public ParticleSystem ps;
    
    // Start is called before the first frame update
    void Start()
    {
        ps = GetComponent<ParticleSystem>();
        biomass = new List<float> { 1.4f, 5.6f, 2.3f, 8.5f, 5.8f, 1.6f, 4.8f };

        for(int i = 0; i < biomass.Count; i++)
        {
            Keyframe key = new Keyframe(i, biomass[i]);
            
            curve.AddKey(key);
            curve.SmoothTangents(i, 1f);
        }

    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale = Vector3.one * curve.Evaluate(handle);

        var size = ps.sizeOverLifetime;
        size.x = curve.Evaluate(handle);
        size.y = curve.Evaluate(handle);
        size.z = curve.Evaluate(handle);
    }
}
