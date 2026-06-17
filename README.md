# INVENTORY MASTERS - SOLUÇÕES INTELIGENTES EM MAPEAMENTO DE ESTOQUE
---
**Unidade SENAI:** Nova Lima  
**Instrutor:** Frederico Martins Aguiar

<div align="center">

## INTEGRANTES DO GRUPO

<p align="center">
  <img src="./Imagens/Equipe.jpeg" width="600" alt="Equipe Inventory Masters" />
</p>

</div>

| Nome | Curso| Especialidade no Projeto |
| :--- | :--- | :--- |
| **Danilo Silva Santos** | Programação de Sistemas | Desenvolvimento e Integração do Sensor Kinect|
| **Marilene da Silva Araujo** | Programação de Sistemas | Desenvolvimento, e Modelagem de Banco de Dados |
| **Miguel Cassio Braga Duarte** |Programação de Sistemas | Desenvolvimento e Lógica do negócio |
| **Diulie Mileide Batista Correia** |Programação de Sistemas | Desenvolvimento e Documentação |

</div>

---

##  Quem somos!

A Inventory Masters é uma plataforma tecnológica dedicada à gestão inteligente de excedentes produtivos.
Atuamos conectando empresas a soluções estratégicas de reaproveitamento de materiais.
Transformamos desperdícios em ativos com potencial de geração de valor econômico.
Promovemos redução de custos, eficiência operacional e responsabilidade ambiental.
Somos inovação aplicada à gestão sustentável e competitiva.

<p align="center">
  <img src="./Imagens/logo.png" width="600" alt="Logo Inventory Masters" />
</p>

-------

## PROBLEMA 

O cenário empresarial atual é caracterizado por elevados níveis de produção e, consequentemente, pela criação contínua de excedentes produtivos. Esses excedentes englobam sobras de matéria-prima, materiais que não atendem aos padrões comerciais, resíduos operacionais e insumos não aproveitados integralmente durante o processo produtivo. Na maioria das organizações, tais materiais não são monitorados de maneira estratégica, sendo erroneamente classificados apenas como resíduos ou custos inevitáveis.

A ausência de sistemas organizados de rastreabilidade e gerenciamento desses excedentes gera impactos consideráveis:

* **Sob a ótica econômica:** as empresas enfrentam prejuízos financeiros decorrentes do desperdício de recursos, da gestão ineficiente de estoques e da destinação imprópria de materiais reutilizáveis.
* **No âmbito ambiental:** o descarte inadequado acelera o acúmulo de resíduos sólidos, sobrecarrega os aterros sanitários e exerce maior pressão sobre os recursos naturais.

Além disso, observa-se que muitas organizações enfrentam dificuldades para integrar práticas sustentáveis às suas rotinas operacionais de maneira eficiente e mensurável. Embora a economia circular seja amplamente discutida como modelo estratégico para o desenvolvimento sustentável, sua aplicação prática ainda é limitada pela escassez de ferramentas tecnológicas acessíveis e integradas à gestão empresarial.

Diante desse contexto, surge a necessidade de soluções inovadoras que permitam transformar excedentes produtivos em ativos econômicos, promovendo a redução de custos, a geração de novas receitas e o fortalecimento da responsabilidade socioambiental corporativa.

É nesse cenário que se insere a proposta da **Inventory Masters**: uma plataforma tecnológica voltada para a gestão inteligente, rastreabilidade e direcionamento estratégico de excedentes produtivos em múltiplos setores da economia. A solução busca estruturar um modelo operacional capaz de conectar empresas geradoras de excedentes a oportunidades de reaproveitamento, criando um ecossistema de valorização de materiais antes subutilizados ou descartados.

Assim, o presente estudo se justifica pela necessidade de desenvolver mecanismos práticos que integrem eficiência operacional, inovação tecnológica e sustentabilidade, contribuindo para a consolidação de modelos empresariais mais competitivos e alinhados às demandas ambientais contemporâneas.

---

## SOLUÇÃO

A **Inventory Masters** é uma plataforma tecnológica desenvolvida para a gestão estratégica e inteligente de excedentes produtivos em diferentes setores da economia. Seu propósito é oferecer às organizações um sistema estruturado de rastreabilidade, controle e direcionamento de materiais que, tradicionalmente, seriam tratados apenas como descarte operacional.

Por meio de monitoramento sistematizado, organização de dados e análise de fluxos produtivos, a plataforma identifica resíduos e sobras operacionais com potencial de reaproveitamento, promovendo sua reinserção estratégica na cadeia produtiva. Dessa forma, materiais antes considerados perdas passam a ser reconhecidos como **ativos capazes de gerar valor econômico**, otimizar processos e reduzir desperdícios.

A plataforma atua como um elo integrador entre empresas geradoras de excedentes e parceiros aptos a reutilizá-los, estruturando um ecossistema colaborativo orientado à eficiência operacional e à sustentabilidade empresarial. Ao conectar a oferta e a demanda de materiais reaproveitáveis, a solução contribui simultaneamente para:

* **Redução de custos** operacionais e de descarte;
* **Melhoria da performance** organizacional;
* **Mitigação de impactos ambientais** negativos.

Mais do que uma iniciativa sustentável, a proposta configura-se como um modelo escalável de inovação aplicada à gestão empresarial, alinhado às tendências contemporâneas de responsabilidade socioambiental, competitividade de mercado e transformação digital.

---

### ÁREA TECNOLÓGICA DA SOLUÇÃO
A solução Inventory Masters está inserida no ecossistema da **Indústria 4.0**, convergindo tecnologias de hardware e software para a automação de processos. As principais áreas abrangidas são:
* **Visão Computacional:** Utilização do sensor infravermelho e câmera de profundidade (RGB-D) para o mapeamento volumétrico de objetos e espaços.
* **Internet das Coisas (IoT):** Integração de sensores físicos com uma interface digital para monitoramento em tempo real.
* **Sistemas de Informação:** Processamento de dados via plataforma **.NET 8** e armazenamento estruturado em **SQLite**, permitindo a rastreabilidade completa dos excedentes.

### JUSTIFICATIVA
A implementação deste projeto justifica-se pela ineficiência dos métodos tradicionais de inventário manual, que são lentos, propensos a erros humanos e caros. No cenário de sustentabilidade atual, empresas que não monitoram seus excedentes perdem duas vezes: financeiramente (pelo valor do material parado) e ambientalmente (pelo descarte inadequado). 
A Inventory Masters oferece uma alternativa de **baixo custo** ao utilizar hardware legado (Kinect), democratizando o acesso à tecnologia de ponta para pequenas e médias empresas que buscam se adequar à economia circular e reduzir perdas operacionais.

