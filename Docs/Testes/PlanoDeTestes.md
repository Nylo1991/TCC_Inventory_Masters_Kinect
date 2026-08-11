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

A estratégia de qualidade do *Inventory Masters* emprega uma combinação de abordagens sistemáticas e direcionadas, cobrindo desde componentes isolados até o comportamento integrado do ecossistema tecnológico, acompanhadas de seus respectivos exemplos práticos de execução:

* **Testes Funcionais:** Validação pontual para assegurar que cada funcionalidade implementada atenda rigorosamente aos requisitos especificados (como rotinas de cadastro, gerenciamento de estoque, autenticação por token e envio de notificações a parceiros).
  * *Exemplo 1:* Inserir um endereço de e-mail válido na tela de login e verificar se o sistema gera e envia o token temporário corretamente.
 * *Exemplo 2:* Cadastrar um novo usuário preenchendo todos os campos obrigatórios e validar se o registro é salvo com sucesso na listagem.
   
* **Testes Não Funcionais:** Avaliação de características estruturais e de desempenho do sistema que não se limitam às regras de negócio diretas, mas que impactam a experiência, a eficiência, a segurança e a estabilidade da aplicação WPF, do sensor Kinect e da comunicação.
  * *Exemplo 1 (Desempenho e Tempo de Resposta):* Medir o tempo de processamento e renderização do cálculo volumétrico capturado pelo Kinect, garantindo que o resultado em metros cúbicos ($m^3$) seja exibido na tela WPF em menos de 2 segundos.
  * *Exemplo 2 (Confiabilidade e Resiliência de Rede):* Simular uma queda abrupta de conexão com a internet durante a sincronização de dados com o Microsoft Azure, verificando se o sistema armazena as informações em uma fila local sem corromper o banco SQL Server ou travar a aplicação.
  * *Exemplo 3 (Desempenho em Tempo Real):* Avaliar a latência na transmissão de dados via SignalR, assegurando que as atualizações enviadas para o dashboard web ocorram de forma quase instantânea (atraso inferior a 1 segundo) mesmo sob uso concomitante de múltiplos operadores.

 * **Testes de Integração:** Validação da comunicação e da troca de dados entre os diferentes módulos e ambientes, verificando a persistência transacional com o banco de dados local (SQL Server), a sincronização com a infraestrutura em nuvem (Microsoft Azure) e a entrega de mensagens em tempo real via SignalR.
  * *Exemplo 1:* Realizar uma atualização de estoque na interface WPF e validar se a transação é gravada corretamente no SQL Server local, garantindo que não haja perda de integridade referencial.
  * *Exemplo 2:* Simular uma atualização de volume ($m^3$) no sistema local e validar a latência e a consistência da sincronização: verificar se o dado é persistido no Azure e se o *hub* do SignalR dispara a notificação para o dashboard web sem divergência nos valores informados.
 
    * **Testes de Sistema:** Verificação integrada de todo o comportamento do software como um todo, avaliando o fluxo operacional completo — desde a captura volumétrica tridimensional por sensor/Kinect até a consolidação dos dados no ecossistema local e remoto.
  * *Exemplo 1 (Fluxo Operacional de Captura e Fechamento de Lote):* Executar o fluxo ponta a ponta onde o operador posiciona a carga diante do sensor Kinect, realiza a leitura volumétrica tridimensional, valida o cálculo automático convertido para metros cúbicos ($m^3$) na interface WPF, confirma o salvamento que transaciona o SQL Server local, aguarda a sincronização bem-sucedida com a nuvem Microsoft Azure e verifica a exibição consolidada e em tempo real dos dados atualizados no dashboard web por meio da mensageria SignalR.
  * *Exemplo 2 (Fluxo de Exceção e Resiliência Operacional):* Simular uma falha de comunicação na metade de um ciclo completo de inventário para verificar se o sistema gerencia o estado da aplicação WPF corretamente, mantendo a integridade do processo até que a conexão seja reestabelecida e o lote seja totalmente consolidado no ecossistema remoto.

