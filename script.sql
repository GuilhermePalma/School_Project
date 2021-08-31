DROP DATABASE IF EXISTS school_project;
CREATE DATABASE if not exists school_project;
use school_project;

CREATE TABLE IF NOT EXISTS state_city(
  code_state_city INT NOT NULL AUTO_INCREMENT,
  city VARCHAR(80),
  state VARCHAR(2),
  PRIMARY KEY(code_state_city)
);

CREATE TABLE IF NOT EXISTS address(
  code_address INT NOT NULL AUTO_INCREMENT,
  logradouro VARCHAR(80) NOT NULL,
  PRIMARY KEY(code_address)
);

CREATE TABLE if not exists client(
  cpf VARCHAR(11) NOT NULL,
  name VARCHAR(80) NOT NULL,
  code_state_city INT NOT NULL,
  code_address INT NOT NULL,
  residential_number INT NOT NULL,
  complement VARCHAR(40) DEFAULT NULL,
  PRIMARY KEY(cpf)
);

CREATE TABLE if not exists seller(
  cnpj VARCHAR(14) NOT NULL,
  name VARCHAR(80) NOT NULL,
  code_state_city INT NOT NULL,
  code_address INT NOT NULL,
  residential_number INT NOT NULL,
  complement VARCHAR(40) DEFAULT NULL,
  PRIMARY KEY(cnpj)
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

INSERT INTO client(cpf,name,code_state_city,code_address,residential_number,complement)VALUES
("12345678901", "Roberta", 3,1,1202,"Apto 12, Bl 9"),
("15963452810", "Julian Lopa", 2, 3, 652,"Bl 8, Apto 62"),
("32084613252", "Ian", 1, 4, 156,"Bl 1 Apto365"),
("24932085105", "Juan Mius", 4,5,56, "Bl 5 Apto203"),
("12048631065", "Olis", 5,2,995,"Apto 103, Bl 1");
INSERT INTO client(cpf,name,code_state_city,code_address,residential_number)VALUES
("65894632386", "Roberto", 4,5,369),
("10541320834", "Marcia Nupi", 4,5,278),
("15386305564", "Liana Kilas", 1,4,399),
("46108634186", "Polar", 5,2,106);
SELECT * FROM client;

INSERT INTO seller(cnpj,name,code_state_city,code_address,residential_number,complement)VALUES
("18984552000147", "Loac", 2,3,4125,"Apto 12, Bl 9"),
("00224212000152", "Mulis Loia", 2, 3, 156,"Bl 8, Apto 62"),
("12345679000123", "Polu", 2, 3, 564,"Bl 1 Apto365"),
("52307798000112", "Mise Pliune", 4,5,135, "Bl 5 Apto203"),
("24290772000173", "Noput", 4,5,795,"Apto 103, Bl 1");
INSERT INTO seller(cnpj,name,code_state_city,code_address,residential_number)VALUES
("90896136000114", "Birse", 4,5,456),
("11111111000111", "Macke", 3,1,345),
("59317469000100", "Loas Maru", 3,1,454),
("41727260000140", "Pleint Loa", 3,1,385);
SELECT * FROM seller;

/*Verificar se Os Codigos da Cidade foram colocados corretamente com os Codigos de Endereço*/
SELECT * FROM client ORDER BY code_state_city ASC;
SELECT * FROM seller ORDER BY code_state_city ASC;
