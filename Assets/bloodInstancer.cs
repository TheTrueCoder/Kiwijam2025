using UnityEngine;

public class bloodInstancer : MonoBehaviour
{
    public GameObject blood;
    public Transform blood_pos;
    public void spawnblood()
    {
        Instantiate(blood,blood_pos);
    }
}
