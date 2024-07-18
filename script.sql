create database consultorio;
GO

USE consultorio;
GO

CREATE TABLE medico(
	id int IDENTITY(1,1) NOT NULL,
	crm char(9) NOT NULL,
	nome varchar(100) NULL,
	CONSTRAINT pk_medico PRIMARY KEY (id),
	CONSTRAINT uk_medico_crm UNIQUE (crm)
);

CREATE TABLE paciente(
	codigo int IDENTITY(1,1) NOT NULL,
	nome varchar(100) NOT NULL,
	datanascimento date NOT NULL,
	CONSTRAINT pk_codigo PRIMARY KEY (codigo)
);

CREATE TABLE medicamento(
	id int IDENTITY(1,1) NOT NULL,
	nome varchar(100) NOT NULL,
	datafabricacao date NOT NULL,
	datavencimento date NULL,
	CONSTRAINT pk_medicamento PRIMARY KEY (id)
);

CREATE TABLE consulta(
	id int IDENTITY(1,1) NOT NULL,
	data datetime NULL,
	idmedico int NOT NULL,
	codigopaciente int NOT NULL,
 	CONSTRAINT pk_consulta PRIMARY KEY (id),
	CONSTRAINT fk_consulta_medico FOREIGN KEY (idmedico) references medico (id),
 	CONSTRAINT fk_consulta_paciente FOREIGN KEY (codigopaciente) references paciente (codigo)
);

CREATE TABLE usuario(
	id int IDENTITY(1,1) NOT NULL,
	nome varchar(100) NOT NULL,
	email varchar(100) NOT NULL,
	senha char(32) NOT NULL,
	CONSTRAINT pk_usuario PRIMARY KEY (id),
	CONSTRAINT uk_usuario_email UNIQUE (email)
);

CREATE TABLE especialidade(
	id int not null identity (1,1),
	nome varchar(100) not null,
	constraint pk_especialidade primary key (id)
);

CREATE TABLE medicoespecialidade(
	idmedico int NOT NULL,
	idespecialidade int NOT NULL,
	constraint pk_medicoespecialidade primary key (idmedico,idespecialidade),
	CONSTRAINT fk_medicoespecialidade_medico FOREIGN KEY (idmedico) references medico (id),
	CONSTRAINT fk_medicoespecialidade_especialidade FOREIGN KEY (idespecialidade) references especialidade (id),
);

insert into usuario (nome, email, senha) values ('Administrador', 'adm@adm.br', '202CB962AC59075B964B07152D234B70');