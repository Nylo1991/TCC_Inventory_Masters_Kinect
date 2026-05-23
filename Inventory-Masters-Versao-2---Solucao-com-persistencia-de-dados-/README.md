# INVENTORY MASTERS - SOLUÇÕES INTELIGENTES EM MAPEAMENTO DE ESTOQUE
---
**Unidade SENAI:** Nova Lima  
**Instrutor:** Frederico Martins Aguiar

<div align="center">

## INTEGRANTES DO GRUPO

<p align="center">
  <img src="./imagens/Equipe.jpeg" width="600" alt="Equipe Inventory Masters" />
</p>

</div>

<div align="center">

| Nome | Curso| Especialidade no Projeto |
| :--- | :--- | :--- |
| **Danilo Silva Santos** | Programação de Sistemas | Desenvolvimento e Integração do Sensor Kinect|
| **Marilene da Silva Araujo** | Programação de Sistemas | Desenvolvimento, e Modelagem de Banco de Dados |
| **Miguel Cassio Braga Duarte** |Programação de Sistemas | Desenvolvimento e Lógica do negócio |
| **Diulie Mileide Batista Correia** |Programação de Sistemas | Desenvolvimento e Documentação |

</div>

---

##  Quem somos!

A Inventory Masters é uma plataforma tecnológica voltada para a gestão inteligente de excedentes produtivos.
Atuamos conectando empresas a soluções estratégicas de reaproveitamento de materiais.
Transformamos desperdícios em ativos com potencial de geração de valor econômico.
Promovemos redução de custos, eficiência operacional e responsabilidade ambiental.
Somos inovação aplicada à gestão sustentável e competitiva.

<p align="center">
  <img src="./imagens/logo.png" width="600" alt="Logo Inventory Masters" />
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
  <img src="./imagens/Diagrama de Caso de Uso.png" width="600" alt="Diagrama de Caso de Uso" />
</p>

| Nome  |  Funcionalidade       |Perfil         | Descrição                                                                            |
|-------|-----------------------|---------------|--------------------------------------------------------------------------------------|
|UC01:  | Efetuar Login         | Administrador / Operador | Logar no Sistema                                                          |
|UC02:  | Configurar Parâmetros | Administrador | Definir os limites de volume ($m^3$) para disparo de alertas.                        |
|UC03:  | Manter Parceiros      | Administrador | Cadastrar, editar ou excluir empresas que receberão os excedentes.                   |
|UC04:  | Registrar Medição     | Sistema / Operador | Captura automática via câmera ou inserção manual do volume atual.               |
|UC05:  | Monitorar Dashboard   | Operador | Visualizar em tempo real o status dos resíduos/estoque.                                   |
|UC06:  | Notificar Parceiros   | Sistema | (Automático)<< include >> no UC03. Se o volume atingir o limite, o sistema envia o alerta. |
|UC07:  | Gerar Relatório       | Administrador |Exportar histórico de medições e eficiência de destinação.                            |
|UC08:  | Efetuar Log Out       | Administrador / Operador| Deslogar do Sistema                                                        |

---

## Diagrama de Fluxo 

<p align="center">
  <img src="./imagens/Diagrama de Fluxo.png" width="600" alt="Diagrama de Fluxo Inventory Masters" />
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
### Modelo Conceitual:

<img width="1349" height="616" alt="InventoryMaster_ModeloConceitual" src="https://github.com/user-attachments/assets/c6d86792-4619-4497-9876-0407c63a003f" />

| Nome da Entidade        | Atributos                                                                       | Chave Primária | Chave Secundária                     |
| ----------------------- | ------------------------------------------------------------------------------- | -------------- | ------------------------------------ |
| **Parceiro**            | Id, Nome, Empresa, Email, Telefone, Endereco, Ativo, Data_Cadastro              | Id             | —                                    |
| **ParametrosSistema**   | Id, Volume_Maximo, Volume_Minimo, Email_Notificacao_Ativo, Data_Atualizacao     | Id             | —                                    |
| **Usuario**             | Id, Nome, Email, Senha, Perfil, Data_Cadastro, Ativo                            | Id             | —                                    |
| **MedicaoVolume**       | Id, Data_Hora, Volume_Medido, Origem_Leitura                                    | Id             | —                                    |
| **Notificacao**         | Id, Data_Envio, Volume_Momento, Status_Envio, Mensagem, Quantidade_Destinatario | Id             | —                                    |
| **NotificacaoParceiro** | Status_Entrega                                                                  | —              | Ligação entre Parceiro e Notificacao |

