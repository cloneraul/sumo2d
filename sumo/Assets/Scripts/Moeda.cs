using UnityEngine;

public class Moeda : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D colisor)
    {
        BolinhaController bolinha = colisor.GetComponent<BolinhaController>();

        if (bolinha != null)
        {
            // Ativa os modificadores permanentes na bolinha
            bolinha.ColetarMoedaEvolutiva();
            
            // Destrói a moeda da arena
            Destroy(gameObject);
        }
    }
}