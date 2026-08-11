# Plano de Testes — Inventory Masters

## 1. Identificação do Projeto
* **Nome do sistema:** Inventory Masters
* **Versão:** 2.0.0
* **Equipe responsável:** Danilo, Diulie, Marilene e Miguel
* **Data do planejamento:** 08/08/2026

---

## 2. Objetivos dos Testes

O foco desta etapa de testes é garantir a robustez, a segurança e a integridade de todos os fluxos críticos e componentes estruturais do sistema *Inventory Masters*, prevenindo falhas sistêmicas que possam comprometer a operação, causar corrupção de dados ou paralisar a aplicação.

* **O que a equipe pretende verificar?**
  * **Autenticação e Segurança:** A eficácia do algoritmo de login, a validação rigorosa de e-mails cadastrados/ativos, e a lógica de geração, validade e expiração dos tokens temporários em cenários de sucesso e insucesso.
  * **Captura Volumétrica e Hardware:** A precisão milimétrica das medições de espaço expressas estritamente em metros cúbicos ($m^3$) capturadas via sensor/Kinect, avaliando a resiliência do sistema perante variações de calibração e interferências externas.
  * **Persistência de Dados:** A consistência, integridade e correta gravação das transações e do estoque no banco de dados local (SQL Server).
  * **Sincronização e Nuvem:** A perfeita sincronização e redundância de dados entre o ambiente local e a nuvem (Microsoft Azure).
  * **Comunicação em Tempo Real (SignalR):** A fluidez, a ausência de atrasos perceptíveis e a confiabilidade na transmissão de dados em tempo real entre a aplicação local e o dashboard web na nuvem.
  * **Gestão de Estados e Resiliência:** O comportamento da aplicação WPF perante entradas de dados inválidas, falhas de conexão ou interrupções abruptas nos serviços de mensageria e banco de dados.

* **Quais riscos pretende reduzir?**
  * **Falhas de Autenticação:** Riscos de acesso não autorizado, burla de tokens expirados/malformados ou processamento de fluxos sem a devida validação de credenciais de operador.
  * **Distorções Volumétricas:** Inconsistências ou erros de cálculo no volume de itens ($m^3$) provocados por falhas de calibração ou interferências no campo de visão do Kinect.
  * **Perda de Dados e Falhas de Sincronização:** Riscos de corrupção ou perda de informações de estoque decorrentes de falhas de conectividade entre o SQL Server local e a infraestrutura do Microsoft Azure.
  * **Instabilidade em Tempo Real:** Atrasos, perda de pacotes ou falhas na mensageria via SignalR que comprometam a visualização instantânea das operações no dashboard web.
  * **Travamentos e Exceções Não Tratadas:** Prevenir falhas críticas na interface WPF ou exceções não tratadas (*unhandled exceptions*) em cenários de estresse operacional e inserção de dados inválidos.

* **O que será considerado como evidência de qualidade?**
  * **Matriz de Cobertura de Testes:** Documentação contendo a execução bem-sucedida dos casos de teste prioritários (abrangendo caminhos de sucesso e insucesso para autenticação, Kinect, SQL Server, Azure e SignalR) com seus respectivos resultados obtidos devidamente registrados.
  * **Ausência de Defeitos Críticos:** Inexistência de defeitos bloqueantes, falhas de concorrência ou corrupção de dados em aberto nos módulos principais.
  * **Logs de Execução e Auditoria:** Registros limpos e consistentes que comprovem o tratamento adequado de exceções, tentativas de login inválidas e transações de sincronização.
  * **Relatórios de Desempenho e Validação:** Laudos e evidências visuais (prints ou vídeos) que comprovem a precisão dos dados volumétricos em $m^3$, a estabilidade sob estresse de dados e a latência imperceptível na comunicação via SignalR.
    
---

## 3. Escopo

