# Introdu��o
Este projeto � uma demostra��o de envio de telemetria de aplica��es distribuidas para o **Application Insights**.

## Premissas
- Validar como o insights faz o correlacionamento de m�tricas e logs distribuidos das mesma opera��o em workspaces separados;
- Validar o trackeamento de depend�ncias de bibliotecas de terceiros.

## Projeto
![demo_configuration_workspace_diagram](./resources/demo_configuration_workspace_diagram.png "demo_configuration_workspace_diagram")

## Refer�ncias

| Nome  |  Descri��o |
| :------------ | :------------ |
| Microsoft.ApplicationInsights.AspNetCore | Biblioteca para coleta de telemetria para aplica��es Web (http)  |
| Microsoft.ApplicationInsights.WorkerService |  Biblioteca para coleta de telemetria para aplica��es No-Http (console, workers, consumers, hostedservice, etc.)  |
| Microsoft.Extensions.Azure  |  Biblioteca para clients de comunica��o com os servi�os da azure que integra com o ILogger e Activity  |
| Azure.Messaging.ServiceBus  |  Biblioteca para envio de mensagens pora o service-bus |
| Microsoft.Extensions.Caching.StackExchangeRedis  | Biblioteca para integrar o IDistribuitedCache com o Redis  |
| MongoDB.ApplicationInsights  | Biblioteca para coletar a telemetria do mongoDB com o application insights  |
| MongoDB.ApplicationInsights.DependencyInjection  | Biblioteca para injetar as depend�ncias do MongoDB.ApplicationInsights  |
