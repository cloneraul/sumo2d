using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuSelecaoController : MonoBehaviour
{
    public Button botaoIniciarJogo;

    private void Start()
    {
        if (botaoIniciarJogo != null)
        {
            botaoIniciarJogo.interactable = false;
        }
    }

    public void DefinirTipoJ1(int idTipo)
    {
        DadosGlobais.Instancia.tipoEscolhidoJ1 = idTipo;
        ValidarSelecao();
    }

    public void DefinirTipoJ2(int idTipo)
    {
        DadosGlobais.Instancia.tipoEscolhidoJ2 = idTipo;
        ValidarSelecao();
    }

    private void ValidarSelecao()
    {
        if (DadosGlobais.Instancia.tipoEscolhidoJ1 > 0 && DadosGlobais.Instancia.tipoEscolhidoJ2 > 0)
        {
            if (botaoIniciarJogo != null)
            {
                botaoIniciarJogo.interactable = true;
            }
        }
        else
        {
            if (botaoIniciarJogo != null)
            {
                botaoIniciarJogo.interactable = false;
            }
        }
    }

    public void CarregarGameplay()
    {
        SceneManager.LoadScene("Gameplay");
    }
}
