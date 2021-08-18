#School Project

<br />
<h4 align="center">Projeto em ASP .NET Framework desenvolvido para a Aula de PW (Programação Web)</h4>

Este Projeto está em Desevolvimento. **Pode haver erros** durante a Execução do Codigo.

### Como Executar
- [ ] - Baixe o repositorio [School_Project](https://github.com/GuilhermePalma/School_Project)
	- Via GIT: 
		- 1. Tenha o [GIT Instalado](https://git-scm.com/downloads)
		- 2. Inicie o GIT e Digite: "https://github.com/GuilhermePalma/School_Project.git"
		- 3. Entre na Pasta do Projeto: "cd School_Project"
	- Via GitHub:
		- 1. Acesse o [Repositorio no GitHub](https://github.com/GuilhermePalma/School_Project)
		- 2. Clique em **Code** e selecione a Opção **Download ZIP**
		- 3. Aguarde o Download Terminar e Acesse a Pasta do Projeto (School_Project)
- [ ] - Acesse o Arquivo [Web.Config](web.config) 
- [ ] - Navegue na Linha 10 (name="connection") e insira no Campo **connectionString** os dados na seguinte Ordem:
	- Servidor: **Server=*seu_servidor_de_banco_de_dados*;**
	- Porta: **Port=*porta_do_seu_sgbd*;**
	- Usuraio: **UID=*usuario_do_banco_de_dados*;**
	- Senha: **Password=*senha_do_banco_de_dados*;**
	- * **Mantenha a Opção 'DATABASE=school_project' e 'SslMode=none' para evitar Conflitos**
	> Caso esteja utilizando um SGBD **diferente do MySql**, esse projeto não irá Funcionar
- [ ] - Execute o [Script SQL](script.sql) no seu SGBD
	> CUIDADO: É recomandado que faça um **BACKUP** do seu SGBD **antes** de Executar o Script. Uma vez que o
	 [Script SQL](script.sql) for executado, caso exista um Banco de Dados chamado de "school_project" ele
	 será **EXCLUIDO**
- [ ] - Inicie o [Visual Studio](https://visualstudio.microsoft.com/pt-br/) e Abra a Solução **[SchoolProject.sln](SchoolProject.sln)**

Obs: SGBD = Sistema de Gerenciamento de Banco de Dados


### Referencias
[Lista dos Estados](https://gist.github.com/quagliato/9282728)
[Icone DropDown Menu](https://pt.stackoverflow.com/questions/330840/bootstrap-3-dropdown-toggle-mudar-de-menu-com-ele-aberto-é-possível)