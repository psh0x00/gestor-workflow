# 🛠️ Workflow Engine

![C#](https://img.shields.io/badge/c%23-%23239120.svg?style=for-the-badge&logo=c-sharp&logoColor=white)
![.Net](https://img.shields.io/badge/.NET-5C2D91?style=for-the-badge&logo=.net&logoColor=white)
![React](https://img.shields.io/badge/react-%2320232a.svg?style=for-the-badge&logo=react&logoColor=%2361DAFB)
![SQL Server](https://img.shields.io/badge/SQLServer-CC292B?style=for-the-badge&logo=microsoft-sql-server&logoColor=white)

A visual tool and management engine for custom workflows, built on finite state machines (FSM), integrated into a web application.

## 📌 Objective

To develop a robust component for web applications that allows the creation, visualization, and management of dynamic workflows, including:

- Definition of **states**
- Definition of **transitions** between states
- Granular **permissions** control
- Definition of **actions**, **pre-conditions**, and **post-conditions** associated with each state transition

## 🚀 Key Features

- Visual workflow editor
- Role/user-based permission system
- Interactive visualization of state machines
- Transition and action history logging
- Seamless integration with a SQL Server database
- N-tier RESTful API built with C# .NET Core

## 🧱 Project Structure

```
gestor-workflow/
│
├── docs/                        # Project documentation
│   ├── requisitos/              # Requirements specification
│   └── relatórios/              # Technical reports and user manual
│
├── backend/                     # C# .NET Core API
│   ├── GestorWorkflow.API/     # Main API project
│   ├── GestorWorkflow.Core/    # Business logic (state machine, permissions, etc.)
│   └── GestorWorkflow.Data/    # Data access layer (SQL Server)
│
├── frontend/                    # Web Interface
│   ├── react/                   # React application
│   └── shared/                 # Shared components (e.g. state visualizer)
│
├── tests/                       # Automated tests
│   ├── backend/                # Unit and integration tests (API)
│   └── frontend/               # UI tests (e.g. Cypress, Jest)
│
├── .gitignore                   # Ignore unnecessary files
├── README.md                    # Project overview (this file)
└── LICENSE                      # License (e.g. MIT)
```

## 🛠️ Technologies

- **Backend**: C# .NET Core
- **Frontend**: React
- **Database**: SQL Server
- **UI Components**: DevExtreme

## 💻 How to Run (Development)

### Backend Setup (.NET Core)
1. Ensure you have the [.NET SDK](https://dotnet.microsoft.com/download) installed.
2. Update the SQL Server connection string in `backend/GestorWorkflow.API/appsettings.json`.
3. Apply database migrations:
   ```bash
   cd backend/GestorWorkflow.API
   dotnet ef database update
   ```
4. Run the API:
   ```bash
   dotnet run
   ```
   The API will typically be available at `https://localhost:5001` or `http://localhost:5000`.

### Frontend Setup (React)
1. Ensure you have [Node.js](https://nodejs.org/) installed.
2. Install dependencies and start the development server:
   ```bash
   cd frontend/react
   npm install
   npm start
   ```
   The frontend will be available at `http://localhost:3000`.

## 📅 Project Stages

1. Research and analysis of similar tools
2. Requirements gathering and specification
3. Component and data structure modeling
4. Backend and frontend implementation and testing
5. Web application integration
6. Technical report and user manual elaboration

## 👥 Team

Project developed within the scope of the **Project IV** course (Informatics Engineering, IPVC).

## 📄 License

This project is licensed under the [MIT License](LICENSE).
