# School Project

<h3 align="center">Projeto Web em ASP .NET Framework</h3>

Tabela de conteúdos
=================
<!--ts-->
 * [Sobre o projeto](#sobre-o-projeto)
 	* [Como executar o projeto](#como-executar-o-projeto)
 		* [Pré-requisitos](#pré-requisitos)
	* [Tecnologias](#tecnologias)
   	* [Referências](#referências)
   		* [Artigos](#artigos-usados)
   		* [Design](#design)
   		* [Correções](#correções)
<!--te-->

# Sobre o Projeto
Projeto desnvolvido para as aulas de Programação Web II. Este Projeto visa atender os seguintes requisitos:

- [X] Normalização de Tabelas do Banco de Dados
- [X] Tratamento de Exceções
- [X] Personalização de Rotas
- [X] Utilização da Estrutura DAO e ADO (Database)
- [X] Mascara nos Campos de CPF/CNPJ e Telefone
- [X] Desenvolver Interfaces Web de "Clientes" e "Vendedores" com os atributos:
	- [X] Nome
	- [X] Documento de Registro (CPF/CNPJ)
	- [X] Endereço (Logradouro, Cidade, Estado)
	- [X] Numero de Telefone 
- [X] Desenvolver Interfaces Web de "Produtos", com os atributos
	- [X] Nome
	- [X] Nome do Vendedor
	- [X] Descrição
	- [X] Preço
- [ ] Disponibilziar o Site de Forma Online


## Como executar o Projeto

### Pré-requisitos
Antes de começar, você vai precisar ter instalado em sua máquina:

- [Git](https://git-scm.com/downloads) → Versionamento no Codigo
- [Visual Studio 2019](https://visualstudio.microsoft.com/pt-br/) → Ambiente de Desenvolvimento da Microsoft

### Let's Code

* Abra o Git no local desejado e Baixe o repositorio [School Project](https://github.com/GuilhermePalma/School_Project)
```bash
# Clone/Intale este repositório
$ git clone https://github.com/GuilhermePalma/School_Project.git

# Acesse a pasta do projeto no Terminal/Cmd
$ cd School_Project
```
* Acesse o Arquivo [SchoolProject/Web.config](SchoolProject/Web.config)
* Navegue até a Linha 11 e altere os dados no Campo **connectionString** na seguinte Ordem:
	- Servidor: **Server=*seu_servidor_de_banco_de_dados*;**
	- Porta: **Port=*porta_do_seu_sgbd*;**
	- Usuraio: **UID=*usuario_do_banco_de_dados*;**
	- Senha: **Password=*senha_do_banco_de_dados*;**
	- Mantenha a Opção **'DATABASE=school_project' e 'SslMode=none'** para evitar Conflitos
	- Esse Projeto só funcionará no SGBD **MySql**
* Execute o [Script SQL](script.sql) no seu SGBD (Sistema de Gerenciamento de Banco de Dados)
	> Caso exista um Banco de Dados chamado de "school_project", ao executar o [Script SQL](script.sql) ele
	 será **EXCLUIDO**
* Inicie o [Visual Studio](https://visualstudio.microsoft.com/pt-br/) e Abra a Solução **[SchoolProject.sln](SchoolProject.sln)**


## Tecnologias

### Utilitarios

- Editor Online (C#): [Paiza](https://paiza.io/en)
- JQuery: [JQuery](https://jquery.com)
- Bootstrap: [Bootstrap 3.4](https://getbootstrap.com/docs/3.4)


## Referências

### Artigos Usados

- Pagina de Erro Customizada: [QaStack](https://qastack.com.br/programming/619895/how-can-i-properly-handle-404-in-asp-net-mvc)

- Posição de um Item no IEnumerable: [StackOverflow](https://stackoverflow.com/questions/1290603/how-to-get-the-index-of-an-element-in-an-ienumerable)

- Listas/Arrays Utilizadas: 
    - Estados → [Gist](https://gist.github.com/quagliato/9282728) 
    - DDDs Validos → [Gist](https://gist.github.com/ThadeuLuz/797b60972f74f3080b32642eb36481a5)

- Comparação entre Objetos de mesma Classe: [GabsFerreira](http://gabsferreira.com/a-maneira-certa-de-comparar-objetos-em-c/)

### Design

- Design: Pagina Inicial → [Bootstrap](https://getbootstrap.com/docs/3.4/examples/carousel/#)

- Icones: 
    - Icones do Site  → [TablerIcons](https://tablericons.com)
    - Cliente - Pagina Inicial  → [FlatIcon](https://www.flaticon.com/free-icon/shopper_835900)
    - Vendedor - Pagina Inicial  → [FlatIcon](https://www.flaticon.com/free-icon/business-person_1256183)
    - Produtos - Pagina Inicial  → [FlatIcon](https://www.flaticon.com/free-icon/shopping-bag_1255280)
    - Produtos - Tabela Produtos  → [FlatIcon](https://www.flaticon.com/free-icon/online-shopping_3081415)
    - Icone do Dropdown Menu: [StackOverflow](https://pt.stackoverflow.com/questions/330840/bootstrap-3-dropdown-toggle-mudar-de-menu-com-ele-aberto-é-possível)

- Imagens:
    - Cliente - Pagina Inicial  → [Freepik](https://br.freepik.com/vetores-gratis/ilustracao-de-suporte-ao-cliente-plano-organico_13184987.htm)
    - Vendedor - Pagina Inicial → [Freepik](https://br.freepik.com/vetores-gratis/conceito-de-feedback-plano-ilustrado_13718569.htm)
    - Produtos - Pagina Inicial → [Freepik](https://br.freepik.com/vetores-gratis/jovens-em-pe-perto-do-caixa-no-supermercado-contador-pagamento-ilustracao-em-vetor-plana-do-comprador-alimentos-refeicoes-e-produtos_10174096.htm)
    - Imagem Error404 → [Freepik](https://br.freepik.com/vetores-gratis/opa-erro-404-com-uma-ilustracao-do-conceito-de-robo-quebrado_8030430.htm)

### Correções

- Correção IDispose C#: [StackOverflow](https://pt.stackoverflow.com/questions/6913/fechar-conexão-com-banco-de-dados-c)
- Correção Metodo POST: [TI-Enxame](https://www.ti-enxame.com/pt/asp.net-mvc/como-tornar-o-html.editorfor-desativado/1043356389/)
