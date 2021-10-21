# VSSummit 2021

Olá, pessoal! Se você chegou nessa página, provavelmente é porque você participou do Visual Studio Summit 2021. Espero que a minha palestra tenha sido interessante para você e que você tenha saído com uma ferramenta a mais em mente para aumentar a qualidade do software que você produz.

## Meus contatos

Caso você queira entrar em contato comigo para tirar dúvidas ou trocar experiências, pode me adicionar no LinkedIn: https://www.linkedin.com/in/fabiogouw/

## Demo apresentada

O código da demo que apresentei, presente neste repositório, é uma aplicação de votação que segue o modelo de estruturação de arquiteturas Clean Architecture (Uncle Bob).

Nela, eu utilizei a biblioteca Testcontainers para a criação de containers Docker descartáveis para a execução de testes de integração, o que aumenta a confiança na qualidade das alterações que fazemos no nosso código durante as várias implementações de funcionalidades.

Para ficar mais fácil de achar os códigos que apresentei, segue abaixo a lista:
- [MySQLPollRepository](../src/SuperSurvey.Adapters/MySQLPollRepository.cs) - aqui eu mostro alguns testes de integração para validar cenários usando uma base de dados MySQL.
- [SQSVoteRepository](../src/SuperSurvey.Adapters/SQSVoteRepository.cs) - e aqui eu exercito a integração com o envio de mensagens para o serviço de fila gerenciado da AWS, o SQS, utilizando o [Localstack](https://localstack.cloud/) para simulação do serviço.

## Links

Por fim, seguem os links que mostrei ao final da apresentação, com a adição de mais alguns que achei bem interessante citar.

- https://github.com/HofmeisterAn/dotnet-testcontainers - este é o projeto da biblioteca Testcontainers, versão .NET
- https://www.testcontainers.org/ - este é o site da versão original da biblioteca, em Java
- https://martinfowler.com/articles/microservice-testing/ - material muito interessante a respeito de estratégias de testes e que dão uma aprofundada nos diversos tipos de testes que existem
- https://martinfowler.com/articles/practical-test-pyramid.html - outro artigo do site do Martin Fowler, a respeito do conceito de pirâmide de testes
- https://kentcdodds.com/blog/the-testing-trophy-and-testing-classifications uma outra abordagem para o conceito de pirâmide de testes, com um formato diferente