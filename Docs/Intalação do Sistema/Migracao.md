# Migração do Banco de Dados

## Dados do sistema
* **Nome do Sistema:** Inventory Masters
* **Versão:** 3.0.0
* **Ambiente de Aplicação:** Aplicação desktop (WPF) e aplicação WEB integrada com hardware Kinect SDK 1.8, arquitetura híbrida de dados e comunicação em tempo real via **SignalR (`MedicaoHub` e `NotificacaoHub`)**.
  
---

## Banco utilizado

| Banco | Versão | Descrição |
| :--- | :--- | :--- |
| **Firebase** | Google.Cloud v4.2.0 | Armazenamento centralizado em nuvem NoSQL gerenciado via Google Cloud SDK através de credenciais de conta de serviço (`service_account`) e integração com o FirebaseDb. |
| **SQLite local** | SQLiteStudio v3.4.21 | SQLite local na aplicação desktop (`inventorymasters_acesso.db` ou dinamicamente como `inventorymasters_{empresa}.db`). |

---

## Estrutura atual

| Componente / Camada | Tecnologias / Pacotes | Descrição Técnica |
| :--- | :--- | :--- |
| **Persistência Local** | Entity Framework (v6.5.1)<br>`Stub.System.Data.SQLite.Core.NetFramework`<br>`System.Data.SQLite.Core` (v1.0.119)<br>`SourceGear.sqlite3` (v3.50.4.5) | Utilização de arquivos `.db` via SQLite gerados dinamicamente (como `inventorymasters_acesso.db` ou instâncias isoladas por tenant `inventorymasters_{empresa}.db`), integrados ao Entity Framework e gerenciados em nível de infraestrutura de desktop por meio de dependências nativas e provedores dedicados. |
| **Persistência Remota** | FirebaseAdmin (v3.5.0)<br>`Google.Cloud` (v4.2.0)<br>`FirebaseService` | Arquitetura centralizada em nuvem NoSQL baseada no Firebase Cloud (coleção `Empresas`), instanciada programmaticamente com autenticação por conta de serviço e gerenciamento de pacotes dedicados. |
| **Camada de Comunicação** | ASP.NET Core SignalR<br>`MedicaoHub`<br>`NotificacaoHub` | Topologia híbrida baseada em mensageria bidirecional em tempo real, operando através do `MedicaoHub` (orquestração de eventos de captura volumétrica via SDK do Kinect, persistência remota e propagação assíncrona do payload `NovaMedicao`) e do `NotificacaoHub` (dispatch dinâmico de alertas e avisos operacionais aos clientes conectados). |

> **Notas Técnicas:**
> * **Instâncias Isoladas por Tenant:** Padrão arquitetural onde cada empresa (*tenant*) possui um banco de dados SQLite próprio (`inventorymasters_{empresa}.db`), garantindo o isolamento e a segurança dos dados.
> * **Instanciação Programmaticamente com Autenticação por Conta de Serviço:** Conexão com o Firebase estabelecida via código (`FirebaseService`) utilizando um arquivo de credenciais do tipo *service_account* para autorização segura no Firebase.
> * **Payload:** O pacote de dados útil (como volume medido e data/hora) transportado em tempo real pelo `MedicaoHub` para as telas clientes.
> * **Dispatch:** Ação de disparar ou despachar de forma imediata e automatizada alertas e avisos operacionais pelo `NotificacaoHub` para os clientes conectados.

---

## Alterações previstas

* **Migração de Banco de Dados Local:** Substituição definitiva do SQLite (utilizado para fins acadêmicos e de desenvolvimento) por **SQL Server** no ambiente de produção do cliente, exigindo reconfiguração das strings de conexão e adaptação dos mapeamentos do Entity Framework.
* **Transição de Persistência em Nuvem (Multi-tenant Azure):** Substituição da arquitetura atual baseada no Firebase (onde o isolamento de dados entre empresas precisava de ajustes para evitar o acesso cruzado) por uma estrutura robusta hospedada no **Microsoft Azure**, implementando o modelo de múltiplos inquilinos (*multi-tenant*) com bancos de dados isolados e associados diretamente a cada cliente respectivo.
* **Evolução Estrutural:** Inclusão de novas tabelas, campos e índices conforme atualizações e novas entregas de requisitos do sistema.
* **Adaptação de Componentes:** Ajustes nas rotinas de persistência e nas assinaturas de métodos para compatibilidade com eventuais atualizações de SDKs ou bibliotecas de terceiros (troca dos provedores de nuvem do Google/Firebase para os serviços equivalentes e gerenciados do ecossistema Microsoft Azure / SQL Server).
  
