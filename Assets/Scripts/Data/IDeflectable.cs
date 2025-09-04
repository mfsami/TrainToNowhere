using UnityEngine;

public interface IDeflectable
{
    public void Deflect(Vector3 direction);
    public float ReturnSpeed {get; set;}
}
