# Migração do Sistema

## Dados do sistema
* **Nome do Sistema:** Inventory Masters
* **Versão:** III
* **Ambiente de Aplicação:** Aplicação desktop (WPF) e aplicação WEB integrada com hardware Kinect SDK 1.8, arquitetura híbrida de dados e comunicação em tempo real via **SignalR (`MedicaoHub` e `NotificacaoHub`)**.

## Banco utilizado

| Banco | Versão | Descrição |
| :--- | :--- | :--- |
| **Firebase** | Google.Cloud v4.2.0 | Armazenamento centralizado em nuvem NoSQL gerenciado via Google Cloud SDK através de credenciais de conta de serviço (`service_account`) e integração com o FirebaseDb. |
| **SQLite local** | SQLiteStudio v3.4.21 | SQLite local na aplicação desktop (`inventorymasters_acesso.db` ou dinamicamente como `inventorymasters_{empresa}.db`). |

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

