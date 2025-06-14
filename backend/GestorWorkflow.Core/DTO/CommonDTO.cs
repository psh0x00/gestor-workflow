namespace GestorWorkflow.Core.DTO;

// ==============================
// RESPONSES E UTILITÁRIOS
// ==============================

public class ResultadoValidacaoResponse
{
    public bool EhValido { get; set; }
    public List<string> Erros { get; set; } = new();
    public List<string> Avisos { get; set; } = new();
}

public class ResultadoOperacaoResponse<T>
{
    public bool Sucesso { get; set; }
    public string? Mensagem { get; set; }
    public T? Dados { get; set; }
    public List<string> Erros { get; set; } = new();

    public static ResultadoOperacaoResponse<T> ComSucesso(T dados, string? mensagem = null)
    {
        return new ResultadoOperacaoResponse<T>
        {
            Sucesso = true,
            Dados = dados,
            Mensagem = mensagem
        };
    }

    public static ResultadoOperacaoResponse<T> ComErro(string erro)
    {
        return new ResultadoOperacaoResponse<T>
        {
            Sucesso = false,
            Erros = new List<string> { erro }
        };
    }

    public static ResultadoOperacaoResponse<T> ComErros(List<string> erros)
    {
        return new ResultadoOperacaoResponse<T>
        {
            Sucesso = false,
            Erros = erros
        };
    }
}

public class ResultadoPaginadoResponse<T>
{
    public List<T> Dados { get; set; } = new();
    public int TotalItens { get; set; }
    public int PaginaAtual { get; set; }
    public int TamanhoPagina { get; set; }
    public int TotalPaginas => (int)Math.Ceiling((double)TotalItens / TamanhoPagina);
    public bool TemPaginaAnterior => PaginaAtual > 1;
    public bool TemProximaPagina => PaginaAtual < TotalPaginas;
}