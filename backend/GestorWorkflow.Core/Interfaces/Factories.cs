using GestorWorkflow.Core.DTO;
using GestorWorkflow.Core.Entities;
using GestorWorkflow.Core.Enums;

namespace GestorWorkflow.Core.Interfaces;

public interface IEstadoFactory
{
    EstadoEntity CriarEstado(CriarEstadoDTO dto, int id);
    EstadoDTO CriarEstadoDTO(EstadoEntity estadoEntity);
    IEnumerable<EstadoDTO> CriarEstadoDTOs(IEnumerable<EstadoEntity> estados);
    void AtualizarEstado(EstadoEntity estadoEntity, AtualizarEstadoDTO dto);
}

public interface ITransicaoFactory
{
    TransicaoEntity CriarTransicao(CriarTransicaoDTO dto, int id);
    TransicaoDTO CriarTransicaoDTO(TransicaoEntity transicaoEntity);
    IEnumerable<TransicaoDTO> CriarTransicaoDTOs(IEnumerable<TransicaoEntity> transicoes);
    void AtualizarTransicao(TransicaoEntity transicaoEntity, AtualizarTransicaoDTO dto);
}

public interface IWorkflowModeloFactory
{
    WorkflowModeloEntity CriarWorkflowModelo(CriarWorkflowModeloDTO dto, int id);
    WorkflowModeloDTO CriarWorkflowModeloDTO(WorkflowModeloEntity workflowModeloEntity);
    IEnumerable<WorkflowModeloDTO> CriarWorkflowModeloDTOs(IEnumerable<WorkflowModeloEntity> workflowModelos);
    void AtualizarWorkflowModelo(WorkflowModeloEntity workflowModeloEntity, AtualizarWorkflowModeloDTO dto);
}

public interface IWorkflowInstanciaFactory
{
    WorkflowInstanciaEntity CriarWorkflowInstancia(CriarWorkflowInstanciaDTO dto, int id);
    WorkflowInstanciaDTO CriarWorkflowInstanciaDTO(WorkflowInstanciaEntity workflowInstanciaEntity);
    IEnumerable<WorkflowInstanciaDTO> CriarWorkflowInstanciaDTOs(IEnumerable<WorkflowInstanciaEntity> workflowInstancias);
    void AtualizarWorkflowInstancia(WorkflowInstanciaEntity workflowInstanciaEntity, AtualizarWorkflowInstanciaDTO dto);
}

public interface IUtilizadorFactory
{
    UtilizadorEntity CriarUtilizador(CriarUtilizadorDTO dto, int id);
    UtilizadorDTO CriarUtilizadorDTO(UtilizadorEntity utilizadorEntity);
    IEnumerable<UtilizadorDTO> CriarUtilizadorDTOs(IEnumerable<UtilizadorEntity> utilizadores);
    void AtualizarUtilizador(UtilizadorEntity utilizadorEntity, AtualizarUtilizadorDTO dto);
}

public interface IPermissaoFactory
{
    PermissaoEntity CriarPermissao(CriarPermissaoDTO dto, int id);
    PermissaoDTO CriarPermissaoDTO(PermissaoEntity permissaoEntity);
    IEnumerable<PermissaoDTO> CriarPermissaoDTOs(IEnumerable<PermissaoEntity> permissoes);
    void AtualizarPermissao(PermissaoEntity permissaoEntity, AtualizarPermissaoDTO dto);
}

public interface IPreCondicaoFactory
{
    PreCondicaoEntity CriarPreCondicao(CriarPreCondicaoDTO dto, int id);
    PreCondicaoDTO CriarPreCondicaoDTO(PreCondicaoEntity preCondicaoEntity);
    IEnumerable<PreCondicaoDTO> CriarPreCondicaoDTOs(IEnumerable<PreCondicaoEntity> preCondicoes);
    void AtualizarPreCondicao(PreCondicaoEntity preCondicaoEntity, AtualizarPreCondicaoDTO dto);
}

public interface IPosCondicaoFactory
{
    PosCondicaoEntity CriarPosCondicao(CriarPosCondicaoDTO dto, int id);
    PosCondicaoDTO CriarPosCondicaoDTO(PosCondicaoEntity posCondicaoEntity);
    IEnumerable<PosCondicaoDTO> CriarPosCondicaoDTOs(IEnumerable<PosCondicaoEntity> posCondicoes);
    void AtualizarPosCondicao(PosCondicaoEntity posCondicaoEntity, AtualizarPosCondicaoDTO dto);
}