* **Testes de Aceitação:** Validação final realizada mediante os critérios de aceitação estipulados nas histórias de usuário, garantindo que o sistema atenda às expectativas operacionais e de negócio do cenário logístico.
  * *Exemplo 1 (Consulta de Volumetria por Operador):* Simular a rotina diária de um operador de armazém utilizando os critérios de aceitação da história de usuário, validando se a interface WPF permite buscar, filtrar e exibir a volumetria consolidada de um lote específico de produtos em menos de 3 segundos, assegurando agilidade na tomada de decisão no estoque.
  * *Exemplo 2 (Validação de Permissão e Perfil de Acesso):* Executar o cenário de aceitação onde um operador tenta acessar funções administrativas restritas (como inativação de usuários ou alteração de parâmetros globais do sistema), verificando se a interface bloqueia o acesso e exibe a mensagem de restrição conforme estipulado nos critérios de aceitação da história de usuário correspondente.

* **Testes Baseados em Cenários:** Execução de roteiros práticos fundamentados em rotinas reais do dia a dia de um estoque logístico, simulando o comportamento conjugado de operadores e administradores ao longo de um fluxo operacional completo.
  * *Exemplo 1 (Turno Operacional Completo):* Simular um ciclo operacional onde o administrador acessa o sistema para cadastrar novos operadores e atualizar permissões de acesso, gerando automaticamente notificações integradas para os parceiros comerciais. Em seguida, alternar o perfil para o operador, que executa lotes sucessivos de captura volumétrica tridimensional via Kinect de múltiplas medições no estoque, validando a atualização em tempo real do estoque local (SQL Server) e a reflexão imediata dos dados consolidados no dashboard web via SignalR.
  * *Exemplo 2 (Cenário de Gestão de Divergência e Inventário Periódico):* Simular uma rotina na qual um volume de resíduos armazenados sofre recontagem volumétrica e o sistema cruza os dados capturados com o estoque cadastrado anteriormente, validando se a interface WPF alerta o operador sobre divergências e se o processo de ajuste e sincronização com a nuvem Microsoft Azure ocorre sem interrupções operacionais.
    
* **Testes Exploratórios:** Investigação dinâmica e livre realizada pela equipe de desenvolvimento e testes para identificar vulnerabilidades, exceções não tratadas (*unhandled exceptions*) ou comportamentos inesperados não mapeados nos roteiros formais do sistema.
  * *Exemplo 1 (Validação de Robustez em Campos de Entrada e Formulários):* Navegar livremente pelas telas da aplicação WPF inserindo dados fora do padrão esperado — como caracteres especiais, SQL Injection simulado, emojis ou strings excessivamente longas — nos campos de login, busca de resíduos e formulários de cadastro, com o objetivo de identificar falhas de validação, comportamentos instáveis ou exceções não tratadas (*unhandled exceptions*).
  * *Exemplo 2 (Estresse de Navegação e Concorrência de Tela):* Executar ações de forma rápida, aleatória e fora da sequência lógica padrão (como clicar repetidamente em botões de salvamento durante requisições assíncronas, fechar janelas abruptamente enquanto há sincronização com o Azure ou alternar perfis de usuário de maneira frenética) para testar a estabilidade da interface e a resiliência do ecossistema local e remoto.

* **Reteste:** Execução direcionada e controlada para certificar a efetividade da correção de um defeito reportado após a intervenção no código-fonte do *Inventory Masters*.
  * *Exemplo 1 (Validação de Correção de Alerta de Acesso):* Reexecutar o cenário de envio de token para um e-mail não cadastrado após a correção do bug da mensagem genérica, validando se agora o sistema exibe o alerta específico informando que o usuário não possui acesso.
  * *Exemplo 2 (Validação de Correção no Cálculo Volumétrico):* Executar novamente o processo de captura e conversão volumétrica de resíduos no sensor Kinect após um ajuste no algoritmo de conversão para $m^3$, certificando-se de que o erro de arredondamento anterior foi corrigido e o valor é persistido corretamente no banco de dados local.

* **Testes de Regressão:** Reexecução sistemática de cenários já validados anteriormente para garantir que atualizações, correções de bugs ou novas implementações não introduziram novos defeitos ou efeitos colaterais em módulos estáveis do sistema.
  * *Exemplo 1 (Impacto na Persistência e Autenticação):* Após aplicar uma atualização no módulo de persistência do SQL Server, reexecutar o fluxo de login e validação de token para garantir que o mecanismo de autenticação continua funcionando perfeitamente sem efeitos colaterais.
  * *Exemplo 2 (Regressão na Sincronização e Tempo Real):* Após atualizar a biblioteca de comunicação em nuvem ou o *hub* do SignalR, reexecutar a rotina de envio de dados de resíduos para o dashboard web, certificando-se de que a entrega em tempo real e a integridade da sincronização com o Microsoft Azure permaneceram intactas e sem falhas colaterais.
