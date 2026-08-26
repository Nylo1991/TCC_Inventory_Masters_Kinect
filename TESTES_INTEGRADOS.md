# Testes integrados — Inventory Masters

Integração do arquivo Testes.zip dos quatro integrantes, validada em 26/08/2026.
Alterações aplicadas nesta cópia: TCC_Inventory_Masters_Kinect.Diulie.
O ZIP original foi preservado. Nenhum commit ou push foi realizado.

## Resultado verificado

| Integrante | Camada | MVC | Kinect |
| --- | --- | ---: | ---: |
| Diulie | ViewModels e Hubs | 40 | 61 |
| Danilo | Services | 49 | 63 |
| Miguel Duarte | Controllers | 60 | — |
| Marilene | Repositories (cenários adaptados) | 102 | 14 |
| **Total aprovado** | | **251** | **138** |

Total: 389 casos executados, zero falhas e zero ignorados.
As linhas de Theory contam como casos separados. Esses números não representam percentual de cobertura.

Os comprovantes locais estão em TestResults/mvc-integracao.trx e
TestResults/kinect-integracao.trx. A pasta TestResults é ignorada pelo Git.
Os relatórios antigos dos integrantes não foram atualizados automaticamente.

## Organização

Há somente dois projetos de testes na solução:

- MVC_InventoryMasters.Tests/MVC_InventoryMasters.Tests.csproj — .NET 8.
- InventoryMastersTests/TCC_Inventory_Masters_Kinect.Tests.csproj — .NET Framework 4.8, x64.

Cada projeto contém pastas por integrante. Os nomes dos assemblies são compartilhados,
sem nome pessoal; namespaces existentes foram preservados quando possível.
No Test Explorer, agrupe por Traits/Características e use Integrante para localizar a autoria.
Infrastructure contém o suporte de isolamento e não é uma camada da aplicação.

## Correções e adaptações

- Interfaces dos repositórios e serviços foram incluídas e registradas na injeção de dependência.
  Os controllers recebem interfaces; a aplicação continua usando as implementações reais.
- AcessoControllerTests deixou de criar objetos não inicializados por FormatterServices.
- As referências entre projetos, namespaces e InternalsVisibleTo foram compatibilizadas.
  Construtores de suporte continuam internos, visíveis somente ao assembly de testes.
- Os serviços Kinect de Danilo foram integrados ao projeto net48/x64, com as referências
  Kinect e SignalR compatíveis com a aplicação.
- O teste de ausência do Kinect agora injeta a ausência de sensor; não depende de desconectar
  fisicamente o equipamento.
- Os cenários de repositório da Marilene foram adaptados: dados sintéticos por teste,
  valores esperados explícitos, falhas simuladas e verificações do que foi realmente persistido.
  Foram removidas as dependências de usuários já existentes e as asserções condicionais/vazias.
- O teste de falha de ListarTodos em MedicaoVolumeRepository foi ajustado para verificar
  a exceção que o método realmente propaga, em vez de esperar uma lista vazia.
- Corrigido um defeito no modelo ParametrosSistema: Range(0, 100) no campo double do raio
  arredondava valores como -0,1 e 100,1. Range(0d, 100d) valida corretamente os limites
  decimais de 0 a 100 declarados pelo modelo. Isso não afirma alcance físico de 100 m do Kinect.

## Isolamento e limites da validação

- FirebaseService possui um construtor interno que recebe FirestoreDb.
  Nos testes, FirestoreMemory fornece um transporte gRPC em memória: não cria canal de rede,
  não carrega credenciais e não acessa o projeto Firebase real.
- Esse transporte suporta apenas as operações usadas nos cenários. Não valida regras,
  índices, latência, transações, concorrência ou comportamento completo do servidor Firestore.
- KinectRepository recebe internamente uma fábrica de contexto. Em produção, o adaptador
  KinectDataContext continua usando AppDbContext/SQLite; nos testes, usa listas em memória.
  Os testes validam lógica de consulta e persistência solicitada, não a tradução SQL do EF.
- O destino dos logs é substituído antes dos testes Kinect por uma fila em memória,
  evitando gravações no banco da aplicação.
- As suítes não enviam e-mail, não conectam ao SignalR remoto e não iniciam o sensor.
  SMTP real, autenticação externa, calibração física e captura real exigem testes de integração.
- A execução paralela dentro de cada assembly está desativada devido ao estado compartilhado
  de logs/contexto presente no código.
- ObterMedicoesEmOrdemCrescente atualmente usa o banco selecionado pela empresa do construtor;
  os parâmetros usuario/empresa do método não acrescentam filtros. Essa regra não foi alterada
  nesta integração nem declarada como isolamento por usuário nos testes.

## Como executar

Preferencialmente, abra TCC_Inventory_Masters_Kinect.slnx no Visual Studio,
selecione Debug/x64, restaure os pacotes, compile e use Testar > Executar todos os testes.
É necessário o SDK .NET 8, o targeting pack .NET Framework 4.8 e as dependências
do projeto Kinect presentes no repositório (Bibliotecas/Kinect e pacotes NuGet restaurados).

Pelo PowerShell, na raiz desta cópia:

```powershell
.\scripts\Executar-Testes.ps1
```

O script encontra o MSBuild/VSTest pelo vswhere do Visual Studio, compila os projetos,
executa as duas suítes e produz os arquivos TRX. Falha de compilação ou de teste encerra
o script com erro; não há configuração de Firebase/Gmail a fornecer.

## Avisos remanescentes

O MVC ainda apresenta avisos preexistentes de nullable/obsolescência e NU1701:
HelixToolkit.Wpf e System.Data.SQLite.Linq direcionados ao .NET Framework no projeto net8.
As execuções acima passaram, mas esses avisos de dependências não equivalem a garantia
de compatibilidade de todas as funcionalidades da aplicação.

## Antes do commit único

Revise os dois projetos de testes, a solução, os contratos/injeção de dependência,
os pontos internos de isolamento, a correção do raio e esta documentação.
As mudanças anteriores de ViewModel que já estavam nesta cópia foram preservadas e são
dependências dos testes de Diulie; revise-as juntamente com o conjunto.

Não faça git add -A sem revisar: já havia dois instaladores .exe no staging
em Docs/Intalação do Sistema e uma alteração em MVC_InventoryMasters/appsettings.json.
Eles não foram alterados nem retirados do staging por esta integração.
Desmarque os instaladores na revisão do commit se não fizerem parte da entrega.
Não publique senhas SMTP, credenciais Firebase, bin, obj, .vs, TestResults ou bancos .db.

Para os próximos trabalhos, cada integrante deve editar sua pasta no mesmo par de projetos,
combinar mudanças nos arquivos compartilhados e atualizar a branch antes de integrar.
Concentrar em um commit ajuda a entrega atual, mas não elimina conflitos futuros por si só.