### Nova Estrutura Proposta (Ambiente de Produção / Cliente)

#### Bibliotecas, Dependências e Arquitetura

| Componente / Camada | Tecnologias / Pacotes | Descrição Técnica e Finalidade |
| :--- | :--- | :--- |
| **Persistência Local** | Entity Framework<br>`Microsoft.EntityFrameworkCore`<br>`Microsoft.EntityFrameworkCore.SqlServer`<br>`Microsoft.EntityFrameworkCore.Tools` | Substituição definitiva do SQLite por **SQL Server** no ambiente de produção do cliente para o gerenciamento e persistência local dos dados da estação de trabalho, exigindo reconfiguração das strings de conexão e adaptação dos mapeamentos via Entity Framework Core (com suporte a migrações direcionadas). |
| **Persistência Remota** | Microsoft Azure<br>Azure SQL Database / Serviços Gerenciados<br>`Azure SDK for .NET` (opcional) | Transição da arquitetura NoSQL em nuvem do Firebase para uma infraestrutura robusta hospedada no **Microsoft Azure**, implementando um modelo multi-tenant com bancos de dados isolados e associados diretamente a cada cliente respectivo, garantindo total segurança, privacidade e infraestrutura base no gerenciamento dos dados. |
| **Camada de Comunicação** | ASP.NET Core SignalR<br>`Microsoft.AspNetCore.SignalR.Client`<br>`Microsoft.AspNetCore.App`<br>`MedicaoHub`<br>`NotificacaoHub` | Topologia híbrida baseada em mensageria bidirecional em tempo real integrada ao ecossistema Microsoft Azure/SQL Server, operando através do `MedicaoHub` (orquestração de eventos de captura volumétrica via SDK do Kinect, persistência remota e propagação assíncrona do payload `NovaMedicao`) e do `NotificacaoHub` (dispatch dinâmico de alertas e avisos operacionais aos clientes conectados). |
| **SDKs e Runtimes do Ambiente** | **.NET 8.0 SDK / Runtime**<br>**Microsoft SQL Server** (ou SQL Server Express) | Infraestrutura base no servidor ou estação do cliente para sustentar a execução da aplicação web/serviços e o gerenciamento dos bancos de dados relacionais. |

---
  
## Estratégia de Backup e Mitigação de Riscos para Migração

| Fase da Migração | Tipo de Backup / Ação | Ferramentas / Recursos Utilizados | Descrição Técnica e Procedimento Detalhado |
| :--- | :--- | :--- | :--- |
| **1. Pré-Migração (Linha de Base)** | **Backup Frio Completo (Cold Backup)** | • Scripts de automação em C# / PowerShell<br>• Compactadores de arquivos nativos (.NET `ZipFile`) | • Cópia integral e isolada de todos os arquivos de banco de dados locais (`.db` do SQLite, como `inventorymasters_acesso.db` e `inventorymasters_{empresa}.db`) e exportação dos arquivos de dados da nuvem do Firebase antes de iniciar qualquer alteração.<br>• Armazenamento desses arquivos em diretório seguro e compactado (ex: `.zip` com timestamp) para garantir um ponto de restauração (*rollback*) imediato caso ocorra falha crítica na rotina de conversão. |
| **2. Transição e Validação** | **Backup Transacional de Segurança (SQL Server / Azure)** | • SQL Server Management Studio (SSMS)<br>• T-SQL (`BACKUP DATABASE` / `Recovery Model: Full`) | • Geração de um backup completo (*Full Backup*) da nova estrutura de banco de dados no SQL Server logo após a criação das tabelas e antes da importação em massa dos dados legados.<br>• Utilização de arquivos de log de transação ativados (*Recovery Model: Full*) no SQL Server para permitir recuperações granulares até o exato segundo da migração. |
| **3. Pós-Migração (Homologação)** | **Snapshot de Validação (Azure SQL)** | • Azure Portal / Azure CLI<br>• Recursos nativos de *Restore Points* e *Geo-Redundancy* do Azure | • Criação de um ponto de restauração ou *Snapshot* instantâneo na infraestrutura do Azure SQL Database após a conclusão bem-sucedida da carga de dados.<br>• Validação da integridade referencial e das instâncias por *tenant* antes de liberar o sistema em ambiente de produção para o cliente. |
| **4. Plano de Rollback (Contingência)** | **Estratégia de Reversão Rápida** | • Rotinas de restauração automatizadas<br>• Arquivos de backup frio pré-gravados | • Caso ocorra qualquer inconsistência ou corrupção de dados durante a janela de migração, os serviços são interrompidos, o banco de produção é descartado, e a instância anterior é restaurada a partir do Backup Frio inicial, garantindo zero perda de dados operacionais para o cliente. |

