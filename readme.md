# School Project

<h3 align="center">Projeto Web em ASP .NET Framework</h3>

---
<!--
##Topicos -->

### Como Executar

* Baixe o repositorio [School Project](https://github.com/GuilhermePalma/School_Project)
	- Via GIT: 
		- 1. Tenha o [GIT Instalado](https://git-scm.com/downloads)
		- 2. Inicie o GIT e Digite: `git clone https://github.com/GuilhermePalma/School_Project.git`
		- 3. Entre na Pasta do Projeto:  `cd School_Project`
	- Via GitHub:
		- 1. Acesse o [Repositorio no GitHub](https://github.com/GuilhermePalma/School_Project)
		- 2. Clique em **Code** e selecione a Opção **Download ZIP**
		- 3. Aguarde o Download Terminar e Acesse a Pasta do Projeto (School_Project)
* Acesse o Arquivo [Web.Config](Web.config) 
* Na Linha 10 (name="connection") insira no Campo **connectionString** os dados na seguinte Ordem:
	- Servidor: **Server=*seu_servidor_de_banco_de_dados*;**
	- Porta: **Port=*porta_do_seu_sgbd*;**
	- Usuraio: **UID=*usuario_do_banco_de_dados*;**
	- Senha: **Password=*senha_do_banco_de_dados*;**
	- Mantenha a Opção **'DATABASE=school_project' e 'SslMode=none'** para evitar Conflitos
	- Esse Projeto só funcionará no SGBD **MySql**
* Execute o [Script SQL](script.sql) no seu SGBD
	> Caso exista um Banco de Dados chamado de "school_project", ao executar o [Script SQL](script.sql) ele
	 será **EXCLUIDO**
* Inicie o [Visual Studio](https://visualstudio.microsoft.com/pt-br/) e Abra a Solução **[SchoolProject.sln](SchoolProject.sln)**

Obs: SGBD = Sistema de Gerenciamento de Banco de Dados

---

### Referencias

[Lista dos Estados](https://gist.github.com/quagliato/9282728)

[Icone DropDown Menu](https://pt.stackoverflow.com/questions/330840/bootstrap-3-dropdown-toggle-mudar-de-menu-com-ele-aberto-é-possível)
