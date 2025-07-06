namespace GestorWorkflow.Core.Interfaces;

public interface IMapper<TDomain, TData>
{
    TDomain MapToDomain(TData dataModel);
    TDomain MapToDomainWithDetails(TData dataModel); // se necessário
    TData MapToDataModel(TDomain domainModel);
    TData MapToDataModel(TDomain domainModel, int workflowModeloId);
    void MapToExistingDataModel(TDomain domainModel, TData dataModel);
}