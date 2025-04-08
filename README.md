# 🛠️ Gestor de Workflows

Ferramenta de visualização e gestão de workflows personalizados, baseada em máquinas de estado, com integração numa aplicação web.

## 📌 Objetivo

Desenvolver um componente para aplicações web que permita a criação, visualização e gestão de workflows dinâmicos, incluindo:

- Definição de **estados**
- Definição de **transições** entre estados
- Controlo de **permissões**
- Definição de **ações**, **pré-condições** e **pós-condições** associadas a cada estado

## 🚀 Funcionalidades principais (previstas)

- Editor visual de workflows
- Sistema de permissões por papel/utilizador
- Visualização interativa das máquinas de estado
- Histórico de transições e ações
- Integração com base de dados SQL Server
- API RESTful desenvolvida em .NET Core

## 🧱 Estrutura do Projeto

```
gestor-workflow/
│
├── docs/                        # Documentação do projeto
│   ├── requisitos/              # Especificação de requisitos
│   └── relatórios/              # Relatório técnico e manual de utilização
│
├── backend/                     # API em C# .NET Core
│   ├── GestorWorkflow.API/     # Projeto principal
│   ├── GestorWorkflow.Core/    # Lógica de negócio (máquina de estados, permissões, etc.)
│   └── GestorWorkflow.Data/    # Acesso a dados (SQL Server)
│
├── frontend/                    # Interface Web
│   ├── react/                   # React
│   └── shared/                 # Componentes partilhados (ex: visualizador de estados)
│
├── tests/                       # Testes automáticos
│   ├── backend/                # Testes unitários e de integração (API)
│   └── frontend/               # Testes de UI (ex: Cypress, Jest)
│
├── .gitignore                   # Ignorar ficheiros desnecessários
├── README.md                    # Explicação geral do projeto
└── LICENSE                      # Licença de uso (ex: MIT)
```

## 🛠️ Tecnologias

- **Backend**: C# .NET Core
- **Frontend**: React
- **Base de Dados**: SQL Server
- **Outras libs**: DevExtreme, outras a avaliar durante o projeto

## 📅 Etapas do Projeto

1. Pesquisa e análise de ferramentas semelhantes
2. Levantamento e especificação de requisitos
3. Modelação do componente e estrutura de dados
4. Implementação e testes do backend e frontend
5. Integração numa aplicação web
6. Elaboração de relatório técnico e manual de utilização

## 👥 Equipa

Projeto desenvolvido no âmbito da unidade curricular **Projeto IV** (Engenharia Informática, IPVC), em colaboração com a empresa **Coollink**.

## 📄 Licença

Este projeto está licenciado sob a licença [MIT](LICENSE).