---

## Estratégia de Migração

* **Justificativa de Ausência de Migração de Dados Legados:**  
  A captura de dados do **Inventory Masters** baseia-se na interação física com o sensor Kinect (SDK 1.8), portanto a ausência de um sistema legado tradicional fundamenta-se no fato de que os dados gerados são nativamente estruturados pela primeira vez a partir da nova tecnologia de hardware e visão computacional.
  
  Como o sistema introduz uma solução inovadora de automação e controle de estoque, a aplicação opera com um fluxo de informações dinâmico e físico que difere totalmente dos métodos tradicionais de digitação ou planilhas estáticas. Portanto, não se faz necessária uma migração de dados de um sistema legado anterior pelos seguintes motivos:
  * **Natureza Inovadora da Captura:** Os dados de movimentação e inventário são gerados primariamente em tempo real através da interação com o sensor Kinect, tornando obsoletos registros puramente textuais ou manuais pré-existentes.
  * **Inexistência de Base Prévia Compatível:** As empresas ou ambientes de teste que adotam a solução partem de um cenário onde o controle de estoque era inexistente ou manual, não havendo estrutura de dados digitalizada com o mesmo padrão espacial e métrico exigido pela aplicação.
  * **Inicialização Limpa (Clean State):** O ecossistema híbrido do sistema (banco local SQLite e sincronização com o Firebase) é populado a partir do primeiro inventário físico assistido por hardware, garantindo total integridade e eliminando o risco de inconsistências decorrentes da importação de bases legadas incompatíveis.

* **Cenário de Migração e Integração com Dados Pré-Existentes:**  
  Embora o ecossistema do **Inventory Masters** seja concebido nativamente sob a premissa de uma inicialização limpa (*clean state*) voltada à captura física por visão computacional, o sistema prevê diretrizes técnicas robustas para cenários corporativos onde há a necessidade de aproveitamento e migração de bases legadas. Isso abrange tanto a transição de armazenamentos locais embarcados (como **SQLite**) quanto de estruturas descentralizadas em nuvem leve (como **Firebase**), consolidando-as em definitivo para uma arquitetura centralizada e multi-empresa baseada em **SQL Server e Azure**.
  
| Fase Operacional | Foco da Migração | Detalhes Técnicos, Passos e Abordagem da Execução |
| :--- | :--- | :--- |
| **1. Planejamento e Estruturação do Ambiente** | **Modelagem Multi-tenant e Schema** | • Mapeamento inicial das entidades no Entity Framework Core, alterando o provedor de banco de dados de `UseSqlite` para `UseSqlServer`.<br>• Injeção obrigatória e automatizada da coluna discriminadora de isolamento por inquilino (`EmpresaId`) em todas as tabelas relacionais para garantir a segurança dos dados por cliente. |
| **2. Extração e Tratamento de Dados (ETL Local)** | **Conversão de SQLite para SQL Server** | • Exportação dos dados brutos locais e ajuste de dialetos e funções nativas incompatíveis (ex: conversão de `DATETIME('now')` para `GETDATE()`).<br>• Adaptação das chaves primárias e sequências de identificadores auto-incrementais (transformando `INTEGER PRIMARY KEY` do SQLite em `IDENTITY(1,1)` do T-SQL).<br>• Execução de rotinas de carga assistida via scripts C# transacionais ou comandos otimizados de `BULK INSERT`, preservando rigorosamente as chaves estrangeiras e a integridade referencial por tenant. |
| **3. Reestruturação e Nuvem (Cloud Migration)** | **Substituição do Firebase para Azure** | • Descomissionamento gradual dos ouvintes e serviços de escuta em tempo real do Firebase.<br>• Redirecionamento da mensageria e dos fluxos em tempo real para a Azure, utilizando recursos como *Azure SQL Database Change Tracking* ou *Azure SignalR Service*.<br>• Centralização definitiva de todos os logs, metadados espaciais e métricas capturadas pelo sensor Kinect nas tabelas estruturadas do SQL Server na Azure. |
| **4. Homologação, Cutover e Validação** | **Congelamento e Integridade** | • Definição de uma janela de manutenção controlada (*Cutover*) com interrupção temporária das gravações locais no SQLite e Firebase para evitar perdas de dados recentes do Kinect.<br>• Execução de auditorias de contagem de registros e validações matemáticas por *checksums* comparativos entre as origens e o destino final.<br>• Homologação do ambiente corporativo centralizado para liberar o acesso seguro e segregado às empresas. |

