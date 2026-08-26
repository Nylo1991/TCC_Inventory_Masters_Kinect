# Testes Unitários — Inventory Masters

## 1. Identificação

**Nome do aluno:** Diulie Mileide Batista Coreia  
**Equipe:** Inventory Masters  
**Data:** 23/08/2026  
**Projeto:** TCC Inventory Masters Kinect  

---

# 2. Testes — Inventory Masters Kinect

## 2.1 Componente escolhido

**Componente escolhido:** ViewModel

**Classes testadas:** `BaseViewModel`, `KinectLoginViewModel` e `MainViewModel`, incluindo suas classes parciais de espaço, histórico, interface, Kinect e volume.

---

## 2.2 Descrição do componente

O componente escolhido para a realização dos testes foi a camada ViewModel da aplicação Inventory Masters Kinect.

A ViewModel é responsável por fazer a comunicação entre as Views, desenvolvidas em XAML, e as regras e serviços utilizados pela aplicação. Ela controla os dados exibidos na tela, os comandos executados pelos botões, as mensagens apresentadas ao usuário e os estados da interface.

Entre as responsabilidades testadas estão:

- Notificação de alterações de propriedades para a interface;
- Controle dos comandos da aplicação;
- Solicitação e validação de tokens;
- Criação da sessão do usuário;
- Bloqueio e desbloqueio da sessão;
- Validação dos dados de espaço;
- Cálculo e exibição do volume;
- Carregamento do histórico de medições;
- Tratamento da conexão com o Kinect;
- Tratamento de erros dos serviços.

A escolha da ViewModel foi feita porque ela possui comportamentos importantes para o funcionamento da aplicação. Um erro nessa camada pode gerar dados incorretos na tela, permitir entradas inválidas, impedir o login ou causar falhas durante uma medição.

---

## 2.3 Justificativa da escolha

A ViewModel foi escolhida porque concentra grande parte da lógica utilizada pela interface do sistema.

Antes da separação da lógica, alguns comportamentos estavam presentes nos arquivos `XAML.cs`. Com a aplicação do padrão MVVM, esses comportamentos foram transferidos para classes ViewModel, deixando as Views responsáveis principalmente pela apresentação.

A criação dos testes permite verificar essas regras sem precisar abrir todas as telas manualmente. Também possibilita testar situações de erro, valores limites e falhas de serviços de maneira controlada.

---

## 2.4 Estratégia de testes

A suíte foi desenvolvida utilizando o framework **xUnit** e o padrão **Arrange, Act e Assert**.

### Arrange

Nesta etapa são preparados:

- A ViewModel que será testada;
- Os dados de entrada;
- A sessão do usuário;
- Os serviços falsos utilizados pelo teste;
- O estado inicial esperado.

### Act

Nesta etapa é executado o comportamento que está sendo testado, como:

- Alterar uma propriedade;
- Executar um comando;
- Solicitar um token;
- Validar um token;
- Bloquear ou desbloquear a sessão;
- Calcular o percentual de ocupação;
- Carregar o histórico.

### Assert

Nesta etapa é verificado se o resultado corresponde ao comportamento esperado, como:

- Valor de uma propriedade;
- Mensagem exibida;
- Estado da sessão;
- Disparo do evento `PropertyChanged`;
- Habilitação de comandos;
- Quantidade de registros;
- Tratamento correto de exceções.

Foram criados serviços falsos, também chamados de *fakes*, para evitar que os testes dependessem do servidor MVC, envio de e-mail, banco de dados ou equipamento Kinect.

Dessa forma, os testes são unitários, repetíveis e podem ser executados sem depender de recursos externos.

---

# 3. Casos de Teste — Kinect

## 3.1 BaseViewModel

Os testes da classe `BaseViewModel` verificam o funcionamento da notificação de propriedades utilizada pelas demais ViewModels.

Foram testados os seguintes comportamentos:

- Alteração de uma propriedade;
- Disparo do evento `PropertyChanged`;
- Não disparar o evento quando o valor permanece igual;
- Retorno correto do método responsável por atualizar uma propriedade.

Esses testes são importantes porque a interface depende dessas notificações para atualizar os componentes visuais.

---

## 3.2 KinectLoginViewModel

Os testes de login verificam:

- Estado inicial da tela;
- Existência dos comandos;
- Alternância entre as etapas de solicitação e validação;
- E-mail vazio;
- Token vazio;
- Token com quantidade inválida de caracteres;
- Token contendo caracteres inválidos;
- Limpeza das mensagens anteriores;
- Token válido;
- Token recusado pelo serviço;
- Exceção durante a validação;
- Solicitação de token realizada com sucesso;
- Solicitação recusada pelo serviço;
- Exceção durante a solicitação;
- Criação correta da sessão;
- Abertura da tela principal após o login.

Esses testes são importantes porque impedem a validação de dados incompletos e verificam se falhas do serviço são informadas corretamente ao usuário.

---

## 3.3 MainViewModel

Os testes principais da `MainViewModel` verificam:

- Rejeição de sessões inválidas;
- Inicialização com uma sessão válida;
- Valores iniciais das propriedades;
- Criação dos comandos da interface;
- Notificação de alteração das propriedades;
- Estado inicial do monitoramento.

Foram utilizadas diferentes combinações de dados de sessão, incluindo valores nulos, vazios e incompletos.

---

## 3.4 MainViewModel — Espaço

Os testes relacionados ao espaço verificam:

- Nome do espaço vazio;
- Percentual não informado;
- Percentual não numérico;
- Percentual igual a zero;
- Percentual negativo;
- Percentual maior que 100;
- Ausência de calibração;
- Valores válidos;
- Limite mínimo válido de 1%;
- Valor intermediário de 80%;
- Limite máximo válido de 100%.

Esses testes garantem que somente configurações válidas sejam utilizadas pela aplicação.

---

## 3.5 MainViewModel — Volume

Os testes de volume verificam:

- Conversão de centímetros cúbicos para metros cúbicos;
- Formatação do volume;
- Volume máximo não configurado;
- Volume abaixo do limite;
- Volume acima do limite;
- Limitação do percentual máximo em 100%;
- Tentativa de medição com o Kinect desconectado.

Foram testados valores como zero, um milhão e dois milhões e quinhentos mil centímetros cúbicos.

---

## 3.6 MainViewModel — Histórico

Os testes do histórico verificam:

- Carregamento inicial das medições;
- Atualização da coleção apresentada na tela;
- Substituição dos dados antigos pelos dados recebidos;
- Tratamento de exceção no repositório;
- Apresentação de mensagem de erro.

O repositório real foi substituído por um repositório falso para que os testes não dependessem do banco de dados.

---

## 3.7 MainViewModel — Interface e Sessão

Os testes da interface verificam:

- Bloqueio da aplicação após inatividade;
- Tentativa de desbloqueio com token vazio;
- Recusa de token pertencente a outro usuário;
- Desbloqueio com token pertencente à mesma sessão;
- Comparação do identificador da sessão sem diferenciar letras maiúsculas e minúsculas;
- Solicitação de novo token;
- Mensagem de confirmação;
- Tratamento de falha durante a solicitação.

---

## 3.8 MainViewModel — Kinect

Os testes relacionados ao Kinect verificam se a aplicação consegue encerrar o monitoramento com segurança quando nenhum sensor está conectado.

As operações que dependem diretamente da leitura física do Kinect não foram executadas como testes unitários, pois necessitam de hardware e devem ser verificadas por testes de integração ou testes manuais.

---

# 4. Técnicas Utilizadas nos Testes do Kinect

## 4.1 Testes de cenários válidos

Foram utilizados dados corretos para verificar o funcionamento esperado dos comandos, validações e cálculos.

## 4.2 Testes de cenários inválidos

Foram utilizados campos vazios, valores não numéricos, tokens inválidos e sessões incompletas.

## 4.3 Análise de valores limites

Foram verificados limites como:

- Percentual de 0%;
- Percentual de 1%;
- Percentual de 100%;
- Percentual de 101%;
- Volume igual a zero;
- Percentual de ocupação limitado a 100%.

## 4.4 Testes de exceções

Os serviços falsos foram configurados para lançar exceções. Isso permitiu verificar se a ViewModel trata o erro e apresenta uma mensagem adequada.

## 4.5 Testes parametrizados

Foram utilizados `Theory` e `InlineData` para executar o mesmo comportamento com diferentes valores de entrada.

## 4.6 Test Doubles

Foram criados serviços e repositórios falsos para representar:

- Serviço de autenticação;
- Repositório de medições;
- Resultado de solicitação de token;
- Resultado de validação de token.