---

## 6. Técnicas de Projeto de Testes

A elaboração dos casos de teste do *Inventory Masters* fundamenta-se em técnicas estruturadas de projeto, garantindo alta eficiência na detecção de falhes e o alinhamento rigoroso com as especificações do ecossistema tecnológico:

* **Particionamento de Equivalência:** Técnica aplicada para segmentar os dados de entrada em classes de equivalência válidas e inválidas (como a inserção de dimensões volumétricas em metros cúbicos ou a contagem de resíduos no estoque), permitindo a validação de comportamento correto com um conjunto reduzido e representativo de casos de teste.
* **Análise de Valor Limite:** Estratégia direcionada aos extremos das faixas de operação e medição do sistema (como os limites mínimos e máximos de alcance volumétrico suportados pelo sensor/Kinect ou os limiares de paginação de registros na interface WPF), focando na detecção de falhas típicas nas bordas das condições lógicas.
* **Tabela de Decisão:** Metodologia utilizada para estruturar combinações complexas de regras de negócio condicionais (como a matriz de permissões de acesso e ações permitidas por perfil de usuário — administrador versus operador — combinada com o status dos resíduos armazenados).
* **Testes Baseados em Cenários:** Utilizados para modelar e executar a jornada operacional completa do operador de armazém utilizando o ecossistema do *Inventory Masters*, simulando fluxos transacionais do mundo real.
* **Testes Exploratórios:** Aplicados em sessões livres e dinâmicas para avaliar a robustez da interface WPF, o comportamento sob concorrência de requisições e a experiência de usabilidade geral da aplicação.
---

## 7. Casos de Teste (Organizados e Reenumerados)

