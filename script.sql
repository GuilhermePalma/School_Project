DROP DATABASE IF EXISTS school_project;
CREATE DATABASE if not exists school_project;
use school_project;

CREATE TABLE if not exists user(
  cpf VARCHAR(11) NOT NULL,
  name VARCHAR(80) NOT NULL,
  code_state_city INT NOT NULL,
  code_address INT NOT NULL,
  residential_number INT NOT NULL,
  complement VARCHAR(40) DEFAULT NULL,
  PRIMARY KEY(cpf)
);

CREATE TABLE if not exists state_city(
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

/* Criação das Chaves Estrangeiras */
ALTER TABLE user
ADD FOREIGN KEY (code_state_city) REFERENCES state_city(code_state_city);
ALTER TABLE user
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

INSERT INTO user(cpf,name,code_state_city,code_address,residential_number,complement)VALUES
("12345678901", "Roberta", 3,1,1202,"Apto 12, Bl 9"),
("15963452810", "Julian", 2, 3, 652,"Bl 8, Apto 62"),
("32084613252", "Ian", 1, 4, 156,"Bl 1 Apto365"),
("24932085105", "Juan", 4,5,56, "Bl 5 Apto203"),
("12048631065", "Olis", 5,2,995,"Apto 103, Bl 1");
INSERT INTO user(cpf,name,code_state_city,code_address,residential_number)VALUES
("65894632386", "Roberto", 4,5,369),
("10541320834", "Marcia", 4,5,278),
("15386305564", "Liana", 1,4,399),
("46108634186", "Polar", 5,2,106);
SELECT * FROM user;

/*Verificar se Os Codigos da Cidade foram colocados corretamente com os Codigos de Endereço*/
SELECT * FROM user ORDER BY code_state_city ASC;