* **O que será testado:**
  * **Módulo de Autenticação e Controle de Acesso:** O fluxo completo de login, validação rigorosa de e-mails cadastrados/ativos, lógica de geração, validade e expiração de tokens temporários, e restrição de permissões por perfil de operador.
  * **Gestão de Cadastros e Estoque:** As funcionalidades de cadastro, atualização, gerenciamento de medições e a resiliência da interface WPF perante entradas de dados inválidas.
  * **Captura Volumétrica e Hardware:** A integração e o processamento de dados tridimensional via sensor/Kinect, validando a precisão milimétrica dos cálculos volumétricos expressos em metros cúbicos ($m^3$) sob diferentes condições de ambiente.
  * **Persistência Local (SQL Server):** A consistência, integridade absoluta e o salvamento transacional das informações e movimentações de estoque no banco de dados local.
  * **Sincronização em Nuvem (Microsoft Azure):** O mecanismo de redundância, resiliência em modo offline com fila de sincronização e o envio de dados para o ambiente em nuvem.
  * **Comunicação em Tempo Real (SignalR):** A estabilidade, a fluidez e a entrega de dados via *payload* em tempo real entre a aplicação local e o dashboard web hospedado na nuvem, garantindo ausência de atrasos perceptíveis.

* **O que não será testado:**
  * O desempenho, falhas físicas nativas ou defeitos de fabricação internos do hardware de terceiros (como falhas estruturais do sensor Kinect).
  * Quedas generalizadas, instabilidades globais ou problemas de infraestrutura nativa do provedor de nuvem Microsoft Azure.
  * Integrações complexas com sistemas ERP externos legados que fujam do escopo arquitetural estabelecido para este TCC.

* **Funcionalidades prioritárias:** 
  1. Fluxo crítico de autenticação, validação de tokens e restrição de acesso por perfil.
  2. Captura e cálculo volumétrico preciso de itens no armazém em metros cúbicos ($m^3$).
  3. Sincronização e persistência íntegra de dados entre o SQL Server local e o Microsoft Azure.
  4. Comunicação instantânea via SignalR para atualização do dashboard web em tempo real.
  5. Rotinas essenciais de cadastro, gerenciamento, atualização de estoque e notificação de parceiros na interface WPF.
---

## 4. Base de Teste

A concepção, o planejamento e o projeto dos casos de teste do *Inventory Masters* fundamentam-se nos artefatos técnicos, funcionais e arquiteturais do projeto:

* **Requisitos do Sistema:** Especificações detalhadas dos requisitos funcionais e não funcionais estabelecidos para o TCC.
* **Regras de Negócio:** Diretrizes e restrições voltadas à lógica de gestão de estoque, validação de acesso, notificações de parceiros e precisão milimétrica nas medições volumétricas.
* **Histórias de Usuário e Critérios de Aceitação:** Mapeamento comportamental das funcionalidades esperadas pelos operadores do sistema.
* **Especificações Técnicas e Arquiteturais:** Padrões de desenvolvimento da aplicação WPF estruturada em MVVM, protocolos de comunicação em tempo real via SignalR, além da modelagem dos bancos de dados locais (SQL Server) e da infraestrutura em nuvem (Microsoft Azure).
* **Documentação de Projeto:** Diagramas de fluxo de dados, especificações de endpoints e arquitetura de integração entre os componentes locais e os serviços remotos.

---

## 5. Abordagem de Testes
As seguintes abordagens serão adotadas durante o ciclo de qualidade do sistema:
* **Testes Funcionais:** Validação se as funções do sistema realizam o que os requisitos determinam (ex: cadastros, relatórios e buscas).
* **Testes Não Funcionais:** Avaliação de características como usabilidade da interface WPF e confiabilidade da sincronização.
* **Testes de Sistema:** Verificação do comportamento do sistema integrado como um todo, testando o fluxo ponta a ponta.
* **Testes de Integração:** Validação da comunicação entre a aplicação local, o banco SQL Server e os serviços em nuvem Microsoft Azure.
* **Testes de Aceitação:** Validação final realizada com base nos critérios de aceitação definidos nas histórias de usuário.
* **Testes Exploratórios:** Investigação livre do software pela equipe para descobrir defeitos não previstos nos roteiros formais.
* **Testes Baseados em Cenários:** Execução de testes fundamentados em rotinas reais do dia a dia de um armazém logístico.
* **Reteste:** Execução direcionada para certificar se um defeito reportado foi corrigido com sucesso após a alteração do código.
* **Regressão:** Reexecução de testes em funcionalidades já validadas para garantir que correções ou novas implementações não introduziram novos defeitos no sistema.

---

