# Migração

## Dados do sistema
* **Nome do Sistema:** Inventory Masters
* **Versão:** III
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

* **Cenários Alternativos de Migração e Integração (Casos de Uso Corporativo):**  
  Embora o núcleo do **Inventory Masters** nasça de uma inicialização limpa baseada em captura física, a adoção do sistema em clientes de médio e grande porte exige diretrizes para cenários onde dados pré-existentes precisem ser considerados:

| Cenário Alternativo | Abordagem e Direcionamento Técnico | Finalidade e Impacto Operacional |
| :--- | :--- | :--- |
| **Cenário de Carga Inicial de Cadastros**<br>*(Planilhas e ERPs)* | Caso o cliente possua catálogos de produtos, SKUs e dados de fornecedores em sistemas legados (ERPs ou planilhas de Excel), a estratégia prevê a execução de scripts de importação estática via rotinas ETL (Extract, Transform, Load) diretamente nas novas tabelas do SQL Server / Azure. | Popular apenas as entidades relacionais básicas antes da ativação do hardware. |
| **Cenário de Convivência Híbrida**<br>*(API Gateway / Integração de Sistemas)* | Para ambientes corporativos que mantêm sistemas legados de gestão de armazéns (WMS), a estratégia de migração evolui para uma abordagem de integração contínua. O Inventory Masters atua como a ponta de captura física (Kinect), despachando os payloads de medição via SignalR e gravando os dados consolidados no SQL Server/Azure. | Disponibilizar webhooks ou endpoints para sincronização com o software legado do cliente. |
| **Cenário de Transição de Base**<br>*(Legacy to Production Cutover)* | Na eventualidade de substituição de uma versão piloto baseada em arquivos SQLite locais para o ambiente corporativo definitivo em nuvem, a estratégia adota uma janela de manutenção programada (cutover), onde as bases locais são consolidadas e validadas por checksum. | Migrar os dados em lote para a respectiva instância tenant no Azure SQL Database. |

---

## Testes de Validação de Backup

| ID do Teste | Tipo de Validação | Descrição Detalhada do Procedimento | Critério de Aceite (Sucesso) |
| :--- | :--- | :--- | :--- |
| **TVB-01** | **Homologação de Restauração Local (Cold Backup)** | • Extração do arquivo compactado (`.zip` com timestamp) contendo a base SQLite local (`inventorymasters_acesso.db` ou `inventorymasters_{empresa}.db`).<br>• Inserção do arquivo restaurado em um diretório de testes isolado e inicialização da aplicação WPF.<br>• Execução de consultas de leitura e escrita nas tabelas principais para verificar a legibilidade do arquivo. | A aplicação deve carregar o banco de dados sem exceções de *I/O*, corrupção de arquivo ou falhas de leitura no Entity Framework. |
| **TVB-02** | **Validação de Integridade Relacional e Chaves** | • Execução de scripts de verificação de integridade estrutural (ex: `DBCC CHECKDB` no SQL Server/Azure ou comandos de PRAGMA no SQLite) após a restauração do backup.<br>• Conferência exata de chaves primárias (*Primary Keys*), chaves estrangeiras (*Foreign Keys*) e restrições de unicidade por *tenant*. | Ausência total de órfãos relacionais, violações de restrição ou corrupção de páginas de dados estruturais. |
| **TVB-03** | **Simulação de Rollback em Janela de Migração** | • Simulação intencional de falha durante a rotina de carga de dados para a nova infraestrutura SQL Server / Azure.<br>• Interrupção imediata do processo, descarte do banco corrompido e execução do script automatizado de restauração a partir do Backup Frio inicial. | Retorno do ambiente ao estado exato anterior à tentativa de migração, sem perda de arquivos e com a aplicação pronta para reavaliar a rotina. |
| **TVB-04** | **Teste de Conectividade e Assinatura por Tenant Pós-Restore** | • Restauração de um banco de dados específico de uma empresa (*tenant*) no ambiente Azure SQL Database.<br>• Inicialização do `MedicaoHub` e do `NotificacaoHub` para testar o mapeamento de instâncias e o direcionamento correto dos payloads do Kinect. | O sistema deve reconhecer o *tenant* restaurado, mantendo o isolamento estrito dos dados e permitindo o tráfego em tempo real via SignalR sem cruzamento de informações. |
