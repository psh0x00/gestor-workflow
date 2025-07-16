# API Endpoint Recommendations Based on Data Models

## 🔄 Adjusted Core Endpoints

### Workflow Templates
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET    | `/api/workflow-modelos` | List all workflow templates |
| POST   | `/api/workflow-modelos` | Create new template |
| GET    | `/api/workflow-modelos/{id}` | Get specific template |
| PUT    | `/api/workflow-modelos/{id}` | Update template |
| PUT    | `/api/workflow-modelos/{id}/versao` | Update version |
| POST   | `/api/workflow-modelos/{id}/ativar` | Activate template |
| POST   | `/api/workflow-modelos/{id}/desativar` | Deactivate template |
| GET    | `/api/workflow-modelos/{id}/transicoes` | Get template transitions |
| GET    | `/api/workflow-modelos/{id}/validar` | Validate template structure |

### State Templates
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET    | `/api/estados-modelo` | List all states |
| POST   | `/api/estados-modelo` | Create state |
| GET    | `/api/estados-modelo/{id}` | Get specific state |
| PUT    | `/api/estados-modelo/{id}` | Update state |
| PUT    | `/api/estados-modelo/{id}/cor` | Update state color |
| POST   | `/api/estados-modelo/{id}/ativar` | Activate state |
| POST   | `/api/estados-modelo/{id}/desativar` | Deactivate state |
| GET    | `/api/tipos-estado` | List state types |

### Transition Templates
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET    | `/api/workflow-modelos/{modeloId}/transicoes-modelo` | Get template transitions |
| POST   | `/api/workflow-modelos/{modeloId}/transicoes-modelo` | Create transition |
| GET    | `/api/transicoes-modelo/{id}` | Get specific transition |
| PUT    | `/api/transicoes-modelo/{id}` | Update transition |
| DELETE | `/api/transicoes-modelo/{id}` | Delete transition |
| GET    | `/api/transicoes-modelo/{id}/permissoes` | Get transition permissions |

## 📋 Instance Management

### Workflow Instances
| Method | Endpoint | Description |
|--------|----------|-------------|
| POST   | `/api/workflow-modelos/{modeloId}/instancias` | Create instance |
| GET    | `/api/workflow-instancias` | List all instances |
| GET    | `/api/workflow-instancias/{id}` | Get specific instance |
| PUT    | `/api/workflow-instancias/{id}/status` | Update instance status |
| GET    | `/api/workflow-instancias/{id}/transicoes-instancia` | Get instance history |
| POST   | `/api/workflow-instancias/{id}/executar-transicao` | Execute transition |
| GET    | `/api/workflow-instancias/por-status/{statusId}` | Filter by status |

### Instance Transitions
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET    | `/api/transicoes-instancia/{workflowInstanciaId}` | Get instance history |
| POST   | `/api/transicoes-instancia` | Register transition execution |
| GET    | `/api/transicoes-instancia/{id}` | Get transition details |

## 🔐 Permissions & Users

### User Management
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET    | `/api/utilizadores` | List all users |
| POST   | `/api/utilizadores` | Create user |
| GET    | `/api/utilizadores/{id}` | Get specific user |
| PUT    | `/api/utilizadores/{id}` | Update user |
| GET    | `/api/utilizadores/{id}/permissoes` | Get user permissions |
| POST   | `/api/utilizadores/{id}/permissoes/{permissaoId}` | Assign permission |
| DELETE | `/api/utilizadores/{id}/permissoes/{permissaoId}` | Remove permission |

### Permission Management
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET    | `/api/permissoes` | List all permissions |
| POST   | `/api/permissoes` | Create permission |
| GET    | `/api/permissoes/{id}` | Get specific permission |
| PUT    | `/api/permissoes/{id}` | Update permission |

## 🆕 Specialized Endpoints

### Status Management
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET    | `/api/status` | List all status types |
| GET    | `/api/workflow-instancias/por-status/{statusId}` | Get instances by status |

### Conditions Management
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET    | `/api/condicoes` | List all conditions |
| POST   | `/api/condicoes` | Create condition |
| GET    | `/api/pre-condicoes` | List pre-conditions |
| POST   | `/api/pre-condicoes` | Create pre-condition |
| PUT    | `/api/pre-condicoes/{id}` | Update pre-condition |
| POST   | `/api/pre-condicoes/{id}/testar` | Test SQL condition |
| POST   | `/api/pre-condicoes/{id}/ativar` | Activate pre-condition |
| POST   | `/api/pre-condicoes/{id}/desativar` | Deactivate pre-condition |

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET    | `/api/pos-condicoes` | List post-conditions |
| POST   | `/api/pos-condicoes` | Create post-condition |
| PUT    | `/api/pos-condicoes/{id}` | Update post-condition |
| POST   | `/api/pos-condicoes/{id}/testar` | Test SQL action |
| POST   | `/api/pos-condicoes/{id}/ativar` | Activate post-condition |
| POST   | `/api/pos-condicoes/{id}/desativar` | Deactivate post-condition |

### Authentication
| Method | Endpoint | Description |
|--------|----------|-------------|
| POST   | `/api/auth/register` | Register a new user |
| POST   | `/api/auth/login` | User login |

## 📊 Reporting Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET    | `/api/relatorios/criados-por/{utilizadorId}` | Workflows created by user |
| GET    | `/api/relatorios/alterados-por/{utilizadorId}` | Workflows modified by user |
| GET    | `/api/relatorios/execucoes-por-utilizador/{utilizadorId}` | Transitions executed by user |
| GET    | `/api/relatorios/tempo-medio-execucao/{modeloId}` | Average execution time |

## 🔍 Advanced Queries

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET    | `/api/workflow-modelos/{id}/instancias-ativas` | Active instances by template |
| GET    | `/api/estados-modelo/{id}/instancias-no-estado` | Instances in specific state |
| GET    | `/api/utilizadores/{id}/transicoes-executadas` | User's transition history |
| GET    | `/api/transicoes-modelo/{id}/historico-execucoes` | Transition execution history |

## Key Implementation Notes

1. **Many-to-Many Relationships**: Special endpoints for User-Permission management
2. **Audit Fields**: All models include creation/modification fields - API should expose this
3. **Status Entity**: Status is a separate table (not just enum) - requires specific endpoints
4. **Rich Navigation**: Models have bidirectional navigation - leverage for efficient queries
5. **NotMapped Fields**: Enums suggest API should handle both IDs and enum values
6. **History Structure**: TransitionInstance is a complete audit entity - needs specific reporting endpoints