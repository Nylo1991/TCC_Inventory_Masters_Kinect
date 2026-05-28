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
# MODELAGEM DO SISTEMA
---
## Diagrama de Caso de Uso

<p align="center">
  <img src="./Imagens/Diagrama de Caso de Uso.png" width="600" alt="Diagrama de Caso de Uso" />
</p>

| **ID** | **Nome da Funcionalidade** | **Perfil** | **Descrição** |
| :--- | :--- | :--- | :--- |
| UC01 | Efetuar Login | Admin / Operador | Autenticação no sistema. |
| UC02 | Configurar Parâmetros | Administrador | Definição dos limites volumétricos ($m^3$) para alertas. |
| UC03 | Manter Parceiros | Administrador | Gestão de empresas que receberão os excedentes. |
| UC04 | Registrar Medição | Sistema / Operador | Captura via Kinect (WPF) ou inserção manual. |
| UC05 | Monitorar Dashboard | Operador | Visualização em tempo real do status do inventário. |
| UC06 | Notificar Parceiros | Sistema | (Automático) <<include>> no UC04. Disparo de alertas ao atingir limites. |
| UC07 | Gerar Relatório | Administrador | Exportação de histórico e eficiência de destinação. |
| UC08 | Efetuar Log Out | Admin / Operador | Encerramento da sessão. |

---

## Diagrama de Fluxo 

<p align="center">
  <img src="./Imagens/Diagrama de Fluxo.png" width="600" alt="Diagrama de Fluxo Inventory Masters" />
</p>

## Diagrama de Fluxo de Dados (1º Nível)

  ### Entidades Externas
  • **Câmera / Visão Computacional:** Origem dos dados de volume. <br />
  • **Usuário (Adm/Op):** Interage com as configurações e relatórios. <br />
  • **Parceiro:** Destinatário final dos alertas de excedentes.

  ### Processos Principais
  • **P1: Coletar e Validar Medição:** Recebe o sinal da câmera, calcula o volume e atribui a confiabilidade da leitura. <br />
  • **P2: Monitorar Limites (Gatilho):** Compara o volume recebido com os limites gravados em ParametrosSistema. <br />
  • **P3: Gerenciar Notificações:** Se o limite for atingido, busca os parceiros ativos e formata a mensagem. <br />
  • **P4: Gerar Inteligência de Dados:** Consolida medições para o Dashboard e relatórios de auditoria.

  ### Depósitos de Dados 
  • **D1: MedicoesVolume:** Histórico de todas as leituras. <br />
  • **D2: ParametrosSistema:** Regras de negócio (limite max/min). <br />
  • **D3: Parceiros:** Cadastro de quem pode receber o excedente. <br />
  • **D4: Notificacoes:** Registro de logs de envios realizados.
 
## Detalhamento do Fluxo de Execução 

1. **Entrada de Dados:** O sensor (Câmera) envia o *VolumeMedido* para o Processo 1. <br />
2. **Persistência:** O sistema grava a medição no banco de dados **D1**. <br />
3. **Verificação de Regra:** O Processo 2 lê o *VolumeMaximoPermitido* de **D2**. <br />
4. **Tomada de Decisão:** Caso $VolumeMedido > VolumeMaximo$, o fluxo segue para o Processo 3. <br />
5. **Saída de Notificação:** O sistema consulta **D3** (Parceiros), registra o envio em **D4** e dispara o e-mail/alerta para o **Parceiro Externo**.

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

| Campo | Tipo |
|---|---|
| medicaoId | String |
| dataHora | Timestamp |
| volumeMedido | Number |
| origemLeitura | String |
| status | String |

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
| volumeMinimo | Number |
| emailNotificacaoAtivo | Boolean |
| dataAtualizacao | Timestamp |

---

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
