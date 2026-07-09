using UnityEngine;

public class Moeda : MonoBehaviour
{
    // 1. TESTE VIA TRIGGER (Gatilho - com 'Is Trigger' marcado)
    private void OnTriggerEnter2D(Collider2D colisor)
    {
        // Força uma mensagem no console para qualquer coisa que encostar
        Debug.Log($"[GATILHO] Algo entrou na moeda: {colisor.gameObject.name}");

        // Tenta pegar o componente do jogador direto, sem ligar para o nome
        BolinhaController bolinha = colisor.GetComponent<BolinhaController>();

        if (bolinha != null)
        {
            Debug.Log("[SUCESSO] Jogador identificado via Trigger! Aplicando status...");
            bolinha.ColetarMoedaEvolutiva();
            Destroy(gameObject);
        }
    }

    // 2. TESTE VIA COLLISION (Física rígida - caso o 'Is Trigger' esteja desativado sem querer)
    private void OnCollisionEnter2D(Collision2D colisao)
    {
        Debug.Log($"[COLISÃO FÍSICA] Algo bateu na moeda: {colisao.gameObject.name}");

        BolinhaController bolinha = colisao.gameObject.GetComponent<BolinhaController>();

        if (bolinha != null)
        {
            Debug.Log("[SUCESSO] Jogador identificado via Colisão Física! Aplicando status...");
            bolinha.ColetarMoedaEvolutiva();
            Destroy(gameObject);
        }
    }
}