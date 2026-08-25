# Inventory Masters — Testes Unitários

## Documentação da Suíte de Testes

Este documento apresenta uma visão geral da suíte de testes unitários desenvolvida para o **Inventory Masters — Gestão de Estoque Inteligente**.

A suíte foi criada para verificar os principais comportamentos das controllers da aplicação, contemplando:

* cenários de sucesso;
* validações de entrada;
* tratamento de exceções;
* respostas HTTP;
* persistência simulada;
* regras de negócio;
* integração simulada com SignalR;
* registros de log;
* redirecionamentos;
* operações de cadastro, edição, exclusão e alteração de status.

Os testes utilizam mocks e objetos simulados para manter o isolamento em relação à infraestrutura e evitar dependências reais de banco de dados, serviços de e-mail e conexões externas.

## Tecnologias e recursos utilizados

* **C#**
* **ASP.NET Core MVC**
* **xUnit**
* **Moq**
* `DefaultHttpContext`
* `ConfigurationBuilder`
* `LoggerFactory`
* `IHubContext`
* `TempData`
* `ViewBag`
* `IExceptionHandlerPathFeature`

## Objetivo dos testes

O objetivo principal é garantir que as controllers apresentem o comportamento esperado tanto em situações normais quanto em situações de erro.

A suíte não se limita ao chamado "caminho feliz". Ela também verifica entradas inválidas, identificadores nulos ou vazios, registros inexistentes, exceções, regras de negócio, chamadas que não devem ocorrer e comunicação com componentes externos simulados.

---

# Controllers testadas

A suíte documentada contempla as seguintes classes:

1. `AcessoControllerTests`
2. `DashboardControllerTests`
3. `HomeControllerTests`
4. `KinectApiControllerTests`
5. `MedicoesControllerTests`
6. `NotificacoesControllerTests`
7. `ParametrosControllerTests`
8. `ParceirosControllerTests`
9. `PerfisControllerTests`
10. `UsuariosControllerTests`

---

## 1. AcessoControllerTests

Testa as rotas relacionadas ao acesso e à autenticação.

### Principais cenários

* `Login_DeveRetornarView`

  * Verifica o retorno de `ViewResult`.
  * Confirma o uso de `LoginEmailViewModel`.

* `ValidarToken_DeveRetornarView`

  * Verifica a tela de validação do código.
  * Confirma o uso de `ValidarTokenViewModel`.

* `Negado_DeveRetornarView`

  * Verifica a página de acesso negado.
  * Representa o fluxo HTTP 403.

### Estratégia

As dependências são isoladas utilizando objetos não inicializados para evitar conexões reais com banco de dados ou provedores de e-mail. O `DefaultHttpContext` é utilizado para simular o contexto HTTP.

---

## 2. DashboardControllerTests

Testa a agregação das informações utilizadas no dashboard.

### Principais cenários

* `Index_DeveRetornarViewComDashboardViewModel_QuandoTudoOcorreBem`

  * Verifica o retorno de `DashboardViewModel`.
  * Valida o cálculo do percentual de ocupação.
  * Exemplo documentado: volume medido de `5000` para capacidade máxima de `10000`, resultando em `50%`.

* `Index_DeveRedirecionarParaHomeError_QuandoOcorreExcecao`

  * Simula uma exceção no repositório.
  * Verifica o redirecionamento para `Home/Error`.

### Estratégia

São utilizados mocks para:

* `IMedicaoVolumeRepository`;
* `INotificacaoRepository`;
* `IParceirosRepository`;
* `IParametrosSistemaRepository`;
* `IUsuariosRepository`;
* `ILogger<DashboardController>`.

Isso permite testar o cálculo e a montagem do dashboard sem acesso ao banco de dados.

---

## 3. HomeControllerTests

Testa a porta de entrada da aplicação e o tratamento global de erros.

### Principais cenários

* `Index_DeveRedirecionarParaDashboard`

  * Confirma o redirecionamento de `Home/Index` para `Dashboard/Index`.

* `Error_DeveRetornarViewComModel_ERegistrarLog_QuandoHouverExcecao`

  * Simula uma exceção.
  * Verifica a geração do `RequestId`.
  * Confirma o registro do log com nível `Error`.

