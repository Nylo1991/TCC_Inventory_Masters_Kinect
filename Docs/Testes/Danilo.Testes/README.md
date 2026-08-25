# Suíte Individual de Testes Unitários — Inventory Masters

Documentação da contribuição individual de **Danilo da Silva Santos** para a validação dos Services dos projetos MVVM e MVC do TCC **Inventory Masters**.

## 1. Identificação

| Item | Informação |
|---|---|
| Aluno | Danilo da Silva Santos |
| Turma | Desenvolvimento de Sistemas |
| Equipe | Inventory Masters |
| Data | 25/08/2026 |
| Componentes escolhidos | Services dos módulos MVVM e MVC |
| Classes/módulos | `TCC_Inventory_Masters_Kinect` e `MVC_InventoryMasters` |
| Suítes desenvolvidas | 11 suítes de testes unitários |
| Resultado final | 112 testes aprovados |

As suítes foram desenvolvidas e documentadas **sem estabelecer hierarquia ou definir qualquer componente como principal**. Cada Service foi avaliado conforme sua responsabilidade no sistema.

## 2. Descrição dos componentes

Os componentes avaliados pertencem à camada de Services dos módulos MVVM e MVC. Eles concentram regras de negócio, processamento de dados e integrações que não devem ficar diretamente nas telas da aplicação.

As responsabilidades verificadas incluem:

- ciclo de vida, câmera, profundidade, calibração e cálculo de volume do Kinect;
- autenticação e comunicação em tempo real entre o WPF e o MVC;
- geração de token, hash, perfis e permissões;
- identificação do usuário, empresa e perfil no contexto HTTP;
- tratamento da ausência de configuração SMTP;
- validação e localização das credenciais do Firebase.

Essa separação reduz o acoplamento da interface e permite respostas previsíveis diante de dados inválidos, ausência de hardware, falhas de configuração ou indisponibilidade de serviços externos.

## 3. Estratégia de testes

Os testes foram definidos a partir dos comportamentos públicos e observáveis de cada Service. Para cada caso foram identificados o cenário, a entrada, a ação executada e o resultado esperado.

Foram considerados:

- cenários válidos e inválidos;
- entradas nulas, vazias ou inesperadas;
- valores limite;
- exceções e falhas controladas;
- estados iniciais e transições de estado;
- chamadas repetidas e idempotência;
- regras de negócio;
- isolamento de hardware, rede e serviços externos.

Os nomes seguem, sempre que aplicável, o formato:

```text
Metodo_Cenario_ResultadoEsperado
```

Cada teste foi organizado com o padrão **Arrange → Act → Assert**:

1. **Arrange:** preparação do Service, dos dados, das configurações e do estado necessário;
2. **Act:** execução do método ou leitura da propriedade avaliada;
3. **Assert:** verificação do retorno, estado, exceção, mensagem ou evento esperado.

## 4. Casos de teste

### Projeto Kinect — 63 testes

| Suíte | Quantidade | Comportamentos verificados | Resultado |
|---|---:|---|---|
| `KinectServiceTests` | 5 | Criação, estado da conexão, início sem sensor e finalização segura | Aprovada |
| `KinectServiceCameraTests` | 7 | Captura de câmera e profundidade sem Kinect conectado | Aprovada |
| `KinectServiceVolumeTests` | 13 | Entradas inválidas, cálculo, arredondamento, suavização e limite do histórico | Aprovada |
| `AutenticacaoMvcServiceTests` | 10 | Cliente HTTP, timeout, validação de e-mail e token e tratamento de exceções | Aprovada |
| `KinectServiceCalibrationTests` | 11 | Captura de mapas, detecção do chão e cálculo do volume de referência | Aprovada |
| `SignalRServiceTests` | 17 | Estado inicial, ausência de conexão, eventos, erros, envio e desconexão segura | Aprovada |

### Projeto MVC — 49 testes

| Suíte | Quantidade | Comportamentos verificados | Resultado |
|---|---:|---|---|
| `TokenAcessoKinectServiceTests` | 10 | Validade, geração do hash SHA-256 e formato do token numérico | Aprovada |
| `PermissaoServiceTests` | 13 | Permissões por perfil, normalização e autorizações concedidas ou negadas | Aprovada |
| `ContextoUsuarioServiceTests` | 14 | Empresa, usuário, perfil, claims e autenticação no contexto HTTP | Aprovada |
| `EmailTokenServiceTests` | 6 | Tratamento seguro da ausência de configuração SMTP | Aprovada |
| `FirebaseServiceTests` | 6 | Validação e resolução do caminho do arquivo de credenciais | Aprovada |

## 5. Técnicas e recursos utilizados

| Técnica ou recurso | Aplicação |
|---|---|
| Arrange, Act, Assert | Separação entre preparação, execução e verificação |
| Particionamento de equivalência | Entradas válidas, inválidas, nulas, vazias e sem configuração |
| Análise de valores limite | Volume zero ou negativo, altura mínima, quantidade de pontos e limites de token e histórico |
| Testes negativos | Ausência de Kinect, conexão, contexto HTTP, SMTP ou credenciais |
| Testes de exceção | Verificação de falhas esperadas e mensagens produzidas |
| Transição de estado e idempotência | Estados de conexão e chamadas repetidas de parada ou desconexão |
| Testes determinísticos | Hash conhecido, repetição de entradas e validação de formatos |
| Isolamento de dependências | Execução sem hardware, rede, Hub MVC, SMTP e Firebase reais |
| Asserções xUnit | Igualdade, nulo, verdadeiro/falso, exceções, mensagens e eventos |
| Visual Studio Test Explorer | Execução individual, por suíte, por projeto e consolidada |