### OBJETIVOS

**Objetivo Geral:**
Desenvolver e implementar uma plataforma automatizada de mapeamento volumétrico para a gestão inteligente e direcionamento estratégico de excedentes produtivos.

**Objetivos Específicos:**
* Configurar a integração entre o hardware Kinect Xbox 360 e o ambiente de desenvolvimento C# (.NET).
* Criar um algoritmo capaz de converter os dados de profundidade do sensor em métricas de volume (m³).
* Desenvolver um sistema de alertas automáticos para notificação de parceiros quando o estoque atingir níveis críticos.
* Reduzir o tempo de resposta na destinação de materiais, conectando a oferta (excedente) à demanda (parceiros) de forma ágil.

### DESENVOLVIMENTO
O desenvolvimento do projeto foi estruturado em fases cíclicas para garantir a precisão técnica e a usabilidade do sistema:

1.  **Levantamento de Requisitos e Modelagem:** Nesta etapa, foram definidos os diagramas de Caso de Uso e Fluxo de Dados para entender como a medição se transforma em notificação.
2.  **Arquitetura de Dados:** Criação do modelo relacional no banco de dados para garantir que cada medição de volume esteja vinculada a uma origem e que cada notificação seja registrada para auditoria.
3.  **Integração do Sensor:** Utilização do *Microsoft Kinect SDK 1.8* para extrair a "nuvem de pontos" (point cloud) do ambiente, permitindo ao sistema "enxergar" o volume ocupado no estoque.
4.  **Desenvolvimento da Interface Web:** Construção do Dashboard utilizando ASP.NET Core Razor Pages, onde o operador visualiza o status do estoque e configura os parâmetros de alerta.
5.  **Testes e Calibração:** Ajuste da sensibilidade do sensor para diferentes tipos de materiais e validação do envio de e-mails/alertas automáticos.
---
# REGRA DE NEGOCIO

As regras de negócio definem o comportamento esperado para garantir a precisão logística, a integridade do hardware e a reatividade do sistema.
Abaixo, elenco as regras de negócio agrupadas por domínio técnico-operacional:

#### 1. Regras de Gestão de Estoque e Parâmetros
* **RN01 - Validação de Limites:** Toda medição deve ser comparada com os limites de capacidade (Mínima/Máxima) configurados pelo administrador (`UC30`, `UC31`).
* **RN02 - Persistência Segura:** Configurações de parâmetros não podem ser salvas sem validação prévia de consistência de dados no Firestore (`UC32`, `UC36`).
* **RN03 - Vinculação de Entidade:** O sistema deve garantir que toda operação esteja vinculada a um parceiro. Caso não haja um parceiro ativo na sessão, o sistema deve utilizar o "Parceiro Padrão" configurado (`UC35`).

#### 2. Regras de Processamento de Visão Computacional (Kinect)
* **RN04 - Critério de Aceitação da Malha:** Uma medição só pode ser considerada válida se a densidade da nuvem de pontos for suficiente, eliminando medições com oclusões (`UC29`).
* **RN05 - Padronização de Unidade:** O sistema deve converter obrigatoriamente os dados brutos de $cm^3$ para $m^3$ antes de qualquer cálculo volumétrico ou broadcast para dashboards (`UC61`).
* **RN06 - Rastreabilidade Espacial:** Toda medição válida deve gerar um *snapshot* do estado do sensor no momento da captura para fins de auditoria (`UC19`).
* **RN07 - Limpeza de Memória:** O sistema deve executar rotinas de limpeza de *buffer* pós-processamento para evitar *memory leaks* decorrentes da alta demanda de processamento 3D (`UC39`).

#### 3. Regras de Integração Reativa (SignalR)
* **RN08 - Broadcast Obrigatório:** Sempre que uma nova medição for validada e processada, o sistema deve atualizar todos os clientes conectados (Dashboards) em tempo real, sem necessidade de *refresh* manual (`UC62`).
* **RN09 - Resiliência e Cache:** Em caso de perda de conectividade com o servidor, os clientes devem manter os dados em cache local até que a conexão seja reestabelecida (`UC58`).
* **RN10 - Ciclo de Vida da Conexão:** O sistema deve gerenciar ativamente as conexões do SignalR, identificando eventos de `OnConnected` e `OnDisconnected` para garantir que apenas clientes ativos recebam atualizações (`UC64`).

#### 4. Regras de Segurança e Acesso
* **RN11 - Segregação de Perfis:**
    * **Admin:** Acesso a relatórios históricos e alteração de parâmetros (`UC10`, `UC26`, `UC31`).
    * **Operador:** Operação de hardware e monitoramento em tempo real (`UC18`, `UC21`, `UC28`).
* **RN12 - Autenticação no Hub:** A conexão com os Hubs de integração (SignalR) exige validação de token JWT para evitar acesso não autorizado a dados industriais (`UC57`).
* **RN13 - Log de Segurança:** Qualquer tentativa de acesso não autorizado aos serviços de integração deve ser registrada em logs de auditoria de segurança (`UC60`).

#### 5. Regras de Manutenção de Hardware
* **RN14 - Monitoramento de Saúde:** O sistema deve verificar a saúde do Kinect (diagnóstico de conectividade) em cada ciclo de medição (`UC37`).
* **RN15 - Tratamento de Erros:** Falhas físicas (ex: desconexão do cabo) devem ser registradas como logs de erro no Firestore para permitir o diagnóstico técnico remoto (`UC38`).
* **RN16 - Auto-recuperação:** O sistema deve tentar o re-handshake automático com o hardware antes de notificar o erro ao operador (`UC50`).

### Fluxo de Processamento de Negócio

O sistema opera através de um **fluxo determinístico**, garantindo que apenas dados validados alcancem a interface de monitoramento. Caso ocorra uma falha em qualquer etapa, o processo é interrompido para evitar inconsistências nos dados de estoque.

#### Etapas do Processo:
1. **Verificação de Hardware:** Diagnóstico do sensor e plano de referência (`UC37`, `UC45`).
2. **Captura e Filtro:** Coleta dos dados espaciais e remoção de ruídos (`UC23`, `UC42`).
3. **Cálculo e Conversão:** Processamento da malha 3D e conversão de unidade ($cm^3 \rightarrow m^3$) (`UC24`, `UC61`).
4. **Validação de Limites:** Comparação do volume obtido com as capacidades configuradas (`UC30`).
5. **Persistência e Broadcast:** Salvamento no Firestore e atualização em tempo real dos Dashboards via SignalR (`UC32`, `UC62`).