## 6. Técnicas de Projeto de Testes
* **Particionamento de Equivalência:** Utilizado para dividir os dados de entrada em partições válidas e inválidas (ex: campos de quantidade de itens ou dimensões volumétricas), reduzindo o número de casos de teste sem perder cobertura.
* **Análise de Valor Limite:** Aplicada nos limites das faixas de medição e capacidade do sistema (ex: valores mínimos e máximos aceitos pelo sensor ou limites de paginação de estoque).
* **Tabela de Decisão:** Empregada para testar combinações complexas de regras de negócio (ex: permissões de acesso associadas ao perfil do usuário e status do item).
* **Testes Baseados em Cenários:** Utilizados para simular a jornada completa do operador de estoque utilizando o *Inventory Masters*.
* **Testes Exploratórios:** Aplicados em sessões cronometradas para testar a robustez da interface WPF e usabilidade geral.

---

## 7. Casos de Teste

| ID | Funcionalidade | Cenário | Entrada | Resultado Esperado | Resultado Obtido | Status |
| :--- | :--- | :--- | :--- | :--- | :--- | :--- |
| CT-001 | Login | Credenciais válidas | Usuário e senha cadastrados e corretos | Usuário acessa o sistema com sucesso e visualiza o painel principal | — | Pendente |
| CT-002 | Login | Senha inválida | Senha incorreta para usuário existente | Sistema rejeita o acesso e exibe mensagem de erro de credenciais | — | Pendente |
| CT-003 | Captura Volumétrica | Objeto dentro dos parâmetros | Posicionamento correto do item no campo de visão do Kinect | Sistema calcula o volume e exibe as dimensões corretas na interface | — | Pendente |
| CT-004 | Sincronização | Conexão com a nuvem ativa | Disparo manual ou automático de sync com dados locais válidos | Dados do SQL Server são salvos/atualizados com sucesso no Microsoft Azure | — | Pendente |
| CT-005 | Cadastro de Estoque | Inclusão de item com campos obrigatórios vazios | Envio de formulário de cadastro sem o nome do produto | Sistema bloqueia o cadastro e destaca os campos obrigatórios pendentes | — | Pendente |

---

## 8. Análise de Riscos

| Risco | Impacto | Probabilidade | Ação |
| :--- | :--- | :--- | :--- |
| Falha na conexão com o SQL Server local | Alto | Média | Criar cenário de indisponibilidade simulada, testando mensagens de exceção e rotinas de recuperação de transação. |
| Perda de conectividade com a nuvem Microsoft Azure | Alto | Média | Garantir persistência local em cache no SQL Server e testar o comportamento da aplicação em modo offline/fila de sincronização. |
| Distorção na medição volumétrica por interferência externa | Médio | Alta | Aplicar testes com variação de iluminação e obstruções parciais, validando os limites de tolerância do sensor. |
| Entrada de dados inválidos pelo operador | Médio | Alta | Utilizar particionamento de equivalência e análise de valor limite para blindar os campos de entrada na interface WPF. |

---

## 9. Reteste e Regressão
Para garantir a estabilidade do sistema após alterações, o seguinte processo será adotado em **5 cenários críticos** que frequentemente apresentam defeitos:
1. *Autenticação de Usuários*
2. *Processamento de Captura Volumétrica*
3. *Sincronização de Dados Local-Nuvem*
4. *Atualização de Status de Inventário*
5. *Exportação de Relatórios de Estoque*

* **Reteste:** Após a correção de um defeito identificado em um desses cenários, a equipe executará exatamente o mesmo caso de teste que falhou para certificar-se de que o erro foi solucionado (*A correção resolveu o defeito identificado?*).
* **Regressão:** Após validar a correção, a equipe selecionará e executará os testes associados às funcionalidades adjacentes que interagem com o módulo alterado para responder à pergunta: *A alteração afetou alguma funcionalidade que anteriormente funcionava?* Funcionalidades centrais de persistência no SQL Server e comunicação com o Microsoft Azure serão sempre retestadas após mudanças estruturais relevantes.

---

## 10. Critérios de Entrada e Saída

### Critérios de Entrada
O início da execução dos testes do *Inventory Masters* exige o atendimento às seguintes condições:
* Ambiente de desenvolvimento e testes configurado (SQL Server e ambiente WPF operacionais).
* Versão estável do software instalada na máquina de teste.
* Massa de dados de teste previamente preparada.
* Funcionalidades liberadas pelo time de desenvolvimento.
* Requisitos e critérios de aceitação formalmente definidos.

