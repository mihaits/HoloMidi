using UnityEngine;

public class SmoothRotation : MonoBehaviour
{
    private int _frames = 10;
    public int Frames = 10;

    private Quaternion[] _buffer;
    private int _index;

    public void Start()
    {
        _buffer = new Quaternion[Frames];
        for (var i = 0; i < Frames; ++ i)
            _buffer[i] = Quaternion.identity;
    }

    public void Update()
    {
        _index = (_index + 1) % Frames;
        _buffer[_index] = transform.parent.rotation;

        transform.rotation = Avg();
    }

    private Quaternion Avg()
    {
        var qAvg = Quaternion.identity;
        var averageWeight = 1f / Frames;

        for (var i = 0; i < Frames; ++i)
        {
            qAvg *= Quaternion.Slerp(Quaternion.identity, _buffer[i], averageWeight);
        }

        return qAvg;
    }

    public void OnValidate()
    {
        if (_frames != Frames)
        {
            _buffer = new Quaternion[Frames];
            for (var i = 0; i < Frames; ++i)
                _buffer[i] = Quaternion.identity;

            _frames = Frames;
        }
    }
}
