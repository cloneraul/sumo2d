using UnityEngine;

public class DadosGlobais : MonoBehaviour
{
    public static DadosGlobais Instancia;

    public int tipoEscolhidoJ1 = 0;
    public int tipoEscolhidoJ2 = 0;

    private void Awake()
    {
        if (Instancia == null)
        {
            Instancia = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