### Critérios de Saída 
A conclusão da etapa de testes e liberação do sistema considerará:
* 100% dos casos de teste prioritários executados.
* Ausência total de defeitos classificados com severidade Crítica ou Alta não resolvidos.
* Quantidade aceitável e controlada de defeitos menores conhecidos (documentados e com plano de contorno).
* Riscos residuais avaliados e aceitos pela equipe.
* Objetivos de qualidade definidos no planejamento alcançados.

---

## 11. Evidências e Documentação
Os resultados da execução dos testes serão devidamente registrados utilizando os seguintes artefatos:
* Capturas de tela (*prints*) das telas do sistema WPF e painéis de controle.
* Gravações em vídeo para cenários complexos de captura volumétrica via sensor, quando necessário.
* Registros de execução e logs do sistema (SQL Server e Azure).

### Registro de Defeitos
Para cada falha ou inconsistência encontrada, um registro de defeito contendo os seguintes campos obrigatórios será aberto:
* **Título:** Resumo claro do problema.
* **Descrição:** Detalhamento do comportamento incorreto observado.
* **Passos para reprodução:** Sequência exata de ações para gerar o erro.
* **Resultado esperado:** O comportamento correto previsto na base de teste.
* **Resultado obtido:** O que de fato aconteceu no sistema.
* **Evidência:** Print ou log anexado.
* **Severidade:** (Baixa, Média, Alta, Crítica) — indicando o impacto técnico da falha.
* **Prioridade:** (Baixa, Média, Alta, Urgente) — indicando a ordem de correção.
* **Versão:** Versão do *Inventory Masters* em que o erro foi encontrado.
* **Ambiente:** Especificações do ambiente de teste (ex: máquina local, versão do SQL Server, status da conexão Azure).

----
# Cenários Testados:

### Caso de Teste: CT-Login-01 — Validação do Campo de E-mail e Autenticação por Token

* **Objetivo:** Validar o comportamento visual, a usabilidade, a obrigatoriedade, o tratamento de mensagens, o fluxo de envio/validação de tokens e o retorno à tela inicial no módulo de autenticação.
* **Pré-condições:** A aplicação deve estar aberta na tela de login inicial.

#### 1. Execução dos Passos
* **Ação 1:** Iniciar o processo de login e interagir com o campo destinado à digitação do e-mail (Tela 1).
* **Ação 2:** Submeter um endereço de e-mail não cadastrado no sistema (Tela 2).
* **Ação 3:** Inserir um e-mail cadastrado e avançar para a solicitação e validação do token (Telas 3 e 4).
* **Ação 4:** Acionar a opção de solicitar um novo token e verificar o fluxo de retorno (Tela 5).

#### 2. Resultados Esperados e Observados

* **Cenário Positivo (Validação de Interface, Usabilidade e Fluxos):**
  * **Layout e Preenchimento:** A tela apresenta indicação visual clara que determina especificamente o campo onde o usuário deve digitar o e-mail, com espaçamento adequado dentro da caixa de texto.
  * **Validação de Obrigatoriedade:** Caso o usuário tente avançar sem preencher o campo de e-mail, o sistema dispara imediatamente uma mensagem informando a obrigatoriedade do preenchimento.
  * **Design Visual e Destaque:** A interface possui paleta de cores harmônica que não sobrecarrega a visão do operador, e as cores das caixas de mensagens se destacam claramente para indicar quando há incorreções.
  * **Campos e Instruções:** A tela exibe o campo de solicitação e validação de token de forma clara e intuitiva.
  * **Fluxo de Retorno:** Ao solicitar um novo token, o sistema redireciona o fluxo perfeitamente de volta à tela inicial para que o usuário redigite o e-mail.

* **Cenário Negativo (Oportunidades de Melhoria e Alinhamento Técnico):**
  * **Aviso de Primeiro Acesso:** Ao acessar a aplicação pela primeira vez, o sistema é omisso em informar que o endereço de e-mail precisa prévia e obrigatoriamente ser cadastrado pelo administrador do sistema.
  * **Tratamento de Mensagem:** A mensagem exibida para e-mails não cadastrados é genérica, pois deixa de informar explicitamente que o usuário não possui acesso ao sistema.
  * **Ausência de Tooltips:** Falta de dicas de contexto (tooltips) nos botões de ação da tela de login, especificamente nos botões de validar e de solicitar novo token.








