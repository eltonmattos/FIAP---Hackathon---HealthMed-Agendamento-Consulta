# Health&Med - Agendamentos de Consultas Médicas

API Backend para consulta, cadastro e notificações de Agendamentos de Consultas Médicas.

## Requisitos


### Médico - Cadastro

- Cadastro do Usuário (Médico): O médico deverá poder se cadastrar, preenchendo os campos obrigatórios: Nome, CPF, Número CRM, E-mail e Senha.

- Autenticação do Usuário (Médico): O sistema deve permitir que o médico faça login usando o E-Mail e uma Senha.

- Cadastro/Edição de Horários Disponiveis (Médico): O sistema deve permitir que o médico faça o Cadastro e Edição de seus horários disponíveis para agendamento de consultas


### Paciente - Cadastro

- Cadastro do Usuário (Paciente): O paciente poderá se cadastrar, preenchendo os campos: Nome, CPF, E-mail e Senha.

- Autenticação do Usuário (Paciente): O sistema deve permitir que o paciente faça login usando o E-Mail e Senha.

### Paciente - Agendamento

- Busca por Médicos (Paciente): O sistema deve permitir que o paciente visualize a listagem dos médicos disponíveis

- Agendamento de Consultas (Paciente): Após selecionar o médico, o paciente deve poder visualizar as consultas com o médico com os horários disponíveis e efetuar o agendamento.
 
- O sistema deve ser capaz de suportar múltiplos acessos simultâneos e garantir que apenas uma marcação de consulta seja permitida para um determinado horário.

- O sistema deve validar a disponibilidade do horário selecionado em tempo real, assegurando que não haja sobreposição de horários para consultas agendadas.

### Notificação de consulta marcada (Médico): 

Após o agendamento, feito pelo usuário Paciente, o médico deverá receber um e-mail contendo:

Título do e-mail:
>ˮHealth&Med - Nova consulta agendadaˮ

Corpo do e-mail:

>ˮOlá, Dr. {nome_do_médico}!
>
>Você tem uma nova consulta marcada! Paciente: {nome_do_paciente}.
> 
>Data e horário: {data} às {horário_agendado}.ˮ

## Estrutura de Dados

![image](https://github.com/user-attachments/assets/e50eae1a-7205-43ba-a8d5-c4881b2d76d7)