> **Nota de Integridade:** Qualquer falha detectada durante estas etapas interrompe imediatamente a propagação do dado, assegurando que o Dashboard exiba apenas informações íntegras, precisas e validadas.
---
## Requisitos do Sistema

#### 1. Requisitos Funcionais (RF)

| Categoria | ID | Requisito Funcional | Casos de Uso |
| :--- | :--- | :--- | :--- |
| **Gestão** | RF01 | Cadastro, edição, exclusão e listagem de usuários/parceiros. | UC01-UC07 |
| | RF02 | Gestão de permissões e perfis de acesso. | UC08, UC09 |
| | RF03 | Auditoria de alterações via logs. | UC10, UC44, UC49 |
| **Medição** | RF04 | Captura volumétrica via Kinect e processamento de nuvem de pontos. | UC21, UC23, UC24 |
| | RF05 | Calibração de sensor e verificação de estabilidade da malha 3D. | UC28, UC29, UC45 |
| | RF06 | Conversão de medições brutas ($cm^3$) para ($m^3$). | UC61 |
| **Monitoramento**| RF07 | Painel em tempo real de ocupação e volume. | UC51, UC52 |
| | RF08 | Disparo de alertas baseados em regras de limite. | UC14, UC17, UC30, UC34, UC55 |
| **Integração** | RF09 | Comunicação assíncrona via SignalR (Hubs). | UC53, UC62, UC64 |
| | RF10 | Persistência de dados no Firestore. | UC32, UC36, UC46 |

#### 2. Requisitos Não Funcionais (RNF)

| ID | Tipo | Requisito Não Funcional | Casos de Uso |
| :--- | :--- | :--- | :--- |
| **RNF01** | Desempenho | Processamento de malha 3D e atualização em tempo real (baixa latência). | - |
| **RNF02** | Resiliência | Auto-recuperação (re-handshake) e cache local para evitar perda de dados. | UC50, UC58 |
| **RNF03** | Segurança | Autenticação JWT e criptografia de payloads. | UC57, UC59 |
| **RNF04** | Confiabilidade | Diagnóstico contínuo de hardware e gestão de memória. | UC37, UC39 |
| **RNF05** | Manutenibilidade| Uso de Injeção de Dependência para desacoplamento. | UC63 |

 ---
# MODELAGEM DO SISTEMA

## Diagrama de Caso de Uso

### O diagrama de caso de uso, descreve as funcionalidades do sistema, segregadas por módulos e níveis de responsabilidade, conforme definido na arquitetura.

<p align="center">
  <img src="./Imagens/InventoryMastersUC_MVC.png" width="800" alt="Diagrama de Caso de Uso" />
</p>

### Tabela Consolidada de Casos de Uso

| **ID** | **Nome da Funcionalidade** | **Perfil** | **Descrição** |
| :--- | :--- | :--- | :--- |
| **UC01** | Listar Registros (Pag) | Admin | Listagem paginada de usuários ou parceiros. |
| **UC02** | Filtrar Dados | Admin | Filtros avançados para busca de usuários ou parceiros. |
| **UC03** | Cadastro de Entidade | Admin | Inclusão de novos usuários/parceiros (com validação). |
| **UC04** | Buscar por ID | Admin | Localização de registros específicos via identificador. |
| **UC05** | Atualizar/Excluir | Admin | Manutenção, edição e remoção de registros cadastrais. |
| **UC06** | Visualizar Detalhes | Admin | Exibição detalhada de um registro selecionado. |
| **UC07** | Atualizar Detalhes Parceiro | Admin | Edição de informações específicas de parceiros. |
| **UC08** | Inclusão de Perfil | Admin | Associação de níveis de acesso aos usuários. |
| **UC09** | Gestão de Acessos | Admin | Controle de permissões do sistema. |
| **UC10** | Auditoria de Logs | Admin | Rastreamento de alterações realizadas no sistema. |
| **UC11** | Visualizar Histórico | Admin | Exibição do histórico de notificações do sistema. |
| **UC12** | Filtrar Notificações | Admin | Segmentação de alertas por data ou status. |
| **UC13** | Aceitar Coleta | Admin | Registro de aceite de coleta via repositório. |
| **UC14** | Notificar Clientes | Sistema | Broadcast de alertas via SignalR (NotificacaoHub). |
| **UC15** | Integrar Perfil Repos. | Sistema | Conexão entre módulo de notificação e repositório. |
| **UC16** | Verificar Pendências | Admin | Verificação de notificações e tarefas pendentes. |
| **UC17** | Gestão de Alertas Visuais | Admin | Configuração de gatilhos de alerta no Dashboard. |
| **UC18** | Monitorar Ocupação | Operador | Acompanhamento em tempo real da ocupação do espaço. |
| **UC19** | Registrar Snapshot Espacial | Sistema | Armazenamento de estado da captura para auditoria. |
| **UC20** | Validação de Conexões | Sistema | Monitoramento de handshake entre cliente e servidor. |
| **UC21** | Iniciar Medição | Operador | Ativação da captura de dados pelo sensor Kinect. |
| **UC22** | Visualizar Fluxo Profund. | Operador | Monitoramento visual do fluxo de profundidade. |
| **UC23** | Processar Nuvem Pontos | Sistema | Validação e filtro de dados espaciais brutos. |
| **UC24** | Gerar Malha 3D | Sistema | Processamento geométrico para cálculo de volume. |
| **UC25** | Histórico Medições | Admin | Consulta de logs e medições passadas. |
| **UC26** | Exportar Dados | Admin | Exportação de relatórios volumétricos e periódicos. |
| **UC27** | Relatório de Período | Admin | Geração de análise de volume por intervalo de tempo. |
| **UC28** | Calibração de Sensor | Operador | Rotina de ajuste e definição do plano de referência. |
| **UC29** | Verificação de Estabilidade de Malha | Sistema | Confirma que a malha gerada possui densidade suficiente para um cálculo preciso, evitando erros de leitura por oclusão. |
| **UC30** | Validação de Regras de Limite | Sistema | Compara o volume calculado com os parâmetros de "capacidade máxima" definidos pelo Admin antes de disparar alertas. |
| **UC31** | Ajustar Parâmetros | Admin | Configuração de limites mínimos e máximos de estoque. |
| **UC32** | Persistir Configurações | Sistema | Validação e salvamento de parâmetros no Firestore. |
| **UC33** | Adicionar Parceiro | Admin | Inclusão de novo parceiro via módulo de parâmetros. |
| **UC34** | Ativar Alerta Dash | Admin | Ativação de alertas visuais no painel de controle. |
| **UC35** | Definir Parceiro Padrão | Admin | Definição de entidade padrão para fluxos operacionais. |
| **UC36** | Persistir Configs | Sistema | Validação e salvamento de parâmetros no Firestore. |
| **UC37** | Diagnóstico de Conectividade | Sistema | Verifica em tempo real se a comunicação entre o Kinect e o módulo MVC está ativa. |
| **UC38** | Log de Erros de Hardware | Sistema | Registra falhas de hardware (ex: desconexão física do Kinect) no Firestore para análise do suporte. |
| **UC39** | Redefinição de Buffer de Dados | Sistema | Limpa o buffer de memória após medições concluídas para evitar vazamentos de memória (Memory Leak). |
| **UC40** | Validação de Snapshot | Sistema | Validação da integridade do snapshot capturado pelo Kinect. |
| **UC41** | Normalização de Dados | Sistema | Padronização dos dados brutos para cálculo volumétrico. |
| **UC42** | Tratamento de Ruído | Sistema | Remoção de interferências (ruído visual) na nuvem de pontos. |
| **UC43** | Cálculo de Superfície | Sistema | Determinação da área de topo do estoque para cálculo do volume. |
| **UC44** | Rastreamento de Auditoria | Sistema | Registro de logs de transações espaciais para rastreabilidade. |
| **UC45** | Verificação de Calibração | Operador | Check-up preventivo do estado de calibração do sensor. |
| **UC46** | Sincronização de Histórico | Sistema | Consistência entre medições locais e banco na nuvem. |
| **UC47** | Gerenciamento de Coletas | Admin | Controle de ciclo de vida de coletas realizadas por parceiros. |
| **UC48** | Auditoria de Medições | Admin | Verificação de conformidade das medições vs. capacidade. |
| **UC49** | Backup de Logs | Sistema | Rotina de persistência secundária de logs de erro. |
| **UC50** | Estabilização de Conexão | Sistema | Tratamento de re-handshake automático para o Kinect. |
| **UC51** | Visualizar Consolidado | Admin | Visão geral do volume e ocupação do estoque. |
| **UC52** | Monitorar Gráficos | Admin | Visualização de tendências e ocupação em tempo real. |
| **UC53** | Receber Atualizações | Sistema | Integração assíncrona (Hub) para o Dashboard. |
| **UC54** | Lista de Notificações | Admin | Exibição da lista de últimas notificações filtradas. |
| **UC55** | Configurar Alertas Visuais | Admin | Personalização de cores e gatilhos de alerta no Dashboard. |
| **UC56** | Acessar Parâmetros | Admin | Acesso rápido ao módulo de configuração. |
| **UC57** | Autenticação de Hub | Sistema | Validação de tokens de segurança JWT para acesso aos Hubs SignalR. |
| **UC58** | Cache de Estado Local | Sistema | Armazenamento temporário de medições no cliente para evitar perda de dados em micro-quedas de rede. |
| **UC59** | Criptografia de Payload | Sistema | Garantia de que os dados volumétricos estejam protegidos durante a transmissão via rede. |
| **UC60** | Log de Eventos de Segurança | Sistema | Registro de tentativas de acesso não autorizadas aos Hubs de integração. |
| **UC61** | Processar Medição Hub | Sistema | Conversão $cm^3 \rightarrow m^3$ e broadcast. |
| **UC62** | Broadcast Clientes | Sistema | Envio de nova medição para todos os Dashboards. |
| **UC63** | Injeção de Dependência | Sistema | Inicialização automática e resolução de instâncias de repositórios (Firestore/MVC) necessária para o ciclo de vida do SignalR Hub. |
| **UC64** | Gerenciar Conexões | Sistema | Controle de ciclo de vida das sessões (SignalR). |

