# Plano de Testes — Inventory Masters

## 1. Identificação do Projeto
* **Nome do sistema:** Inventory Masters
* **Versão:** 1.0.0
* **Equipe responsável:** Equipe de Desenvolvimento Inventory Masters
* **Data do planejamento:** 08/08/2026

---

## 2. Objetivos dos Testes
* **O que a equipe pretende verificar?** Verificar se o sistema *Inventory Masters* atinge os requisitos funcionais e não funcionais estabelecidos, garantindo a integridade nas operações de inventário, a precisão das medições volumétricas (capturadas via sensor/Kinect), a correta sincronização entre o banco de dados local (SQL Server) e a nuvem (Microsoft Azure), além de assegurar a estabilidade das aplicações WPF em MVVM[cite: 1, 2].
* **Quais riscos pretende reduzir?** Falhas de sincronização de estoque entre o ambiente local e a nuvem, distorções nas medições volumétricas causadas por calibração incorreta ou interferências externas, indisponibilidade do banco de dados SQL Server ou da infraestrutura Azure, e falhas de autenticação ou controle de acesso dos operadores[cite: 1, 2].
* **O que será considerado como evidência de qualidade?** A execução bem-sucedida dos casos de teste prioritários com seus respectivos resultados obtidos documentados, ausência de defeitos críticos ou bloqueantes não resolvidos, logs de execução limpos, estabilidade comprovada em cenários de estresse de dados e relatórios de validação dos dados volumétricos[cite: 1, 2].

---

## 3. Escopo
* **O que será testado:**
  * Módulo de autenticação e controle de acesso de usuários.
  * Funcionalidades de cadastro e gerenciamento de itens do inventário.
  * Integração de captura volumétrica e processamento de dados via sensor.
  * Persistência e integridade das informações no banco de dados local (SQL Server).
  * Mecanismo de sincronização de dados com o ambiente em nuvem (Microsoft Azure).
  * Tratamento de entradas inválidas e mensagens de erro na interface WPF.
* **O que não será testado:**
  * Desempenho e falhas físicas nativas do hardware de terceiros (como falhas de fabricação internas do sensor Kinect ou instabilidades globais da infraestrutura do provedor de nuvem Microsoft Azure).
  * Integrações com sistemas ERP externos legados que não façam parte do escopo atual do TCC.
* **Funcionalidades prioritárias:**
  1. Processo de login e permissões de acesso.
  2. Captura e cálculo volumétrico de itens no armazém.
  3. Sincronização de dados críticos entre o SQL Server local e o Microsoft Azure.
  4. Fluxo de cadastro e atualização de estoque.

---

## 4. Base de Teste
As informações e artefatos utilizados para projetar os testes englobam:
* Requisitos funcionais e não funcionais documentados do TCC.
* Regras de negócio voltadas à gestão de estoque e medição volumétrica.
* Histórias de usuário e critérios de aceitação mapeados para o *Inventory Masters*.
* Especificações técnicas da arquitetura WPF (MVVM) e dos bancos de dados (SQL Server e Microsoft Azure).
* Documentação de arquitetura e diagramas de fluxo do sistema.

---

## 5. Abordagem de Testes
As seguintes abordagens serão adotadas durante o ciclo de qualidade do sistema[cite: 1, 2]:
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








