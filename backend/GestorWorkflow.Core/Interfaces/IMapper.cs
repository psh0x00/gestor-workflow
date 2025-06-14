namespace GestorWorkflow.Core.Interfaces;

public interface IMapper<TDomain, TData>
{
    TDomain MapToDomain(TData dataModel);
    TDomain MapToDomainWithDetails(TData dataModel); // se necessário
    TData MapToDataModel(TDomain domainModel);
    void MapToExistingDataModel(TDomain domainModel, TData dataModel);
}