### Ferramentas e configuração

- xUnit 2.5.3;
- Microsoft.NET.Test.Sdk 17.8.0;
- Visual Studio Test Explorer;
- `coverlet.collector` 6.0.0 instalado;
- projeto WPF direcionado para `net8.0-windows` com `UseWPF=true`;
- cliente `Microsoft.AspNetCore.SignalR.Client` na mesma versão utilizada pela aplicação.

## 6. Resultados

| Projeto | Aprovados | Reprovados | Ignorados |
|---|---:|---:|---:|
| MVC — Danilo | 49 | 0 | 0 |
| Kinect — Danilo | 63 | 0 | 0 |
| **Total** | **112** | **0** | **0** |

A execução consolidada foi concluída em aproximadamente **1,8 segundo**, conforme o Visual Studio Test Explorer.

### Evidências

O relatório individual contém **24 imagens em sequência**, associadas às respectivas suítes e situações documentadas:

- imagens 1 a 23: implementação, configuração, organização dos arquivos e execução de cada suíte;
- imagem 24: execução consolidada dos 112 testes, com 112 aprovados, 0 reprovados e 0 ignorados.

As evidências foram mantidas no relatório na ordem em que os testes são apresentados.

## 7. Problemas encontrados

### 7.1 Referência WPF ausente

Os testes de câmera e profundidade identificaram que o projeto de testes não reconhecia `BitmapSource`. O projeto foi ajustado para `net8.0-windows` e recebeu `UseWPF=true`, permitindo a inclusão das referências `PresentationCore` e `WindowsBase`.

**Resultado:** os sete testes de câmera e profundidade foram executados com sucesso.

### 7.2 Dependência SignalR ausente

Durante a implementação dos testes do `SignalRService`, o projeto não reconhecia `HubConnectionState`. Foi adicionada a dependência `Microsoft.AspNetCore.SignalR.Client` na mesma versão utilizada pela aplicação.

**Resultado:** o projeto compilou e os 17 testes de SignalR foram aprovados.

### 7.3 Limitação de testabilidade da conexão real

O `SignalRService` cria o `HubConnection` internamente. Por esse motivo, os caminhos de conexão, reconexão e envio bem-sucedido exigiriam acesso a um Hub real e seriam testes de integração.

A suíte unitária foi delimitada aos comportamentos observáveis sem conexão. Como melhoria futura, recomenda-se utilizar uma interface ou fábrica de conexão que permita substituir o Hub por uma implementação simulada.

### 7.4 Divergência entre evidências do KinectServiceTests

Uma captura inicial registra seis testes em `KinectServiceTests`, enquanto a execução consolidada final registra cinco. A documentação utiliza como resultado oficial a execução consolidada mais recente: **5 testes nessa suíte e 112 no total**.

## 8. Situações fora do escopo

Não fizeram parte desta suíte unitária:

- uso do Kinect físico em diferentes computadores e ambientes;
- conexão real, reconexão e falhas de rede do SignalR;
- envio real de e-mail por SMTP;
- autenticação externa e acesso real ao Firebase;
- testes de desempenho, carga, concorrência e integração ponta a ponta.

Esses cenários dependem de hardware, rede ou infraestrutura externa e devem ser tratados em testes de integração ou de sistema.

## 9. Organização no repositório

```text
Docs/
└── Testes/
    └── Danilo/
        ├── TCC_Inventory_Masters_Kinect.Danilo.Tests/
        │   └── Service_Test_MVVM/
        │       ├── AutenticacaoMvcServiceTests.cs
        │       ├── KinectServiceCalibrationTests.cs
        │       ├── KinectServiceCameraTests.cs
        │       ├── KinectServiceTests.cs
        │       ├── KinectServiceVolumeTests.cs
        │       └── SignalRServiceTests.cs
        ├── MVC_InventoryMasters.Danilo.Tests/
        │   └── Service_Test_MVC/
        │       ├── ContextoUsuarioServiceTests.cs
        │       ├── EmailTokenServiceTests.cs
        │       ├── FirebaseServiceTests.cs
        │       ├── PermissaoServiceTests.cs
        │       └── TokenAcessoKinectServiceTests.cs
        ├── Relatorio_Suite_Testes_Services_MVVM_MVC_Danilo_da_Silva_Santos.pdf
        └── README.md
```

## 10. Execução dos testes

No Visual Studio, abra o **Gerenciador de Testes**, compile a solução e selecione **Executar Todos**.

```

## 11. Conclusão

As 11 suítes permitiram validar comportamentos relevantes dos Services dos módulos MVVM e MVC de forma rápida, repetível e isolada. Os testes documentam os resultados esperados, ajudam a detectar regressões e tornam futuras alterações mais seguras.

O resultado final de **112 testes aprovados** comprova a execução da contribuição individual e amplia a confiabilidade dos módulos de processamento do Kinect, autenticação, comunicação, segurança, permissões, contexto do usuário e configurações externas.

---

**Autor:** Danilo da Silva Santos  
**Equipe:** Inventory Masters  
**Atividade:** Suíte Individual de Testes Unitários Aplicada ao Projeto de TCC