---
### Modelo Lógico:

<img width="1080" height="614" alt="InventoryMaster_ModeloLogico" src="https://github.com/user-attachments/assets/08a23fd9-58b8-4b26-8be3-b3bf43bcd847" />

| Nome da Entidade    | Descrição                                                                        | Atributos               | Tipo de Dado | PK/FK |
| ------------------- | -------------------------------------------------------------------------------- | ----------------------- | ------------ | ----- |
| Parceiro            | Guarda informações das empresas parceiras que recebem notificações.              | Id                      | INTEGER      | PK    |
|                     |                                                                                  | Nome                    | VARCHAR      |       |
|                     |                                                                                  | Empresa                 | VARCHAR      |       |
|                     |                                                                                  | Email                   | VARCHAR      |       |
|                     |                                                                                  | Telefone                | VARCHAR      |       |
|                     |                                                                                  | Endereco                | VARCHAR      |       |
|                     |                                                                                  | Ativo                   | BOOLEAN      |       |
|                     |                                                                                  | Data_Cadastro           | DATE         |       |
| Usuario             | Representa os usuários cadastrados no sistema.                                   | Id                      | INTEGER      | PK    |
|                     |                                                                                  | Nome                    | VARCHAR      |       |
|                     |                                                                                  | Email                   | VARCHAR      |       |
|                     |                                                                                  | Senha                   | VARCHAR      |       |
|                     |                                                                                  | Perfil                  | VARCHAR      |       |
|                     |                                                                                  | Data_Cadastro           | DATE         |       |
|                     |                                                                                  | Ativo                   | BOOLEAN      |       |
| Notificacao         | Registra notificações enviadas pelo sistema.                                     | Id                      | INTEGER      | PK    |
|                     |                                                                                  | Data_Envio              | DATE         |       |
|                     |                                                                                  | Volume_Momento          | DECIMAL      |       |
|                     |                                                                                  | Status_Envio            | VARCHAR      |       |
|                     |                                                                                  | Mensagem                | VARCHAR      |       |
|                     |                                                                                  | Quantidade_Destinatario | INTEGER      |       |
|                     |                                                                                  | fk_Usuario_Id           | INTEGER      | FK    |
| MedicaoVolume       | Armazena medições de volume registradas no sistema.                              | Id                      | INTEGER      | PK    |
|                     |                                                                                  | Data_Hora               | DATE         |       |
|                     |                                                                                  | Volume_Medido           | DECIMAL      |       |
|                     |                                                                                  | Origem_Leitura          | VARCHAR      |       |
|                     |                                                                                  | fk_Usuario_Id           | INTEGER      | FK    |
| ParametrosSistema   | Define os parâmetros de controle do sistema.                                     | Id                      | INTEGER      | PK    |
|                     |                                                                                  | Volume_Maximo           | DECIMAL      |       |
|                     |                                                                                  | Volume_Minimo           | DECIMAL      |       |
|                     |                                                                                  | Email_Notificacao_Ativo | BOOLEAN      |       |
|                     |                                                                                  | Data_Atualizacao        | DATE         |       |
| NotificacaoParceiro | Tabela associativa que relaciona notificações aos parceiros que irão recebê-las. | Id                      | INTEGER      | PK    |
|                     |                                                                                  | fk_Parceiro_Id          | INTEGER      | FK    |
|                     |                                                                                  | fk_Notificacao_Id       | INTEGER      | FK    |
|                     |                                                                                  | Status_Entrega          | VARCHAR      |       |

---
### Modelo Fisíco:

<table align="center">
  <tr>
    <td><img src="https://github.com/user-attachments/assets/3552237e-01ba-4191-98a3-ebccb8b9e5cd" width="400" alt="imagem 1"></td>
    <td><img src="https://github.com/user-attachments/assets/6fe7cd76-1829-4d89-b7f8-9cce91d0c49c" width="400" alt="imagem 2"></td>
  </tr>
</table>

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