## 4.7 Injeção de dependências

As dependências utilizadas pelas ViewModels podem ser fornecidas durante os testes. Isso evita chamadas reais para o servidor, e-mail, banco de dados e Kinect.

---

# 5. Problemas Encontrados nos Testes do Kinect

Durante a preparação da suíte foram encontrados problemas de configuração no projeto de testes.

O projeto estava utilizando uma configuração de framework incompatível com a aplicação WPF. A aplicação utiliza **.NET Framework 4.8**, enquanto o projeto de testes estava configurado de maneira diferente.

Também existia uma referência do Kinect utilizando um caminho específico de outra máquina. Isso poderia impedir a compilação em outros computadores.

Esses problemas foram corrigidos com:

- Configuração do projeto de testes para .NET Framework 4.8;
- Configuração da plataforma para x64;
- Correção da referência da biblioteca `Microsoft.Kinect.dll`;
- Inclusão correta do projeto de testes na solução.

Durante a primeira execução, um teste de formatação do volume apresentou falha. O teste esperava um separador decimal diferente daquele definido pelo comportamento atual da aplicação.

Após a análise, foi identificado que o código utiliza a representação `0.000 m3`. A expectativa do teste foi ajustada para representar corretamente o comportamento especificado.

Depois da correção, todos os **61 testes** foram executados com sucesso.

---

# 6. Situações Não Testadas — Kinect

Não foram realizados testes unitários das seguintes situações:

- Captura física de dados pelo Kinect;
- Movimento real do usuário diante do sensor;
- Falha física do equipamento;
- Envio real de e-mail pelo Gmail;
- Comunicação real com o servidor MVC;
- Gravação real no Firebase;
- Aparência visual das telas XAML.

Essas situações não fazem parte dos testes unitários porque dependem de equipamentos, rede, serviços externos ou avaliação visual. Elas devem ser verificadas por testes de integração, testes de interface ou testes manuais.

---

# 7. Resultado Final dos Testes do Kinect

| Resultado | Quantidade |
|---|---:|
| Testes executados | 61 |
| Testes aprovados | 61 |
| Testes reprovados | 0 |
| **Resultado final** | **Suíte aprovada com sucesso** |

---

# 8. Testes — Inventory Masters MVC

## 8.1 Componentes adicionais escolhidos

Além das ViewModels do aplicativo Kinect, foram testados os Hubs e as ViewModels do projeto MVC Inventory Masters.

Os componentes adicionais testados foram:

- `MedicaoHub`;
- `NotificacaoHub`;
- `BaseViewModel`;
- `DashboardViewModel`;
- `LoginEmailViewModel`;
- `SolicitarTokenKinectRequest`;
- `ValidarTokenViewModel`;
- `ValidarTokenRequest`;
- `ValidacaoTokenResultadoViewModel`;
- `ErrorViewModel`.

---

## 8.2 Descrição dos Componentes do MVC

### MedicaoHub

O `MedicaoHub` é responsável por receber as medições enviadas pelo Kinect por meio do SignalR.

Entre suas responsabilidades estão:

- Receber o volume em centímetros cúbicos;
- Converter o volume para metros cúbicos;
- Salvar a medição;
- Verificar se o volume atingiu o percentual de alerta;
- Criar uma notificação automática;
- Evitar notificações pendentes duplicadas;
- Transmitir a nova medição para os clientes conectados;
- Informar erros de processamento ao cliente que enviou a medição.

### NotificacaoHub

O `NotificacaoHub` é responsável por enviar notificações em tempo real para todos os clientes conectados ao sistema.

Também possui métodos relacionados à conexão e desconexão dos clientes.

### ViewModels do MVC

As ViewModels do MVC são utilizadas para transportar e validar os dados apresentados ou recebidos pelas Views e APIs.

Elas controlam informações como:

- E-mail utilizado para solicitar token;
- Token informado pelo usuário;
- Resultado da validação do token;
- Dados exibidos no Dashboard;
- Quantidade de medições, alertas, parceiros e usuários;
- Ordenação das notificações mais recentes;
- Identificação do usuário e da página;
- Identificação de erros da requisição.

---

# 9. Justificativa dos Testes do MVC

Os Hubs foram escolhidos porque realizam operações importantes para a comunicação entre o Kinect e o sistema MVC.

Um erro no `MedicaoHub` poderia causar:

- Conversão incorreta do volume;
- Perda da medição;
- Criação duplicada de notificações;
- Ausência de alertas;
- Falha na atualização do Dashboard;
- Ausência de uma mensagem adequada quando ocorrer um erro.

As ViewModels do MVC foram testadas porque recebem dados fornecidos pelo usuário e organizam as informações exibidas nas Views.

Os testes ajudam a impedir que dados inválidos sejam processados e verificam se os indicadores apresentados no Dashboard correspondem aos dados armazenados.

---

# 10. Estratégia Utilizada nos Testes do MVC

Os testes foram desenvolvidos com **xUnit** e organizados utilizando **Arrange, Act e Assert**.

Para testar os Hubs sem acessar o Firebase e sem iniciar uma conexão SignalR real, foram criadas implementações falsas das seguintes dependências:

- Repositório de medições;
- Repositório de parâmetros;
- Repositório de notificações;
- Clientes do SignalR;
- Contexto da conexão.

Também foram criadas interfaces para os repositórios utilizados pelo `MedicaoHub`. Isso permitiu substituir os repositórios reais por objetos controlados durante os testes.

Nenhum teste do MVC envia e-mail, acessa o Firebase ou estabelece uma conexão SignalR real.

---

# 11. Casos de Teste — MVC

## 11.1 MedicaoHub

Os testes do `MedicaoHub` verificam os seguintes comportamentos:

- Conversão de zero centímetros cúbicos;
- Conversão de 1.000.000 cm³ para 1 m³;
- Conversão de 2.500.000 cm³ para 2,5 m³;
- Armazenamento da origem da medição como Kinect;
- Armazenamento do status inicial como Normal;
- Registro da data e hora da medição;
- Envio da medição para todos os clientes conectados;
- Volume abaixo do percentual de alerta;
- Volume exatamente no limite do alerta;
- Criação de uma notificação pendente;
- Prevenção de notificações duplicadas;
- Ausência dos parâmetros do sistema;
- Capacidade máxima igual a zero;
- Capacidade máxima negativa;
- Falha ao salvar uma medição;
- Envio de mensagem de erro somente para o cliente responsável;
- Falha durante a criação da notificação;
- Continuidade do envio da medição mesmo quando a notificação falha;
- Conexão de um cliente;
- Desconexão normal;
- Desconexão causada por uma exceção.

### Importância dos testes do MedicaoHub

Esses testes garantem que o volume enviado pelo Kinect seja convertido corretamente e que os clientes recebam os dados esperados.

Os testes de alerta também garantem que uma notificação seja criada somente quando o percentual configurado for atingido e quando ainda não existir uma notificação pendente.

---

## 11.2 NotificacaoHub

Os testes do `NotificacaoHub` verificam:

- Envio da notificação para todos os clientes;
- Nome correto do método SignalR utilizado pelo cliente;
- Conteúdo da mensagem enviada;
- Conexão de um cliente;
- Desconexão de um cliente.

Esses testes garantem que as mensagens em tempo real sejam encaminhadas pelo método correto.

---

## 11.3 LoginEmailViewModel

Foram testados:

- E-mail vazio;
- E-mail em formato inválido;
- E-mail válido;
- Nome de exibição do campo.

---

## 11.4 SolicitarTokenKinectRequest

Foram testados:

- E-mail vazio;
- E-mail inválido;
- E-mail válido.

---

## 11.5 ValidarTokenViewModel

Foram testados:

- Token nulo;
- Token vazio;
- Token com menos de seis caracteres;
- Token com exatamente seis caracteres;
- Token com mais de seis caracteres;
- Token com doze caracteres;
- Token com treze caracteres.

---

## 11.6 ValidarTokenRequest

Foi verificado se o token recebido pela API é armazenado corretamente na propriedade correspondente.

---

## 11.7 ValidacaoTokenResultadoViewModel

Foi verificado o armazenamento dos seguintes resultados:

- Token válido;
- E-mail validado;
- Nome do usuário;
- Empresa;
- E-mail;
- Mensagem da validação.

---

## 11.8 DashboardViewModel

Foram testados:

- Inicialização das listas;
- Inicialização dos parâmetros;
- Mensagem de erro inicial;
- Total de medições;
- Total de alertas;
- Total de parceiros;
- Total de usuários;
- Comportamento quando as coleções são nulas;
- Retorno das cinco notificações mais recentes;
- Ordenação das notificações pela data;
- Nome do usuário;
- Título da página;
- Percentual de ocupação;
- Parâmetros do sistema;
- Mensagem de erro controlada.

---

## 11.9 ErrorViewModel

Foram testados:

- Identificador da requisição nulo;
- Identificador vazio;
- Identificador preenchido;
- Exibição condicional do identificador da requisição.

---

# 12. Técnicas Utilizadas nos Testes do MVC

Foram utilizadas as seguintes técnicas:

- Cenários válidos;
- Cenários inválidos;
- Análise de valores limites;
- Testes parametrizados com `Theory` e `InlineData`;
- Testes de exceções;
- Test doubles;
- Repositórios falsos;
- Contexto SignalR falso;
- Injeção de dependências;
- Verificação de coleções;
- Verificação de mensagens transmitidas;
- Verificação de propriedades;
- Verificação de validações com Data Annotations.

---

# 13. Problema Encontrado nos Testes do MVC

Durante os testes da `ValidarTokenViewModel`, foi encontrada uma inconsistência na validação do token.

A mensagem apresentada pela ViewModel informava que o token deveria possuir seis caracteres. O serviço responsável pela geração do token também cria um token numérico de exatamente seis dígitos.

Entretanto, a configuração da propriedade permitia tokens entre seis e doze caracteres.

## 13.1 Comportamento esperado

Aceitar somente tokens com exatamente seis caracteres.

## 13.2 Comportamento encontrado

Tokens entre seis e doze caracteres eram aceitos pela validação da ViewModel.

## 13.3 Correção realizada

A validação foi alterada para exigir o tamanho mínimo e máximo de seis caracteres.

Depois da correção, foram executados novamente os testes de limite:

- Cinco caracteres: inválido;
- Seis caracteres: válido;
- Sete caracteres: inválido;
- Doze caracteres: inválido;
- Treze caracteres: inválido.

---

# 14. Situações Não Testadas no MVC

Não foram realizados testes unitários com:

- Conexão real do SignalR;
- Firebase real;
- Envio real de e-mail;
- Vários navegadores conectados simultaneamente;
- Queda real da conexão;
- Servidor publicado;
- Aparência visual das páginas;
- Desempenho com grande quantidade de clientes.

Esses comportamentos dependem de recursos externos e devem ser verificados por testes de integração, testes de sistema ou testes manuais.

---

# 15. Resultado Final dos Testes do MVC

| Resultado | Quantidade |
|---|---:|
| Testes executados | 40 |
| Testes aprovados | 40 |
| Testes reprovados | 0 |
| Testes ignorados | 0 |
| **Resultado final** | **Suíte aprovada com sucesso** |

---

# 16. Resultado Geral

A execução das duas suítes de testes apresentou o seguinte resultado:

| Projeto | Testes aprovados |
|---|---:|
| Inventory Masters Kinect | 61 |
| Inventory Masters MVC | 40 |
| **Total geral** | **101** |

**Resultado geral: 101 testes unitários aprovados.**

---

# 17. Conclusão

A implementação das suítes permitiu verificar os principais comportamentos das ViewModels da aplicação Inventory Masters Kinect e dos componentes do sistema MVC.

Os testes validam cenários de sucesso, falha, valores inválidos, valores limites e exceções. Também verificam comportamentos relacionados ao login, sessão, espaço, histórico, volume, conexão com o Kinect, processamento de medições, geração de alertas e comunicação em tempo real.

A utilização de serviços falsos, repositórios falsos e injeção de dependências permitiu executar os testes sem acessar diretamente banco de dados, Firebase, serviços de e-mail ou o equipamento Kinect.

Os testes do MVC também permitiram identificar uma inconsistência real na validação da quantidade de caracteres do token, demonstrando a importância dos testes para encontrar problemas que poderiam afetar o acesso ao sistema.

Ao final, foram executados com sucesso:

- **61 testes unitários do Inventory Masters Kinect;**
- **40 testes unitários do Inventory Masters MVC;**
- **101 testes unitários aprovados no total.**

A suíte criada contribui para a qualidade do projeto porque permite verificar rapidamente se novas modificações causaram algum problema nos comportamentos que já estavam funcionando, aumentando a confiabilidade e facilitando a manutenção do projeto Inventory Masters.
