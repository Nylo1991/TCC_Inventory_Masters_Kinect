# Testes unitários do MVC Inventory Masters

Este projeto testa os componentes escolhidos da aplicação MVC sem utilizar
Firebase, Gmail ou conexões SignalR reais.

## Escopo

- `MedicaoHub`: conversão, persistência, transmissão, alertas, duplicidade,
  parâmetros inválidos e falhas controladas.
- `NotificacaoHub`: envio para todos os clientes e ciclo de conexão.
- ViewModels do MVC: validações de e-mail e token, valores iniciais, totais,
  ordenação das notificações e armazenamento dos resultados.
- `ErrorViewModel`: exibição condicional do identificador da requisição.

## Execução

```powershell
dotnet test MVC_InventoryMasters.Tests\MVC_InventoryMasters.Tests.csproj
```
