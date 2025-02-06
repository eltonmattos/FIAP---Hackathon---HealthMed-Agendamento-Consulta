USE [HealthMedAgendamento]
GO

/****** Object:  Table [dbo].[Medico]    Script Date: 04/02/2025 17:46:11 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Medico](
	[Id] [varchar](36) NOT NULL,
	[Nome] [nvarchar](255) NOT NULL,
	[CPF] [varchar](11) NOT NULL,
	[CRM] [varchar](50) NOT NULL,
	[Email] [varchar](256) NOT NULL,
	[Senha] [binary](50) NOT NULL,
	[DuracaoConsulta] [int] NULL,
	[ValorConsulta] [money] NULL,
 CONSTRAINT [PK_Medico] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[Paciente](
	[Id] [varchar](36) NOT NULL,
	[Nome] [nvarchar](255) NOT NULL,
	[CPF] [varchar](11) NOT NULL,
	[Email] [varchar](256) NOT NULL,
	[Senha] [binary](50) NOT NULL,
 CONSTRAINT [PK_Paciente] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[DisponibilidadeMedico](
	[Id] [varchar](36) NOT NULL,
	[DiaSemana] [int] NULL,
	[InicioPeriodo] [time](7) NULL,
	[FimPeriodo] [time](7) NULL,
	[Validade] [datetime] NULL,
	[IdMedico] [varchar](36) NULL,
 CONSTRAINT [PK_NewTable] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[DisponibilidadeMedico]  WITH CHECK ADD  CONSTRAINT [FK_DisponibilidadeMedico_Medico] FOREIGN KEY([IdMedico])
REFERENCES [dbo].[Medico] ([Id])
GO

ALTER TABLE [dbo].[DisponibilidadeMedico] CHECK CONSTRAINT [FK_DisponibilidadeMedico_Medico]
GO


CREATE TABLE [dbo].[Agendamento](
	[Id] [varchar](36) NOT NULL,
	[DataInicio] [datetime] NULL,
	[DataFim] [datetime] NULL,
	[IdMedico] [varchar](36) NULL,
	[IdPaciente] [varchar](36) NOT NULL,
	[Status] [int] NULL,
	[ValorConsulta] [money] NULL,
	[MotivoCancelamento] [nvarchar](2048) NULL,
 CONSTRAINT [PK_Agendamento] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Agendamento]  WITH CHECK ADD  CONSTRAINT [FK_Agendamento_Medico] FOREIGN KEY([IdMedico])
REFERENCES [dbo].[Medico] ([Id])
GO

ALTER TABLE [dbo].[Agendamento] CHECK CONSTRAINT [FK_Agendamento_Medico]
GO

ALTER TABLE [dbo].[Agendamento]  WITH CHECK ADD  CONSTRAINT [FK_Agendamento_Paciente] FOREIGN KEY([IdPaciente])
REFERENCES [dbo].[Paciente] ([Id])
GO

ALTER TABLE [dbo].[Agendamento] CHECK CONSTRAINT [FK_Agendamento_Paciente]
GO



CREATE TABLE [dbo].[rel_Especialidades_Medico](
	[IdMedico] [nvarchar](36) NOT NULL,
	[IdEspecialidade] [int] NOT NULL
) ON [PRIMARY]
GO



CREATE TABLE [dbo].[Especialidades](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Nome] [nvarchar](255) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO


