# Health&Med - API de Agendamento de Consultas

## Visão Geral

Este projeto é uma aplicação ASP .NET Core, composta de uma API com funcionalidades para que médicos possam cadastrar horários onde estão disponíveis para agendar consultas, e para que os pacientes possam localizar esses médicos e realizar o agendamento das consultas considerando os horários disponíveis para cada médico.

- Acesso: http://128.203.145.89:8080/
- Definições da API: [HealthMed API.postman_collection.json.zip](https://github.com/user-attachments/files/18722502/HealthMed.API.postman_collection.json.zip)

## Estrutura da API

![azure-Página-2 drawio](https://github.com/user-attachments/assets/b279ea2a-b35c-4e10-907b-e39ef29458e5)

- **Autenticação**
   - `POST /auth/LoginMedico` → Login do médico, usando o CRM
   - `POST /auth/LoginPaciente` → Login do paciente, usando o endereço de E-Mail
- **Médico**
   - `GET /api/Medico/{idMedico}` → Buscar informações de um médico
   - `GET /api/Medico` → Buscar médicos filtrando por especialidade, estado ou CRM
   - `POST /api/Medico` → Cadastrar um novo médico
- **Paciente**
   - `GET /api/Paciente/{idPaciente}` → Buscar informações de um paciente
   - `POST /api/Paciente` → Cadastrar um novo paciente
- **Disponibilidade do Médico**
   - `GET /api/DisponibilidadeMedico/{idMedico}` → Obter agenda de disponibilidade de um médico
   - `POST /api/DisponibilidadeMedico` → Cadastrar disponibilidade de médicos
   - `POST /api/DisponibilidadeMedico/{idMedico}` → Cadastrar disponibilidade de médicos, com apoio de inteligência artificial (utilizando o modelo gemini-2.0-flash)
   - `PUT /api/DisponibilidadeMedico/{idMedico}/{idDisponibilidadeMedico}` → Atualizar bloco de disponibilidade
- **Agendamentos**
   - `GET /api/Agendamento/{idMedico}` → Listar agendamentos de um médico
   - `GET /api/Agendamento/{idMedico}/{Data}` → Buscar agendamentos em uma data específica
   - `POST /api/Agendamento` → Criar um novo agendamento
   - `PUT /api/Agendamento/AprovarAgendamento/{idMedico}` → Aprovar um agendamento
   - `PUT /api/Agendamento/RecusarAgendamento/{idMedico}` → Recusar um agendamento
   - `PUT /api/Agendamento/CancelarAgendamento/{idMedico}` → Cancelar um agendamento

## Estrutura do Projeto

![azure drawio](https://github.com/user-attachments/assets/e8664873-1a87-48cb-9bb8-3e53145caadf)

## Persistência de Dados

![027b1b4b-061d-435d-a617-b22b0440415e](https://github.com/user-attachments/assets/9c4bb09b-a20e-4235-a0d2-556dbb608790)

Usamos o Dapper para alta performance e simplicidade na manipulação de dados, permitindo consultas rápidas e eficientes, com baixo overhead, em um ambiente seguro e escalável. Isso melhora a velocidade e a produtividade do desenvolvimento.

API está hospedada no Azure SQL, que oferece escalabilidade, alta disponibilidade, segurança robusta, integração simplificada com outras ferramentas do Azure. Essa combinação melhora o desempenho e a eficiência da API.

## Testes

O projeto inclui testes unitários para garantir a qualidade e a funcionalidade das APIs. 

## Serviço de Notificação

O projeto inclui o Azure Communication Services para disparo de Emails, que oferece integração fácil com outros serviços Azure, alta escalabilidade, segurança robusta e capacidade de rastreamento e análise de métricas, melhorando a eficiência e o gerenciamento das comunicações por email.
- Email para notificar o médico sobre uma nova solicitação de agendamento
- Email para notificar o médico sobre o cancelamento de uma solicitação
- Email para notificar o paciente sobre aprovação/recusa de agendamento

## Orquestração e Gerenciamento de Containeres

Este projeto utiliza **Azure Kubernetes Services (AKS)** para gerenciar a implantação, escalabilidade e resiliência dos microsserviços. o Azure Kubernetes Services (AKS) oferece gerenciamento simplificado, integração com Azure, alta segurança, escalabilidade automática e economia de tempo e custos. 

- Implementamos **ReplicaSets** para garantir que o número necessário de réplicas da API esteja sempre em execução, aumentando a disponibilidade.
- **Deployments** são usados para gerenciar as atualizações da API de maneira declarativa e segura, garantindo alta disponibilidade durante mudanças.
- **Services** foram configurados para garantir a disponibilidade da API e a descoberta automática dos mesmos no cluster.

## Automatização e Build

Criamos dois workflows no GitHub Actions para isolar os testes em dev, e construção e publicação das imagens no DockerHub e no AKS:

1. Build & Test: O workflow do GitHub Actions realiza as seguintes etapas:
   - **Disparadores:** Executa em push ou pull request na branch `dev`.
   - **Job de Build:**
      - Clona o repositório.
      - Configura o .NET Core na versão 8.0.x.
      - Instala as dependências com `dotnet restore`.
      - Constrói o projeto em modo Release sem restaurar as dependências novamente.
      - Executa os testes no modo Release.
1. Deploy:  O workflow do GitHub Actions  realiza as seguintes etapas:
   - **Disparadores:** Executa em push na branch `master` ou manualmente.
   - **Job de Build:**
      - Clona o repositório.
      - Constrói e atualiza a imagem healthmed_agendamento para o Docker Hub.
   - **Job de Deploy:**
      - Aplica os arquivos `deployment.yaml` e `service.yaml` no Kubernetes (AKS), realizando o deploy da imagem atualizada para o Azure Kubernetes Services.

