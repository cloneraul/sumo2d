using UnityEngine;

public class Moeda : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D colisor)
    {
        Debug.Log($"[MOEDA] Objeto colidiu com a moeda: {colisor.gameObject.name}");

        if (colisor.gameObject.name == "Teste_J1" || colisor.gameObject.name == "Teste_J2")
        {
            BolinhaController bolinha = colisor.GetComponent<BolinhaController>();

            if (bolinha != null)
            {
                bolinha.ColetarMoedaEvolutiva();
                Destroy(gameObject);
            }
        }
    }
}