--- 

## Diagrama de Fluxo 

<p align="center">
  <img src="./Imagens/Diagrama_de_Fluxo.png" width="600" alt="Diagrama de Fluxo Inventory Masters" />
</p>

### Detalhamento do Diagrama de Fluxo de Dados (1º Nível) 

#### Entidades Externas
* **Câmera / Visão Computacional:** Origem dos dados de volume (sensor).
* **Usuário (Adm/Op):** Define configurações de parâmetros e consome relatórios.
* **Parceiro:** Entidade externa que recebe alertas automáticos de excedentes.

#### Processos Principais

* **P1: Capturar e Processar Dados Kinect:** Recebe os frames de profundidade, gera a Point Cloud 3D, realiza o pré-processamento dos dados e calcula o Volume Medido.
* **P2: Calibrar e Mapear Espaço:** Executa a calibração do chão, identifica os limites do ambiente e calcula o Volume Total do Espaço.
* **P3: Calcular Ocupação e Validar Limites:** Calcula o volume ocupado pelos objetos presentes no ambiente, espaço livre, percentual de ocupação e verifica se o Volume Medido ultrapassa o Volume Máximo Permitido.
* **P4: Gerar Snapshot Espacial:** Cria registros periódicos contendo o estado atual do ambiente mapeado.
* **P5: Persistir Medições:** Armazena medições, snapshots e histórico de ocupação no banco SQLite.
* **P6: Gerenciar Notificações:** Responsável por identificar situações de excedente, consultar parceiros ativos e registrar os alertas gerados.
* **P7: Sincronizar Dados com MVC:** Envia volume atual, percentual de ocupação e status do sistema para a aplicação web através do SignalR.



#### Depósitos de Dados (Datastores)

* **D1: MedicoesVolume:** Histórico de medições calculadas pelo Kinect.
* **D2: EspacosMapeados:** Informações dos ambientes cadastrados, limites físicos e parâmetros operacionais.
* **D3: HistoricoOcupacao:** Registro histórico da evolução da ocupação do espaço.
* **D4: SnapshotsEspaciais:** Armazena capturas periódicas do estado do ambiente.
* **D5: Parceiros:** Cadastro de contatos responsáveis pelo recebimento de alertas.
* **D6: Notificacoes:** Log histórico de alertas e eventos gerados pelo sistema.


#### Detalhamento do Fluxo de Execução

1. **Cadastro do Espaço:** O usuário informa o nome do espaço e define o percentual de alerta desejado.
2. **Inicialização do Kinect:** O sistema ativa o sensor e inicia a captura dos dados de profundidade.
3. **Calibração do Ambiente:** O Kinect identifica o plano do chão e realiza o mapeamento estrutural do espaço.
4. **Geração da Point Cloud:** Os pontos tridimensionais são capturados e transformados em uma representação 3D do ambiente.
5. **Mapeamento do Volume Total:** O sistema calcula automaticamente o Volume Total do Espaço disponível.
6. **Definição do Limite Operacional:** O Volume Máximo Permitido é calculado com base no percentual de alerta definido pelo usuário.
7. **Captura e Persistência da Medição:** O Kinect calcula o Volume Medido e registra a leitura em **D1**.
8. **Cálculo da Ocupação:** O sistema calcula:
   * Volume Ocupado
   * Espaço Livre
   * Percentual de Ocupação