> **Notas Técnicas:**
> * **Multi-tenant**: (Multilocatário) Arquitetura de software onde uma única instância do sistema atende a vários clientes (empresas ou "tenants"). Os dados de todos os clientes residem no mesmo banco de dados, mas são isolados de forma lógica e segura, geralmente por meio de uma chave de identificação (como `EmpresaId`).
> * **Schema**: O "esqueleto" ou modelo estrutural do banco de dados. Ele define como os dados são organizados logicamente, incluindo a especificação de tabelas, colunas, tipos de dados, chaves primárias e relacionamentos (chaves estrangeiras).
> * **ETL (Extract, Transform, Load) Local**: Processo que envolve **Extrair** dados da origem (SQLite), **Transformá-los** (ajustar formatos, converter tipos de dados) e **Carregá-los** no banco de destino (SQL Server). O termo "Local" refere-se à execução dessa rotina diretamente no ambiente ou nas bases embarcadas antes da subida para a nuvem.
> * **T-SQL (Transact-SQL)**: É a extensão proprietária da Microsoft para a linguagem padrão SQL, utilizada no SQL Server. Ela adiciona recursos avançados de programação, variáveis, controle de fluxo e funções nativas específicas (como o `GETDATE()`).
> * **Database Change Tracking**: (Rastreamento de Alterações de Banco de Dados) Recurso nativo do SQL Server e Azure SQL que identifica e registra quais linhas de uma tabela foram modificadas (inseridas, atualizadas ou excluídas), sendo extremamente útil para sincronizar dados com aplicações em tempo real sem sobrecarregar o banco.
> * **Cutover**: O momento exato (ou janela de transição) em que a operação do sistema antigo é congelada/desligada e o novo sistema corporativo assume oficialmente o ambiente de produção. É o "ponto de virada" da migração.
> * **Checksums**: (Somas de Verificação) Técnica de validação que utiliza cálculos matemáticos sobre um bloco de dados para verificar sua integridade. Na migração, serve para atestar que os dados saíram da origem e chegaram ao destino exatamente iguais, sem corrupção ou perda de informações.

---

## Testes de Validação de Backup

| ID do Teste | Tipo de Validação | Descrição Detalhada do Procedimento | Critério de Aceite (Sucesso) |
| :--- | :--- | :--- | :--- |
| **TVB-01** | **Homologação de Restauração Local (Cold Backup)** | • Extração do arquivo compactado (`.zip` com timestamp) contendo a base SQLite local (`inventorymasters_acesso.db` ou `inventorymasters_{empresa}.db`).<br>• Inserção do arquivo restaurado em um diretório de testes isolado e inicialização da aplicação WPF.<br>• Execução de consultas de leitura e escrita nas tabelas principais para verificar a legibilidade do arquivo. | A aplicação deve carregar o banco de dados sem exceções de *I/O*, corrupção de arquivo ou falhas de leitura no Entity Framework. |
| **TVB-02** | **Validação de Integridade Relacional e Chaves** | • Execução de scripts de verificação de integridade estrutural (ex: `DBCC CHECKDB` no SQL Server/Azure ou comandos de PRAGMA no SQLite) após a restauração do backup.<br>• Conferência exata de chaves primárias (*Primary Keys*), chaves estrangeiras (*Foreign Keys*) e restrições de unicidade por *tenant*. | Ausência total de órfãos relacionais, violações de restrição ou corrupção de páginas de dados estruturais. |
| **TVB-03** | **Simulação de Rollback em Janela de Migração** | • Simulação intencional de falha durante a rotina de carga de dados para a nova infraestrutura SQL Server / Azure.<br>• Interrupção imediata do processo, descarte do banco corrompido e execução do script automatizado de restauração a partir do Backup Frio inicial. | Retorno do ambiente ao estado exato anterior à tentativa de migração, sem perda de arquivos e com a aplicação pronta para reavaliar a rotina. |
| **TVB-04** | **Teste de Conectividade e Assinatura por Tenant Pós-Restore** | • Restauração de um banco de dados específico de uma empresa (*tenant*) no ambiente Azure SQL Database.<br>• Inicialização do `MedicaoHub` e do `NotificacaoHub` para testar o mapeamento de instâncias e o direcionamento correto dos payloads do Kinect. | O sistema deve reconhecer o *tenant* restaurado, mantendo o isolamento estrito dos dados e permitindo o tráfego em tempo real via SignalR sem cruzamento de informações. |
