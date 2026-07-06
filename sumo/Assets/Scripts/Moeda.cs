using UnityEngine;

public class Moeda : MonoBehaviour
{
    // Mudamos de OnTriggerEnter2D para OnCollisionEnter2D
    private void OnCollisionEnter2D(Collision2D colisao)
    {
        // Pega o colisor do objeto que bateu na moeda
        BolinhaController bolinha = colisao.collider.GetComponent<BolinhaController>();

        if (bolinha != null)
        {
            // Ativa os modificadores permanentes e acumulativos na bolinha
            bolinha.ColetarMoedaEvolutiva();
            
            // Destrói a moeda da arena
            Destroy(gameObject);
        }
    }
}