9. **Tomada de Decisão:** O sistema verifica:
   * **Volume Medido > Volume Máximo Permitido?**
   * **Se NÃO:** O fluxo continua normalmente, mantendo os registros históricos.
   * **Se SIM:** O sistema direciona o fluxo para o processo de notificações.
10. **Geração de Snapshot:** Um Snapshot Espacial é criado contendo o estado atual do ambiente.
11. **Persistência dos Dados:** As informações calculadas são armazenadas nos depósitos **D1**, **D3** e **D4**.
12. **Gerenciamento de Alertas:** Em situações de excedente, o sistema consulta os contatos cadastrados em **D5**, gera a notificação e registra a operação em **D6**.
13. **Sincronização Web:** O volume calculado e o status operacional são enviados para o sistema MVC via SignalR.
14. **Monitoramento Contínuo:** O processo permanece executando enquanto o Kinect estiver ativo, atualizando os cálculos, snapshots e verificações em tempo real.

---

## Diagrama de Sequência

<p align="center">
  <img src="./Imagens/Diagrama_de_Sequencia.png" width="600" alt="Diagrama de Sequência" />
</p>

### Detalhamento do Fluxo de Sequência

O fluxo inicia-se quando o sensor Kinect realiza uma nova captura do ambiente monitorado, seguindo uma sequência de processamento, persistência e sincronização em tempo real entre o módulo Kinect e a plataforma Web.

#### 1. Captura, Processamento e Persistência (Passos 2 e 3)

* O sensor **Kinect** captura os dados de profundidade (*Depth Frame*) do ambiente monitorado.
* O módulo de processamento converte os dados capturados em uma **nuvem de pontos tridimensional (Point Cloud)**, representando digitalmente o espaço ocupado pelos materiais.
* A partir da nuvem de pontos, o sistema realiza os cálculos geométricos necessários para determinar o **VolumeMedido** do estoque.
* Após a validação da leitura, os dados são armazenados localmente no banco **SQLite**, garantindo persistência e funcionamento mesmo em situações de indisponibilidade da rede.
* Em seguida, a medição é sincronizada com a aplicação Web por meio do **SignalR**, permitindo atualização imediata dos dados operacionais.

#### 2. Processamento das Regras de Negócio (Passos 4 e 5)

* Ao receber a medição, a aplicação consulta a coleção **parametrosSistema** para obter os limites volumétricos configurados.
* O sistema executa a lógica de negócio comparando o **VolumeMedido** com os parâmetros estabelecidos.
* Caso os valores estejam dentro da faixa operacional, a medição é registrada apenas para fins de histórico e monitoramento.

#### 3. Gestão de Exceções e Notificações (Passos 6, 7 e 8)

* Quando o volume registrado ultrapassa os limites definidos, o sistema identifica a ocorrência como um possível excedente produtivo.
* A aplicação consulta a coleção **parceiros** para localizar os contatos aptos a receber notificações.
* O evento é registrado na coleção **notificacoes**, garantindo rastreabilidade e auditoria do processo.
* Em seguida, os alertas são enviados automaticamente aos parceiros e operadores por meio dos canais configurados pela plataforma.

#### 4. Atualização Operacional e Inteligência de Dados (Passos 9, 10 e 11)

* Independentemente da ocorrência de excedentes, o status operacional é atualizado em tempo real para os usuários conectados através do **SignalR**.
* As informações recebidas alimentam o histórico de medições e os indicadores operacionais da plataforma.
* Os dados consolidados são processados para geração de relatórios, acompanhamento da ocupação do estoque e suporte à tomada de decisão, mantendo o Dashboard sempre atualizado com a situação atual do ambiente monitorado.
---

## MODELAGEM DO BANCO DE DADOS
---

Com a evolução arquitetural do projeto, a solução passou a utilizar uma abordagem híbrida de armazenamento de dados, combinando banco relacional local e banco NoSQL em nuvem.

O módulo responsável pela captura volumétrica via Kinect (MVVM) utiliza o banco local **SQLite**, garantindo baixo custo computacional, operação offline e persistência temporária das leituras do sensor.

Já a aplicação Web MVC, responsável pelo gerenciamento operacional, autenticação, dashboard, notificações e integração entre usuários e parceiros, passou a utilizar o **Firebase Firestore**, um banco de dados NoSQL orientado a documentos e altamente escalável.

Diferentemente da modelagem relacional tradicional, o Firestore organiza os dados em:
- Coleções;
- Documentos;
- Subcoleções;
- Referências por identificadores (IDs).

Essa abordagem elimina a necessidade de relacionamentos complexos e operações JOIN, proporcionando maior desempenho em aplicações distribuídas e em tempo real.

---

## Arquitetura de Persistência de Dados

### Módulo Kinect (MVVM)
- Banco Local: SQLite;
- Responsável por:
  - armazenamento temporário das medições;
  - persistência dos parâmetros do sensor;
  - funcionamento offline;
  - processamento local das leituras do Kinect.

### Aplicação MVC Web
- Banco em Nuvem: Firebase Firestore;
- Responsável por:
  - autenticação de usuários;
  - gerenciamento de parceiros;
  - dashboard operacional;
  - envio de notificações;
  - mensageria;
  - armazenamento centralizado das medições;
  - sincronização em tempo real.

---

## Modelagem NoSQL Firebase Firestore (MVC)

<p align="center">
  <img src="./Imagens/Modelagem_NoSQL_MVC.png" width="1200" alt="Modelagem NoSQL Firebase Firestore MVC" />
</p>

---

## Estrutura das Coleções Firestore

| Coleção | Finalidade |
|---|---|
| **usuarios** | Armazena os usuários cadastrados no sistema |
| **parceiros** | Mantém os parceiros responsáveis pelo reaproveitamento dos excedentes |
| **medicoes** | Registra todas as medições volumétricas enviadas pelo Kinect |
| **notificacoes** | Controla os alertas automáticos disparados pelo sistema |
| **mensagens** | Gerencia a comunicação entre usuários |
| **parametrosSistema** | Armazena as regras de negócio e limites volumétricos |

---

## Descrição das Coleções

### Coleção: usuarios

Responsável pelo armazenamento das informações de autenticação e controle de acesso da plataforma.

| Campo | Tipo |
|---|---|
| usuarioId | String |
| nome | String |
| email | String |
| perfil | String |
| ativo | Boolean |
| dataCadastro | Timestamp |

