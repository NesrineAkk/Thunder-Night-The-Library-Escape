using UnityEngine;
using UnityEngine.SceneManagement; 

public class NewEmptyCSharpScript : MonoBehaviour 
{
    public void LancerMario()
    {
        Loader.Load(Loader.Scene.SampleScene);
    }
}