* `Error_DeveRetornarViewComModel_SemRegistrarLog_QuandoNaoHouverExcecao`

  * Simula o acesso direto à página de erro.
  * Verifica a utilização do `TraceIdentifier`.

### Estratégia

O teste utiliza `IExceptionHandlerPathFeature`, `IFeatureCollection`, `RequestServices`, `ITempDataDictionary` e `TraceIdentifier` para simular o pipeline de diagnóstico do ASP.NET Core.

---

## 4. KinectApiControllerTests

Testa os endpoints relacionados à autenticação do dispositivo Kinect.

### Principais cenários

* `SolicitarToken_DeveRetornarBadRequest_QuandoModelStateInvalido`

  * Verifica resposta HTTP `400 Bad Request`.

* `SolicitarToken_DeveRetornarOkEEnviarEmail_QuandoSucesso`

  * Verifica resposta HTTP `200 OK`.
  * Confirma a geração do token.
  * Confirma o envio do e-mail.
  * Confirma o registro de auditoria.

* `ValidarToken_DeveRetornarOk_QuandoTokenValido`

  * Confirma a liberação do acesso com token válido.

* `ValidarToken_DeveRetornarUnauthorized_QuandoTokenInvalido`

  * Verifica resposta HTTP `401 Unauthorized`.

### Estratégia

São utilizados mocks de:

* `ITokenAcessoKinectService`;
* `IEmailTokenService`;
* `ILogsSistemaRepository`;
* `IConfiguration`.

A configuração `KinectAccess:TokenValidityMinutes` também é simulada.

---

## 5. MedicoesControllerTests

Testa a listagem e o resumo das medições realizadas pelo sistema.

### Principais cenários

* `Index_DeveRetornarViewComListaPaginadaEViewBagPreenchida`

  * Valida a listagem paginada.
  * Confirma os totais de registros.
  * Confirma a quantidade de medições normais.
  * Confirma a quantidade de medições em alerta.
  * Confirma o volume médio.

* `Index_QuandoOcorrerExcecao_DeveRedirecionarParaError`

  * Simula falha no acesso às medições.
  * Verifica o redirecionamento para `Home/Error`.

* `Summary_DeveRetornarJsonComResumo`

  * Verifica o retorno de `JsonResult`.
  * Confirma o objeto `MedicaoSummary`.

* `Summary_QuandoOcorrerExcecao_DeveRetornarStatusCode500`

  * Verifica o retorno HTTP `500 Internal Server Error`.

### Estratégia

O `IHubContext<MedicaoHub>` é simulado para manter o teste independente das conexões SignalR.

---

## 6. NotificacoesControllerTests

Testa o gerenciamento de notificações e o processo de confirmação de coleta.

### Principais cenários

* `Index_DeveRetornarViewComListaPaginadaEViewBagPreenchida`

  * Verifica a listagem.
  * Confirma os contadores de sucesso, erro e pendência.

* `Index_QuandoOcorrerExcecao_DeveRedirecionarParaError`

  * Verifica o tratamento de falhas na busca.

* `AceitarColeta_ComIdNuloOuVazio_DeveRetornarBadRequest`

  * Verifica resposta HTTP `400`.

* `AceitarColeta_ComSucesso_DeveAtualizarStatusENotificarSignalR`

  * Verifica a atualização para o status `Aceito`.
  * Confirma a comunicação via SignalR.

* `AceitarColeta_QuandoFalharAtualizacao_DeveRetornar500`

  * Simula falha na atualização.
  * Verifica resposta HTTP `500`.

* `AceitarColeta_QuandoOcorrerExcecao_DeveRetornar500`

  * Simula uma exceção.
  * Verifica resposta HTTP `500`.

### Estratégia

O teste simula a hierarquia:

`IHubContext → Clients.All → IClientProxy`

Assim, é possível verificar se a mensagem `ReceberNotificacao` é enviada aos clientes.

---

## 7. ParametrosControllerTests

Testa o gerenciamento das configurações operacionais do sistema.

Entre as configurações estão:

* capacidade mínima;
* capacidade máxima;
* percentual de alerta;
* calibração do sensor Kinect;
* restauração dos padrões.

### Principais cenários

* `Index_DeveRetornarViewComParametros`
* `Index_QuandoOcorrerExcecao_DeveRetornarViewComModeloVazioETempDataErro`
* `Salvar_ComModelStateInvalido_DeveRetornarViewIndexComModelo`
* `Salvar_ComCapacidadeMinimaMaiorOuIgualAMaxima_DeveAdicionarErroNoModelState`
* `Salvar_SemAlteracoes_DeveDefinirTempDataAvisoERedirecionarParaIndex`
* `Salvar_ComAlteracoes_DeveSalvarEDefinirTempDataSucesso`
* `Salvar_QuandoOcorrerExcecao_DeveDefinirTempDataErroERetornarViewIndex`
* `IniciarCalibracao_ComSucesso_DeveAtivarCalibracaoESalvar`
* `IniciarCalibracao_QuandoOcorrerExcecao_DeveDefinirTempDataErroERedirecionar`
* `RestaurarPadroes_ComSucesso_DeveObterPadroesESalvar`
* `RestaurarPadroes_QuandoOcorrerExcecao_DeveDefinirTempDataErroERedirecionar`

### Regras importantes verificadas

A suíte garante que:

* a capacidade mínima deve ser menor que a máxima;
* salvar sem alterações não deve executar uma gravação;
* a calibração deve ativar `AtivarSistemaCalibracao`;
* erros durante a persistência devem ser tratados;
* a restauração deve recuperar e salvar os padrões.

---

## 8. ParceirosControllerTests

Testa o ciclo completo de gerenciamento dos parceiros.

### Funcionalidades verificadas

* listagem;
* filtragem;
* paginação;
* detalhes;
* criação;
* edição;
* exclusão;
* alteração de status.

### Principais cenários

* `Index_DeveRetornarViewComListaPaginada`
* `Index_QuandoOcorrerExcecao_DeveRedirecionarParaError`
* `Details_ComIdVazio_DeveRetornarBadRequest`
* `Details_QuandoNaoEncontrado_DeveRetornarNotFound`
* `Details_ComIdValido_DeveRetornarViewComParceiro`
* `Create_Get_DeveRetornarViewComNovoParceiroAtivo`
* `Create_Post_ComModelStateInvalido_DeveRetornarView`
* `Create_Post_ComSucesso_DeveAdicionarERedirecionar`
* `Edit_Post_SemAlteracoes_DeveDefinirViewBagAviso`
* `Edit_Post_ComAlteracoes_DeveAtualizarERedirecionar`
* `DeleteConfirmed_ComSucesso_DeveExcluir`
* `AlternarStatus_AtivoTrue_DeveInativar`
* `AlternarStatus_AtivoFalse_DeveAtivar`

### Regras importantes verificadas

O teste de edição verifica que uma diferença apenas na máscara do telefone não seja interpretada como alteração.

Também é verificado que a `Data_Cadastro` original seja preservada durante a edição.

---

## 9. PerfisControllerTests

Testa o gerenciamento de perfis e permissões de acesso.

### Principais cenários

* `Index_RetornaViewResult_ComListaPaginadaDePerfis`
* `Create_Post_ComDadosValidos_RedirecionaParaIndex`
* `Edit_Get_ComIdNuloOuVazio_RetornaBadRequest`
* `Details_ComIdExistente_RetornaViewComModel`
* `Inativar_ComIdValido_ExecutaInativacaoERedireciona`

### Estratégia

A interface `IPerfisRepository` é simulada para testar a associação entre perfis e permissões sem acesso ao banco.

A inativação é utilizada para preservar a rastreabilidade histórica, evitando a exclusão física do perfil.

---

## 10. UsuariosControllerTests

Testa o gerenciamento dos usuários.

### Funcionalidades verificadas

* listagem paginada;
* carregamento de perfis;
* criação;
* edição;
* exclusão;
* alteração de status.

### Principais cenários