---

### Coleção: parceiros

Armazena as empresas parceiras aptas a receber excedentes produtivos.

| Campo | Tipo |
|---|---|
| parceiroId | String |
| nome | String |
| empresa | String |
| email | String |
| telefone | String |
| endereco | Map |
| ativo | Boolean |

---

### Coleção: medicoes

Responsável pelo armazenamento das medições volumétricas capturadas pelo Kinect ou inseridas manualmente.

### Coleção: medicoes


| Campo | Tipo |
|---|---|
| medicaoId | String |
| dataHora | Timestamp |
| volumeMedido | Number |
| volumeTotalEspaco | Number |
| volumeLivre | Number |
| percentualOcupacao | Number |
| quantidadePontos3D | Number |
| confiabilidadeLeitura | Number |
| origemLeitura | String |
| status | String |
| espacoId | Reference |


---

### Coleção: notificacoes

Registra os alertas automáticos enviados pelo sistema aos parceiros e operadores.

| Campo | Tipo |
|---|---|
| notificacaoId | String |
| mensagem | String |
| dataEnvio | Timestamp |
| statusEnvio | String |
| volumeMomento | Number |
| percentualOcupacao | Number |
| usuarioId | Reference |
| parceiroId | Reference |


---

### Coleção: mensagens

Responsável pela comunicação interna entre usuários da plataforma.

| Campo | Tipo |
|---|---|
| mensagemId | String |
| remetenteId | Reference |
| destinatarioId | Reference |
| texto | String |
| dataHora | Timestamp |
| lida | Boolean |

---

### Coleção: parametrosSistema

Define os parâmetros operacionais utilizados pelos gatilhos automáticos da aplicação.

| Campo | Tipo |
|---|---|
| volumeMaximo | Number |
| percentualAlerta | Number |
| emailNotificacaoAtivo | Boolean |
| intervaloCapturaSegundos | Number |
| intervaloSnapshotSegundos | Number |
| dataAtualizacao | Timestamp |

---

###  espacosMapeados

Representa os ambientes cadastrados e mapeados pelo Kinect.

| Campo | Tipo |
|---|---|
| espacoId | String |
| nomeEspaco | String |
| volumeTotalEspaco | Number |
| volumeMaximoPermitido | Number |
| percentualAlerta | Number |
| ativo | Boolean |
| dataMapeamento | Timestamp |

---

###  snapshotsEspaciais

Armazena os estados periódicos do ambiente.

| Campo | Tipo |
|---|---|
| snapshotId | String |
| espacoId | Reference |
| dataHora | Timestamp |
| volumeAtual | Number |
| espacoLivre | Number |
| percentualOcupacao | Number |
| quantidadePontos3D | Number |

---

###  historicoOcupacao

Permite gerar gráficos e dashboards.

| Campo | Tipo |
|---|---|
| historicoId | String |
| espacoId | Reference |
| dataHora | Timestamp |
| percentualOcupacao | Number |
| volumeOcupado | Number |
| volumeLivre | Number |



## Considerações Arquiteturais

A adoção do Firebase Firestore trouxe benefícios importantes para a solução:

- Escalabilidade em nuvem;
- Sincronização em tempo real;
- Redução da complexidade relacional;
- Melhor desempenho para aplicações distribuídas;
- Facilidade de integração com aplicações Web e Mobile;
- Estrutura orientada a documentos adequada para IoT e monitoramento contínuo.

A arquitetura híbrida implementada no projeto permite que o módulo Kinect opere de maneira independente localmente, enquanto a aplicação MVC centraliza e distribui as informações operacionalmente em ambiente cloud.

---

## Modelo Conceitual — MVVM Kinect

<p align="center">
  <img src="./Imagens/Modelo_Conceitual_MVVM.png" width="1000" alt="Modelo Conceitual MVVM Kinect" />
</p>

### Descrição das Entidades

| Entidade | Finalidade |
|---|---|
| **MedicaoVolume** | Armazena as leituras volumétricas realizadas pelo sensor Kinect |
| **ParametrosSistema** | Define os limites operacionais e parâmetros utilizados pelo sistema |

---

### Entidade: MedicaoVolume

| Campo | Tipo |
|---|---|
| id | Integer |
| data_hora | DateTime |
| volume_medido | Decimal |
| origem_leitura | String |

---

### Entidade: ParametrosSistema

| Campo | Tipo |
|---|---|
| id | Integer |
| volume_maximo | Decimal |
| volume_minimo | Decimal |
| email_notificacao_ativo | Boolean |
| data_atualizacao | DateTime |

---

## Modelo Lógico — MVVM Kinect

<p align="center">
  <img src="./Imagens/Modelo_Logico_MVVM.png" width="1000" alt="Modelo Lógico MVVM Kinect" />
</p>

### Estrutura Relacional

| Tabela | Descrição |
|---|---|
| **MedicaoVolume** | Histórico das medições capturadas pelo sensor |
| **ParametrosSistema** | Configurações e regras de negócio locais |

---

### Tabela: MedicaoVolume

| Campo | Tipo | Restrição |
|---|---|---|
| id | INTEGER | PK |
| data_hora | DATETIME | NOT NULL |
| volume_medido | DECIMAL | NOT NULL |
| origem_leitura | VARCHAR | NOT NULL |

---

### Tabela: ParametrosSistema

| Campo | Tipo | Restrição |
|---|---|---|
| id | INTEGER | PK |
| volume_maximo | DECIMAL | NOT NULL |
| volume_minimo | DECIMAL | NOT NULL |
| email_notificacao_ativo | BOOLEAN | NOT NULL |
| data_atualizacao | DATETIME | NOT NULL |

---

## Modelo Físico — MVVM Kinect

O modelo físico do módulo MVVM foi implementado utilizando:
- SQLite;
- Entity Framework Core;
- Migrations;
- Persistência local embarcada.

A estrutura física é gerada automaticamente pelo Entity Framework através das migrations da aplicação.

<p align="center">
  <img src="./Imagens/Modelo_Fisico_MVVM.png" width="1000" alt="Modelo Físico MVVM Kinect" />
</p>

---

## Viabilidade técnica

### Introdução
Nosso projeto, propõe o mapeamento volumétrico inteligente de estoques utilizando o sensor **Kinect (Xbox 360)** integrado a um sistema desenvolvido na plataforma **.NET 8**. A solução foca na identificação, monitoramento e classificação de excedentes produtivos, promovendo eficiência operacional e sustentabilidade com baixo custo de implementação.

