# Desafio Desenvolvedor Backend .NET

## Definições

- Leia todo o conteúdo antes de iniciar e busque entender de fato o desafio proposto.
- Faça um clone desse repositório para iniciar o projeto. Lembre-se de deixar o seu repositório privado e compartilhar com a conta do GitHub [MarcosVRSDev](https://github.com/MarcosVRSDev).
- Utilizar o .Net na sua versão 5 ou superior.

## Desafio

Criar uma API de conta bancária. Será somente possível cadastrar empresas por CNPJ.

## Orientações

- Recursos:
  - Conta bancária (CRUD).
  - Saque.
  - Depósito.
  - Transações (Uma conta para outra).
  - Retornar saldo e extrato.

- Sugestão de tabelas:
  - **Conta**: Campos: (id, nome, CNPJ, número da conta, agência e imagem do documento)
  - **Transações**: Campos: (id, valor, tipo, conta_id)

## Informações Adicionais

- Utilizar padrão REST, Postgres ou MySQL, e efetuar todas as validações necessárias.
- Ao realizar a abertura da conta, o nome da empresa não vai poder ser informado na model, deve ser obtido através da API pelo CNPJ informado. [ReceitaWS API](https://developers.receitaws.com.br/#/operations/queryCNPJFree) (Atenção ao limite, tem um nível gratuito, tratar erros).
- O documento da conta pode ser uma foto aleatória, fica a critério a forma de envio (Base64 ou MultipartFormData) salvar fisicamente em um diretório.

## O que será Avaliado

- Implementação dos recursos solicitados.
- Validações e tratamento de erros.
- Organização do código e estrutura do projeto.
- Uso adequado das tecnologias mencionadas (REST, banco de dados, .Net 5+).
- Clareza e qualidade do código.
- Uso de boas práticas de desenvolvimento.
- Documentação do projeto.

Qualquer dúvida pode ser enviada para o e-mail: marcos.rezende@inovamobil.com.br
