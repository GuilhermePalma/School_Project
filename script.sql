DROP DATABASE IF EXISTS school_project;
CREATE DATABASE IF NOT EXISTS school_project;
use school_project;

CREATE TABLE IF NOT EXISTS state_city(
  code_state_city INT NOT NULL AUTO_INCREMENT,
  city VARCHAR(80) NOT NULL,
  state VARCHAR(2) NOT NULL,
  PRIMARY KEY(code_state_city)
);

CREATE TABLE IF NOT EXISTS address(
  code_address INT NOT NULL AUTO_INCREMENT,
  logradouro VARCHAR(80) NOT NULL UNIQUE,
  PRIMARY KEY(code_address)
);

CREATE TABLE if not exists client(
  cpf VARCHAR(11) NOT NULL,
  name VARCHAR(80) NOT NULL,
  code_state_city INT NOT NULL,
  code_address INT NOT NULL,
  residential_number INT NOT NULL,
  complement VARCHAR(40) DEFAULT NULL,
  phone VARCHAR(9) NOT NULL,
  ddd VARCHAR(3) NOT NULL,
  PRIMARY KEY(cpf)
);

CREATE TABLE if not exists seller(
  cnpj VARCHAR(14) NOT NULL,
  name VARCHAR(80) NOT NULL,
  code_state_city INT NOT NULL,
  code_address INT NOT NULL,
  residential_number INT NOT NULL,
  complement VARCHAR(40) DEFAULT NULL,
  phone VARCHAR(9) NOT NULL,
  ddd VARCHAR(3) NOT NULL,
  PRIMARY KEY(cnpj)
);

CREATE TABLE if not exists products(
  id_product INT NOT NULL AUTO_INCREMENT,
  cnpj_seller VARCHAR(14) NOT NULL,
  name VARCHAR(80) NOT NULL,
  code_state_city INT NOT NULL,
  code_address INT NOT NULL,
  quantity INT NOT NULL,
  description TEXT NOT NULL,
  price DECIMAL(8,2) NOT NULL,
  PRIMARY KEY(id_product)
);

/* Criação das Chaves Estrangeiras */
ALTER TABLE client
ADD FOREIGN KEY (code_state_city) REFERENCES state_city(code_state_city);
ALTER TABLE client
ADD FOREIGN KEY (code_address) REFERENCES address(code_address);

ALTER TABLE seller
ADD FOREIGN KEY (code_state_city) REFERENCES state_city(code_state_city);
ALTER TABLE seller
ADD FOREIGN KEY (code_address) REFERENCES address(code_address);

ALTER TABLE products
ADD FOREIGN KEY (cnpj_seller) REFERENCES seller(cnpj);

INSERT INTO state_city(code_state_city, city, state) VALUES
(1, "São Paulo", "SP"),
(2, "Ribeirão Preto", "SP"),
(3, "Bauru", "SP"),
(4, "Recife", "PE"),
(5, "Belo Horizonte", "MG");
SELECT * FROM state_city;

INSERT INTO address(logradouro) VALUE
("Rua Rei de Luis");
INSERT INTO address(code_address,logradouro) VALUES
(2, "Avenida Pautris"),
(3, "Avenida Jauna Ruiz"),
(4, "Rua de Laberal"),
(5, "Rua Kinu Luo");
SELECT * FROM address;

INSERT INTO client(cpf,name,code_state_city,code_address,residential_number,complement,phone,ddd)VALUES
("12345678901", "Roberta", 3,1,1202,"Apto 12, Bl 9","996315236","011"),
("15963452810", "Julian Lopa", 2, 3, 652,"Bl 8, Apto 62","965236542","016"),
("32084613252", "Ian", 1, 4, 156,"Bl 1 Apto365","997586324","021"),
("24932085105", "Juan Mius", 4,5,56, "Bl 5 Apto203","168735495","091"),
("12048631065", "Olis", 5,2,995,"Apto 103, Bl 1","278631422","073");
INSERT INTO client(cpf,name,code_state_city,code_address,residential_number,phone,ddd)VALUES
("65894632386", "Roberto", 4,5,369,"605862475","044"),
("10541320834", "Marcia Nupi", 4,5,278,"120267895","037"),
("15386305564", "Liana Kilas", 1,4,399,"284103862","054"),
("46108634186", "Polar", 5,2,106,"856324318","064");
SELECT * FROM client;

INSERT INTO seller(cnpj,name,code_state_city,code_address,residential_number,complement,phone,ddd)VALUES
("18984552000147", "Loac", 2,3,4125,"Apto 12, Bl 9","997064352","081"),
("00224212000152", "Mulis Loia", 2, 3, 156,"Bl 8, Apto 62","986413056","077"),
("12345679000123", "Polu", 2, 3, 564,"Bl 1 Apto365","970632631","048"),
("52307798000112", "Mise Pliune", 4,5,135, "Bl 5 Apto203","963044582","054"),
("24290772000173", "Noput", 4,5,795,"Apto 103, Bl 1","963100256","088");
INSERT INTO seller(cnpj,name,code_state_city,code_address,residential_number,phone,ddd)VALUES
("90896136000114", "Birse", 4,5,456,"903642015","032"),
("11111111000111", "Macke", 3,1,345,"930411011","042"),
("59317469000100", "Loas Maru", 3,1,454,"963320225","071"),
("41727260000140", "Pleint Loa", 3,1,385,"998088631","037");
SELECT * FROM seller;

/*Verificar se Os Codigos da Cidade foram colocados corretamente com os Codigos de Endereço*/
SELECT * FROM client ORDER BY code_state_city ASC;
SELECT * FROM seller ORDER BY code_state_city ASC;


INSERT INTO products(id_product,cnpj_seller,name,code_state_city,code_address,quantity,description,price)VALUES
(1, "18984552000147", "Televisão 60 Polegadas - 4K", 2, 3,10,"Por meio da Televisão de 60 Polegadas é possivel ter uma melhor experencias ao assistir programas,series e filmes. A Qualidade em 4K permite que as cores sejam mais vivas e intensas.", 8000.00),
(2, "12345679000123", "Mesa de Jantar - 6 Lugares", 2, 3,20,"Esta mesa pode atender 6 Pessoas com conforto. Mesa de Madeira Bruta, feita a partir de arvores reflorestavel e com baixa emissão de Carbono na sua produção", 8010.80),
(3, "90896136000114", "Arte Digital - Desenho por Encomenda", 4, 5,40,"Encomende um desenho a partir de uma foto do objeto, pessoa ou animal desejado", 5);
SELECT * FROM products;


/* 
- CEP (Normalização e PK)
- Mascara (CEP, Preço)
- Verificar Script

 */