### 2. Descrição da Solução
A solução utiliza a tecnologia de luz estruturada e sensores infravermelhos do Kinect para a captura tridimensional (RGB-D) do espaço físico destinado ao armazenamento.
* **Processamento:** Os dados de profundidade são processados em **C#**, onde algoritmos de geometria analítica convertem a "nuvem de pontos" (*point cloud*) em métricas de volume real ($m^3$).
* **Interface:** Desenvolvida em **ASP.NET Core**, a interface permite o monitoramento em tempo real e acesso via navegador, facilitando a operação sem necessidade de infraestruturas complexas de software local.

### 3. Requisitos de Hardware
Para a execução estável do sistema, definiu-se a seguinte configuração mínima:
* **Estação de Trabalho:** Processador Intel Core i7, 16 GB de memória RAM, SSD de 500 GB;
* **Sensor:** Kinect Xbox 360 com Adaptador USB e Fonte de Alimentação Própria;
* **Infraestrutura:** Estrutura de suporte rígida para fixação zenital (superior) do sensor, garantindo um ângulo de visão constante e livre de obstruções.

### 4. Organização Tecnológica
A arquitetura de software foi planejada para garantir escalabilidade e performance:
* **Plataforma:** .NET SDK 8 (Ambiente de desenvolvimento Rider/Visual Studio);
* **Linguagem:** C#;
* **Bibliotecas e APIs:** * *Microsoft Kinect SDK 1.8*: Para captura bruta de dados do sensor;
    * *Math.NET Numerics*: Para cálculos matemáticos e estatísticos dos pontos capturados;
    * *Interoperabilidade*: Camada de compatibilidade para comunicação entre o SDK legado (32-bit) e o ambiente moderno .NET 8 (64-bit).
* **Banco de Dados:** SQLite para persistência do histórico de medições e parâmetros de sistema.

### 5. Metodologia de Implementação
O processo de implementação segue etapas rigorosas para garantir a precisão:
1.  **Calibração de Campo:** Instalação física e definição do plano de referência (chão do estoque).
2.  **Desenvolvimento Web:** Criação das *Razor Pages* para visualização de dados e indicadores operacionais.
3.  **Algoritmo de Volume:** Integração matemática para traduzir a ocupação do espaço em dados numéricos.
4.  **Definição de Gatilhos:** Programação de alertas automáticos baseados em limites volumétricos pré-configurados.
5.  **Testes de Precisão:** Validação da detecção digital comparando com medições físicas reais para ajuste de sensibilidade.
6.  **Deploy:** Disponibilização do sistema em rede interna para acesso corporativo.

### 6. Benefícios Técnicos
* **Precisão Automatizada:** Redução drástica do erro humano comum em inventários manuais;
* **Custo-Benefício:** Uso de hardware acessível em substituição a sensores LiDAR industriais de alto custo;
* **Escalabilidade:** Possibilidade de replicar a solução em diferentes pontos de armazenamento com o mesmo núcleo de processamento;
* **Integração Digital:** Facilidade de exportação de relatórios e conexão com outros sistemas de gestão (ERP).
  
---

## Viabilidade econômica

### 1. Custos Estimados de Implantação
O projeto **Inventory Masters** foi concebido como uma solução tecnológica de baixo custo, utilizando hardware acessível e desenvolvimento próprio. Esta abordagem reduz drasticamente o investimento inicial quando comparado a sistemas industriais de mapeamento volumétrico baseados em sensores LiDAR de alta gama.

#### Investimento em Hardware
| Item | Quantidade | Valor Unitário | Total |
| :--- | :---: | :---: | :---: |
| Kinect Xbox 360 | 1 | R$ 30,00 | R$ 30,00 |
| Cabo/adaptador USB com fonte | 1 | R$ 80,00 | R$ 80,00 |
| CPU Core i3 | 1 | R$ 800,00 | R$ 800,00 |
| **Subtotal Hardware** | | | **R$ 910,00** |

---

### 2. Custo de Desenvolvimento (Mão de Obra)
O desenvolvimento do sistema foi realizado pela equipe técnica, com uma dedicação total de 30 horas produtivas. Para fins de cálculo de viabilidade, considerou-se um valor de mercado base para desenvolvedores juniores/estagiários.

* **Horas totais:** 30 horas (3 integrantes × 10 horas)
* **Valor/Hora estimado:** R$ 15,00
* **Total Mão de Obra:** **R$ 450,00**

---

### 3. Custo Total do Projeto
| Categoria | Valor |
| :--- | :---: |
| Hardware | R$ 910,00 |
| Mão de Obra | R$ 450,00 |
| **Total Geral** | **R$ 1.360,00** |

O valor total demonstra a alta atratividade financeira da solução, especialmente frente a sistemas comerciais de automação que podem custar até dez vezes mais.

---

### 4. Análise da Estrutura de Investimento
A análise percentual do investimento revela que:
* **CPU (Equipamento Principal):** Representa aproximadamente **48,7%** do custo.
* **Mão de Obra:** Representa **27,4%**.
* **Sensores e Acessórios:** Representam os **23,9%** restantes.

Esta estrutura evidencia que, em empresas que já possuam infraestrutura computacional disponível, o custo de implementação pode ser reduzido para menos de **R$ 600,00**, aumentando ainda mais a competitividade do projeto.

---

### 5. Benefícios Econômicos e Operacionais
A implementação da plataforma gera ganhos diretos e indiretos para a organização:
* **Redução de Perdas:** Identificação precoce de matéria-prima excedente antes da depreciação ou perda de validade.
* **Economia Circular:** Geração de receita através da venda ou redistribuição de excedentes para parceiros estratégicos.
* **Otimização de Processos:** Redução do tempo gasto em inventários manuais, permitindo que a mão de obra foque em atividades de maior valor agregado.
* **Dados para Decisão:** Apoio à tomada de decisão baseada em volumetria real, evitando compras desnecessárias de insumos.

---

### 6. Conclusão da Viabilidade Financeira
O investimento inicial de **R$ 1.360,00** é consideravelmente inferior ao custo de manutenção de um inventário manual. 

Considerando o salário mínimo projetado para 2025 de **R$ 1.518,00**, somado aos encargos trabalhistas (estimados em 70%), o custo mensal de um único funcionário pode ultrapassar **R$ 2.580,00**. 

Portanto, o sistema Inventory Masters apresenta um **Payback (retorno do investimento)** estimado em **menos de um mês**, consolidando-se como uma alternativa economicamente viável, escalável e de alto impacto para a sustentabilidade empresarial.

---
## Resultados e conclusão

A implementação do projeto **Inventory Masters** demonstrou que a convergência entre hardware acessível e software moderno é uma solução eficaz para os desafios da logística reversa e economia circular. 