| ID | Funcionalidade | Cenário | Entrada | Resultado Esperado | Resultado Obtido | Status | Responsável |
| :--- | :--- | :--- | :--- | :--- | :--- | :--- | :--- |
| CT-001 | Autenticação (Login) | Credenciais válidas | E-mail e senha cadastrados corretamente | O sistema gera e envia o token temporário, permitindo o acesso bem-sucedido ao painel principal | — | Pendente | Analista de Qualidade |
| CT-002 | Autenticação (Login) | E-mail não cadastrado (Reteste) | E-mail inexistente no sistema | O sistema bloqueia o acesso e exibe a mensagem de alerta específica informando que o usuário não possui acesso | — | Pendente | Analista de Qualidade |
| CT-003 | Autenticação (Login) | Senha incorreta | Senha divergente para um e-mail válido cadastrado | O sistema rejeita o acesso e exibe a mensagem de falha de credenciais | — | Pendente | Analista de Qualidade |
| CT-004 | Captura Volumétrica (Kinect) | Objeto dentro dos parâmetros válidos | Posicionamento correto do resíduo diante do sensor Kinect | O sistema calcula o volume e exibe o resultado convertido em metros cúbicos ($m^3$) na interface WPF em menos de 2 segundos | — | Pendente | Analista de Testes / Automação |
| CT-005 | Captura Volumétrica (Kinect) | Limite mínimo de captação do Kinect | Posicionamento de objeto exatamente no limiar mínimo de detecção suportado pelo sensor | O sistema realiza a leitura com sucesso ou identifica o limite inferior com precisão | — | Pendente | Analista de Testes / Automação |
| CT-006 | Captura Volumétrica (Kinect) | Correção de arredondamento volumétrico (Reteste) | Reexecução da conversão de leitura do Kinect após ajuste no algoritmo para $m^3$ | O erro de arredondamento anterior é eliminado e o valor correto é persistido no banco local | — | Pendente | Analista de Qualidade |
| CT-007 | Dados Volumétricos | Dimensões volumétricas válidas | Inserção de valores numéricos positivos dentro da faixa permitida para o resíduo | O sistema aceita a entrada e processa corretamente o cálculo em metros cúbicos ($m^3$) | — | Pendente | Analista de Qualidade |
| CT-008 | Dados Volumétricos | Dimensões volumétricas inválidas | Inserção de valores negativos ou texto alfanumérico nos campos de tamanho | O sistema bloqueia a transação e exibe mensagens de validação de formato inválido | — | Pendente | Analista de Qualidade |
| CT-009 | Integração Local e Nuvem | Atualização de estoque no SQL Server | Alteração de dados de resíduos na aplicação local | O registro é persistido com integridade no SQL Server, sincronizado com o Microsoft Azure e refletido instantaneamente via SignalR no dashboard web | — | Pendente | Analista de Integração |
| CT-010 | Integração Local e Nuvem | Impacto em tempo real pós-atualização | Reexecução do envio de dados de resíduos após atualização no ecossistema de nuvem e SignalR | A entrega em tempo real para o dashboard web continua funcionando perfeitamente sem efeitos colaterais | — | Pendente | Analista de Regressão |
| CT-011 | Cadastro de Usuários / Estoque | Preenchimento de campos obrigatórios | Inserção de dados válidos para cadastro de novo usuário ou item de resíduo | O sistema valida as informações e salva o registro com sucesso na listagem correspondente | — | Pendente | Analista de Qualidade |
| CT-012 | Gestão de Estoque | Limite máximo de paginação de estoque | Solicitação do limite máximo de registros permitidos por página na grade WPF | O sistema exibe exatamente a quantidade limite de itens sem corromper o layout | — | Pendente | Analista de Qualidade |
| CT-013 | Gestão de Estoque | Turno operacional completo | Execução sequencial de cadastro de operadores, notificações a parceiros e contagem volumétrica sucessiva de resíduos | O fluxo completo do turno ocorre sem gargalos, consolidando o estoque localmente e atualizando o dashboard via SignalR | — | Pendente | Analista de Integração |
| CT-014 | Gestão de Estoque | Gestão de divergência em inventário periódico | Recontagem volumétrica de um lote de resíduos gerando divergência em relação ao estoque anterior | A interface WPF alerta o operador sobre a discrepância e executa o ajuste com sincronização Azure sem interrupções | — | Pendente | Analista de Integração |
| CT-015 | Controle de Acesso (Permissões) | Matriz de Permissões (Administrador) | Acesso a funções restritas com perfil de Administrador | O sistema concede acesso total às operações de cadastro, gerenciamento de usuários e parâmetros globais | — | Pendente | Analista de Sistemas |
| CT-016 | Controle de Acesso (Permissões) | Matriz de Permissões (Operador) | Tentativa de acesso a funções administrativas utilizando o perfil de Operador | O sistema bloqueia o acesso, restringe as opções visíveis e exibe alerta de permissão insuficiente | — | Pendente | Analista de Sistemas |
| CT-017 | Teste de Aceitação | Consulta de volumetria por perfil operador | Solicitação de busca de lote de resíduos na interface WPF | O sistema retorna e exibe a volumetria consolidada do lote em menos de 3 segundos, atendendo à história de usuário | — | Pendente | Product Owner / QA |
| CT-018 | Teste de Sistema | Fluxo operacional completo de inventário (Ponta a Ponta) | Captura volumétrica tridimensional de resíduos via Kinect e confirmação de salvamento | Execução integrada desde a leitura local, processamento em $m^3$, transação no SQL Server até a consolidação remota no ecossistema Azure e dashboard | — | Pendente | Analista de Sistemas |
| CT-019 | Teste Não Funcional | Desempenho e Vazamento de Memória | Execução contínua da aplicação WPF por 2 horas | O sistema mantém a estabilidade de renderização sem apresentar *memory leak* ou lentidão excessiva nas telas | — | Pendente | Analista de Desempenho |
| CT-020 | Teste Não Funcional | Confiabilidade de Rede (Resiliência) | Queda abrupta de conexão com a internet durante a sincronização de dados | O sistema armazena as informações em fila local sem corromper o banco SQL Server ou travar a aplicação | — | Pendente | Analista de Desempenho |
| CT-021 | Teste Exploratório | Robustez contra entradas inválidas/maliciosas | Inserção de caracteres especiais, emojis ou strings longas em campos de busca | O sistema lida com a entrada de forma segura, prevenindo comportamentos inesperados ou exceções não tratadas (*unhandled exceptions*) | — | Pendente | Analista Exploratório |
| CT-022 | Teste Exploratório | Estresse de navegação e concorrência | Cliques repetidos em botões de salvamento e fechamento abrupto de janelas durante requisições assíncronas | O sistema mantém a estabilidade da interface WPF e preserva a integridade transacional | — | Pendente | Analista Exploratório |
| CT-023 | Teste de Regressão | Verificação pós-atualização de persistência | Execução do fluxo de login após atualização no módulo SQL Server | O mecanismo de autenticação e validação de token continua funcionando perfeitamente sem efeitos colaterais em módulos estáveis | — | Pendente | Analista de Regressão |

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