* `Index_RetornaViewResult_ComListaPaginadaDeUsuarios`
* `Create_Get_CarregaPerfisERetornaView`
* `Create_Post_ModeloValido_RedirecionaParaIndex`
* `Edit_Get_IdNulo_RetornaBadRequest`
* `Edit_Post_ComAlteracao_AtualizaERedirecionaParaIndex`
* `DeleteConfirmed_ComIdValido_ExcluiERedireciona`
* `AlternarStatus_ComIdValido_InverteStatusERedireciona`

### Regras importantes verificadas

Durante a edição, o registro original é recuperado para evitar que campos não editáveis, como senha ou empresa, sejam sobrescritos indevidamente.

Também é verificada a alteração de status sem necessidade de remoção do usuário.

---

# Estratégias de isolamento

A suíte utiliza diferentes estratégias para manter os testes independentes da infraestrutura real.

## Mocks com Moq

As interfaces dos repositórios e serviços são substituídas por mocks.

Exemplo:

```csharp
_mockRepository
    .Setup(r => r.Buscar())
    .Returns(dadosEsperados);
```

As chamadas também podem ser verificadas:

```csharp
_mockRepository.Verify(
    r => r.Salvar(It.IsAny<ParametrosSistema>()),
    Times.Once);
```

Ou garantindo que uma operação não seja executada:

```csharp
_mockRepository.Verify(
    r => r.Salvar(It.IsAny<ParametrosSistema>()),
    Times.Never);
```

## Contexto HTTP simulado

O `DefaultHttpContext` é utilizado para reproduzir elementos do ambiente ASP.NET Core, como:

* `HttpContext`;
* `TempData`;
* `RequestServices`;
* `TraceIdentifier`;
* recursos de diagnóstico.

## SignalR simulado

As dependências de SignalR são substituídas por mocks para verificar se as mensagens são enviadas sem estabelecer conexões reais.

## Tratamento de exceções

Os testes simulam exceções com `ThrowsAsync` ou configurações equivalentes para garantir que as controllers apresentem respostas controladas.

---

# Tipos de comportamento cobertos

| Comportamento                   | Cobertura |
| ------------------------------- | --------- |
| Retorno de Views                | Sim       |
| Retorno de ViewModels           | Sim       |
| Redirecionamentos               | Sim       |
| Bad Request (400)               | Sim       |
| Unauthorized (401)              | Sim       |
| Not Found (404)                 | Sim       |
| Internal Server Error (500)     | Sim       |
| Validação de ModelState         | Sim       |
| Regras de negócio               | Sim       |
| Tratamento de exceções          | Sim       |
| Registro de logs                | Sim       |
| Persistência simulada           | Sim       |
| Verificação de chamadas com Moq | Sim       |
| SignalR                         | Sim       |
| Paginação e filtros             | Sim       |
| CRUD                            | Sim       |
| Alteração de status             | Sim       |

---

# Estrutura conceitual dos testes

Os testes seguem, de maneira geral, o padrão:

**Arrange → Act → Assert**

### Arrange

Preparação do cenário, criação dos objetos e configuração dos mocks.

### Act

Execução da ação da controller que será avaliada.

### Assert

Verificação do resultado obtido e das chamadas realizadas nas dependências.

Exemplo:

```csharp
// Arrange
_repositoryMock
    .Setup(r => r.BuscarPorId("123"))
    .ReturnsAsync(parceiro);

// Act
var resultado = await _controller.Details("123");

// Assert
var view = Assert.IsType<ViewResult>(resultado);
Assert.NotNull(view.Model);
```

---

# Resultado esperado

A suíte tem como finalidade aumentar a confiabilidade do Inventory Masters, verificando não somente os fluxos de sucesso, mas também comportamentos alternativos e situações de falha.

Os testes documentados demonstram que as controllers possuem verificações para entradas inválidas, registros inexistentes, exceções, regras de negócio, respostas HTTP e interações com dependências externas simuladas.

Dessa forma, alterações futuras no sistema podem ser realizadas com maior segurança, pois os principais comportamentos documentados podem ser executados novamente para identificar regressões.

---

# Autor

**Miguel Cássio Braga Duarte**

**Projeto:** Inventory Masters — Gestão de Estoque Inteligente

**Documento de referência:** Documentação da suíte de testes unitários.