**Resultados Alcançados:**
* **Precisão Volumétrica:** O uso do sensor Kinect permitiu uma leitura tridimensional com margem de erro mínima, eliminando as inconsistências das medições manuais.
* **Agilidade na Destinação:** O sistema de gatilhos reduziu o tempo de permanência de excedentes no estoque, conectando-os rapidamente a parceiros de reaproveitamento.
* **Impacto Econômico:** Validou-se um modelo de baixíssimo custo (R$ 1.360,00), com retorno sobre o investimento (ROI) inferior a 30 dias.

**Conclusão:**
Conclui-se que a Inventory Masters não é apenas uma ferramenta de medição, mas um elo estratégico para a sustentabilidade industrial. Ao transformar desperdício em dados e ativos, a plataforma cumpre seu papel de promover eficiência operacional e responsabilidade socioambiental, provando ser uma solução escalável e tecnicamente robusta para o mercado atual.

---

## ANEXOS

### BMG CANVAS (Business Model Canvas)

O quadro abaixo resume o modelo de negócio da Inventory Masters, destacando como a empresa cria, entrega e captura valor.

| Parcerias Principais | Atividades-Chave | Propostas de Valor | Relacionamento com Clientes | Segmentos de Clientes |
| :--- | :--- | :--- | :--- | :--- |
| • Empresas de Reciclagem<br>• Gestores de Resíduos<br>• Fornecedores de Hardware (Kinect) | • Desenvolvimento de Software<br>• Calibração de Sensores<br>• Gestão de Dados | • Redução de custos em inventários<br>• Destinação estratégica de excedentes<br>• Mapeamento 3D de baixo custo | • Suporte Técnico<br>• Relatórios de Sustentabilidade (ESG)<br>• Interface Intuitiva | • Indústrias de Manufatura<br>• Centros Logísticos<br>• Pequenas e Médias Empresas (PMEs) |
| **Recursos Principais** | | **Canais** | | **Estrutura de Custos** |
| • Algoritmo de Visão Computacional<br>• Equipe Técnica<br>• Plataforma .NET 8 | | • Dashboard Web<br>• E-mail / Notificações Push<br>• Consultoria Técnica | | • Manutenção do Software<br>• Aquisição de Hardware<br>• Marketing e Vendas |

**Fluxos de Receita:**
* Licenciamento da Plataforma (SaaS);
* Taxa de conexão por material reaproveitado;
* Consultoria para implementação de Economia Circular.

---

### Situação de aprendizagem

Este projeto foi desenvolvido como resposta a uma **Demanda Setorial** mediada pelo **SENAI**, originada especificamente das necessidades do setor de **Indústria Gráfica**. O desafio proposto exigiu a criação de uma solução tecnológica capaz de gerenciar o elevado volume de excedentes produtivos — como aparas de papel, sobras de substratos e insumos — que frequentemente não possuem rastreabilidade automatizada.

A **Inventory Masters** foi projetada para resolver o gargalo de identificação e cubagem desses materiais, transformando o que antes era tratado como resíduo gráfico em ativos rastreáveis para a economia circular. 

**Versatilidade e Escalabilidade:**
Embora o desenvolvimento inicial tenha sido pautado pelo cenário de uma **Gráfica**, a arquitetura do sistema foi construída sob os pilares da **Indústria 4.0**, o que permite sua total adaptação a qualquer cenário industrial. A lógica de visão computacional e o algoritmo de cálculo volumétrico são agnósticos ao tipo de material, tornando a solução pronta para ser implementada em:
* **Fábricas de móveis** (sobras de madeira/MDF);
* **Indústrias metalúrgicas** (sucatas e retalhos metálicos);
* **Centros de distribuição e logística** (otimização de espaços e paletização).

Dessa forma, o projeto entrega uma resposta precisa à demanda da indústria gráfica, ao mesmo tempo que se consolida como uma ferramenta versátil de gestão de ativos para o ecossistema fabril de forma ampla.

---

# Anexo I

---

# Guia de Configuração do Ambiente — Inventory Masters Kinect

Este guia orienta os colaboradores sobre como configurar a máquina local para rodar o projeto **Inventory Masters Kinect** utilizando o banco de dados local **SQLite** e o **Entity Framework Core**.

---

## 1. Pré-requisitos do Ambiente

Como o SQLite roda diretamente como um arquivo dentro do projeto, não é necessário instalar nenhum servidor de banco de dados pesado (como SQL Server). No entanto, é preciso garantir que o ecossistema do .NET 6 esteja completo.

### A. Runtime do .NET 6 (Desktop / WPF)
Certifique-se de que possui a carga de trabalho de desenvolvimento desktop para .NET instalada no Visual Studio. Caso precise instalar ou atualizar os runtimes do .NET 6 via terminal, execute o seguinte comando no PowerShell como Administrador:

```powershell
winget install Microsoft.DotNet.DesktopRuntime.6
```
### B. Ferramenta Global do Entity Framework Core (`dotnet-ef`)
Para evitar conflitos de versão entre o .NET 6 do projeto e versões mais recentes do SDK instaladas em sua máquina, instale globalmente a ferramenta de linha de comando do EF Core na versão correspondente:

```powershell
dotnet tool install --global dotnet-ef --version 6.0.36
```

*(Nota: Se você já tiver uma versão mais recente instalada e encontrar erros, desinstale-a primeiro com `dotnet tool uninstall --global dotnet-ef` antes de rodar o comando acima).*

---

## 2. Primeiros Passos após o `Git Pull`
Assim que você puxar as últimas alterações da branch (que já incluem a infraestrutura do banco de dados e os pacotes NuGet configurados), siga o passo a passo abaixo para gerar o seu banco de dados local:

1. Abra a Solution (`.sln`) no **Visual Studio 2022**.
2. Abra o **Package Manager Console** (Console do Gerenciador de Pacotes):
   * Vá em: `Tools` > `NuGet Package Manager` > `Package Manager Console`
3. No topo do console, verifique se o campo **Default project** (Projeto padrão) está apontando exatamente para:
   * `InventoryMastersKinect`
4. Execute o comando para aplicar as migrações existentes e criar o banco de dados:

```powershell
Update-Database
```

## 3. Estrutura e Onde o Banco Fica Salvo
* Após a execução bem-sucedida do comando, o Entity Framework gerará o arquivo local **`inventory_masters.db`** na raiz do diretório do seu projeto executável.
* Os pacotes de inicialização nativa do driver (`SQLitePCLRaw`) já estão configurados no código do projeto, garantindo o mapeamento correto das tabelas de medição.
---
