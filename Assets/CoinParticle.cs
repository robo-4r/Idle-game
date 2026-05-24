using UnityEngine;

public class CoinParticle : MonoBehaviour
{
    void Update()
    {
        if(this.gameObject.transform.position.y < -8)
        {
            Destroy(this.gameObject);
        }
    }
}
